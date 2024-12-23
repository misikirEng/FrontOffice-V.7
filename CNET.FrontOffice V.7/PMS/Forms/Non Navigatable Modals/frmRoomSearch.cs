using CNET.ERP.Client.Common.UI;
using CNET.ERP.Client.Common.UI.Library;
using CNET.FrontOffice_V._7.PMS.Contracts;

using System;
using System.Linq;
using System.Collections.Generic;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using System.Windows.Forms;
using CNET.ERP.ResourceProvider;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars;

using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraGrid.Editors;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors.Repository;
using System.Text;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.Group_Registration;
using CNET.FrontOffice_V._7.Forms.State_Change;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraRichEdit.Import.Doc;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRoomSearch : UILogicBase, ILogicHelper
    {
        DateTime _arrivalDate;
        DateTime _departureDate;
        int? roomType = null;
        string roomTypeDescription = "";

        public DateTime CurrentTime { get; set; }

        private const string VACANT = "Vacant";
        private const string OCCUPIED = "Occupied";


        //private List<RoomSearchVM> _roomSearchVM = null; 
        private List<VwVoucherDetailWithRoomDetailViewDTO> avRooms = null;
        private List<RoomDetailDTO> roomList = null;
        private List<int> pseudoRooms = null;
        private List<VwRoomFeatureViewDTO> roomFeatures = null;
        private List<VwRoomFeatureViewDTO> selectedRoomFeatures = new List<VwRoomFeatureViewDTO>();
        private List<RoomSearchVM> roomSearchVMs = new List<RoomSearchVM>();
        public bool MultipleRoom { get; set; }
        public List<int> _FilterroomTypeList = null;
        private List<RoomTypeDTO> _roomTypeList = null;

        private bool checkHK = false;

        public frmRoomSearch()
        {
            InitializeComponent();
            InitializeUI();

            DateTime? date = UIProcessManager.GetServiceTime();
            if (date != null)
            {
                CurrentTime = date.Value;
            }
            else
            {
                XtraMessageBox.Show("Error Getting DateTime From Server", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ApplyIcons();
        }
        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("Search", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);
            bbiSearch.Glyph = Image;
            bbiSearch.LargeGlyph = Image;
        }

        public void InitializeUI()
        {
            //       if (!DesignMode)
            //       {
            //////     Utility.AdjustRibbon(lciRibbonHolder);
            //       }
            CNETFooterRibbon.ribbonControl = rcRoomSearch;
            gv_roomSearch.RowCellClick += frmRoomSearch_RowCellClick;
            this.StartPosition = FormStartPosition.CenterScreen;


            //Room Types

            //  _roomTypeList = UIProcessManager.SelectAllRoomType().Where(r => r.isActive && (r.activationDate != null && r.activationDate.Value.Date >= CurrentTime.Date)).ToList();
            //_roomTypeList = UIProcessManager.SelectAllRoomTypebyBranch(SelectedHotelcode).Where(r => r.isActive && (r.activationDate != null && r.activationDate.Value.Date >= CurrentTime.Date)).ToList();



            // load features lookup edit
            roomFeatures = UIProcessManager.GetRoomFeatureView().GroupBy(f => f.Feature).Select(f => f.First()).ToList();
            GridColumn column = risle_feature.View.Columns.AddField("featureDescription");
            column.Visible = true;
            risle_feature.DisplayMember = "featureDescription";
            risle_feature.ValueMember = "feature";
            risle_feature.DataSource = roomFeatures;

            gv_risleFeature.SelectionChanged += gv_risleFeature_SelectionChanged;
            risle_feature.Popup += risle_feature_Popup;
            risle_feature.AddNewValue += risle_feature_AddNewValue;



            rite_room.EditValueChanged += rite_room_EditValueChanged;
        }

        void risle_feature_AddNewValue(object sender, DevExpress.XtraEditors.Controls.AddNewValueEventArgs e)
        {
            if (selectedRoomFeatures == null) return;

            FilterRoomsByFeature(selectedRoomFeatures);
        }



        void risle_feature_Popup(object sender, EventArgs e)
        {
            IPopupControl popupControl = sender as IPopupControl;
            PopupSearchLookUpEditForm f = popupControl.PopupWindow as PopupSearchLookUpEditForm;
            if (f != null)
            {
                SearchEditLookUpPopup popup = f.ActiveControl as SearchEditLookUpPopup;
                LayoutControlItem clearBtn = (LayoutControlItem)popup.lcgAction.Items[0];

                if (clearBtn != null)
                {
                    clearBtn.Control.Text = "Clear";
                    clearBtn.Control.Click += clearButton_Click;

                }

                LayoutControlItem addBtn = (LayoutControlItem)popup.lcgAction.Items[1];
                if (addBtn != null)
                {
                    addBtn.Control.Text = "GO";
                }

            }

        }




        void clearButton_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gv_risleFeature.GetSelectedRows();

            gv_risleFeature.BeginUpdate();
            for (
                int i = 0; i < selectedRows.Length; i++)
            {
                gv_risleFeature.UnselectRow(selectedRows[i]);

            }

            gv_risleFeature.EndUpdate();
            selectedRoomFeatures.Clear();
            FilterRoomsByFeature(selectedRoomFeatures);
        }

        void gv_risleFeature_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            selectedRoomFeatures.Clear();
            GridView view = sender as GridView;
            gv_risleFeature = view;
            if (view == null) return;
            int[] selectedRows = view.GetSelectedRows();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                VwRoomFeatureViewDTO roomFeature = gv_risleFeature.GetRow(selectedRows[i]) as VwRoomFeatureViewDTO;
                if (roomFeature != null)
                    selectedRoomFeatures.Add(roomFeature);

            }
        }



        void frmRoomSearch_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2 && !MultipleRoom)// Double Click
            {
                HandleRoomSelection();
            }
        }

        public void InitializeData()
        {
            try
            {
                if (SelectedHotelcode == null)
                {

                    XtraMessageBox.Show("Please Select Hotel First!!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                _roomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value);
                if (_roomTypeList == null || _roomTypeList.Count == 0)
                {
                    XtraMessageBox.Show("No RoomType Found !!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                _roomTypeList = _roomTypeList.Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= CurrentTime.Date)).ToList();

                if (_roomTypeList == null || _roomTypeList.Count == 0)
                {
                    XtraMessageBox.Show("No RoomType Found !!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

                repoLukRoomTypes.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
                repoLukRoomTypes.DisplayMember = "Description";
                repoLukRoomTypes.ValueMember = "Id";
                repoLukRoomTypes.DataSource = _roomTypeList.Where(r => !r.IspseudoRoomType.Value).ToList();

                List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                if (pseudoRoomList != null)
                    pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();

                List<int> stateList = new List<int>() { CNETConstantes.CHECKED_IN_STATE, CNETConstantes.OSD_WAITLIST_STATE, CNETConstantes.SIX_PM_STATE, CNETConstantes.GAURANTED_STATE, CNETConstantes.OSD_WAITLIST_STATE };//, CNETConstantes.OSD_Category_Transaction };

                if (roomType != null)
                {
                    avRooms = UIProcessManager.GetAvailabeRoomsByDateAndState(_arrivalDate.Date, _departureDate.Date, stateList, roomType.Value).ToList();

                    if (_FilterroomTypeList != null && _FilterroomTypeList.Count > 0)
                        avRooms = avRooms.Where(x => _FilterroomTypeList.Contains(x.RoomTypeCode)).ToList();

                    beiFeature.Enabled = false;

                    if (avRooms != null && avRooms.Count > 0)
                    {
                        if (avRooms.FirstOrDefault().IspseudoRoomType != null && avRooms.FirstOrDefault().IspseudoRoomType.Value)
                        {
                            tp_rooms.PageEnabled = false;
                        }
                        else
                        {
                            tp_pseudoRooms.PageEnabled = false;
                        }
                    }
                    else
                    {
                        tp_pseudoRooms.PageEnabled = false;
                    }
                }
                else
                {
                    avRooms = UIProcessManager.GetAvailabeRoomsByDateAndState(_arrivalDate, _departureDate, stateList, null).OrderBy(r => r.RoomTypeDesc).ToList();

                    if (_FilterroomTypeList != null && _FilterroomTypeList.Count > 0)
                        avRooms = avRooms.Where(x => _FilterroomTypeList.Contains(x.RoomTypeCode)).ToList();

                    beiFeature.Enabled = true;
                    tp_pseudoRooms.PageEnabled = true;
                }
                if (avRooms == null) return;

                List<RegistrationStatusDTO> foStatusList = UIProcessManager.GetRegistrationStatusList(avRooms.Select(r => r.Code).ToList(), _arrivalDate.Date);

                foreach (var rooms in avRooms)
                {
                    //if room type is not active or its activation date is not meet, continue
                    RoomTypeDTO rt = _roomTypeList.FirstOrDefault(r => r.Id == rooms.RoomTypeCode);
                    if (rt != null && rt.ActivationDate != null)
                    {
                        if (!rt.IsActive || CurrentTime.Date < rt.ActivationDate.Value.Date)
                            continue;
                    }
                    else
                    {
                        continue;
                    }

                    RoomSearchVM rsvm = new RoomSearchVM();
                    rsvm.RoomDetailView = rooms;


                    // get FO status
                    //var foStatus = UIProcessManager.GetRegistrationStatus(rooms.code, CurrentTime.Date);
                    //if (foStatus != null)
                    //{
                    //    rsvm.FOStatus = foStatus.FOStatus == "0" ? VACANT : OCCUPIED;
                    //}

                    //Get FO Status
                    RegistrationStatusDTO foStatus = null;
                    if (foStatusList != null && foStatusList.Count > 0)
                    {
                        foStatus = foStatusList.FirstOrDefault(f => f.RoomNumber == rooms.Code);
                    }
                    if (foStatus != null)
                    {
                        rsvm.FOStatus = foStatus.FOStatus == "0" ? VACANT : OCCUPIED;
                    }
                    else
                    {
                        rsvm.FOStatus = VACANT;
                    }

                    //get features
                    if (roomFeatures != null)
                    {
                        List<string> features = roomFeatures.Where(rf => rf.RoomTypeDescription == rooms.RoomTypeDesc).Select(rf => rf.FeatureDescription).ToList();
                        StringBuilder sb = new StringBuilder();
                        if (features != null)
                        {
                            foreach (string f in features)
                            {
                                sb.Append(f);
                                sb.Append(", ");
                            }

                            rsvm.Feature = sb.ToString();
                        }
                    }

                    roomSearchVMs.Add(rsvm);


                }


                // Bind to grid
                gc_roomSearch.DataSource = roomSearchVMs;
                gc_roomSearch.RefreshDataSource();
                gv_roomSearch.RefreshData();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing room search. Detail:" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private UILogicBase requesterForm = null;
        public void LoadData(UILogicBase requesterForm, object args)
        {
            if (requesterForm.GetType() == typeof(frmReservation) || requesterForm.GetType() == typeof(frmCheckIn) || requesterForm.GetType() == typeof(frmGroupRegistration))
            {
                this.requesterForm = requesterForm;

                var arguments = (Dictionary<string, string>)args;

                if (!string.IsNullOrEmpty(arguments["Guest Name"]))
                {
                    Text += ("-" + arguments["Guest Name"]);
                }
                beiDate.EditValue = arguments["Arrival"];
                if (!string.IsNullOrEmpty(arguments["Nights"]))
                {
                    beiNights.EditValue = arguments["Nights"];
                }
                _arrivalDate = Convert.ToDateTime(arguments["Arrival"]);
                _departureDate = Convert.ToDateTime(arguments["Departure"]);
                string romtypevalue = arguments["RoomType"];
                if (!string.IsNullOrEmpty(romtypevalue))
                    roomType = Convert.ToInt32(romtypevalue);
                else
                    roomType = null;
                roomTypeDescription = arguments["RoomTypeDescription"];
                rpgAdd.Visible = true;

                if (arguments["CHECK_HK"] == "YES")
                    checkHK = true;
            }
            if (requesterForm.GetType() == typeof(frmDateAmendment) || requesterForm.GetType() == typeof(frmRoomMove) || requesterForm.GetType() == typeof(frmMultipleRoomCheckIn) || requesterForm.GetType() == typeof(frmRegistrationDetail))
            {
                this.requesterForm = requesterForm;
                var arguments = (Dictionary<string, string>)args;
                beiDate.EditValue = arguments["Arrival"];
                _arrivalDate = Convert.ToDateTime(arguments["Arrival"]);
                _departureDate = Convert.ToDateTime(arguments["Departure"]);
                roomType = Convert.ToInt32(arguments["RoomType"]);

                if (arguments["CHECK_HK"] == "YES")
                    checkHK = true;
                rpgAdd.Visible = true;
            }
        }
        private void bbiAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            HandleRoomSelection();

        }

        private RoomDetailDTO GetSelectedRoom()
        {
            RoomSearchVM selectVM = gv_roomSearch.GetFocusedRow() as RoomSearchVM;
            if (selectVM == null) return null;

            RoomDetailDTO rd = new RoomDetailDTO()
            {
                Id = selectVM.RoomDetailView.Code,
                Description = selectVM.RoomDetailView.RoomDescription,
                MaxOccupnancy = selectVM.RoomDetailView.MaxOccupancy,
                RoomType = selectVM.RoomDetailView.RoomTypeCode,
                PhoneNumber = selectVM.RoomDetailView.PhoneNumber,
                LastState = selectVM.RoomDetailView.RoomLastState,
                Remark = selectVM.RoomDetailView.RoomDetailRemark,
                Space = selectVM.RoomDetailView.RoomDetailSpace

            };
            return rd;
        }

        List<RoomDetailDTO> SelectesRoomDetailList { get; set; }
        private List<RoomDetailDTO> GetSelectedRoomList()
        {
            gv_roomSearch.PostEditor();
            SelectesRoomDetailList = new List<RoomDetailDTO>();
            List<RoomSearchVM> RoomDataSourceList = (List<RoomSearchVM>)gv_roomSearch.DataSource;

            if (RoomDataSourceList != null && RoomDataSourceList.Count > 0)
            {
                List<RoomSearchVM> SelectedRoomDataSourceList = RoomDataSourceList.Where(x => x.select).ToList();
                if (SelectedRoomDataSourceList != null && SelectedRoomDataSourceList.Count > 0)
                {
                    SelectesRoomDetailList = SelectedRoomDataSourceList.Select(selectVM =>
                        new RoomDetailDTO()
                        {
                            Id = selectVM.RoomDetailView.Code,
                            Description = selectVM.RoomDetailView.RoomDescription,
                            MaxOccupnancy = selectVM.RoomDetailView.MaxOccupancy,
                            RoomType = selectVM.RoomDetailView.RoomTypeCode,
                            PhoneNumber = selectVM.RoomDetailView.PhoneNumber,
                            LastState = selectVM.RoomDetailView.RoomLastState,
                            Remark = selectVM.RoomDetailView.RoomDetailRemark,
                            Space = selectVM.RoomDetailView.RoomDetailSpace

                        }
                        ).ToList();
                }
            }
            return SelectesRoomDetailList;
        }
        private bool CheckRoomHKStatus()
        {

            bool isOk = true;
            RoomSearchVM selectVM = gv_roomSearch.GetFocusedRow() as RoomSearchVM;

            if (selectVM.RoomDetailView.IspseudoRoomType != null && selectVM.RoomDetailView.IspseudoRoomType.Value) return true;

            //check room HK
            if (selectVM != null)
            {
                if (checkHK)
                {
                    if (selectVM.RoomDetailView != null && selectVM.RoomDetailView.RoomLastState != null)
                    {
                        ActivityDefinitionDTO ad = UIProcessManager.GetActivityDefinitionById(selectVM.RoomDetailView.RoomLastState.Value);
                        if (ad != null)
                        {
                            if (ad.Description == CNETConstantes.CLEAN || ad.Description == CNETConstantes.INSPECTED)
                            {
                                isOk = true;
                            }
                            else
                            {
                                XtraMessageBox.Show("The selected room is " + selectVM.RoomDetailView.LastStateDesc + ". Please, select another room.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                isOk = false;

                            }
                        }
                        else
                        {
                            DialogResult result = XtraMessageBox.Show("Unable to get room HK status. Do you want to assign this room?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (result == DialogResult.Yes)
                            {
                                isOk = true;
                            }
                            else
                            {
                                isOk = false;
                            }
                        }
                    }
                    else
                    {
                        DialogResult result = XtraMessageBox.Show("Unable to get room HK status. Do you want to assign this room?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            isOk = true;
                        }
                        else
                        {
                            isOk = false;
                        }
                    }
                }

                if (isOk)
                {
                    //check room occupancy
                    if (selectVM.FOStatus != null)
                    {
                        if (selectVM.FOStatus == OCCUPIED)
                        {
                            XtraMessageBox.Show("The selected room is " + OCCUPIED + ". Please, select another room.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            isOk = false;
                        }
                        else
                        {
                            isOk = true;
                        }

                    }
                    else
                    {
                        DialogResult result = XtraMessageBox.Show("Unable to get room FO status. Do you want to assign this room?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            isOk = true;
                        }
                        else
                        {
                            isOk = false;
                        }
                    }

                }

                return isOk;

            }
            else
            {
                XtraMessageBox.Show("No Room is Selected", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }


        }

        private void HandleRoomSelection()
        {
            if (!CheckRoomHKStatus())
            {
                return;
            }

            //if(MultipleRoom )
            //{
            //    GetSelectedRoomList(); 
            //    if ( SelectesRoomDetailList != null && SelectesRoomDetailList.Count > 0)
            //    {
            //        List<int> SelectedRoomTypeList = SelectesRoomDetailList.Select(x => x.RoomType).Distinct().ToList();
            //        if (SelectedRoomTypeList != null && SelectedRoomTypeList.Count > 1)
            //        {
            //            XtraMessageBox.Show("Please Select the Same RoomType !!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }
            //    }
            //}

            this.SubForm = this;
            if (requesterForm.GetType() == typeof(frmReservation))
            {
                ((frmReservation)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();
            }
            else if (requesterForm.GetType() == typeof(frmGroupRegistration))
            {
                if (MultipleRoom)
                    ((frmGroupRegistration)requesterForm).SelectedRoomListHandler.Invoke(GetSelectedRoomList());
                else
                    ((frmGroupRegistration)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();

            }



            if (requesterForm.GetType() == typeof(frmDateAmendment))
            {
                ((frmDateAmendment)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();
            }
            if (requesterForm.GetType() == typeof(frmRoomMove))
            {
                ((frmRoomMove)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();
            }
            if (requesterForm.GetType() == typeof(frmRegistrationDetail))
            {

                ((frmRegistrationDetail)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();
            }
            if (requesterForm.GetType() == typeof(frmMultipleRoomCheckIn))
            {
                ((frmMultipleRoomCheckIn)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();
            }

            if (requesterForm.GetType() == typeof(frmCheckIn))
            {
                ((frmCheckIn)requesterForm).SelectedRoomHandler.Invoke(GetSelectedRoom());

                this.Close();
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

        private void frmRoomSearch_Load(object sender, EventArgs e)
        {
            // Progress_Reporter.Show_Progress("Loading data.Please Wait...");

            InitializeData();

            tc_roomSearch.SelectedTabPage = tp_rooms;
            if (!MultipleRoom)
            {
                gv_roomSearch.OptionsBehavior.Editable = false;
                gCol_Select.Visible = false;
            }

            ////CNETInfoReporter.Hide();
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Dispose();
        }

        private void gv_roomSearch_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();
            if (e.Column.Caption == "HK Status")
            {
                string color = (string)view.GetRowCellValue(e.RowHandle, "RoomDetailView.color");
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml(color);
                if (color == "black" || color == "blue")
                    e.Appearance.ForeColor = System.Drawing.ColorTranslator.FromHtml("White");
                else
                    e.Appearance.ForeColor = System.Drawing.ColorTranslator.FromHtml("Black");
            }
        }

        private void bci_pseudoRoom_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //if (_roomSearchVM != null && pseudoRooms != null) 
            //{
            //    if (bci_pseudoRoom.Checked)
            //    {

            //        List<RoomSearchVM> filtered = _roomSearchVM.Where(r => pseudoRooms.Contains(r.RoomType)).ToList();
            //        gc_roomSearch.DataSource = filtered;
            //        gv_roomSearch.RefreshData();

            //    }
            //    else
            //    {
            //        gc_roomSearch.DataSource = _roomSearchVM;
            //        gv_roomSearch.RefreshData();
            //    }
            //}

        }

        private List<RoomSearchVM> GetNonPseduoRooms()
        {
            List<RoomSearchVM> filtered = new List<RoomSearchVM>();
            if (avRooms != null && pseudoRooms != null)
            {

                filtered = roomSearchVMs.Where(r => pseudoRooms.Contains(r.RoomDetailView.RoomTypeCode) == false).ToList();
                gc_roomSearch.DataSource = filtered;
                gv_roomSearch.RefreshData();


            }

            return filtered;
        }

        private void tc_roomSearch_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

            if (tc_roomSearch.SelectedTabPage == tp_pseudoRooms)
            {
                beiFeature.Enabled = false;
                beiRoom.Enabled = false;
                beiRoomType.Enabled = false;

                if (avRooms != null && pseudoRooms != null)
                {

                    List<RoomSearchVM> filtered = roomSearchVMs.Where(r => pseudoRooms.Contains(r.RoomDetailView.RoomTypeCode)).ToList();
                    gc_roomSearch.DataSource = filtered;
                    gv_roomSearch.RefreshData();


                }
            }
            else if (tc_roomSearch.SelectedTabPage == tp_rooms)
            {
                beiFeature.Enabled = true;
                beiRoom.Enabled = true;
                beiRoomType.Enabled = true;
                if (avRooms != null && pseudoRooms != null)
                {

                    gc_roomSearch.DataSource = GetNonPseduoRooms();
                    gv_roomSearch.RefreshData();


                }
            }
        }

        private void beiFeature_EditValueChanged(object sender, EventArgs e)
        {
            beiFeature.EditValue = null;
            if (selectedRoomFeatures != null && selectedRoomFeatures.Count > 0)
            {
                beiFeature.Edit.NullText = "feature = " + selectedRoomFeatures.Count;
            }
            else
            {
                beiFeature.Edit.NullText = "No feature";

            }

        }

        private void FilterRoomsByFeature(List<VwRoomFeatureViewDTO> selectedFeatures)
        {
            List<string> filterRoomFeatures = new List<string>();
            if (selectedFeatures != null && selectedFeatures.Count > 0)
            {
                foreach (var rmFeature in selectedFeatures)
                {
                    List<string> featureRoomTypes = roomFeatures.Where(f => f.Feature == rmFeature.Feature).Select(f => f.RoomTypeDescription).ToList();
                    if (featureRoomTypes != null && featureRoomTypes.Count > 0)
                        filterRoomFeatures.AddRange(featureRoomTypes);

                }
                if (filterRoomFeatures != null)
                {
                    List<RoomSearchVM> filtered = GetNonPseduoRooms().Where(r => filterRoomFeatures.Contains(r.RoomDetailView.RoomTypeDesc)).ToList();
                    gc_roomSearch.DataSource = filtered;
                    gc_roomSearch.RefreshDataSource();
                }
            }
            else
            {
                gc_roomSearch.DataSource = GetNonPseduoRooms();
                gc_roomSearch.RefreshDataSource();
            }
        }


        private void FilterRoomsByRoomNumber(string roomNumber)
        {
            List<RoomSearchVM> filterd = GetNonPseduoRooms();
            if (filterd == null) return;
            if (!string.IsNullOrEmpty(roomNumber))
            {
                filterd = filterd.Where(r => r.RoomDetailView.RoomDescription.Contains(roomNumber.ToUpper())).ToList();
            }
            gv_roomSearch.BeginUpdate();
            gc_roomSearch.DataSource = filterd;
            gc_roomSearch.RefreshDataSource();
            gv_roomSearch.EndUpdate();

        }

        private void rite_room_EditValueChanged(object sender, EventArgs e)
        {
            TextEdit item = sender as TextEdit;
            if (item == null) return;
            FilterRoomsByRoomNumber(item.Text);
        }

        private void bbiFO_ItemClick(object sender, ItemClickEventArgs e)
        {
            //VwVoucherDetailWithRoomDetailViewDTO selectVM = gv_roomSearch.GetFocusedRow() as VwVoucherDetailWithRoomDetailViewDTO;
            //if (selectVM != null)
            //{
            //    //// get FO status
            //    var foStatus = UIProcessManager.GetRegistrationStatus(selectVM.code, DateTime.Now);
            //    if (foStatus != null)
            //    {
            //        if(foStatus.FOStatus == "0")
            //             XtraMessageBox.Show("FO Status: VACANT", "FO Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        else
            //            XtraMessageBox.Show("FO Status: OCCUPIED", "FO Status", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
            //    }

            //}
        }

        private void beiRoomType_EditValueChanged(object sender, EventArgs e)
        {
            if (beiRoomType.EditValue != null && !string.IsNullOrEmpty(beiRoomType.EditValue.ToString()))
            {
                List<RoomSearchVM> filterd = GetNonPseduoRooms();
                if (filterd == null && filterd.Count == 0) return;
                filterd = filterd.Where(r => r.RoomDetailView.RoomTypeCode == Convert.ToInt32(beiRoomType.EditValue)).ToList();
                gc_roomSearch.DataSource = filterd;
                gv_roomSearch.RefreshData();

            }
        }

        public int? SelectedHotelcode { get; set; }
    }
}
