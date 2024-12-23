using CNET_V7_Domain.Domain.TransactionSchema;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.Mobile.Payments
{
    public partial class frmPaymentProgress : XtraForm
    {
        private System.Timers.Timer status_Timer = new System.Timers.Timer();

        private string consigneeCode { get; set; }
        private string consigneeUnitCode { get; set; }
        private string voucher_Code { get; set; }
        private DateTime issued_Date { get; set; }
        private bool is_Completed { get; set; }
        private bool is_Canceled { get; set; }

      
        public frmPaymentProgress(int voucherdef)
        {
            InitializeComponent();
            PaymentClient.Initalize_Mobile_Payment(voucherdef);
            status_Timer.Elapsed += status_Timer_Elapsed;
            status_Timer.Interval = 2000;
            status_Timer.AutoReset = false;
        }

        public void Start_Payment_Processing(string consigneeCode, string consigneeUnitCode, string voucher_Code, DateTime issued_Date)
        {
            try
            {
                is_Canceled = false;
                is_Completed = false;
                PaymentClient.payment_Trsx = null;

                if (PaymentClient.Customer_Maintain_Activity_Defn == null)
                {
                    XtraMessageBox.Show("Please Define Work Flow To Maintain Person Customer!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (PaymentClient.consignee_Customer_Pref == null)
                {
                    XtraMessageBox.Show("Please Save Customer Consignee Preference!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.consigneeCode = consigneeCode;
                this.consigneeUnitCode = consigneeUnitCode;
                this.voucher_Code = voucher_Code;
                this.issued_Date = issued_Date;

                tcNavigation.SelectedTabPage = tpProgress;
                this.ShowDialog();
            }
            catch { }
        }

        private void frmPaymentProgress_Load(object sender, EventArgs e)
        {
            status_Timer.Start();  
        }

        private async void status_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (is_Canceled)
                    return;

                if (is_Completed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
                else
                {
                    var res = await PaymentClient.Check_Payment(consigneeCode, consigneeUnitCode, voucher_Code, issued_Date);

                    if (res != null)
                    {
                        if (res.IsResolved)
                        {
                            if (res.IsFulfilled)
                            {
                                PaymentClient.payment_Trsx = PaymentClient.Get_Trsx(res);
                                Show_Notification();
                            }
                            else
                            {
                                XtraMessageBox.Show("Payment Failed!\n" + res.Remark, "CNET_ERP2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                status_Timer.Stop();
                                PaymentClient.payment_Trsx = null;
                                is_Canceled = true;
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.Close();
                                });
                            }
                           
                        }
                        else
                            status_Timer.Start();
                    }
                }
            }
            catch { }
        }
       
        private void Show_Notification()
        {
            try
            {
                tcNavigation.Invoke((MethodInvoker)delegate
                {
                    tcNavigation.SelectedTabPage = tpNotification;
                    tcNavigation.Update();
                });              
                is_Completed = true;
                status_Timer.Start();
            }
            catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                status_Timer.Stop();
                PaymentClient.payment_Trsx = null;
                is_Canceled = true;
                this.Close();
            }
            catch { }
        }

       
    }
}
