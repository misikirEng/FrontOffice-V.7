
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Setting_and_Miscellaneous.DTO;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms
{
    //public partial class frmSpaceGenerator : XtraForm
    public partial class frmSpaceGenerator : UILogicBase, ILogicHelper, ICanCreate, ICanDelete

    {

        public frmSpaceGenerator()
        {
            InitializeComponent();
            FormSize = new Size(350, 440);
            InitializeUI();
        }

        public void OnCreate()
        {
            throw new NotImplementedException();
        }

        public DeleteClickedResult OnDelete()
        {
            throw new NotImplementedException();
        }

        private List<ConsigneeUnitDTO> iOrgUnit;
        public void InitializeUI()
        {
            //            Tag = this;
            Utility.AdjustForm(this);
            Utility.AdjustRibbon(lciSRibbonContainer);

            Location = new Point(350, 200);
            iOrgUnit = LocalBuffer.LocalBuffer.HotelBranchBufferList;
            leHotel.Properties.DisplayMember = "Name";
            leHotel.Properties.ValueMember = "Id";
            leHotel.Properties.DataSource = (iOrgUnit.Select(x => new { x.Id, x.Name })).ToList();

            //            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        internal List<Building> ExistBuildings { get; set; }

        public void InitializeData()
        {
        }

        public void LoadData(UILogicBase requesterForm, object args)
        {
        }

        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (leHotel.EditValue == null || string.IsNullOrEmpty(leHotel.EditValue.ToString()))
            {
                SystemMessage.ShowModalInfoMessage("Select Hotel First !!!", "ERROR");
                return;
            }
            ConsigneeUnit = Convert.ToInt32(leHotel.EditValue);

            if (ExistBuildings.Select(b => b.Name).Contains(teName.Text))
            {
                SystemMessage.ShowModalInfoMessage("There exists a building with this name. Please change its name!!!", "ERROR");
            }
            else
            {


                if (!ValidateRoomNoLength())
                {

                    XtraMessageBox.Show("Please Adjust the Number of Digits set for a room", "Error Applying Room No", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    seRoomNoDigit.Focus();

                    return;

                }

                var repititionResult = ValidateforRoomNumberRepitition();

                if (repititionResult.Any())
                {
                    //var message = "Multiple Rooms with same Room Number exist,Please Adjust Room No Starts From value. " + Environment.NewLine;

                    //message += "Repeating Room Numbers : " + repititionResult.Aggregate((u, v) => u + "," + v).ToString();

                    //XtraMessageBox.Show(message, "Error: Duplicate Room numbers Exist", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                    //seRoomNoDigit.Focus();

                    //return;

                }

                if (dxValidationProvider1.Validate() == true)
                {
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
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
        private bool ValidateRoomNoLength()
        {

            int floorNameCount = seNoOfFloors.Text.Trim().Length;

            int RoomNoCount = Convert.ToInt16(seRoomNoStartsFrom.Value + seRoomsonEach.Value).ToString().Length;

            int digit = Convert.ToInt16(seRoomNoDigit.Value);

            if (digit < (floorNameCount + RoomNoCount)) return false;
            else return true;

        }

        private List<String> ValidateforRoomNumberRepitition()
        {
            int floorCount = Convert.ToInt32(seNoOfFloors.Value);
            int roomCount = Convert.ToInt32(seRoomsonEach.Value);
            int roomNoStartsFrom = Convert.ToInt32(seRoomNoStartsFrom.Value);
            int digits = Convert.ToInt32(seRoomNoDigit.Value);


            List<String> result = new List<string>();

            for (int i = 1; i <= floorCount; i++)
            {
                for (int j = roomNoStartsFrom; j <= (roomCount + roomNoStartsFrom); j++)
                {
                    result.Add(BuildRoomNo(i.ToString(), j.ToString(), digits));
                }

            }
            var col = result.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(g => g.Key)
                             .ToList();

            return col;



        }

        private string BuildRoomNo(string floorName, string roomName, int digits)
        {

            if (floorName.Count() + roomName.Count() < digits)
            {
                int added = digits - (floorName.Count() + roomName.Count());

                String built = String.Empty;

                for (int i = 0; i < added; i++)
                {
                    built += "0";

                }

                return floorName + built + roomName;
            }


            return floorName + roomName;

        }



        public int ConsigneeUnit { get; set; }
    }
}
