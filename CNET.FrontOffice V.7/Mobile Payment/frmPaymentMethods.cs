using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Net;
using System.IO;
using System.Threading; 
using CNET.Mobile.Payments.Models; 
using CNET.Payment.SDK; 
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.CommonTypes;
using ProcessManager;
using CNET.POS.Settings;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Printing.Core.PdfExport.Metafile;

namespace CNET.Mobile.Payments
{
    public partial class frmPaymentMethods : DevExpress.XtraEditors.XtraForm
    {
        #region Declaration

        private PaymentProvider selected_Payment { get; set; }
        private decimal amount { get; set; }
        private string transaction_Number { get; set; }
        private int? selected_Consignee_ID { get; set; }
        public bool payment_Successful { get; set; }
        public MobilePaymentDTO? Mobile_Transaction { get; set; }

        private System.Timers.Timer form_Close_Timer = new System.Timers.Timer();

        private List<PaymentProvider> Available_Payment_Types = null;

        private bool AllowAdditionalCharge;
        private bool AllowDiscount;
        private VoucherBuffer Voucher_Buffer = null;
        private List<LineItemDetails> item_Details;
        private LineItemDetails item_Detail;
        private LineItemBuffer calculated_Line_Item;
        private VoucherFinalDTO calculated_Voucher;

        #endregion


        public frmPaymentMethods(int voucherdef)
        {
           // CnetPaymentHandler handler = new CnetPaymentHandler();
            InitializeComponent();

            PaymentClient.Initalize_Mobile_Payment(voucherdef);

            Available_Payment_Types = Task.Run(() => PaymentClient.Get_Payment_Options(LocalBuffer.LocalBuffer.CompanyConsigneeData.Code, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code)).Result;

            Initialize_Payment_Providers(Available_Payment_Types);
            form_Close_Timer.Elapsed += form_Close_Timer_Elapsed;
            form_Close_Timer.Interval = 3000;
            form_Close_Timer.AutoReset = false;
        }

        private void form_Close_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }


        #region Event Handlers

        private void tile_ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                var tileItem = sender as TileItem;
                if (tileItem != null && tileItem.Tag != null)
                {
                    selected_Payment = (PaymentProvider)tileItem.Tag;

                    if (selected_Payment.is_Customer_Init)
                    {
                        //YET TO BE DONE BY SALMAN

                        ////Progress_Reporter.Show_Progress("Getting Unapproved Payments...", "Please Wait...");
                        //grdUnapprovedPayments.DataSource = await PaymentClient.Get_Unapproved_Payments(Buffers.Company.Tin, Buffers.Machine_Consginee_Unit, selected_Payment.provider_OUD);
                        //grdUnapprovedPayments.RefreshDataSource();
                        ////Progress_Reporter.Close_Progress();
                        //tcNavigation.SelectedTabPage = tpMpesa;
                    }
                    else
                    {
                        var paymentProcessor = UIProcessManager.Get_ConsigneeUnit_By_Code(selected_Payment.PaymentProcessorConsigneeUnit.ToString());
                        if (paymentProcessor != null)
                        {
                            if (Full_Calculator(paymentProcessor.Id))
                            {
                                amount = Voucher_Buffer.Voucher.GrandTotal;
                                lblPaymentMethodPhone.Text = selected_Payment.name;
                                pbPhoneNoLogo.Image = tileItem.Elements[0].Image;
                                tcNavigation.SelectedTabPage = tpPhoneNo;
                                this.ActiveControl = txtPhoneNumber;
                                txtPhoneNumber.Text = "";
                                txtPIN.Text = "";
                            }
                            else
                            {
                                XtraMessageBox.Show("Failed To Recalculate Voucher!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            amount = Voucher_Buffer.Voucher.GrandTotal;
                            lblPaymentMethodPhone.Text = selected_Payment.name;
                            pbPhoneNoLogo.Image = tileItem.Elements[0].Image;
                            tcNavigation.SelectedTabPage = tpPhoneNo;
                            this.ActiveControl = txtPhoneNumber;
                            txtPhoneNumber.Text = "";
                            txtPIN.Text = "";
                        }
                    }                    
                }
            }
            catch { }
        }

        private async void btnPhoneNoOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                   // //Progress_Reporter.Show_Progress("Processing Payment...", "Please Wait...");
                    if (selected_Payment.is_Two_Step)
                    {
                        if (await PaymentClient.Authorize_Payment(txtPhoneNumber.Text, LocalBuffer.LocalBuffer.CompanyConsigneeData.Code, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code, transaction_Number, amount, selected_Payment))
                        {
                           // //Progress_Reporter.Close_Progress();
                            lblPaymentMethodPIN.Text = selected_Payment.name;
                            lblPhoneNoPIN.Text = txtPhoneNumber.Text;
                            pbTINLogo.Image = pbPhoneNoLogo.Image;
                            tcNavigation.SelectedTabPage = tpPIN;
                            this.ActiveControl = txtPIN;
                        }
                        else
                        {
                         //   //Progress_Reporter.Close_Progress();
                            //tcNavigation.SelectedTabPage = tpPaymentProvider;
                            return;
                        }
                    }
                    else
                    {
                        if (await Transact_Payment(null))
                        {
                           // //Progress_Reporter.Close_Progress();
                            form_Close_Timer.Start();
                        }
                       // else
                           // //Progress_Reporter.Close_Progress();
                    }
                }
                else
                    XtraMessageBox.Show("Please Enter Phone Number!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
               // //Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Payment Failed!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnPINOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPIN.Text))
                {
                    //Progress_Reporter.Show_Progress("Processing Payment...", "Please Wait...");
                    if (await Transact_Payment(txtPIN.Text))
                    {
                        //Progress_Reporter.Close_Progress();
                        form_Close_Timer.Start();
                    }
                    else
                    {
                        txtPIN.Text = "";
                        //Progress_Reporter.Close_Progress();
                    }
                }
                else
                    XtraMessageBox.Show("Please Enter PIN!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                //Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Payment Failed!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Mobile_Transaction = null;
                payment_Successful = false;
                this.Close();
            }
            catch { }
        }

        private void tcNavigation_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
           
        }

        private void txtPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnPhoneNoOk.PerformClick();
            }
            catch { }
        }

        private void txtPIN_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                 if (e.KeyCode == Keys.Enter)
                     btnPINOK.PerformClick();
            }
            catch { }
        }

        private async void btnMpesaOK_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    var unApproved_Trsx = gvUnapprovedPayments.GetFocusedRow() as UnapprovedPayment;
            //    if (unApproved_Trsx != null)
            //    {
            //        if (amount > (decimal)unApproved_Trsx.Amount)
            //        {
            //            XtraMessageBox.Show("Selected Transaction Is Less Than Receipt Total!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            return;
            //        }
            //        //Progress_Reporter.Show_Progress("Processing Payment...", "Please Wait...");
            //        var resp = await PaymentClient.Approve_Unapproved_Payment(Buffers.Company.Tin, Buffers.Machine_Consginee_Unit, selected_Payment.provider_OUD, transaction_Number, amount, unApproved_Trsx.Id);

            //        if (resp != null && resp.IsFulfilled)
            //        {
            //            Mobile_Transaction = await PaymentClient.Get_Trsx(resp, transaction_Number, amount);

            //            payment_Successful = true;
            //            pbPaymentSuccessful.Image = CNET.Mobile.Payments.Properties.Resources.PyamentConfirmed;
            //            tcNavigation.SelectedTabPage = tpPaymentSuccess;
            //            tcNavigation.Update();
            //            //return true;

            //            //Progress_Reporter.Close_Progress();
            //            //payment_Successful = true;
            //            //pbPaymentSuccessful.Image = CNET_Mobile_Payments.Properties.Resources.PyamentConfirmed;
            //            //tcNavigation.SelectedTabPage = tpPaymentSuccess;
            //            //tcNavigation.Update();
            //            form_Close_Timer.Start();
            //        }
            //        else
            //            //Progress_Reporter.Close_Progress();
            //    }
            //    else
            //        XtraMessageBox.Show("Please Select Transaction!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //catch { }
        }

        #endregion


        #region Methods
        private void Initialize_Payment_Providers(List<PaymentProvider> payment_Providers)
        {
            try
            {
                if (payment_Providers == null || payment_Providers.Count == 0)
                    return;
                foreach (var provider in payment_Providers)
                {
                    var tile = new TileItem();
                    tile.Name = "tile_" + provider.name;
                    tile.ItemSize = TileItemSize.Medium;
                    tile.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(94)))), ((int)(((byte)(132)))));
                    tile.AppearanceItem.Normal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(94)))), ((int)(((byte)(132)))));
                    tile.AppearanceItem.Normal.Font = new System.Drawing.Font("Microsoft New Tai Lue", 10F);
                    tile.AppearanceItem.Normal.ForeColor = System.Drawing.Color.White;
                    tile.AppearanceItem.Normal.Options.UseBackColor = true;
                    tile.AppearanceItem.Normal.Options.UseFont = true;
                    tile.AppearanceItem.Normal.Options.UseForeColor = true;
                    tile.Elements.Add(new TileItemElement
                    {
                        Text = provider.name,
                        TextAlignment = TileItemContentAlignment.MiddleCenter,
                        Image = Get_Image_From_Url(provider.image_URL),
                        ImageToTextAlignment = TileControlImageToTextAlignment.Top,
                        ImageScaleMode = TileItemImageScaleMode.Squeeze
                    });
                    tile.Tag = provider;
                    tile.ItemClick += tile_ItemClick;
                    this.tigPaymentMethods.Items.Add(tile);
                }
            }
            catch { }
        }

        private Image Get_Image_From_Url(string url)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var imageArray = webClient.DownloadData(url);
                    using (MemoryStream stream = new MemoryStream(imageArray))
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch { return null; }
        }

        public bool Check_Mobile_Payment(VoucherBuffer Voucher_Buffer, string voucher_No, int? ConsigneeID, bool AllowDiscount, bool AllowAdditionalCharge)
        {
            try
            {
                this.Voucher_Buffer = Voucher_Buffer;
                this.AllowAdditionalCharge = AllowAdditionalCharge;
                this.AllowDiscount = AllowDiscount;
                payment_Successful = false;
                if (Available_Payment_Types == null || Available_Payment_Types.Count == 0)
                {
                    return false;
                }
                if (LocalBuffer.LocalBuffer.CompanyConsigneeData == null || string.IsNullOrEmpty(LocalBuffer.LocalBuffer.CompanyConsigneeData.Code) || !int.TryParse(LocalBuffer.LocalBuffer.CompanyConsigneeData.Code, out _))
                {
                    XtraMessageBox.Show("Invalid Comapny!\nPlease Check Your Comapny Setting.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData == null || string.IsNullOrEmpty(LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code) || !int.TryParse(LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code, out _))
                {
                    XtraMessageBox.Show("Invalid Device Branch!\nPlease Check Your Device Consignee Unit.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (PaymentClient.Customer_Maintain_Activity_Defn == null)
                {
                    XtraMessageBox.Show("Please Define Work Flow To Maintain Person Customer!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (PaymentClient.consignee_Customer_Pref == null)
                {
                    XtraMessageBox.Show("Please Save Customer Consignee Preference!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                transaction_Number = voucher_No;
                amount = Voucher_Buffer.Voucher.GrandTotal;
                selected_Consignee_ID = ConsigneeID;
                txtPhoneNumber.Text = "";
                txtPIN.Text = "";
                tcNavigation.SelectedTabPage = tpPaymentProvider;
                return true;
            }
            catch { return false; }
        }

        private void Finalize_Payment(PaymentProvider provider, string provider_Customer_Name, string payment_Reference)
        {
            try
            {
                PaymentClient.current_Time = UIProcessManager.GetServiceTime().Value;
                Mobile_Transaction = new MobilePaymentDTO
                {
                    PaymentReference = payment_Reference,
                    PaymentProcessorConsigneeId = provider.PaymentProcessorConsigneeId,
                    PaymentProcessorConsigneeUnit = provider.PaymentProcessorConsigneeUnit,
                    PaymenetProcessorName = provider.name,
                    IssueDate = PaymentClient.current_Time,
                    MaturityDate = PaymentClient.current_Time
                };

                if (!string.IsNullOrEmpty(provider_Customer_Name))
                {
                    Mobile_Transaction.ConsigneeID = PaymentClient.Save_Customer(txtPhoneNumber.Text, selected_Payment.name, provider_Customer_Name);

                    if (Mobile_Transaction.ConsigneeID != null)
                    {
                        if (selected_Consignee_ID == null)
                        {
                            POS_Settings.Selected_Consignee = new POS.Common.Models.Consignee
                            {
                                ID = Mobile_Transaction.ConsigneeID.Value,
                                Code = txtPhoneNumber.Text,
                                Name = provider_Customer_Name,
                                IsPerson = true
                            };
                        }
                    }
                }
                else
                {
                    Mobile_Transaction.ConsigneeID = PaymentClient.Save_Customer(txtPhoneNumber.Text, selected_Payment.name, null);
                    if (Mobile_Transaction.ConsigneeID != null)
                    {
                        if (selected_Consignee_ID == null)
                        {
                            POS_Settings.Selected_Consignee = new POS.Common.Models.Consignee
                            {
                                ID = Mobile_Transaction.ConsigneeID.Value,
                                Code = txtPhoneNumber.Text,
                                Name = selected_Payment.name + "User(" + txtPhoneNumber.Text + ")",
                                IsPerson = true
                            };
                        }
                    }
                }
            }
            catch { }
        }

        private async Task<bool> Transact_Payment(string PIN)
        {
            try
            {
                if (selected_Payment.is_Synchronous)
                {
                    var result = await PaymentClient.Transact_Payment(txtPhoneNumber.Text, LocalBuffer.LocalBuffer.CompanyConsigneeData.Code, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code, transaction_Number, amount, PIN, selected_Payment);
                    if (result != null && result.IsSuccessful)
                    {
                        Finalize_Payment(selected_Payment, result.AdditionalParameters["CustomerName"], result.TransactionReference);
                        payment_Successful = true;
                        pbPaymentSuccessful.Image = FrontOffice_V._7.Properties.Resources.PyamentConfirmed;
                        tcNavigation.SelectedTabPage = tpPaymentSuccess;
                        tcNavigation.Update();
                        return true;
                    }
                    else
                    {
                        //tcNavigation.SelectedTabPage = tpPaymentProvider;
                        return false;
                    }
                }
                else
                {
                    var result = await PaymentClient.Transact_Payment(txtPhoneNumber.Text, LocalBuffer.LocalBuffer.CompanyConsigneeData.Code, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code, transaction_Number, amount, PIN, selected_Payment);
                    if (result != null && result.IsSuccessful)
                    {
                        var status = await PaymentClient.Check_Status(LocalBuffer.LocalBuffer.CompanyConsigneeData.Code, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnitData.Code, transaction_Number);
                        if (status.Item1)
                        {
                            Finalize_Payment(selected_Payment, result.AdditionalParameters["CustomerName"], status.Item2);
                            payment_Successful = true;
                            pbPaymentSuccessful.Image = FrontOffice_V._7.Properties.Resources.PyamentConfirmed;
                            tcNavigation.SelectedTabPage = tpPaymentSuccess;
                            tcNavigation.Update();
                            return true;
                        }
                        else
                        {
                            XtraMessageBox.Show("Payment Failed!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //tcNavigation.SelectedTabPage = tpPaymentProvider;
                            return false;
                        }
                    }
                    else
                    {
                        //tcNavigation.SelectedTabPage = tpPaymentProvider;
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show("Payment Failed!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //tcNavigation.SelectedTabPage = tpPaymentProvider;
                return false;
            }
        }

        private bool Full_Calculator(int PaymentProcessorConsigneeUnit)
        {
            try
            {
                var processorDiscount = UIProcessManager.Get_Value_Factor_Definition_By_Reference_And_Type(PaymentProcessorConsigneeUnit, CNETConstantes.Discount);
                if (processorDiscount == null || processorDiscount.Count == 0)
                    return true;


              


                item_Details = new List<LineItemDetails>();
                foreach (var lineItemDetail in Voucher_Buffer.LineItemsBuffer)
                {
                    item_Detail = new LineItemDetails()
                    {
                        lineItems = lineItemDetail.LineItem,
                        lineItemValueFactor = lineItemDetail.LineItemValueFactors
                    };
                    LineItemDetailPMS calculated_Line_Item = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = Voucher_Buffer.Voucher }, item_Detail.lineItems, Voucher_Buffer.Voucher.Id, processorDiscount == null ? null : processorDiscount.FirstOrDefault().Id,  null, null, null, true, false, false, false);
                   
                    if (calculated_Line_Item != null)
                    {
                        item_Detail = new LineItemDetails()
                        {
                            lineItems = calculated_Line_Item.lineItem,
                            lineItemValueFactor = calculated_Line_Item.lineItemValueFactor
                        };
                        item_Details.Add(item_Detail);
                    }
                    else
                        return false;
                }

                calculated_Voucher = new VoucherFinalCalculator().VoucherCalculation(Voucher_Buffer.Voucher,  item_Details);
                if (calculated_Voucher != null)
                {
                    Voucher_Buffer.LineItemsBuffer = new List<LineItemBuffer>();
                    foreach(LineItemDetails li in item_Details)
                    {
                        LineItemBuffer lineItemBuffer = new LineItemBuffer()
                        {
                            LineItem = li.lineItems,
                            LineItemValueFactors = li.lineItemValueFactor
                        };
                        Voucher_Buffer.LineItemsBuffer.Add(lineItemBuffer);
                    } 
                    Voucher_Buffer.Voucher.SubTotal = calculated_Voucher.voucher.SubTotal;
                    Voucher_Buffer.Voucher.AddCharge = calculated_Voucher.voucher.AddCharge;
                    Voucher_Buffer.Voucher.Discount = calculated_Voucher.voucher.Discount;
                    Voucher_Buffer.Voucher.GrandTotal = calculated_Voucher.voucher.GrandTotal;
                    Voucher_Buffer.TaxTransactions = calculated_Voucher.taxTransactions;

                    return true;
                }
                return false;
            }
            catch { return false; }
            finally
            {
                item_Details = null;
                item_Detail = null;
                calculated_Line_Item = null;
                calculated_Voucher = null;
            }
        }

        #endregion


    }
}