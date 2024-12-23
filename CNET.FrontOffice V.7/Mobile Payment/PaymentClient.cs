using CNET.Mobile.Payments.Models;
using CNET.POS.Settings;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using ProcessManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.Mobile.Payments
{
    public class PaymentClient
    {
        private static CNET.Payment.SDK.V7.CnetPaymentHandlerV7 handler = null;

        private static CNET.Payment.SDK.V7.CnetPaymentHandlerV7 bill_Settlement_handler = null;

        private static CNET.Payment.SDK.V7.CnetPaymentHandlerV7 payment_Status_handler = null;

        private static frmPaymentProgress progress_Reporter = null;

        private static bool Initalized = false;

        internal static ActivityDefinitionDTO Customer_Maintain_Activity_Defn;
        internal static int? reqObjectStateDefnConsignee { get; set; }
        internal static PreferenceDTO consignee_Customer_Pref { get; set; }
        private static ConsigneeBuffer consignee { get; set; }
        internal static DateTime current_Time { get; set; }
        internal static MobilePaymentDTO payment_Trsx { get; set; }


        internal static async Task<List<PaymentProvider>> Get_Payment_Options(string consigneeCode, string consigneeUnitCode)
        {
            try
            {
                handler = new Payment.SDK.V7.CnetPaymentHandlerV7();
                var results = await handler.GetPaymentOptionsAsync(new Payment.SDK.V7.PaymentOptionsRequest
                {
                    SupplierConsigneeId = int.Parse(consigneeCode),
                    SupplierConsigneeUnit = int.Parse(consigneeUnitCode)
                });

                if (results != null && results.Count() > 0)
                {
                    var payment_Options = new List<PaymentProvider>();
                    foreach (var res in results.Where(x => x.PaymentProcessorSpecialization != CNETConstantes.Payment_Method_Specialization_QR))
                    {
                        payment_Options.Add(new PaymentProvider
                        {
                            name = res.PaymentProcessorUnitName,
                            image_URL = res.PaymentProcessorUnitImage,
                            PaymentProcessorConsigneeId = res.PaymentProcessorConsigneeId,
                            PaymentProcessorConsigneeUnit = res.PaymentProcessorConsigneeUnit,
                            is_Synchronous = res.OperationMode.HasFlag(Payment.SDK.V7.PaymentOperationMode.Synchronous),
                            is_Two_Step = res.OperationMode.HasFlag(Payment.SDK.V7.PaymentOperationMode.AuthorizationTransaction),
                            is_Customer_Init = res.OperationMode.HasFlag(Payment.SDK.V7.PaymentOperationMode.CustomerInitiated_CustomerSelect)
                        });
                    }

                    return payment_Options;
                    //var paymentOption = res.First();
                    //var isSynchronous = paymentOption.OperationMode.HasFlag(PaymentOperationMode.Synchronous);
                    //var isTwoStep = paymentOption.OperationMode.HasFlag(PaymentOperationMode.AuthorizationTransaction);
                    //var isAsynchronous = paymentOption.OperationMode.HasFlag(PaymentOperationMode.Asynchronous);
                    //var isTransaction = paymentOption.OperationMode.HasFlag(PaymentOperationMode.Transaction);
                }
                else
                {
                    XtraMessageBox.Show("Failed To Get Available Payment Options!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    XtraMessageBox.Show("Failed To Get Available Payment Options!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    XtraMessageBox.Show("Failed To Get Available Payment Options!\n" + ex.InnerException.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }

        internal static async Task<bool> Authorize_Payment(string phone_Num, string consigneeCode, string consigneeUnitCode, string voucher_Code, decimal amount, PaymentProvider selectedProvider)
        {
            try
            {
                var res = await handler.AuthorizePaymentAsync(new Payment.SDK.V7.PaymentRequest
                {
                    UserMobileNumber = phone_Num,
                    SupplierConsigneeId = int.Parse(consigneeCode),
                    SupplierConsigneeUnit = int.Parse(consigneeUnitCode),
                    TransactionId = voucher_Code,
                    PaymentProcessorConsigneeId = selectedProvider.PaymentProcessorConsigneeId,
                    PaymentProcessorConsigneeUnit = selectedProvider.PaymentProcessorConsigneeUnit,
                    Amount = amount
                });

                if (res != null && res.IsSuccessful)
                {
                    return true;
                }

                else if (res != null && !res.IsSuccessful && res.ErrorMessages != null && res.ErrorMessages.Count > 0)
                {
                    XtraMessageBox.Show("Payment Failed!\n" + res.ErrorMessages.FirstOrDefault(), "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    XtraMessageBox.Show("Payment Failed!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Payment Failed!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        internal static async Task<Payment.SDK.V7.PaymentResponse> Transact_Payment(string phone_Num, string consigneeCode, string consigneeUnitCode, string voucher_Code, decimal amount, string PIN, PaymentProvider selectedProvider)
        {
            try
            {
                var res = await handler.TransactPaymentAsync(new Payment.SDK.V7.PaymentRequest
                {
                    UserMobileNumber = phone_Num,
                    SupplierConsigneeId = int.Parse(consigneeCode),
                    SupplierConsigneeUnit = int.Parse(consigneeUnitCode),
                    TransactionId = voucher_Code,
                    PaymentProcessorConsigneeId = selectedProvider.PaymentProcessorConsigneeId,
                    PaymentProcessorConsigneeUnit = selectedProvider.PaymentProcessorConsigneeUnit,
                    Amount = amount,
                    Pin = PIN
                });

                if (res != null && res.IsSuccessful)
                {
                    return res;
                }

                else if (res != null && !res.IsSuccessful && res.ErrorMessages != null && res.ErrorMessages.Count > 0)
                {
                    XtraMessageBox.Show("Payment Failed!\n" + res.ErrorMessages.FirstOrDefault(), "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                else
                {
                    XtraMessageBox.Show("Payment Failed!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Payment Failed!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        internal static async Task<Tuple<bool,string>> Check_Status(string consigneeCode, string consigneeUnitCode, string voucher_Code)
        {
            try
            {
                var res = await handler.GetPaymentResolutionStatusAsync(new Payment.SDK.V7.PaymentResolutionStatusRequest
                {
                    SupplierConsigneeId = int.Parse(consigneeCode),
                    SupplierConsigneeUnit = int.Parse(consigneeUnitCode),
                    TransactionId = voucher_Code
                });

                if (res != null)
                {
                    if (!res.IsResolved)
                    {
                        Thread.Sleep(8000);
                        for (int i = 0; i < 26; i++)
                        {
                            Thread.Sleep(2000);
                            var res1 = await handler.GetPaymentResolutionStatusAsync(new Payment.SDK.V7.PaymentResolutionStatusRequest
                            {
                                SupplierConsigneeId = int.Parse(consigneeCode),
                                SupplierConsigneeUnit = int.Parse(consigneeUnitCode),
                                TransactionId = voucher_Code
                            });
                            if (res1 != null && res1.IsResolved)
                                return Tuple.Create(res1.IsFulfilled, res1.PaymentProcessorTransactionRef);
                        }
                        return Tuple.Create(false, "");
                    }
                    else
                        return Tuple.Create(res.IsFulfilled, res.PaymentProcessorTransactionRef);
                }
                else return Tuple.Create(false, "");
            }
            catch { return Tuple.Create(false, ""); }
        }

        public static async Task<MobilePaymentDTO> Check_Bill_Status(string consigneeCode, string consigneeUnitCode, string voucher_Code, DateTime issued_Date)
        {
            try
            {
                if (bill_Settlement_handler == null)
                    bill_Settlement_handler = new Payment.SDK.V7.CnetPaymentHandlerV7();
                var res = await bill_Settlement_handler.GetPaymentResolutionStatusAsync(new Payment.SDK.V7.PaymentResolutionStatusRequest
                {
                    SupplierConsigneeId = int.Parse(consigneeCode),
                    SupplierConsigneeUnit = int.Parse(consigneeUnitCode),
                    TransactionId = voucher_Code
                });

                if (res != null && res.IsResolved && res.IsFulfilled)
                {
                    var mobileNumber = res.AdditionalParameters["CustomerMobileNumber"];
                    if (!string.IsNullOrEmpty(mobileNumber))
                    {
                        var mobileTrsx = new MobilePaymentDTO
                        {
                            IssueDate = res.TransactionTime,
                            MaturityDate = res.TransactionTime,
                            PaymentReference = res.PaymentProcessorTransactionRef,
                            PaymentProcessorConsigneeId = res.PaymentProcessorConsigneeId,
                            PaymentProcessorConsigneeUnit = res.PaymentProcessorConsigneeUnit,
                            PaymenetProcessorName = res.PaymenetProcessorName
                        };

                        var fullName = res.AdditionalParameters["CustomerName"];
                        if (!string.IsNullOrEmpty(fullName))
                            mobileTrsx.ConsigneeID = Save_Customer(mobileNumber, res.PaymenetProcessorName, fullName);
                        else
                            mobileTrsx.ConsigneeID = Save_Customer(mobileNumber, res.PaymenetProcessorName, null);

                        return mobileTrsx;
                    }
                    else
                    {
                        XtraMessageBox.Show("Bill Is Settled Online But Mobile Number Not Found!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                else if (res != null && !string.IsNullOrEmpty(res.Remark))
                {
                    XtraMessageBox.Show("Bill Is Not Settled Online!\n" + res.Remark, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                else
                {
                    XtraMessageBox.Show("Bill Is Not Settled Online!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch { return null; }
        }

        public static async Task<Payment.SDK.V7.PaymentResolutionStatus> Check_Payment(string consigneeCode, string consigneeUnitCode, string voucher_Code, DateTime issued_Date)
        {
            try
            {
                if (payment_Status_handler == null)
                    payment_Status_handler = new Payment.SDK.V7.CnetPaymentHandlerV7();

                return await payment_Status_handler.GetPaymentResolutionStatusAsync(new Payment.SDK.V7.PaymentResolutionStatusRequest
                {
                    SupplierConsigneeId = int.Parse(consigneeCode),
                    SupplierConsigneeUnit = int.Parse(consigneeUnitCode),
                    TransactionId = voucher_Code
                });
            }
            catch { return null; }
        }

        public static string Get_QR_Supplier_Identifiers(ConsigneeDTO comapny, ConsigneeUnitDTO branch)
        {
            try
            {
                if (comapny == null || string.IsNullOrEmpty(comapny.Code) || !int.TryParse(comapny.Code, out _))
                {
                    XtraMessageBox.Show("Invalid Comapny!\nPlease Check Your Comapny Setting.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                if (branch == null || string.IsNullOrEmpty(branch.Code) || !int.TryParse(branch.Code, out _))
                {
                    XtraMessageBox.Show("Invalid Device Branch!\nPlease Check Your Device Consignee Unit.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                if (bill_Settlement_handler == null)
                    bill_Settlement_handler = new Payment.SDK.V7.CnetPaymentHandlerV7();

                return bill_Settlement_handler.GetQrCodeSupplierIdentifiers(new Payment.SDK.V7.PaymentOptionsRequest
                {
                    SupplierConsigneeId = int.Parse(comapny.Code),
                    SupplierConsigneeUnit = int.Parse(branch.Code),
                });
            }
            catch { return null; }
        }

        public static int? Save_Customer(string phone_No, string processor_Name, string provider_Customer_Name)
        {
            try
            {
                try
                {
                    var customer = UIProcessManager.Get_Consignee_By_Code(phone_No);
                    if (customer != null)
                        return customer.Id;

                    consignee = new ConsigneeBuffer
                    {
                        consignee = new ConsigneeDTO
                        {
                            Code = phone_No,
                            GslType = CNETConstantes.Consignee_Customer,
                            IsPerson = false,
                            FirstName = !string.IsNullOrEmpty(provider_Customer_Name) ? provider_Customer_Name : processor_Name + " User(" + phone_No + ")",
                            Preference = consignee_Customer_Pref.Id,
                            IsActive = true,
                            CreatedOn = current_Time,
                            LastModified = current_Time,
                            StartDate = current_Time,
                        },
                        consigneeUnits = new List<ConsigneeUnitDTO>
                        {
                            new ConsigneeUnitDTO
                            {
                                Name = "Main Consignee",
                                Type = CNETConstantes.Org_Unit_Type_Branch,
                                IsActive=true,
                                Phone1 = phone_No
                            }
                        },
                        Activity = new CNET_V7_Domain.Domain.CommonSchema.ActivityDTO
                        {
                            ActivityDefinition = Customer_Maintain_Activity_Defn.Id,
                            TimeStamp = current_Time,
                            //Period = "",
                            ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                            Device = LocalBuffer.LocalBuffer.CurrentDevice.Id,
                            Platform = "Desktop",
                            User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id,
                            Day = current_Time.Day,
                            Month = current_Time.Month,
                            Year = current_Time.Year
                        }
                    };
                    if(reqObjectStateDefnConsignee != null)
                    {
                        consignee.ObjectStates = new List<ObjectStateDTO>
                        {
                            new ObjectStateDTO
                            {
                                Pointer = CNETConstantes.Consignee_Customer,
                                ObjectStateDefinition = reqObjectStateDefnConsignee.Value
                            }
                        };
                    }

                    var consignee_ID = UIProcessManager.Save_Consignee_Buffer(consignee);
                    if (consignee_ID != null)
                        return consignee_ID.consignee.Id;
                    else
                    {
                        XtraMessageBox.Show("Failed To Save Telebirr Customer!\nPlease Contact System Administrator.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                catch { return null; }
            }
            catch { return null; }
        }

        internal static MobilePaymentDTO Get_Trsx(Payment.SDK.V7.PaymentResolutionStatus res)
        {
            try
            {
                current_Time = UIProcessManager.GetServiceTime().Value;
                var mobileNumber = res.AdditionalParameters["CustomerMobileNumber"];
                if (!string.IsNullOrEmpty(mobileNumber))
                {
                    var mobileTrsx = new MobilePaymentDTO
                    {
                        IssueDate = res.TransactionTime,
                        MaturityDate = res.TransactionTime,
                        PaymentReference = res.PaymentProcessorTransactionRef,
                        PaymentProcessorConsigneeId = res.PaymentProcessorConsigneeId,
                        PaymentProcessorConsigneeUnit = res.PaymentProcessorConsigneeUnit,
                        PaymenetProcessorName = res.PaymenetProcessorName
                    };

                    var fullName = res.AdditionalParameters["CustomerName"];
                    if (!string.IsNullOrEmpty(fullName))
                        mobileTrsx.ConsigneeID = Save_Customer(mobileNumber, res.PaymenetProcessorName, fullName);
                    else
                        mobileTrsx.ConsigneeID = Save_Customer(mobileNumber, res.PaymenetProcessorName, null);

                    if (mobileTrsx.ConsigneeID != null)
                    {
                        POS_Settings.Selected_Consignee = new POS.Common.Models.Consignee
                        {
                            ID = mobileTrsx.ConsigneeID.Value,
                            Code = mobileNumber,
                            Name = fullName,
                            IsPerson = true
                        };
                    }
                    return mobileTrsx;
                }
                else
                    return null;
            }
            catch { return null; }
        }

        public static void Check_Bill(string consigneeCode, string consigneeUnitCode, string voucher_Code, DateTime issued_Date, int voucherdef)
        {
            try
            {
                if (progress_Reporter == null)
                    progress_Reporter = new frmPaymentProgress(voucherdef);

                progress_Reporter.Start_Payment_Processing(consigneeCode, consigneeUnitCode, voucher_Code, issued_Date);
            }
            catch { }
        }

        public static void Initalize_Mobile_Payment(int Voucher_Definition)
        {
            try
            {
                if (!Initalized)
                {
                    var requiredGslDetail = UIProcessManager.Get_VwRequiredGslDTO_By_VoucherDefn_Type(Voucher_Definition, CNETConstantes.LK_Required_GSL_Consignee, CNETConstantes.Consignee_Customer);
                    if (requiredGslDetail != null)
                        reqObjectStateDefnConsignee = requiredGslDetail.ObjectState;

                   var Activity_Defn = UIProcessManager.GetActivityDefinitionBydescriptionandreference( CNETConstantes.LK_Activity_Maintained, CNETConstantes.Consignee_Customer);
                    Customer_Maintain_Activity_Defn = Activity_Defn != null ? Activity_Defn.FirstOrDefault() : null;

                   var consignee_Cus_Pref = UIProcessManager.Get_Preference_By_Reference(CNETConstantes.Consignee_Customer);

                    if (consignee_Cus_Pref != null && consignee_Cus_Pref.Count > 0)
                        consignee_Customer_Pref = consignee_Cus_Pref.FirstOrDefault();

                    Initalized = true;
                }
            }
            catch { }
        }


        //internal static async Task<List<UnapprovedPayment>> Get_Unapproved_Payments(string TIN, int OUD, string provider_OUD)
        //{
        //    try
        //    {
        //        return await handler.GetUnapprovedPaymentsAsync(new UnapprovedPaymentsRequest
        //        {
        //            SupplierTin = TIN,
        //            SupplierOUD = OUD.ToString(),
        //            PaymentProviderOUD = provider_OUD
        //        });
        //    }
        //    catch { return null; }
        //}
        //internal static async Task<PaymentResolutionStatus> Approve_Unapproved_Payment(string TIN, int OUD, string provider_OUD, string voucher_Code, decimal amount, string unApproved_Payment_ID)
        //{
        //    try
        //    {
        //        // Throws exception if something went wrong
        //        return await handler.ApproveCustomerPaymentAsync(new PaymentRequest
        //        {
        //            SupplierTin = TIN,
        //            SupplierOUD = OUD.ToString(),
        //            PaymentProviderOUD = provider_OUD,
        //            TransactionId = voucher_Code,
        //            Amount = amount,
        //            AdditionalParameters = new Dictionary<string, string>() { { "UnapprovedPaymentId", unApproved_Payment_ID } }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Payment Failed!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return null;
        //    }
        //}

    }
}
