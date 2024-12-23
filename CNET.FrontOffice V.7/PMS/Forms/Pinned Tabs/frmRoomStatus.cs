using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
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

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRoomStatus : UILogicBase
    {
        public DateTime? TodayDateTime = null; 
        public Color InhouseColor = Color.Green;
        public Color WaitingListColor = Color.LightBlue;
        public Color SixPMColor = Color.Green;
        public Color GauranteedColor = Color.LightBlue;
        public Color CheckedOutColor = Color.Green;
        public Color CancelledColor = Color.Green;
        public Color NoshowColor = Color.Green;
        public List<TileItem> AllTileItem { get; set; }
        public SystemConstantDTO CheckedOutObjectState { get; set; }

        public SystemConstantDTO InhouseObjectState { get; set; }

        public SystemConstantDTO WaitingListObjectState { get; set; }

        public SystemConstantDTO SixPMObjectState { get; set; }

        public SystemConstantDTO NoshowObjectState { get; set; }

        public SystemConstantDTO GauranteedObjectState { get; set; }

        public SystemConstantDTO CancelledObjectState { get; set; }

        int Inhousecount = 0;
        int CheckedOutcount = 0;
        int WaitingListcount = 0;
        int SixPMcount = 0;
        int Noshowcount = 0;
        int Gauranteedcount = 0;
        int Cancelledcount = 0;


        public frmRoomStatus()
        {
            InitializeComponent();
            TodayDateTime =  UIProcessManager.GetServiceTime();
            if (TodayDateTime != null)
            {
                TodayDateTime = TodayDateTime.Value;
            }
            else
            {
                XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            deDate.EditValue = TodayDateTime;
            beiGroupBy.EditValue = "By Room Type";

            InhouseObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "Inhouse");

            CheckedOutObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "Check Out");

            WaitingListObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "Waiting List");

            SixPMObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "6PM");

            CancelledObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "Cancelled");

            GauranteedObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "Gauranteed");

            NoshowObjectState = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Description == "No show");

            if (InhouseObjectState != null)
            {
                InhouseColor = ColorTranslator.FromHtml(InhouseObjectState.Value);
                lblInhouseColor.BackColor = InhouseColor;
            }
            if (CheckedOutObjectState != null)
            {
                CheckedOutColor = ColorTranslator.FromHtml(CheckedOutObjectState.Value);
                lblCheckoutColor.BackColor = CheckedOutColor;
            }
            if (WaitingListObjectState != null)
            {
                WaitingListColor = ColorTranslator.FromHtml(WaitingListObjectState.Value);
                lblWaitinglistcolor.BackColor = WaitingListColor;
            }
            if (SixPMObjectState != null)
            {
                SixPMColor = ColorTranslator.FromHtml(SixPMObjectState.Value);
                lblSixPMColor.BackColor = SixPMColor;
            }
            if (CancelledObjectState != null)
            {
                CancelledColor = ColorTranslator.FromHtml(CancelledObjectState.Value);
                lblCancelledcolor.BackColor = CancelledColor;
            }
            if (GauranteedObjectState != null)
            {
                GauranteedColor = ColorTranslator.FromHtml(GauranteedObjectState.Value);
                lblGauranteedColor.BackColor = GauranteedColor;
            }
            if (NoshowObjectState != null)
            {
                NoshowColor = ColorTranslator.FromHtml(NoshowObjectState.Value);
                lblNoshowColor.BackColor = NoshowColor;
            }


            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;

        }

        private void btnShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (deDate.EditValue == null || string.IsNullOrEmpty(deDate.EditValue.ToString()))
            {
                XtraMessageBox.Show("Please Select date First !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (beiGroupBy.EditValue == null || string.IsNullOrEmpty(beiGroupBy.EditValue.ToString()))
            {
                XtraMessageBox.Show("Please Select Group Type First !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            DateTime Date = Convert.ToDateTime(deDate.EditValue.ToString()).Date;
            List<VwRegistrationDocumentViewDTO> _regDocList = UIProcessManager.GetRegistrationDocumentViewByDate(Date);
            List<RoomTypeDTO> RoomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value);


            if (RoomTypeList != null && RoomTypeList.Count > 0)
                RoomTypeList = RoomTypeList.Where(x=> x.IspseudoRoomType == false && x.CanBeMeetingRoom == false).ToList();

            List<int> nonpseudoRoomTypeList = new List<int>();


            if (RoomTypeList != null && RoomTypeList.Count > 0)
                nonpseudoRoomTypeList = RoomTypeList.Select(x => x.Id ).ToList();

            //List<RoomType> RoomTypeList = UIProcessManager.GetAllRoomTypes();
            List<RoomDetailDTO> RoomDetailList = UIProcessManager.SelectAllRoomDetail();

            if (RoomDetailList != null && RoomDetailList.Count > 0 && nonpseudoRoomTypeList!= null && nonpseudoRoomTypeList.Count > 0)
                RoomDetailList = RoomDetailList.Where(x => nonpseudoRoomTypeList.Contains(x.RoomType)).ToList();

            List<VwRoomManagmentViewDTO> roomManagmentList = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);

            if (AllTileItem == null)
            {
                AllTileItem = new List<TileItem>();
            }
            tcRoomStatus.Groups.Clear();

            Inhousecount = 0;
            CheckedOutcount = 0;
            WaitingListcount = 0;
            SixPMcount = 0;
            Noshowcount = 0;
            Gauranteedcount = 0;
            Cancelledcount = 0;

            if (beiGroupBy.EditValue.ToString() == "By Room Type")
            {
                #region Group By Room Type
                int count = 0;
                foreach (RoomTypeDTO Type in RoomTypeList)
                {
                    TileGroup Group = new TileGroup
                    {
                        Text = Type.Description,
                        Visible = true,
                    };
                    tcRoomStatus.Groups.Add(Group);
                    List<RoomDetailDTO> RoomTypeListfilter = RoomDetailList.Where(x => x.RoomType == Type.Id).ToList();
                    CreateTile(_regDocList, roomManagmentList, count, RoomTypeListfilter);
                    count++;
                }

                #endregion
            }
            else if (beiGroupBy.EditValue.ToString() == "By Location")
            {
                #region Group By Location
                List<int> roomList = _regDocList.Where(r => r.Room != null).Select(x => x.Room.Value).Distinct().ToList();

                List<SpaceDTO> ALLSpaceList = UIProcessManager.SelectAllSpace();

                List<SpaceDTO> FloorSpaceList = (from room in RoomDetailList
                                            join space in ALLSpaceList
                                             on room.Space equals space.Id
                                            join parentspace in ALLSpaceList
                                             on space.ParentId equals parentspace.Id
                                            select parentspace).Distinct().OrderBy(x => x.Id).ToList();
                int count = 0;
                foreach (SpaceDTO space in FloorSpaceList)
                {
                    List<SpaceDTO> ALLRoomSpaceList = ALLSpaceList.Where(x=> x.ParentId == space.Id).ToList();
                    List<int> RoomSpaceList = new List<int>();
                    if (ALLRoomSpaceList != null && ALLRoomSpaceList.Count > 0)
                        RoomSpaceList = ALLRoomSpaceList.Select(x => x.Id).ToList();


                    string BuildingName = "";

                    if (space.ParentId != null)
                    {
                        SpaceDTO Buildingspace = ALLSpaceList.FirstOrDefault(x => x.Id == space.ParentId);
                        if (Buildingspace != null)
                            BuildingName = Buildingspace.Description+" Buliding :- ";
                    }

                    TileGroup Group = new TileGroup
                    {
                        Text = BuildingName + space.Description,
                        Visible = true,
                    };
                    tcRoomStatus.Groups.Add(Group);
                    List<RoomDetailDTO> RoomTypeListfilter = RoomDetailList.Where(x => RoomSpaceList.Contains( x.Space)).ToList();
                    CreateTile(_regDocList, roomManagmentList, count, RoomTypeListfilter);
                    count++;

                }


                #endregion
            }

            lcInhouse.Text = "Inhouse (" + Inhousecount + ")";
            lcCheckOut.Text = "Checked Out (" + CheckedOutcount + ")";
            lcSixPM.Text = "SIX PM (" + SixPMcount + ")";
            lcCancelled.Text = "Cancelled (" + Cancelledcount + ")";
            lcGauranteed.Text = "Gauranteed (" + Gauranteedcount + ")";
            lcNoShow.Text = "NoShow (" + Noshowcount + ")";
            lcWaitingList.Text = "WaitingList (" + WaitingListcount + ")";

        }

        private void CreateTile(List<VwRegistrationDocumentViewDTO> _regDocList, List<VwRoomManagmentViewDTO> roomManagmentList, int count, List<RoomDetailDTO> RoomTypeListfilter)
        {
            if (RoomTypeListfilter != null && RoomTypeListfilter.Count > 0)
            {
                RoomTypeListfilter = RoomTypeListfilter.OrderBy(x => x.Description).ToList();
                foreach (RoomDetailDTO Room in RoomTypeListfilter)
                {
                    TileGroup TileGroup = new TileGroup();
                    TileItem ll = new TileItem();
                    ll.ItemSize = TileItemSize.Wide;


                    TileItemElement tileElement = new TileItemElement();

                    tileElement.Appearance.Normal.Font = new System.Drawing.Font("Microsoft New Tai lue", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                    tileElement.Appearance.Normal.Options.UseFont = true;
                    tileElement.TextAlignment = TileItemContentAlignment.MiddleCenter;
                    tileElement.Text = Room.Description;
                    ll.Elements.Add(tileElement);

                    if (chkShowRoomStatus.Checked)
                    {
                        VwRoomManagmentViewDTO roomstatus = roomManagmentList.FirstOrDefault(x => x.roomDetailCode == Room.Id);
                        if (roomstatus != null)
                        {
                            tileElement = new TileItemElement();
                            tileElement.Appearance.Normal.Font = new System.Drawing.Font("Microsoft New Tai lue", 8, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                            tileElement.Appearance.Normal.Options.UseFont = true;
                            tileElement.TextAlignment = TileItemContentAlignment.BottomRight;
                            tileElement.Text = roomstatus.rmstatus;
                            ll.Elements.Add(tileElement);
                        }
                    }

                    List<VwRegistrationDocumentViewDTO> registrationroom = _regDocList.Where(x => x.Room == Room.Id).ToList();

                    if (registrationroom != null && registrationroom.Count > 0)
                    {
                        if (InhouseObjectState.Id == registrationroom.FirstOrDefault().LastState)
                        {
                            Inhousecount++;
                            ll.AppearanceItem.Normal.BackColor = InhouseColor;
                            ll.AppearanceItem.Pressed.BackColor = InhouseColor;
                            ll.AppearanceItem.Selected.BackColor = InhouseColor;
                            ll.AppearanceItem.Hovered.BackColor = InhouseColor;
                        }
                        else
                        {
                            SystemConstantDTO RegistrationColorobj = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Id == registrationroom.FirstOrDefault().LastState);

                            Color RegistrationColor = CheckedOutColor;

                            if (RegistrationColorobj == WaitingListObjectState)
                            {
                                WaitingListcount++;
                                RegistrationColor = WaitingListColor;
                            }
                            else if (RegistrationColorobj == SixPMObjectState)
                            {
                                SixPMcount++;
                                RegistrationColor = SixPMColor;
                            }
                            else if (RegistrationColorobj == NoshowObjectState)
                            {
                                Noshowcount++;
                                RegistrationColor = NoshowColor;
                            }
                            else if (RegistrationColorobj == GauranteedObjectState)
                            {
                                Gauranteedcount++;
                                RegistrationColor = GauranteedColor;
                            }
                            else if (RegistrationColorobj == CancelledObjectState)
                            {
                                Cancelledcount++;
                                RegistrationColor = CancelledColor;
                            }

                            ll.AppearanceItem.Normal.BackColor = RegistrationColor;
                            ll.AppearanceItem.Pressed.BackColor = RegistrationColor;
                            ll.AppearanceItem.Selected.BackColor = RegistrationColor;
                            ll.AppearanceItem.Hovered.BackColor = RegistrationColor;
                            ll.AppearanceItem.Normal.BorderColor = RegistrationColor;
                            ll.AppearanceItem.Pressed.BorderColor = RegistrationColor;
                            ll.AppearanceItem.Selected.BorderColor = RegistrationColor;
                            ll.AppearanceItem.Hovered.BorderColor = RegistrationColor;
                        }
                        ll.AppearanceItem.Normal.ForeColor = Color.Black;
                        ll.AppearanceItem.Pressed.ForeColor = Color.Black;
                        ll.AppearanceItem.Selected.ForeColor = Color.Black;
                        ll.AppearanceItem.Hovered.ForeColor = Color.Black;

                    }
                    else
                    {
                        CheckedOutcount++;
                        ll.AppearanceItem.Normal.BackColor = CheckedOutColor;
                        ll.AppearanceItem.Pressed.BackColor = CheckedOutColor;
                        ll.AppearanceItem.Selected.BackColor = CheckedOutColor;
                        ll.AppearanceItem.Hovered.BackColor = CheckedOutColor;

                        ll.AppearanceItem.Normal.BorderColor = CheckedOutColor;
                        ll.AppearanceItem.Pressed.BorderColor = CheckedOutColor;
                        ll.AppearanceItem.Selected.BorderColor = CheckedOutColor;
                        ll.AppearanceItem.Hovered.BorderColor = CheckedOutColor;
                    }
                    tcRoomStatus.Groups[count].Items.Add(ll);
                    AllTileItem.Add(ll);
                }
            }

        }

        public int? SelectedHotelcode { get; set; }
        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {


            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);
            btnShow.PerformClick();

        }


    }
}
