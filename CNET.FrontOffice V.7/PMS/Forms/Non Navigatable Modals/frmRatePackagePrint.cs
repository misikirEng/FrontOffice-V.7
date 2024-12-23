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
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI;
using System.Net.Sockets;
using System.Net;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.PmsSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRatePackagePrint : DevExpress.XtraEditors.XtraForm
    {
        DeviceDTO OrderPrinter { get; set; }
        List<PackageDTO> PackageDTOList { get; set; }
        string GustCompanyName { get; set; }
        public static Socket mClientSocket { get; set; }
        string OrderPrinterIPParameter { get; set; }
        public static IPAddress mOrderPrinterIp;
        public static String xBrokenLine = "------------------------------------------------";
        public static String xSolidLine = "<______________________________________________>";
        public static String xStarLine = "************************************************";
        public static String xSqouteLine = "<''''''''''''''''''''''''''''''''''''''''''''''>";
        public static String xDotLine = "................................................";
        public static String xEqualLine = "===============================================";
        RegistrationListVMDTO CurrentRegistration { get; set; }
        public static IPEndPoint ipEnd = null;
        int pax = 1;
        DateTime? currentDate = DateTime.Now;

        public frmRatePackagePrint(RegistrationListVMDTO Registration, List<VwPackageToPostViewDTO> RatePackageList)
        {
            InitializeComponent();
            CurrentRegistration = Registration;
            GustCompanyName = Registration.Company;
            txtRegistrationcode.Text = Registration.Registration;
            txtGuestName.Text = Registration.Guest;
            txtRoomNumber.Text = Registration.Room;
            txtNumberofnight.Text = Registration.NumOfNight.ToString();
            pax = Registration.adult + Registration.child;
            txtNoofpax.Text = pax.ToString();
            txtFromDate.EditValue = Registration.Arrival;
            txtToDate.EditValue = Registration.Departure;
            currentDate = UIProcessManager.GetServiceTime();
            beiPrintIndividual.EditValue = false;

            Populatedata(RatePackageList);


            OrderPrinter = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.ORDERPRINTER);
            if (OrderPrinter == null)
            {
                btnPrint.Enabled = false;
            }
            else
            {
                if (!String.IsNullOrEmpty(OrderPrinter.IpAddress))
                {
                    OrderPrinterIPParameter = OrderPrinter.IpAddress;
                }
                else
                {
                    btnPrint.Enabled = false;
                }
            }
            if (pax > 1)
            {
                rpPrintIndividual.Visible = true;
            }
        }

        private void Populatedata(List<VwPackageToPostViewDTO> RatePackageList)
        {
            PackageDTOList = RatePackageList.Select(x => new PackageDTO
            {
                code = x.code,
                select = false,
                Package = x.description,
                date = x.Date,
                printed = string.IsNullOrEmpty(x.remark) ? false : x.remark.Contains("printed") ? true : false,
                consumed = string.IsNullOrEmpty(x.remark) ? false : x.remark.Contains("consumed") ? true : false,
                canprint = x.Date.Date < currentDate.Value.Date ? false : true
            }).ToList();
            gcPackageList.DataSource = PackageDTOList;
            gvPackageList.ExpandAllGroups();
        }

        private void gcPackageList_Click(object sender, EventArgs e)
        {

        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvPackageList.PostEditor();
            List<PackageDTO> DataList = (List<PackageDTO>)gcPackageList.DataSource;
            if (DataList != null && DataList.Count > 0)
            {
                DataList = DataList.Where(x => x.select == true).ToList();
                if (DataList != null && DataList.Count > 0)
                {
                    foreach (PackageDTO pack in DataList)
                    {
                        bool printed = false;
                        if (rpPrintIndividual.Visible == true && beiPrintIndividual.EditValue.ToString() == "True")
                        {
                            for (int i = 0; i < pax; i++)
                            {
                                printed = OrderOperation(OrderPrinterIPParameter, LocalBuffer.LocalBuffer.CompanyName, txtRegistrationcode.Text,
                                                                  txtNumberofnight.Text, "1", GustCompanyName, txtGuestName.Text, pack.date, txtFromDate.Text,
                                                                  txtToDate.Text, txtRoomNumber.Text, pack.Package, pack.code.ToString());

                                if (!printed)
                                {
                                    XtraMessageBox.Show("Printing Faild please check order Printer", "Erreor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            printed = OrderOperation(OrderPrinterIPParameter, LocalBuffer.LocalBuffer.CompanyName, txtRegistrationcode.Text,
                              txtNumberofnight.Text, txtNoofpax.Text, GustCompanyName, txtGuestName.Text, pack.date, txtFromDate.Text,
                              txtToDate.Text, txtRoomNumber.Text, pack.Package, pack.code.ToString());
                            if (!printed)
                            {
                                XtraMessageBox.Show("Printing Faild please check order Printer", "Erreor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                        if (printed)
                        {
                            PackagesToPostDTO packpost = UIProcessManager.GetPackagesToPostById(pack.code);
                            packpost.Remark = "printed";
                            UIProcessManager.UpdatePackagesToPost(packpost);
                        }

                    }
                    List<VwPackageToPostViewDTO> AllPackageList = UIProcessManager.GetPostingPackageToPostViewByRegistrationCode(CurrentRegistration.Id);
                    Populatedata(AllPackageList);
                }
                else
                {
                    XtraMessageBox.Show("There are no Selected packages", "Erreor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("There are no packages", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                ((List<PackageDTO>)gcPackageList.DataSource).Where(x => x.canprint == true && x.printed == false).ToList().ForEach(x => x.select = true);
                gcPackageList.Refresh();
                gcPackageList.RefreshDataSource();

            }
            else
            {
                ((List<PackageDTO>)gcPackageList.DataSource).ForEach(x => x.select = false);
                gcPackageList.Refresh();
                gcPackageList.RefreshDataSource();
            }
        }
        private void GvPackageList_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                PackageDTO Rowdata = (PackageDTO)gvPackageList.GetRow(e.RowHandle);
                if (Rowdata.canprint == false || Rowdata.printed == true || Rowdata.consumed == true)
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.Red;
                    e.Appearance.ForeColor = Color.Black;

                }
                else
                {
                    e.Appearance.BackColor = Color.LightGray;
                    e.Appearance.BackColor2 = Color.LightGray;
                    e.Appearance.ForeColor = Color.Black;
                }


            }
        }
        private void gvPackageList_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            //if (e.RowHandle >= 0)
            //{
            //    PackageDTO Rowdata = (PackageDTO)gvPackageList.GetRow(e.RowHandle);
            //    if (Rowdata.canprint == false || Rowdata.printed == true || Rowdata.consumed == true)
            //    {
            //        e.Appearance.BackColor = Color.Red;
            //        e.Appearance.BackColor2 = Color.Red;
            //        e.Appearance.ForeColor = Color.Black;

            //    }
            //    else
            //    {
            //        e.Appearance.BackColor = Color.LightGray;
            //        e.Appearance.BackColor2 = Color.LightGray;
            //        e.Appearance.ForeColor = Color.Black;
            //    }


            //}
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }



        public static bool OrderOperation(string ip, string OrganizationName, string RegistrationNo,
            string NoofNights, string NumberofPeople, string CompanyName, string GuestName,
            DateTime Date, string ArrivalDate, string DepartureDate, string RoomNumber,
            string PackageName, string barcodestring)
        {

            try
            {
                mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mOrderPrinterIp = IPAddress.Parse(ip);
                ipEnd = new IPEndPoint(mOrderPrinterIp, 9100);
                if (!mClientSocket.Connected)
                {
                    mClientSocket.Connect(ipEnd);
                }

                mClientSocket.NoDelay = true;
                mClientSocket.SendTimeout = 2000;
                mClientSocket.ReceiveTimeout = 2000;

                WakeUp();
                WriteLine(xBrokenLine);
                Centeralign();
                SetSize(1);
                BoldOn();
                Inverted();
                WriteLine(OrganizationName);
                InvertedOff();
                WriteLine("*** Package Voucher ***");
                SetSize(0);
                BoldOff();
                Leftalign();
                WriteLine(xEqualLine);
                WriteLine("Reg No.        : " + RegistrationNo);
                WriteLine("Night & Pax    : " + NoofNights + " , " + NumberofPeople);
                // WriteLine("No. of People : " + NumberofPeople);

                if (!string.IsNullOrEmpty(CompanyName))
                {
                    WriteLine("Company        : " + CompanyName);
                }
                WriteLine("Guest          : " + GuestName);
                BoldOn();
                SetSize(1);
                WriteLine("Room No.      : " + RoomNumber);
                SetSize(0);
                BoldOff();
                WriteLine("Arrival Date   : " + ArrivalDate);
                WriteLine("Departure Date : " + DepartureDate);
                WriteLine("Package Date   : " + String.Format("{0:D}", Date.Date));

                WriteLine(xEqualLine);
                Centeralign();
                SetSize(1);
                BoldOn();
                // Inverted();
                WriteLine(PackageName);
                //  InvertedOff();
                SetSize(0);
                BoldOff();
                Leftalign();
                WriteLine(xBrokenLine);
                Centeralign();
                BarcodeHeight();
                WriteBarcode("{A" + barcodestring + "\n");
                Leftalign();

                WriteLine(xEqualLine);
                Centeralign();
                WriteLine("***Non-Fiscal*****Non-Fiscal***");
                WriteLine("WWW.CNETERP.COM");
                Leftalign();
                WriteLine(xEqualLine);
                EnterEpsonN();
                EnterEpsonN();
                EnterEpsonN();
                EnterEpsonN();
                EnterEpsonN();
                EnterEpsonN();
                EnterEpsonN();
                EnterEpsonN();
                CutPaper();
                CloseSockets();
            }
            catch
            {
                return false;
            }
            return true;
        }




        #region "Close Sockets"
        //Purpose:      Closes Main Socket instanses

        public static void CloseSockets()
        {
            //if (((mMainSocket != null )))
            //{
            // //   mMainSocket.Shutdown(SocketShutdown.Receive);
            //    mMainSocket.Disconnect(true);
            //    mMainSocket.Close();
            //    mMainSocket = null;
            //}
            try
            {
                if (((mClientSocket != null)))
                {
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Disconnect(true);
                    mClientSocket.Close();
                    mClientSocket = null;
                }

            }
            catch (Exception e)
            {

            }
        }

        #endregion
        #region Epson Order Printer

        /// <summary>
        /// Selects large barcode mode.
        /// </summary>
        /// <param name='large'>
        /// Large barcode mode.
        /// </param>
        /// 
        public static void SetLargeBarcode(bool large)
        {
            if (large)
            {
                _writeByte(29);
                _writeByte(119);
                _writeByte(3);
            }
            else
            {
                _writeByte(29);
                _writeByte(119);
                _writeByte(2);
            }
        }

        /// <summary>
        /// Sets the barcode left space.
        /// </summary>
        /// <param name='spacingDots'>
        /// Spacing dots.
        /// </param>
        /// 
        public void SetBarcodeLeftSpace(byte spacingDots)
        {
            _writeByte(29);
            _writeByte(120);
            _writeByte(spacingDots);
        }

        public static void CutPaper()
        {
            byte[] byData = new byte[] { 0x1d, 0x56, 0 };
            mClientSocket.Send(byData);
        }

        public static void WriteToBuffer(string text)
        {
            text = text.Trim('\n').Trim('\r');
            byte[] originalBytes = System.Text.Encoding.UTF8.GetBytes(text);
            mClientSocket.Send(originalBytes);
        }

        private static void _writeByte(byte valueToWrite)
        {
            byte[] tempArray = { valueToWrite };
            mClientSocket.Send(tempArray);
        }

        public static void WriteLine(string text)
        {
            WriteToBuffer(text);
            _writeByte(10);
            //System.Threading.Thread.Sleep(WriteLineSleepTimeMs);
        }

        public static void WriteBarcode(string text)
        {

            byte[] originalBytes = System.Text.Encoding.UTF8.GetBytes(text);
            //  byte[] outputBytes = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(""), originalBytes);


            if (text.Length > 1)
            {
                _writeByte(29);
                _writeByte(107);
                _writeByte(73); //todo: use format 2 (init string : 29,107,73) (0x00 can be a value, too)
                _writeByte(Convert.ToByte(text.Length));
                mClientSocket.Send(originalBytes);
                // _writeByte(0);
                // _writeByte(10);
            }

            /* byte[] originalBytes = System.Text.Encoding.UTF8.GetBytes(text.ToUpper());
             byte[] outputBytes = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("ibm850"), originalBytes);
             if (text.Length == 12 || text.Length == 13)
             {
                 _writeByte(29);
                 _writeByte(107);
                 _writeByte(2);
                 mClientSocket.Send(outputBytes);
                 _writeByte(0);
                 _writeByte(10);
             }*/

        }

        public static void WriteText(string text)
        {
            WriteToBuffer(text);
        }

        public static void WakeUp()
        {
            _writeByte(27);
            _writeByte(61);
            _writeByte(1);
        }
        public static void TabEpsonHorzontally()
        {
            _writeByte(9);
        }
        public static void EnterEpsonN()
        {
            _writeByte(10);
        }
        public static void Centeralign()
        {
            _writeByte(27);
            _writeByte(97);
            _writeByte(1);
        }
        public static void BarcodeHeight()
        {
            _writeByte(29);
            _writeByte(104);
            _writeByte(100);
        }
        public static void Leftalign()
        {
            _writeByte(27);
            _writeByte(97);
            _writeByte(0);
        }
        private static void BoldOn()
        {
            _writeByte(27);
            _writeByte(32);
            _writeByte(1);
            _writeByte(27);
            _writeByte(69);
            _writeByte(1);
        }
        private static void BoldOff()
        {
            _writeByte(27);
            _writeByte(32);
            _writeByte(0);
            _writeByte(27);
            _writeByte(69);
            _writeByte(0);
        }
        private static void Inverted()
        {

            _writeByte(29);
            _writeByte(66);
            _writeByte(1);
        }
        private static void InvertedOff()
        {
            _writeByte(29);
            _writeByte(66);
            _writeByte(0);
        }
        private static void SetSize(int doubleHeight)
        {

            _writeByte(29);
            _writeByte(33);
            _writeByte(Convert.ToByte(doubleHeight));
        }
        #endregion


        private void chkPackageselect_CheckedChanged(object sender, EventArgs e)
        {
            PackageDTO Rowdata = (PackageDTO)gvPackageList.GetFocusedRow();
            if (Rowdata != null && (Rowdata.canprint == false || Rowdata.printed == true || Rowdata.consumed == true))
            {
                ((DevExpress.XtraEditors.CheckEdit)sender).Checked = false;
            }
        }

        private void btnPrintOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // PackagePrintout grid = new PackagePrintout(gcPackageList, "Package Voucher", currentDate.Value.ToShortDateString(), CurrentRegistration);
            DocumentPrint.ReportGenerator pd = new DocumentPrint.ReportGenerator();
            pd.GeneratePackagePrintout(gcPackageList, "Guest Package", currentDate.Value.ToShortDateString(), CurrentRegistration);
        }
    }
    public class PackageDTO
    {
        public int code { get; set; }
        public bool select { get; set; }
        public string Package { get; set; }
        public DateTime date { get; set; }
        public bool printed { get; set; }
        public bool consumed { get; set; }
        public bool canprint { get; set; }
    }
}