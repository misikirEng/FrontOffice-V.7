using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Logics
{
    public class PMSSetting
    {

        // Time Setting
        [Category("Time Setting"), Description("The Check-In time of a registration"), DisplayName("Check-In Time")]
        public DateTime CheckInTime { get; set; }

        [Category("Time Setting"), Description("The Check-out time of a registration"), DisplayName("Check-Out Time")]
        public DateTime CheckOutTime { get; set; }

        [Category("Time Setting"), Description("The time to nofity the start of night audit process"), DisplayName("Night Audit Time")]
        public DateTime NightAuditTime { get; set; }

        private int _undoCheckin = 0;
        [Category("Time Setting"), Description("The minute to process Undo-Check In (Reinstate)"), DisplayName("Undo Check-In Minute")]
        public int UndoCheckinTime
        {
            get
            {
                return _undoCheckin;
            }

            set
            {
                _undoCheckin = value;
                if (value > 120 || value < 0)
                {
                    _undoCheckin = 0;
                }
            }
        }
        //[System.ComponentModel.CategoryAttribute("General Setting"), System.ComponentModel.Description("Gets/Sets the Horizontal start position."), System.ComponentModel.DisplayName("Welcome Message")]
        //public string WelcomeMessage { get; set; }

        //[System.ComponentModel.CategoryAttribute("General Setting"), System.ComponentModel.Description("Gets/Sets the Horizontal start position."), System.ComponentModel.DisplayName("Welcome Message Role")]
        //public string WelcomMessageRole { get; set; }

        [Category("General Setting"), Description("The Minimum amount of money that should remain while doing rate adjustment"), DisplayName("Min. Rate Adjustment Value")]
        public decimal MinimumRateAdujstment { get; set; }

        [Category("General Setting"), Description("The amount of money that is added to the rate amount when the mattress number is increased during group registration"), DisplayName("Mattress Amount")]
        public decimal MattressAmount { get; set; }

        [Category("General Setting"), Description("The amount of money that is used as a threshhold value for customer high balance to compare against the bcf of previous day amount. "), DisplayName("Customer High-Balance")]
        public decimal CustomerHighBalance { get; set; }

        [Category("General Setting"), Description("The flag whether to check the external reference number of Billing Vouchers or not."), DisplayName("Validate External Reference")]
        public bool ValidateExternalReference { get; set; }

        [Category("General Setting"), Description("A default value for fisical bill type during check-out fiscal print"), DisplayName("Default Fiscal Bill Type")]
        public string DefaultFiscalBillType { get; set; }

        [Category("General Setting"), Description("Whether to Room charge during check-in or not"), DisplayName("Charge After Check-In")]
        public bool ChargeAtCheckin { get; set; }

        //Night Audit
        private string _archivePath = @"\\Server";
        [Category("Night Audit"), Description("The pdf archive path for night audit's report archive"), DisplayName("Report Archive Path")]
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public string ArchivePath
        {
            get
            {
                return _archivePath;
            }
            set
            {
                _archivePath = value;
            }
        }

        [Category("Night Audit"), Description("A flag whether to print the arhived pdf automatically or not. "), DisplayName("Auto-Print PDF Archive")]
        public bool ArchivePrint { get; set; }

        [Category("Night Audit"), Description("A flag whether to conduct postine routine (Journalization) or not. "), DisplayName("Enable Journalize")]
        public bool EnableJournalize { get; set; }

        [Category("Night Audit"), Description("A Postine Routine Header to apply in night audit journalization process"), DisplayName("Posting Routine Header")]
        public string PostineRoutine { get; set; }

        //late check-out
        [Category("Late Check-Out"), Description("A flag whether to use late check-out or not."), DisplayName("Use Late Check-Out")]
        public bool UseLateCheckout { get; set; }

        [Category("Late Check-Out"), Description("The minimum time required to charge a guest late check-out additional charge"), DisplayName("Late Check-Out Time")]
        public DateTime LateCheckoutTime { get; set; }

        private double _lateCheckoutRequiredPayment = 0;
        [Category("Late Check-Out"), Description("The percent amount of a room charge to post a guest when late check-out time has been exceeded"), DisplayName("Required Payment (%)")]
        public double LateCheckoutRequiredPayment
        {
            get
            {
                return _lateCheckoutRequiredPayment;
            }
            set
            {
                _lateCheckoutRequiredPayment = value;
            }
        }

        private double _lateCheckoutAdditionalPayment = 0;
        [Category("Late Check-Out"), Description("The percent amount of a room charge to post a guest for every one hour late after late check-out time."), DisplayName("Additional Payment Per Hour(%)")]
        public double LateCheckoutAdditionalPayment
        {
            get
            {
                return _lateCheckoutAdditionalPayment;
            }
            set
            {
                _lateCheckoutAdditionalPayment = value;
            }
        }


        //Door Lock
        [Category("Door Lock"), Description("A flag whether to enforce card return during check-out or not."), DisplayName("Enforce Checkout Card Return")]
        public bool EnforceCardReturn { get; set; }

        //[Editor(typeof(SearchLookUpEdit), typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Door Lock"), Description("Service article code used when charging card-lost fee charge."), DisplayName("Lost Card Fee Article")]
        public string LostFeeArticle { get; set; }


        //Card-Holder Label Print
        private string _labelDesignFile = @"\\Server";
        [Category("Card-Holder Label Print"), Description("The location where the label design file is stored."), DisplayName("Label Design File")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public string LabelDesignFile
        {
            get
            {
                return _labelDesignFile;
            }
            set
            {
                _labelDesignFile = value;
            }
        }

        [Category("Card-Holder Label Print"), Description("Currently active label printer machine."), DisplayName("Label Printer")]
        public string LabelPrinter { get; set; }



    }
}
