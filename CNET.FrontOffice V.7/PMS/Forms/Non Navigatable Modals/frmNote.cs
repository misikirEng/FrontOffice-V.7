using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmNote : XtraForm //UILogicBase
    {
        private string _noteContent = string.Empty;
        public String NoteContent
        {
            get
            {
                return _noteContent;
            }
            set
            {
                _noteContent = value;
            }
        }


        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        /******************* CONSTRUCTOR ***************/
        public frmNote()
        {
            InitializeComponent();

            Utility.AdjustRibbon(lciRibbonHolder);

            Utility.AdjustForm(this);
        }

        #region Helper Methods

        public void ClearText()
        {
            rtbNote.ResetText();
        }

        public void SetNoteText(String text)
        {
            rtbNote.Text = text;
        }


        #endregion

        #region Event Handlers

        private void rtbNote_TextChanged(object sender, EventArgs e)
        {
            //  NoteContent= rtbNote.Text;
        }
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NoteContent = rtbNote.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void bbiClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClearText();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NoteContent = rtbNote.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void frmNote_Load(object sender, EventArgs e)
        {
            rtbNote.Text = NoteContent;
        }

        #endregion



    }
}
