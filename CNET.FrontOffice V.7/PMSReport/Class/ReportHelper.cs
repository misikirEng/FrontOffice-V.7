using CNET.ERP.Client.Common.UI;
using CNET.ERP2016.ServiceInterfaces.Types;
using CNET.ERP2016.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMSReport
{
    class ReportHelper
    {
        private const int DAILY_RES_SUM_THREADS = 10;

        private ResdenceSummaryThreadHandler[] rsThreadHandlres = new ResdenceSummaryThreadHandler[DAILY_RES_SUM_THREADS];
        private ManualResetEvent[] doneEvents = new ManualResetEvent[DAILY_RES_SUM_THREADS];
        

        public List<DailyResidentSummaryColumn> GetDailyResidSummCol(List<RegistrationDocumentDTO> regDocViewList, DateTime date)
        {
            
            List<DailyResidentSummaryColumn> dailyResSumCols = new List<DailyResidentSummaryColumn>();
            foreach (var regDoc in regDocViewList)
            {
                if (regDoc != null && !(dailyResSumCols.Any(drs => drs.Reg_No == regDoc.code)))
                {
                    dailyResSumCols.Add( PopulateDailyResSumCol(regDoc, date));
                  
                    

                }
            }


            return dailyResSumCols;
        }

        private DailyResidentSummaryColumn PopulateDailyResSumCol(RegistrationDocumentDTO registration, DateTime date)
        {
            DailyResidentSummaryColumn drsCol = new DailyResidentSummaryColumn();

            for (int i = 0; i < DAILY_RES_SUM_THREADS; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                ResSumArg resArg = new ResSumArg()
                {
                    Date =date,
                    Idex = i,
                    RegistrationDocView = registration
                };
                ResdenceSummaryThreadHandler rsT = new ResdenceSummaryThreadHandler(doneEvents[i], resArg);

                rsThreadHandlres[i] = rsT;
                ThreadPool.QueueUserWorkItem(rsT.ThreadPoolCallback, i);

            }

            foreach (var t in doneEvents)
            {
                t.WaitOne();
            }
           // WaitHandle.WaitAll(doneEvents);
            //clear done events
            //clear thread handler

            for (int i = 0; i < DAILY_RES_SUM_THREADS; i++)
            {
                ResdenceSummaryThreadHandler rth = rsThreadHandlres[i];
                switch (i)
                {
                    case 0:
                        //drsCol.RateCode = rth.Result;
                        break;

                    case 1:
                        drsCol.RoomRevenue = rth.Result;
                        break;

                    case 2:
                        drsCol.Package = rth.Result;
                        break;

                    case 3:

                        drsCol.ServiceCharge = rth.Result;


                        break;

                    case 4:
                        drsCol.Payment = rth.Result;

                        break;

                    case 5:
                        drsCol.Discount = rth.Result;

                        break;

                    case 6:
                        drsCol.Paidout = rth.Result;

                        break;

                    case 7:
                        drsCol.VAT = rth.Result;

                        break;

                    case 8:
                        drsCol.POSCharge = rth.Result;
                        break;

                    case 9:
                        drsCol.BBF = rth.Result;

                        break;
                }
            }
            drsCol.RoomTotal = String.Format("{0:N}", (Convert.ToDecimal(drsCol.RoomRevenue) + Convert.ToDecimal(drsCol.Package) + Convert.ToDecimal(drsCol.ServiceCharge) + Convert.ToDecimal(drsCol.VAT)));

            drsCol.TodayTotal = String.Format("{0:N}", (Convert.ToDecimal(drsCol.RoomTotal) + Convert.ToDecimal(drsCol.POSCharge)));
            drsCol.toDateTotal = String.Format("{0:N}", (Convert.ToDecimal(drsCol.TodayTotal) + Convert.ToDecimal(drsCol.BBF)));

            drsCol.Reg_No = registration.code;
            drsCol.Guest = registration.name;
            drsCol.Company = String.Format("{0}", registration.tradeName);
            drsCol.Room = registration.RoomNumber;

            var currency = Convert.ToDecimal(drsCol.toDateTotal) - Convert.ToDecimal(drsCol.Payment) - Convert.ToDecimal(drsCol.Discount) - Convert.ToDecimal(drsCol.Paidout);
            if (registration.foStatus == CNETConstantes.CHECKED_OUT_STATE)
            {
                drsCol.BCF = String.Format("{0:N}", 0);
                drsCol.Outstanding = String.Format("{0:N}", currency);
            }
            else
            {
                drsCol.BCF = String.Format("{0:N}", currency);
                drsCol.Outstanding = String.Format("{0:N}", 0);
            }

            return drsCol;
        }
    }

    

    class ResdenceSummaryThreadHandler
    {
        private ManualResetEvent _doneEvent;

        public String Result { get; set; }
        public ResSumArg ResSumArgument { get; set; }

        public ResdenceSummaryThreadHandler(ManualResetEvent doneEvent, ResSumArg arg)
        {
            ResSumArgument = arg;
            _doneEvent = doneEvent;
        }

        private string GetTotalVat(string reg, DateTime date)
        {
            string vat = "";
            try
            {
                decimal totalVat = 0;
                decimal dailVAT = PMSUIProcessManager.GetDailyVat(reg, date, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                decimal rebateVAT = PMSUIProcessManager.GetDailyVat(reg, date, CNETConstantes.CREDIT_NOTE_VOUCHER);
                totalVat = dailVAT - rebateVAT;
                return string.Format("{0:N}", totalVat);
            }
            catch (Exception ex)
            {
                return vat;
            }
        }

        private decimal DailyResidentSummaryPrevBalance(string voucher, DateTime date, string branch)
        {
            decimal bffb = 0;
            DateTime yesterday = date.AddDays(-1);
            var bcf = PMSUIProcessManager.GetDailyResidentSummaryPrevBalance(voucher, yesterday, branch).FirstOrDefault();
            if (bcf == null)
            {
                bffb = Math.Round(PMSUIProcessManager.GetDailyPrevBalance(voucher, date, CNETConstantes.CASHRECIPT), 2);
            }
            else
            {
                bffb = bcf.bcf;
            }
            return bffb;
        }

        public void ThreadPoolCallback(Object argument)
        {

            if (ResSumArgument != null)
            {
                RegistrationDocumentDTO registration = ResSumArgument.RegistrationDocView;
                DateTime date = ResSumArgument.Date;
                switch (ResSumArgument.Idex)
                {
                    case 0:
                        Result = PMSUIProcessManager.SelectRateCode(registration.rateCodeHeader).description;
                        break;

                    case 1:
                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyRoomRevenue(registration.code,
                            date, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, CNETConstantes.ACCOMODATION_PREFERENCE_CODE));
                        break;

                    case 2:
                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyPackageAmount(registration.code, date,
                            CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, CNETConstantes.ACCOMODATION_PREFERENCE_CODE));
                        break;

                    case 3:

                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyServiceCharge(registration.code, date,
                            CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER));


                        break;

                    case 4:
                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyAmount(registration.code, CNETConstantes.CASHRECIPT, date));

                        break;

                    case 5:
                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyAmount(registration.code, CNETConstantes.CREDIT_NOTE_VOUCHER, date));

                        break;

                    case 6:
                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyAmount(registration.code, CNETConstantes.PAID_OUT_VOUCHER, date));

                        break;

                    case 7:
                        Result = GetTotalVat(registration.code, date); 

                        break;

                    case 8:
                        Result = String.Format("{0:N}", PMSUIProcessManager.GetDailyPOSCharges(registration.code, date, CNETConstantes.CREDITSALES,
                            CNETConstantes.CASH_SALES));
                        break;

                    case 9:
                        Result = String.Format("{0:N}", DailyResidentSummaryPrevBalance(registration.code, date,""));
                        break;

                }

            }

            _doneEvent.Set();
        }
    }

    class ResSumArg
    {
        public RegistrationDocumentDTO RegistrationDocView { get; set; }
        public int Idex { get; set; }
        public DateTime Date { get; set; }
    }
}
