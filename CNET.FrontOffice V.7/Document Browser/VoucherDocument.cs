using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraReports.UI;
using DevExpress.XtraBars.Docking;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Utils;
using System.ComponentModel;
using System.IO;
using System.Text;
using DevExpress.XtraEditors.Repository;

namespace DocumentBrowser
{
    public partial class VoucherDocument : UserControl
    {
        private float gridFontSize = 8;
        public string documentBrowserType = "heavydocumentbrowser";
        private string voucherDefnition;
        private string multipleVoucherDefnitions;
        private string code;
        private string name;
        private string consignee;
        private string consigneeName;
        private string dateSearch;
        public string issuedDate;
        public string issueDateEnd;
        private bool? isIssued;
        private bool? isVoid;
        public string period;
        private string periodName;
        private string lastObjectState;
        private string lastObjectStateName;
        private string device;
        private string deviceName;
        private string user;
        private string userName;
        private string orgUnitDef;
        private string orgUnitDefName;
        private string VoucherCodeForJournal { get; set; }
        private string transactionType;
        private string source;
        private string sourceName;
        private string destination;
        private string destinationName;
        private string operatorV;
        private string discount;
        private string grandTotal;
        private string subTotal;
        private string additionalCharge;
        private int? voucherExtIndex;
        private string voucherExtValue;
        private string TransactionRefType;
        private string TransactionRefValue;
        private string vouchExtOne;
        private string vouchExtNumberOne;
        private string vouchExtTwo;
        private string vouchExtNumberTwo;
        private string vouchExtThree;
        private string vouchExtNumberThree;
        private string vouchExtFour;
        private string vouchExtNumberFour;
        private string vATamount;
        private string withholdingAmount;
        private string forwardReferences;
        private string backwardReferences;
        private string internalReferences;
        private int? otherConsIndex;
        private string otherConsigneeOne, otherConsigneeOneName, otherConsigneeTwo,
            otherConsigneeTwoName, otherConsigneeThree, otherConsigneeThreeName, fsNo, mrsNo, waiterName, table, _article_code;

        #region Constructor
        public VoucherDocument(string voucherTypeRef)
        {
            InitializeComponent();
        }

        public VoucherDocument(List<String> voucherDefinitions, string consigneeCode = null, bool hasPrivilege = true, bool isVoucherDashboard = false, string dateCriteria = null)
        {
            InitializeComponent();


        }

        #endregion


        private void BBDateSearch_EditValueChanged(object sender, EventArgs e)
        {
        }


    }

    static class Mapper
    {

        #region object mapper
        public static void MatchAndMap<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class, new()
            where TDestination : class, new()
        {
            if (source != null && destination != null)
            {
                List<System.Reflection.PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList<System.Reflection.PropertyInfo>();
                List<System.Reflection.PropertyInfo> destinationProperties = destination.GetType().GetProperties().ToList<System.Reflection.PropertyInfo>();

                foreach (System.Reflection.PropertyInfo sourceProperty in sourceProperties)
                {
                    System.Reflection.PropertyInfo destinationProperty = destinationProperties.Find(item => item.Name == sourceProperty.Name);
                    if (destinationProperty != null)
                    {
                        try
                        {
                            destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message, x.StackTrace);
                        }
                    }
                }
            }
        }
        public static TDestination MapProperties<TDestination>(this object mapSource) where TDestination : class, new()
        {
            var destination = Activator.CreateInstance<TDestination>();
            MatchAndMap(mapSource, destination);
            return destination;
        }
        #endregion
    }

    public class VoucherEditValueClicked : EventArgs
    {
        public class EditButtonClicked
        {

        }
    }

    public class VoucherRoomPOSChargeClicked : EventArgs
    {
        public class RoomPOSChargeButtonClicked
        {

        }
    }
}

