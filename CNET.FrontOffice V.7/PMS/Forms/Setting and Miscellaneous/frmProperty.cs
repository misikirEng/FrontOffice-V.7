using CNET.ERP.Client.Common.UI;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Setting_and_Miscellaneous.DTO;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CNET.ERP.Client.UI_Logic.PMS.Enum;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Repository;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraMap.Drawing.DirectD3D9;
using CNET.Progress.Reporter;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms
{
    public partial class frmProperty : UILogicBase, ILogicHelper, ICanSave, ICanDelete, ICanCreate
    {
        public List<ConsigneeUnitDTO> OrgAdress;

        #region Fields

        public static Color DefaultTileColor = Color.FromArgb(0x95, 0xD3, 0xF2);

        DevExpress.XtraBars.BarButtonItem bbiBuildingDelete;
        DevExpress.XtraBars.BarButtonItem bbiDeleteFloor;
        DevExpress.XtraBars.BarButtonItem bbiBuildingAdd;
        DevExpress.XtraBars.BarButtonItem bbiAddFloor;

        RoomTypeDTO selectedRoomType = null;

        static int totalNoOfRoomTypes = 2;

        public static Dictionary<String, Color> RoomTypeColor = new Dictionary<string, Color>();

        int intializationTaskCount = 4;
        bool isRoomsLoaded = false;



        List<SpaceDTO> allSpaces = new List<SpaceDTO>();
        List<Floor> allFloors = new List<Floor>();
        List<Room> deletedRooms = new List<Room>();

        Boolean ItemCheckedProcessing = false;

        Floor SelectedFloor = null;
        Boolean ISSelectedBuilding = false;

        Random rand = new Random();

        List<FeatureDataHolder> featureData = new List<FeatureDataHolder>();

        frmSpaceGenerator spaceGenerator = null;

        public Room SelectedSingleRoom = null;

        public List<Room> SelectedRooms = new List<Room>();
        private List<SpaceVM> _spaceVMList = new List<SpaceVM>();

        int digits = 0;
        int roomCheckedCount = 0;
        List<int> HotelStarObjectstateDefList { get; set; }

        List<AmenitiesDTO> RoomAminitiesData { get; set; }
        List<AmenitiesDTO> HotelAminitiesData { get; set; }

        #endregion 

        //************* CONSTRUCTOR ***************//
        public frmProperty()
        {
            InitializeComponent();
        }




        #region Methods
        private bool CheckIfTheUserHasHotelProfileMaintenanceAccess()
        {
            // List<viewFunctWithAccessM> retVal = new List<viewFunctWithAccessM>();

            try
            {
                //String SubSystemComponent = CNETConstantes.Security;
                //string currentRole = "";
                //var role = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.Where(x => x.user == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id).FirstOrDefault();
                //if (role != null)
                //    currentRole = role.role;

                //retVal.AddRange(UIProcessManager.GetFuncwithAccessMatView(currentRole, "Access", SubSystemComponent).Where(x => x.access == true).ToList());
                //if (retVal != null && retVal.FirstOrDefault(x => x.description == "Hotel Profile Maintenance Access") != null)
                //{
                return true;
                //}

            }
            catch (Exception ex)
            {

                return false;
            }
            return false;
        }

        private void RefreshProperty()
        {
            allFloors.Clear();
            LoadBuildingData();
            RefreshTileViewer();
        }

        private void CreateHeaderButtonsMainTree()
        {

            DevExpress.XtraBars.BarSubItem bsiAddBuildingorFloor;
            bsiAddBuildingorFloor = new DevExpress.XtraBars.BarSubItem();
            bsiAddBuildingorFloor.Caption = "Add";
            //bsiAddBuildingorFloor.Glyph = global::CNET.ERP.Client.UI_Logic.PMS.Properties.Resources.add_16x161;
            //bsiAddBuildingorFloor.LargeGlyph = global::CNET.ERP.Client.UI_Logic.PMS.Properties.Resources.add_16x161;
            bsiAddBuildingorFloor.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;

            bbiBuildingAdd = new DevExpress.XtraBars.BarButtonItem();
            bbiAddFloor = new DevExpress.XtraBars.BarButtonItem();
            bbiBuildingAdd.Caption = "Building";
            bbiBuildingAdd.Id = 4;
            bbiAddFloor.Caption = "Floor";
            bbiAddFloor.Id = 3;




            DevExpress.XtraBars.BarSubItem bsiRemoveBuildingorFloor;
            bsiRemoveBuildingorFloor = new DevExpress.XtraBars.BarSubItem();

            bsiRemoveBuildingorFloor.Caption = "Delete";
            //bsiRemoveBuildingorFloor.Glyph = global::CNET.ERP.Client.UI_Logic.PMS.Properties.Resources.delete_16x16;
            //bsiRemoveBuildingorFloor.LargeGlyph = global::CNET.ERP.Client.UI_Logic.PMS.Properties.Resources.delete_16x16;
            bsiRemoveBuildingorFloor.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;


            bbiBuildingDelete = new DevExpress.XtraBars.BarButtonItem();
            bbiDeleteFloor = new DevExpress.XtraBars.BarButtonItem();
            bbiBuildingDelete.Caption = "Building";
            bbiBuildingDelete.Id = 1;
            bbiDeleteFloor.Caption = "Floor";
            bbiDeleteFloor.Id = 2;

        }

        public void InitializeData()
        {
            // Progress_Reporter.Show_Progress("Loading Room Types", intializationTaskCount);

            List<RoomTypeVM> roomTypeListDTOList = new List<RoomTypeVM>();
            RoomTypeVM dto = new RoomTypeVM();

            //List<RoomType> rTypeList = GetRoomTypeData();
            //if (rTypeList != null)
            //{
            //    CreateRoomTypeColors(rTypeList);
            //}

            //int totalRooms = 0;
            //foreach (RoomType rt in rTypeList)
            //{
            //    if (rt.numberOfRooms != null)
            //    {
            //        totalRooms += Convert.ToInt32(rt.numberOfRooms);
            //    }

            //}
            //gc_roomTypeMain.DataSource = rTypeList;


            //if (tcProperty.SelectedTabPage.Name == "rpRoomType")
            //{
            //    Home.ShowStatusBarMessage(this, "Total Number of Rooms = " + totalRooms.ToString());
            //}
            //CreateDemoData();
            // Progress_Reporter.Show_Progress("Loading Room Types", intializationTaskCount);


            List<LookupDTO> RoomfeatureList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.ROOM_FEATURE).ToList(); ;

            if (RoomfeatureList != null)
                RoomAminitiesData = RoomfeatureList.Select(x => new AmenitiesDTO { Id = x.Id, amenities = x.Description }).ToList();

            //clbcFeatures.DataSource = RoomfeatureList;
            //clbcFeatures.DisplayMember = "Description";
            //clbcFeatures.ValueMember = "Id";

            List<LookupDTO> HotelfeatureList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.HOTEL_FEATURE).ToList(); ;

            if (HotelfeatureList != null)
                HotelAminitiesData = HotelfeatureList.Select(x => new AmenitiesDTO { Id = x.Id, amenities = x.Description }).ToList();

            List<SystemConstantDTO> HotelStarList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Category == "Hotel Rating").ToList();

            if (HotelStarList != null)
                HotelStarObjectstateDefList = HotelStarList.Select(x => x.Id).ToList();
            leHotelStars.Properties.DataSource = HotelStarList;
            leHotelStars.Properties.DisplayMember = "Description";
            leHotelStars.Properties.ValueMember = "Id";




            //clbcHotelFeatures.DataSource = HotelfeatureList;
            //clbcHotelFeatures.DisplayMember = "Description";
            //clbcHotelFeatures.ValueMember = "Id";

            cacRoomTypeRooms.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            cacRoomTypeRooms.Properties.Columns.Add(new LookUpColumnInfo("NumberOfRooms", "Total Room Count"));
            cacRoomTypeRooms.Properties.Columns.Add(new LookUpColumnInfo("assignedRooms", "Assigned Room Count"));
            //,new LookUpColumnInfo("abbreviation", "Abb."))
            cacRoomTypeRooms.Properties.DisplayMember = "Description";
            cacRoomTypeRooms.Properties.ValueMember = "Id";

            cacSearchRoomsBy.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            cacSearchRoomsBy.Properties.Columns.Add(new LookUpColumnInfo("numberOfRooms", "Total Room Count"));
            cacSearchRoomsBy.Properties.Columns.Add(new LookUpColumnInfo("assignedRooms", "Assigned Room Count"));
            //,new LookUpColumnInfo("abbreviation", "Abb."))
            cacSearchRoomsBy.Properties.DisplayMember = "Description";
            cacSearchRoomsBy.Properties.ValueMember = "Id";

            Progress_Reporter.Show_Progress("Loading Building and Rooms ", "Please Wait.......");


            tlOrganizationUnit.ParentFieldName = "parentId";
            tlOrganizationUnit.KeyFieldName = "Id";
            tlOrganizationUnit.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.ParentId, Hotel = x.Name }).ToList();
            // LoadBuildingData();

            ////CNETInfoReporter.Hide();

            //CreateBuildingDemoData();


            //cdeTreeRooms.SetFocusedObject(allFloors[0]);

        }

        public SaveClickedResult OnSave()
        {
            return new SaveClickedResult() { SaveResult = Enum.SaveResult.SAVE_SUCESSESFULLY, MessageType = Enum.MessageType.ALLERT };
        }

        public void OnCreate()
        {
            throw new NotImplementedException();
        }

        public void InitializeUI()
        {
            this.CNETFooterRibbon.ribbonControl = rcRoomType;
            Utility.AdjustRibbon(layoutControlItem2);
            Utility.AdjustRibbon(layoutControlItem5, false);


            CreateHeaderButtonsMainTree();

            tList_rooms.ExpandAll();
            sbEditRoom.Click += sbEditRoom_Click;
            tcProperty.SelectedPageChanged += tcProperty_SelectedPageChanged;
            sbAddRoom.Click += sbAddRoom_Click;
            sbDeleteRoom.Click += sbDeleteRoom_Click;
        }



        private void AssignRoomTypeToSelectedRooms(RoomTypeDTO roomType)
        {
            bool isRoomTypeAssigned = false;
            foreach (Room room in SelectedRooms)
            {
                if (!room.SavedObject)
                {
                    room.container = this;
                    room.RoomType = roomType;
                    isRoomTypeAssigned = true;
                }

            }
            if (!isRoomTypeAssigned)
            {
                SystemMessage.ShowModalInfoMessage("Selected Room Type is not assigned to all rooms. Please assign room type by clicking edit button.", "ERROR");
            }
        }
        private void Unselect()
        {
            List<Room> selectedRooms = GetRooms().Where(r => r.IsSelected).ToList();
            foreach (Room room in selectedRooms)
            {
                room.UnSelect();

                RoomCheckedChanged(new TileItemEventArgs() { Item = room.TileItem_ });
            }
            // }
        }

        private void BuildBuildings()
        {
            //if (!string.IsNullOrEmpty(spaceGenerator.OrganizationUnitDef))
            //{
            //    SpaceVM branchVMexist = _spaceVMList.FirstOrDefault(x => x.Code == spaceGenerator.OrganizationUnitDef);
            //    if (branchVMexist == null)
            //    {
            //        OrganizationUnitDefinition Branch = LocalBuffer.LocalBuffer.OrganizationUnitDefinitionBufferList.FirstOrDefault(x => x.code == spaceGenerator.OrganizationUnitDef);
            //        SpaceVM branchVM = new SpaceVM();
            //        branchVM.Code = spaceGenerator.OrganizationUnitDef;
            //        branchVM.Name = Branch.description;
            //        branchVM.ChildFloors = new List<Floor>();
            //        _spaceVMList.Add(branchVM);
            //    }
            //}
            if (_spaceVMList != null && _spaceVMList.Count > 0)
                _spaceVMList.Clear();
            SpaceVM blgVM = new SpaceVM();

            blgVM.Id = Guid.NewGuid().ToString();
            blgVM.Name = spaceGenerator.teName.Text;
            blgVM.ChildFloors = new List<Floor>();
            blgVM.ParentCode = spaceGenerator.ConsigneeUnit.ToString();


            Building building = new Building()
            {
                ID = Guid.NewGuid().ToString(),
                SavedObject = false,
                organazationUnitDefn = spaceGenerator.ConsigneeUnit,
                Name = spaceGenerator.teName.Text

            };

            int currentFloorCount = allFloors.Count();
            int totalRooms = allFloors.SelectMany(u => u.Rooms).Count();
            //Create Floors

            int floorCount = Convert.ToInt32(spaceGenerator.seNoOfFloors.Value);
            int roomCount = Convert.ToInt32(spaceGenerator.seRoomsonEach.Value);

            int floorStartsFrom = Convert.ToInt32(spaceGenerator.seFloorStartsFrom.Value);
            int roomNoStartsFrom = Convert.ToInt32(spaceGenerator.seRoomNoStartsFrom.Value);

            digits = Convert.ToInt32(spaceGenerator.seRoomNoDigit.Value);

            //  String buildingCode = spaceGenerator.teCode.Text;
            String buildingName = spaceGenerator.teName.Text;

            String floorPrefix = "Floor - ";
            String roomPrefix = "Room - ";
            String floorPostfix = String.Empty;
            String roomPostfix = String.Empty;

            // In Order to reset the previous Catched data.
            currentFloorCount = 0;

            if (roomNoStartsFrom == 0) roomNoStartsFrom = totalRooms;



            for (int i = 1; i <= floorCount; i++)
            {

                currentFloorCount++;
                Floor floor = new Floor();
                floor.ID = Guid.NewGuid().ToString();
                floor.Digit = digits;
                floor.Name = (floorPrefix ?? String.Empty) + (currentFloorCount.ToString());
                floor.Code = "FloorCode" + currentFloorCount.ToString();
                floor.Number = currentFloorCount;

                building.AddFloor(floor);
                allFloors.Add(floor);

                SpaceVM flrVM = new SpaceVM();
                flrVM.Id = Guid.NewGuid().ToString();
                flrVM.FakeCode = "FloorCode" + currentFloorCount.ToString();
                flrVM.Name = (floorPrefix ?? String.Empty) + (currentFloorCount.ToString());
                flrVM.FakeParentCode = blgVM.FakeCode;
                flrVM.Floor = floor;
                //flrVM.Parent = blgVM.Code;

                _spaceVMList.Add(flrVM);

                blgVM.ChildFloors.Add(floor);

                //Add Rooms

                if (i < floorStartsFrom) continue;

                for (int j = roomNoStartsFrom; j < roomCount + roomNoStartsFrom; j++)
                {
                    //Home.ShowStatusBarMessage(this, "Creating the " + (j - roomNoStartsFrom + 1).ToString() + "th Room");
                    ShowStatusBarMessage("Creating the " + (j - roomNoStartsFrom + 1).ToString() + "th Room");

                    CreateRoom(currentFloorCount, digits, floor, j);
                }
                //Home.ShowStatusBarMessage(this, "");
                ShowStatusBarMessage("");
            }

            _spaceVMList.Add(blgVM);
            RefreshBuildingTree();


        }


        private Room CreateRoom(int currentFloorCount, int digits, Floor floor, int index)
        {
            Room room = new Room();
            room.ID = Guid.NewGuid().ToString();
            room.isActive = true;
            room.Code = "roomCode" + index.ToString();

            room.AddFloor(floor);
            floor.Number = currentFloorCount;

            room.RoomNo = BuildRoomNo(currentFloorCount.ToString(), index.ToString(), digits);

            room.Name = floor.Name + "- Room" + room.RoomNo;
            // room.RoomDetail.remark = spaceGenerator.meRemark.Text;

            return room;

        }
        private Room CreateRoom(int roomNumber, Floor floor)
        {
            Room room = new Room();
            room.ID = Guid.NewGuid().ToString();
            room.isActive = true;
            //  room.Code = "roomCode" + index.ToString();

            room.AddFloor(floor);



            room.RoomNo = roomNumber.ToString();

            room.Name = floor.Name + "- Room" + room.RoomNo;
            // room.RoomDetail.remark = spaceGenerator.meRemark.Text;

            return room;

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

        private static void CreateRoomTypeColors(List<RoomTypeDTO> roomTypes)
        {
            if (!roomTypes.Any())
            {
                return;
            }
            totalNoOfRoomTypes = roomTypes.Count();
            Random rand = new Random();

            int decrementValue = rand.Next(20, 25);

            Color currentColor = Color.FromArgb(DefaultTileColor.R, DefaultTileColor.G, DefaultTileColor.B);

            RoomTypeColor.Clear();

            foreach (RoomTypeDTO roomtype in roomTypes)
            {
                int newR = Convert.ToInt32(currentColor.R) - decrementValue;
                int newG = Convert.ToInt32(currentColor.G) - decrementValue;
                int newB = Convert.ToInt32(currentColor.B) - decrementValue;

                if (newR <= 0) newR = frmProperty.DefaultTileColor.G;
                if (newG <= 0) newG = frmProperty.DefaultTileColor.R;
                if (newB <= 0) newB = frmProperty.DefaultTileColor.B;

                currentColor = Color.FromArgb(newR, newG, newB);
                if (currentColor == Color.DarkRed)
                {
                    currentColor = DefaultTileColor;
                }
                try //IF SAME TYPE ARE DEFINED
                {

                    RoomTypeColor.Add(roomtype.Description, currentColor);
                }
                catch { }

            }


        }



        public Color GetRoomTypeColor(String roomType)
        {
            Color color = DefaultTileColor;

            RoomTypeColor.TryGetValue(roomType, out color);
            if (color.IsEmpty) color = DefaultTileColor;

            return color;

        }

        public List<Building> GetBuildings()
        {

            return allFloors.Select(u => u.Building_).Distinct<Building>().ToList<Building>();
        }

        public void PopulateCacRoomType(List<RoomTypeDTO> rTypeList)
        {

            List<RoomTypeVM> roomTypeCountList = new List<RoomTypeVM>();
            RoomTypeVM dtoRoom = new RoomTypeVM();
            foreach (RoomTypeDTO rt in rTypeList)
            {
                dtoRoom = new RoomTypeVM();
                dtoRoom.Id = rt.Id;
                dtoRoom.Description = rt.Description;
                dtoRoom.NumberOfRooms = rt.NumberOfRooms;
                dtoRoom.assignedRooms = GetRoomTypeCount(rt.Description);
                roomTypeCountList.Add(dtoRoom);

            }
            cacRoomTypeRooms.Properties.DataSource = (roomTypeCountList);
            cacSearchRoomsBy.Properties.DataSource = (roomTypeCountList);

        }

        public List<Room> GetRooms()
        {
            return allFloors.SelectMany(u => u.Rooms).Distinct<Room>().ToList<Room>();
        }



        public bool DeleteRoom(Room room)
        {
            List<RegistrationDetailDTO> regDetails = new List<RegistrationDetailDTO>();
            bool isDeleted = false;
            if (room.RoomDetail != null)
            {
                regDetails = UIProcessManager.GetRegistrationDetailByroom(room.RoomDetail.Id);
            }
            if (regDetails == null || regDetails.Count == 0)
            {

                deletedRooms.Add(room);


                isDeleted = true;



                tList_rooms.Refresh();
            }
            else
            {
                isDeleted = false;
                SystemMessage.ShowModalInfoMessage("You can not delete this room since registration is made with it.", "ERROR");

            }
            return isDeleted;
        }

        private void RefreshTileViewer()
        {
            int row = tList_rooms.GetNodeIndex(tList_rooms.FocusedNode);
            PopulateTileControl(row);

        }

        private void RefreshBuildingTree()
        {
            // tList_rooms.DataSource = allFloors;
            tList_rooms.DataSource = _spaceVMList;
            tList_rooms.RefreshDataSource();
            tList_rooms.ExpandAll();

        }
        private void PopulateTileControl(int focusedRowIndex)
        {

            cbeSelectUnselect.Text = String.Empty;
            SpaceVM spVM = (SpaceVM)tList_rooms.GetDataRecordByNode(tList_rooms.FocusedNode);
            if (spVM == null) return;

            if (spVM.ParentCode == null && spVM.ChildFloors != null && spVM.ChildFloors.Count == 0)
            {
                if (String.IsNullOrEmpty(spVM.FakeCode))
                {
                    List<RoomTypeDTO> roomTList = GetRoomTypeData(SelectedHotelcode);
                    PopulateCacRoomType(roomTList);
                }
                sbAddRoom.Enabled = false;
                sbAddRoom.ToolTip = "Select a Floor to Add a Room";
                ISSelectedBuilding = true;
                ClearRoomTiles();
            }
            else
            if (spVM.ParentCode == null && spVM.ChildFloors != null && spVM.ChildFloors.Count > 0) // it is a group
            {
                ISSelectedBuilding = true;

                //ISSelectedBuilding = true;

                SelectedFloor = spVM.ChildFloors.FirstOrDefault();
                if (SelectedFloor == null) return;
                if (SelectedFloor.Building_ != null)
                {
                    PopulateRoomsTiles(SelectedFloor.Building_);
                }

                sbAddRoom.Enabled = false;
                sbAddRoom.ToolTip = "Select a Floor to Add a Room";


            }
            else if (spVM.Floor != null) //Floor is selected
            {

                ISSelectedBuilding = false;

                SelectedFloor = spVM.Floor;
                if (spVM.Parent != null && spVM.Parent.ConsigneeUnit > 0)
                {
                    List<RoomTypeDTO> roomTList = GetRoomTypeData(spVM.Parent.ConsigneeUnit);
                    PopulateCacRoomType(roomTList);
                }
                PopulateRoomsTiles(SelectedFloor);

                sbAddRoom.Enabled = true;
                sbAddRoom.ToolTip = "Add a Room to " + SelectedFloor.Name;


            }
            else if (spVM.Floor == null) //Floor is selected
            {
                if (spVM.ChildFloors != null)
                {
                    SelectedFloor = spVM.ChildFloors.FirstOrDefault();
                    if (SelectedFloor == null) return;
                    if (SelectedFloor.Building_ != null)
                    {
                        PopulateRoomsTiles(SelectedFloor.Building_);
                    }
                }
                sbAddRoom.Enabled = false;
                sbAddRoom.ToolTip = "Select a Floor to Add a Room";
                //  ClearRoomTiles();
            }
        }

        private void PopulateRoomsTiles(Floor floor)
        {
            if (floor == null)
                return;

            ClearRoomTiles();

            this.tcRooms.Groups.Add(this.tgPrimary);

            tgPrimary.Text = floor.Name;

            foreach (Room room in floor.Rooms)
                tgPrimary.Items.Add(CreateTileRoom(room));


        }

        private void PopulateRoomsTiles(List<Floor> floors)
        {

            ClearRoomTiles();
            foreach (Floor floor in floors)
            {
                TileGroup group = CreateGroup(floor);

                foreach (Room room in floor.Rooms)
                {
                    if (room.TileItem_ == null)
                        room.TileItem_ = CreateTileRoom(room);

                    group.Items.Add(room.TileItem_);
                }

                this.tcRooms.Groups.Add(group);
            }

        }
        private void ClearRoomTiles()
        {

            tgPrimary.Items.Clear();

            tcRooms.Groups.Clear();

        }

        private void PopulateRoomsTiles(Building building)
        {

            ClearRoomTiles();

            foreach (Floor floor in building.floors)
            {
                //create Group

                CreateFloorRooms(floor);

            }


        }

        private void CreateFloorRooms(Floor floor)
        {

            TileGroup group = CreateGroup(floor);

            foreach (Room room in floor.Rooms)
            {

                if (room.TileItem_ == null)
                    room.TileItem_ = CreateTileRoom(room);

                group.Items.Add(room.TileItem_);


            }

            this.tcRooms.Groups.Add(group);

        }

        private TileGroup CreateGroup(Floor floor)
        {
            DevExpress.XtraEditors.TileGroup group;
            group = new DevExpress.XtraEditors.TileGroup();
            group.Text = floor.Building_.Name + " - " + floor.Name;
            return group;

        }

        private void AddRooms(Floor floor)
        {
            int initial = rand.Next(20);
            int count = rand.Next(5, 10);

            List<Room> rooms = new List<Room>();

            for (int i = 0; i < count; i++)
            {
                Room room = new Room();
                room.Code = "Id" + i.ToString();
                room.AddFloor(floor);
                room.RoomNo = initial.ToString() + i.ToString();
                room.Name = "Room " + room.RoomNo;
            }
        }
        public TileItem CreateTileRoom(Room room)
        {
            DevExpress.XtraEditors.TileItem tiRoom;
            tiRoom = new DevExpress.XtraEditors.TileItem();

            DevExpress.XtraEditors.TileItemElement tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();

            tiRoom.AppearanceItem.Normal.BackColor = DefaultTileColor;
            tiRoom.AppearanceItem.Normal.BorderColor = DefaultTileColor;
            tiRoom.AppearanceItem.Normal.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tiRoom.AppearanceItem.Normal.Options.UseBackColor = true;
            tiRoom.AppearanceItem.Normal.Options.UseBorderColor = true;
            if (!room.isActive)
            {
                tiRoom.AppearanceItem.Normal.ForeColor = Color.DarkRed;
            }
            tiRoom.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement1.Appearance.Normal.Options.UseTextOptions = true;
            tileItemElement1.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            tileItemElement1.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopRight;
            tileItemElement1.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.Stretch;
            tileItemElement1.ImageSize = new System.Drawing.Size(70, 40);
            //if (room.RoomType!=null)
            //{
            //    tileItemElement1.Text = UIProcessManager.SelectRoomType(room.RoomType).description;
            //}
            //else
            //{
            //tileItemElement1.Text = room.Feature;
            //}


            //    if (String.IsNullOrEmpty(value.description))
            //        tileItem_.Elements[0].Text = "";
            //    else
            //        tileItem_.Elements[0].Text = "Type: " + value.description;//       . Aggregate((u, y) => u + "," + y).ToString();

            //tileItem_.AppearanceItem.Normal.BackColor = Random();
            //tileItem_.AppearanceItem.Normal.Options.UseBackColor = true;



            tileItemElement1.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomCenter;
            tileItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tileItemElement2.Appearance.Normal.Options.UseFont = true;
            tileItemElement2.Text = room.RoomNo;
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
            tiRoom.Elements.Add(tileItemElement1);
            tiRoom.Elements.Add(tileItemElement2);
            tiRoom.Id = 1;
            tiRoom.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            tiRoom.Name = "tileItem2";

            tiRoom.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tcRooms_Click);

            tiRoom.CheckedChanged += tiRoom_CheckedChanged;

            tiRoom.Tag = room;
            room.TileItem_ = tiRoom;


            if (room.IsSelected) room.Select();
            else room.UnSelect();


            room.RoomType = room.RoomType;


            return tiRoom;


        }



        private void ChangeTileCheckedStatus(TileItemEventArgs e)
        {
            //ItemCheckedProcessing = true;
            //     e.Item.Checked = !e.Item.Checked;
            //ItemCheckedProcessing = false;

        }

        private List<RoomTypeDTO> GetRoomTypeData(int selectedBranch)
        {
            if (selectedBranch == 0)
            {
                return null;
            }
            List<RoomTypeDTO> rTypeList = new List<RoomTypeDTO>();

            rTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(selectedBranch);
            int totalRooms = 0;
            foreach (RoomTypeDTO rt in rTypeList)
            {
                if (rt.NumberOfRooms != null)
                    if (rt.NumberOfRooms != null)
                    {
                        totalRooms += Convert.ToInt32(rt.NumberOfRooms);
                    }

            }
            // Home.ShowStatusBarMessage(this, "Total Number of Rooms = " + totalRooms.ToString());
            ShowStatusBarMessage("Total Number of Rooms = " + totalRooms.ToString());
            return rTypeList;
        }


        public void RefreshRoomTypeGrid()
        {
            gc_roomTypeMain.DataSource = null;
            gc_roomTypeMain.RefreshDataSource();

            var data = GetRoomTypeData(SelectedHotelcode);

            gc_roomTypeMain.BeginUpdate();
            gc_roomTypeMain.DataSource = data;

            gv_roomTypeMain_FocusedRowChanged(null, null);
            gc_roomTypeMain.RefreshDataSource();
            gc_roomTypeMain.EndUpdate();

        }

        private void RefreshHeaderButtonsEnabledState()
        {

            SelectedRooms.Clear();

            roomCheckedCount = 0;

            int? commonRoomType = null;

            List<Room> allRooms = GetRooms();

            if (allRooms.Any())
            {
                if (allRooms.Where(u => u.IsSelected).Any())
                {
                    var roomType = allRooms.Select(u => u.RoomType).FirstOrDefault();


                    if (roomType == null) commonRoomType = null;
                    else
                        commonRoomType = roomType.Id;
                }

                else
                    commonRoomType = null;
            }

            foreach (Room room in allRooms)
            {
                if (room.IsSelected)
                {
                    SelectedRooms.Add(room);

                    if (room.RoomType != null)
                    {
                        if (commonRoomType != room.RoomType.Id)
                            commonRoomType = null;
                    }
                    else
                        commonRoomType = null;

                    roomCheckedCount++;
                }
            }

            lciSelection.Text = @"Selection (" + roomCheckedCount.ToString() + @") ";

            cbeSelectUnselect.Reset();

            //if (String.IsNullOrEmpty(commonRoomType))
            //    cacRoomTypeRooms.EditValue = "";
            //else
            //    cacRoomTypeRooms.EditValue = commonRoomType;

            if (roomCheckedCount == 0)
            {
                sbEditRoom.Enabled = false;
                sbDeleteRoom.Enabled = false;

            }
            else if (roomCheckedCount == 1)
            {

                sbEditRoom.Enabled = true;
                sbDeleteRoom.Enabled = true;
            }
            else
            {
                sbEditRoom.Enabled = false;
                sbDeleteRoom.Enabled = true;

            }
            string rooms = "";

            foreach (Room rm in SelectedRooms)
            {

                rooms += rm.RoomNo + "  ";

            }
            // Home.ShowStatusBarMessage(this, "Selected Rooms: " + rooms + "  Total Selected Rooms Count: " + SelectedRooms.Count);
            ShowStatusBarMessage("Selected Rooms: " + rooms + "  Total Selected Rooms Count: " + SelectedRooms.Count);

        }


        public DeleteClickedResult OnDelete()
        {
            // List<RoomType> deletedRoomtypes = cdeRoomTypeMain.Logic.GetDeletedRows().Cast<RoomType>().ToList();
            RoomTypeDTO RemoveRoomType = gv_roomTypeMain.GetFocusedRow() as RoomTypeDTO;
            bool canDelete = true;
            List<RoomFeatureDTO> featureList = UIProcessManager.GetRoomFeaturesByreference(RemoveRoomType.Id).ToList();
            if (featureList.Count > 0)
            {
                //foreach (RoomFeature rm in featureList)
                //{

                //    // UIProcessManager.DeleteRoomFeature(rm.code);

                //}
                canDelete = false;
            }

            List<RoomDetailDTO> roomDetailList = UIProcessManager.GetRoomDetailByroomType(RemoveRoomType.Id).ToList();
            if (roomDetailList.Count > 0)
            {
                //foreach (RoomDetail rd in roomDetailList)
                //{

                //    //  UIProcessManager.DeleteRoomFeature(rd.code);

                //}
                canDelete = false;
            }
            List<RateCodeRoomTypeDTO> rateCodeRoomList = UIProcessManager.GetRateCodeRoomTypeByroomType(RemoveRoomType.Id).ToList();
            if (rateCodeRoomList.Count > 0)
            {
                //foreach (RateCodeRoomType rt in rateCodeRoomList)
                //{

                //    //  UIProcessManager.DeleteRateCodeRoomType(rt.code);

                //}
                canDelete = false;
            }
            List<RateCodeDetailRoomTypeDTO> rateCodeDetailRoomList = UIProcessManager.GetRateCodeDetailRoomTypeByroomType(RemoveRoomType.Id).ToList();
            if (rateCodeDetailRoomList.Count > 0)
            {
                //foreach (RateCodeDetailRoomType rc in rateCodeDetailRoomList)
                //{

                //    // UIProcessManager.DeleteRateCodeRoomType(rc.code);

                //}
                canDelete = false;
            }
            List<RoomTypeDTO> roomTypeList = UIProcessManager.GetRoomTypeBycomponentRoom(RemoveRoomType.Id).ToList();
            if (roomTypeList.Count > 0)
            {
                //foreach (RoomType rc in roomTypeList)
                //{


                //    // UIProcessManager.DeleteRoomType(rc.code);

                //}
                canDelete = false;
            }
            if (canDelete)
            {
                if (UIProcessManager.DeleteRoomTypeById(RemoveRoomType.Id))
                {

                    SystemMessage.ShowModalInfoMessage("Deleted Successfully", "MESSAGE");
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Deleting was unsuccessfully", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Can not be deleted. It has other transaction", "ERROR");

            }
            RefreshRoomTypeGrid();
            return new DeleteClickedResult()
            {
                MessageType = MessageType.MESSAGEBOX,
                DeleteResult = DeleteResult.DELETE_THENSHOWNOTHING
            };
        }

        private void PopulateSpaceList()
        {
            _spaceVMList.Clear();
            allSpaces.Clear();
            allSpaces = UIProcessManager.SelectAllSpace();
        }

        private void LoadBuildingData()
        {
            PopulateSpaceList();
            List<SpaceDTO> blgList = allSpaces.Where(s => s.Type == CNETConstantes.Space_Blook).ToList();
            if (SelectedHotelcode > 0)
            {
                blgList = blgList.Where(x => x.ConsigneeUnit == SelectedHotelcode).ToList();
            }

            foreach (SpaceDTO sp in blgList)
            {
                //if (!string.IsNullOrEmpty(sp.OrganizationUnitDef))
                //{  
                //    SpaceVM branchVMexist = _spaceVMList.FirstOrDefault(x => x.Code == sp.OrganizationUnitDef);
                //    if (branchVMexist == null)
                //    {
                //        OrganizationUnitDefinition Branch = LocalBuffer.LocalBuffer.OrganizationUnitDefinitionBufferList.FirstOrDefault(x => x.code == sp.OrganizationUnitDef);

                //        SpaceVM branchVM = new SpaceVM();
                //        branchVM.Code = sp.OrganizationUnitDef;
                //        branchVM.Name = Branch.description;
                //        branchVM.ChildFloors = new List<Floor>();
                //        _spaceVMList.Add(branchVM);

                //    }
                //}

                List<SpaceDTO> flrList = allSpaces.Where(s => s.ParentId == sp.Id && s.Type == CNETConstantes.Space_Floor).ToList();
                int floorcount = flrList.Count();

                SpaceVM blgVM = new SpaceVM();
                blgVM.Id = sp.Id.ToString();
                blgVM.Name = sp.Description;
                blgVM.ChildFloors = new List<Floor>();
                blgVM.ParentCode = sp.ConsigneeUnit.ToString();

                foreach (SpaceDTO flr in flrList)
                {
                    //////CNETInfoReporter.WaitFormChild("Loading Floor: " + flr.Description, intializationTaskCount, floorcount);

                    List<SpaceDTO> roomList = allSpaces.Where(s => s.ParentId == flr.Id && s.Type == CNETConstantes.Space_Room).ToList();

                    Floor floor = AddFloor(flr, sp);
                    blgVM.ChildFloors.Add(floor);

                    if (roomList.Any())
                    {
                        foreach (SpaceDTO rm in roomList)
                        {
                            AddRoom(rm, floor);
                        }

                    }

                    SpaceVM flrVM = new SpaceVM();
                    flrVM.Id = flr.Id.ToString();
                    flrVM.FakeCode = flr.Id.ToString();
                    flrVM.Name = floor.FullName;
                    flrVM.FakeParentCode = sp.Id.ToString();
                    flrVM.ParentCode = blgVM.Id;
                    flrVM.Floor = floor;
                    flrVM.Parent = sp;

                    _spaceVMList.Add(flrVM);


                }// end of foreach: flrList


                _spaceVMList.Add(blgVM);

            }//end of foreach: blgList

            RefreshBuildingTree();



            return;

        }

        #region  Room Data Loading From Service to Control Objects
        #region Data Setter


        public Building CreateBuilding(SpaceDTO building)
        {
            Building blg = new Building();
            blg.Code = building.Id;

            if (allFloors.Select(u => u.Building_).ToList<Building>().Contains(blg)) return allFloors.Select(x => x.Building_).Where(u => u.Code == blg.Code).FirstOrDefault();


            blg.Name = building.Description;
            blg.Space = building;
            blg.SavedObject = true;

            return blg;



        }


        public Floor AddFloor(SpaceDTO floor, SpaceDTO parentBuilding)
        {
            Floor flr = new Floor();
            flr.ID = floor.Id.ToString();

            if (allFloors.Contains(flr)) return allFloors.FirstOrDefault(u => u.Equals(flr));

            flr.Name = floor.Description;
            flr.Building_ = CreateBuilding(parentBuilding);
            flr.Building_.AddFloor(flr);
            flr.Space = floor;

            flr.SavedObject = true;
            flr.BuildingName = flr.Building_ != null ? flr.Building_.Name : "";

            allFloors.Add(flr);

            return flr;
        }


        public void AddRoom(SpaceDTO room, Floor floor)
        {


            Room room_ = CreateRoom(floor.Number, floor.Digit, floor, floor.Rooms.Count() + 1);
            room_.RoomNo = room.Description;
            room_.SavedObject = true;
            RoomDetailDTO rd = UIProcessManager.GetRoomDetailByspace(room.Id);
            room_.RoomDetail = rd;
            room_.container = this;
            if (rd != null)
            {
                room_.Code = rd.PhoneNumber;
                if (rd.IsActive != null) room_.isActive = (bool)rd.IsActive;
                if (rd.RoomType > 0)
                {
                    RoomTypeDTO rt = UIProcessManager.GetRoomTypeById(rd.RoomType);
                    if (rt != null)
                    {
                        room_.RoomType = rt;
                    }
                }
            }

            room_.Space = room;



        }

        #endregion



        #region Data Getters

        public List<Building> GetAddedBuildings()
        {

            return GetBuildings().Where(u => u.SavedObject == false).ToList<Building>();

            //TO DO: After Save set all Building savedObject to true
        }

        public List<Floor> AddedFloors()
        {
            return allFloors.Where(u => u.SavedObject == false).ToList<Floor>();

            //TODO : call SetSavedStatusForFloors after saving
        }

        public List<Floor> DeletedFloors()
        {

            //TODo:
            return new List<Floor>();
        }

        public void SetSavedStatusForFloors()
        {
            allFloors.Where(u => u.SavedObject == false).ToList<Floor>().ForEach(u => u.SavedObject = true);

        }

        public List<Room> AddedRooms()
        {
            return allFloors.SelectMany(u => u.Rooms).Where(u => u.SavedObject == false).ToList<Room>();


        }


        public List<Room> DeletedRooms()
        {
            List<Room> list = deletedRooms.Where(u => u.SavedObject == true).ToList<Room>();

            return list;

            //TODO: clear the deletedRooms List after Saving the changes
        }


        public void SetSavedStatusForRooms()
        {
            GetRooms().Where(u => u.SavedObject == false).ToList<Room>().ForEach(u => u.SavedObject = true);

        }


        #endregion




        #endregion

        #endregion


        #region Event Handlers 

        private void tiRoom_CheckedChanged(object sender, TileItemEventArgs e)
        {
            //RoomCheckedChanged(e);

            //ChangeTileCheckedStatus(e);

        }

        private void sbApplyRoomType_Click(object sender, EventArgs e)
        {
            if (SelectedRooms.Count == 0 || selectedRoomType == null)
            {
                SystemMessage.ShowModalInfoMessage("No room selected or no room type selected!!!", "ERROR");
            }
            else
            {
                if (GetRoomTypeCount(selectedRoomType.Description) + SelectedRooms.Count > selectedRoomType.NumberOfRooms)
                {

                    SystemMessage.ShowModalInfoMessage("You can not assign more than " + selectedRoomType.NumberOfRooms + " Rooms to a Room Type: " + selectedRoomType.Description, "ERROR", "Selected Rooms Count Exceed");

                    cacRoomTypeRooms.EditValue = "";

                }
                else
                {
                    AssignRoomTypeToSelectedRooms(selectedRoomType);
                    List<RoomTypeDTO> rTypeList = GetRoomTypeData(SelectedHotelcode);
                    PopulateCacRoomType(rTypeList);

                }

                Unselect();
                // Home.ShowStatusBarMessage(this, "RoomType: " + selectedRoomType.description + "=" + selectedRoomType.numberOfRooms + "/" + GetRoomTypeCount(selectedRoomType.description));

                ShowStatusBarMessage("RoomType: " + selectedRoomType.Description + "=" + selectedRoomType.NumberOfRooms + "/" + GetRoomTypeCount(selectedRoomType.Description));
                // selectedRoomType = null;
            }
        }
        public void ShowStatusBarMessage(string message)
        {
            tstRoomStatus.Text = message;
        }
        private void sbDeleteRoom_Click(object sender, EventArgs e)
        {

            //  List<Room> rooms = GetRooms();
            foreach (Room room in SelectedRooms)
            {
                //if (room.IsSelected)
                if (DeleteRoom(room))
                {
                    if (room.SavedObject)
                    {
                        if (room.RoomDetail != null)
                        {
                            if (UIProcessManager.DeleteRoomDetailById(room.RoomDetail.Id))
                            {
                                if (room.Space.Id > 0)
                                {
                                    if (UIProcessManager.DeleteSpaceById(room.Space.Id))
                                    {
                                        //Home.ShowModalInfoMessage("Deleted Successfully!","MESSAGE");
                                        room.Floor_.DeleteRoom(room);
                                        RefreshTileViewer();
                                    }
                                    else
                                    {
                                        SystemMessage.ShowModalInfoMessage("Not deleted since it has other transaction!", "ERROR");
                                    }
                                    //  isSaved = true;
                                }
                            }
                        }
                        else
                        {
                            if (room.Space.Id > 0)
                            {
                                if (UIProcessManager.DeleteSpaceById(room.Space.Id))
                                {
                                    // Home.ShowModalInfoMessage("Deleted Successfully!", "MESSAGE");
                                }
                                else
                                {
                                    SystemMessage.ShowModalInfoMessage(room.RoomNo + " not deleted since it has other transaction!", "ERROR");
                                }

                                //  isSaved = true;
                            }
                        }
                    }
                    else
                    {
                        deletedRooms.Add(room);
                        room.Floor_.DeleteRoom(room);
                        RefreshTileViewer();
                    }
                }
            }
            tList_rooms.RefreshDataSource();
            tList_rooms.ExpandAll();

            RefreshHeaderButtonsEnabledState();
            //tlOrganizationUnit_FocusedNodeChanged(null,null);
            List<RoomTypeDTO> roomTList = GetRoomTypeData(SelectedHotelcode);
            PopulateCacRoomType(roomTList);
        }

        void sbAddRoom_Click(object sender, EventArgs e)
        {

            if (!ISSelectedBuilding)
            {
                //Get the maximum Room No
                //String max =  SelectedFloor.Rooms.Select(u=>u.RoomNo).OrderBy<String,String>(u=>u).LastOrDefault();

                //int roomNo = Convert.ToInt16(max) + 1;
                //

                if (SelectedFloor.Space != null)
                {
                    string floorNumber = SelectedFloor.Space.Remark;
                    SelectedFloor.Number = int.Parse(floorNumber);
                }
                int NextRoomNumber = 0;
                if (SelectedFloor.Rooms.Count == 0)
                {
                    Building selectedBlg = SelectedFloor.Building_;
                    Room rm = selectedBlg.floors.SelectMany(u => u.Rooms).FirstOrDefault();
                    int floDigit = 0;
                    floDigit = rm != null ? rm.RoomNo.Length : 3;
                    // string FloorLastR.oomNumber = "0";
                    // NextRoomNumber = int.Parse(FloorLastRoomNumber) + 1;
                    CreateRoom(SelectedFloor.Number, floDigit, SelectedFloor, SelectedFloor.Rooms.Count() + 1);
                }
                else
                {
                    string FloorLastRoomNumber = SelectedFloor.Rooms.Last().RoomNo;
                    string resultString = Regex.Match(FloorLastRoomNumber, @"\d+").Value;
                    int.TryParse(resultString, out NextRoomNumber);
                    NextRoomNumber = NextRoomNumber + 1;
                    CreateRoom(NextRoomNumber, SelectedFloor);
                }


                RefreshTileViewer();

                tList_rooms.RefreshDataSource();
                tList_rooms.ExpandAll();

            }

            RefreshHeaderButtonsEnabledState();

        }

        void tcProperty_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            List<RoomTypeDTO> rTypeList = GetRoomTypeData(SelectedHotelcode);

            if (tcProperty.SelectedTabPage.Name == "rpRoomType")
            {
                int totalRooms = 0;
                foreach (RoomTypeDTO rt in rTypeList)
                {
                    if (rt.NumberOfRooms != null)
                    {
                        totalRooms += Convert.ToInt32(rt.NumberOfRooms);
                    }

                }
                //  Home.ShowStatusBarMessage(this, "Total Number of Rooms = " + totalRooms.ToString());
                ShowStatusBarMessage("Total Number of Rooms = " + totalRooms.ToString());
            }
            else if (tcProperty.SelectedTabPage.Name == "tpRooms")
            {

                // Progress_Reporter.Show_Progress("Loading Data");
                ribbonPageGroup5.Visible = false;
                // if (!isRoomsLoaded)
                // {
                LoadBuildingData();

                // }
                isRoomsLoaded = true;
                PopulateCacRoomType(rTypeList);

                // Home.ShowStatusBarMessage(this, "");
                ShowStatusBarMessage("");
                ////CNETInfoReporter.Hide();
            }
        }

        public int GetRoomTypeCount(String roomTypeDescription)
        {
            int rCo = GetRooms().Count;
            var rooms = GetRooms().Where(u => u.RoomType != null && u.RoomType.Description.Equals(roomTypeDescription)).ToList();

            return rooms.Count;

        }

        private void gv_roomTypeMain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            RoomfeatureList = new List<RoomFeatureDTO>();
            List<HistoryDTO> dtoList = new List<HistoryDTO>();
            List<HistoryDTO> dtoFeature = new List<HistoryDTO>();
            HistoryDTO dtoF = new HistoryDTO();

            if (RoomAminitiesData != null)
                RoomAminitiesData.ForEach(x => x.select = false);

            RoomTypeDTO rt = gv_roomTypeMain.GetFocusedRow() as RoomTypeDTO;

            if (rt != null)
                RoomfeatureList = UIProcessManager.GetRoomFeaturesByreference(rt.Id).ToList();


            RoomFeatureDTO roomlam = null;
            if (RoomAminitiesData != null)
            {
                foreach (AmenitiesDTO rf in RoomAminitiesData)
                {
                    if (RoomfeatureList != null)
                        roomlam = RoomfeatureList.FirstOrDefault(x => x.Feature == rf.Id);
                    if (roomlam != null)
                    {
                        rf.Description = roomlam.Note;
                        rf.select = true;
                    }
                    else
                    {
                        rf.Description = "";
                    }
                }
            }
            gcRoomAmenities.DataSource = RoomAminitiesData;
            gcRoomAmenities.RefreshDataSource();
        }

        List<RoomFeatureDTO> HotelFeatureList { get; set; }
        public void LoadHotelFeatures()
        {
            HotelFeatureList = new List<RoomFeatureDTO>();
            if (HotelAminitiesData != null)
                HotelAminitiesData.ForEach(x => x.select = false);

            RoomFeatureDTO hotelam = null;
            HotelFeatureList = UIProcessManager.GetRoomFeaturesByreference(SelectedHotelcode).ToList();

            if (HotelAminitiesData != null)
            {
                foreach (AmenitiesDTO rf in HotelAminitiesData)
                {
                    if (HotelFeatureList != null)
                        hotelam = HotelFeatureList.FirstOrDefault(x => x.Feature == rf.Id);

                    if (hotelam != null)
                    {
                        rf.select = true;
                        rf.Description = hotelam.Note;
                    }
                    else
                    {
                        rf.Description = "";
                    }
                }
            }
            gcHotelAmenities.DataSource = HotelAminitiesData;
            gcHotelAmenities.RefreshDataSource();
        }

        private void gv_roomTypeMain_DoubleClick(object sender, EventArgs e)
        {
            if (!bbiEditRoomType.Enabled)
            {
                XtraMessageBox.Show("You Are Not Allowed For This Operation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            GridView view = sender as GridView;
            frmRoomTypeEditor roomTypeEditor = new frmRoomTypeEditor();
            roomTypeEditor.SelectedHotelcode = SelectedHotelcode;
            roomTypeEditor.EditedRoomType = (RoomTypeDTO)view.GetFocusedRow() as RoomTypeDTO; ;
            roomTypeEditor.Tag = this;
            // roomTypeEditor.ShowDialog(this.SubForm);
            if (roomTypeEditor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

                RefreshRoomTypeGrid();
                SystemMessage.ShowModalInfoMessage("Edited Successfully", "MESSAGE");
            }
        }

        private void gv_roomTypeMain_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void tList_rooms_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            TreeList view = sender as TreeList;
            int row = view.GetNodeIndex(view.FocusedNode);
            if (row < 0)
                return;
            PopulateTileControl(row);
        }

        private void cacRoomTypeRooms_EditValueChanged(object sender, EventArgs e)
        {
            // RoomTypeVM dto = sender as RoomTypeVM;
            int code = Convert.ToInt32(cacRoomTypeRooms.EditValue);
            if (code > 0)
            {

                selectedRoomType = UIProcessManager.GetRoomTypeById(code);
                //Home.ShowStatusBarMessage(this,
                //    "RoomType: " + selectedRoomType.description + "=" + selectedRoomType.numberOfRooms + "/" +
                //    GetRoomTypeCount(selectedRoomType.description));
                ShowStatusBarMessage("RoomType: " + selectedRoomType.Description + "=" + selectedRoomType.NumberOfRooms + "/" +
                   GetRoomTypeCount(selectedRoomType.Description));
            }

        }

        private void bbiRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            RefreshProperty();
        }

        private void cacSearchRoomsBy_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                int code = Convert.ToInt32(cacSearchRoomsBy.EditValue);
                if (code > 0)
                {
                    List<Floor> filtereFloorsLast = new List<Floor>();
                    selectedRoomType = UIProcessManager.GetRoomTypeById(code);
                    List<Room> filteredRooms = GetRooms().Where(r => r.SavedObject).Distinct().ToList();
                    filteredRooms = filteredRooms.Where(t => t.RoomType.Id == code).ToList();
                    List<Floor> filtereFloors = filteredRooms.Select(rm => rm.Floor_).Distinct().ToList();
                    foreach (Floor flr in filtereFloors)
                    {
                        Floor floor = new Floor();
                        //flr.Rooms.Clear();//RemoveAll(x=>x.filteredRooms.Where(r => !Equals(r.Floor_, flr)));
                        floor.Rooms.AddRange(filteredRooms.Where(r => Equals(r.Floor_, flr)));
                        floor.Building_ = flr.Building_;
                        floor.Name = flr.Name;
                        floor.FullName = flr.FullName;
                        floor.Code = flr.Code;
                        floor.Digit = flr.Digit;
                        floor.Number = flr.Number;
                        floor.SavedObject = flr.SavedObject;
                        floor.Space = flr.Space;
                        filtereFloorsLast.Add(floor);
                    }
                    PopulateRoomsTiles(filtereFloorsLast);
                }
                Unselect();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in filtering rooms! Detail: " + ex.Message, "ERROR");
            }
        }

        private void cacRoomTypeRooms_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                // edit.ClosePopup();
                edit.EditValue = "";

            }
            else
            {
                //edit.ShowPopup();
                edit.Properties.ImmediatePopup = true;
            }
            e.Handled = true;
        }

        private void cacSearchRoomsBy_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                // edit.ClosePopup();
                edit.EditValue = "";
            }
            else
            {
                //edit.ShowPopup();
                edit.Properties.ImmediatePopup = true;
            }
            e.Handled = true;
        }

        private void bbiSaveRooms_ItemClick(object sender, ItemClickEventArgs e)
        {

            string deviceName = SystemInformation.ComputerName;



            SpaceDTO spc = new SpaceDTO();
            bool isSaved = false;
            bool isBuildingExist = false;

            Dictionary<string, string> building = new Dictionary<string, string>();
            List<Building> newlyAddedBulg = GetAddedBuildings();
            List<Room> newlyAddedRooms = AddedRooms();
            List<Floor> newlyAddedFlooor = AddedFloors();

            List<SpaceDTO> blgList = allSpaces.Where(s => s.Type == CNETConstantes.Space_Blook).ToList();

            //Activity Definition for Room Last State
            ActivityDefinitionDTO adDirty = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.Dirty).FirstOrDefault();
            ActivityDefinitionDTO newAdCode = null;
            if (adDirty == null)
            {
                ActivityDefinitionDTO adDef = new ActivityDefinitionDTO()
                {
                    // = CNETConstantes.PMS_Pointer,
                    Reference = CNETConstantes.REGISTRATION_VOUCHER,
                    Description = CNETConstantes.Dirty,
                    State = CNETConstantes.OS_Dirty,
                    Index = 1,
                    IssuingEffect = true,
                };
                newAdCode = UIProcessManager.CreateActivityDefinition(adDef);
            }

            foreach (SpaceDTO variable in blgList)
            {
                if (newlyAddedBulg.Select(t => t.Name).Contains(variable.Description))
                {
                    isBuildingExist = true;
                }
            }
            if (isBuildingExist)
            {
                SystemMessage.ShowModalInfoMessage("Nothing Saved! One or more building names already exist(s)!", "ERROR");
            }
            else
            {
                foreach (Building blg in newlyAddedBulg)
                {

                    spc = new SpaceDTO();
                    SpaceDTO blgCODE = null;
                    spc.Id = 0;
                    spc.Description = blg.Name;
                    spc.Type = CNETConstantes.Space_Blook;
                    spc.Category = CNETConstantes.Hotel_Rooms_Pointer;
                    spc.ConsigneeUnit = blg.organazationUnitDefn;
                    spc.ParentId = null;
                    blgCODE = UIProcessManager.CreateSpace(spc);
                    isSaved = true;
                    blg.SavedObject = true;
                    blg.Space = spc;
                    building.Add(blg.Name, blgCODE.Id.ToString());
                    foreach (Floor flr in blg.floors)
                    {
                        string blgCode = "";
                        SpaceDTO spcCode = null;
                        building.TryGetValue(flr.Building_.Name, out blgCode);
                        spc = new SpaceDTO();
                        spc.Id = 0;
                        spc.Description = flr.Name;
                        spc.Type = CNETConstantes.Space_Floor;
                        spc.Category = CNETConstantes.Hotel_Rooms_Pointer;
                        spc.ConsigneeUnit = blg.organazationUnitDefn;
                        spc.ParentId = Convert.ToInt32(blgCode);
                        spc.Remark = flr.Number.ToString();
                        spcCode = UIProcessManager.CreateSpace(spc);
                        isSaved = true;
                        flr.SavedObject = true;
                        flr.Space = spc;

                        foreach (Room rm in flr.Rooms)
                        {
                            if (rm.RoomType != null)
                            {

                                SpaceDTO rmCode = null;
                                spc = new SpaceDTO();
                                spc.Id = 0;
                                spc.Description = rm.RoomNo;
                                spc.Type = CNETConstantes.Space_Room;
                                spc.Category = CNETConstantes.Hotel_Rooms_Pointer;
                                spc.ConsigneeUnit = blg.organazationUnitDefn;
                                spc.ParentId = spcCode.Id;


                                rmCode = UIProcessManager.CreateSpace(spc);
                                isSaved = true;
                                rm.SavedObject = true;
                                if (rmCode.Id > 0)
                                {
                                    rm.Space = spc;

                                    RoomDetailDTO rd = new RoomDetailDTO();
                                    rd.Space = rmCode.Id;
                                    rd.Description = rm.RoomNo;
                                    rd.MaxOccupnancy = null;
                                    rd.RoomType = rm.RoomType.Id;
                                    rd.PhoneNumber = rm.RoomNo;
                                    rd.IsActive = true;
                                    rd.Area = null;
                                    rm.RoomDetail = rd;
                                    if (adDirty != null)
                                    {
                                        rd.LastState = adDirty.Id;
                                    }
                                    else
                                    {
                                        if (newAdCode.Id > 0)
                                        {
                                            rd.LastState = newAdCode.Id;
                                        }
                                    }
                                    UIProcessManager.CreateRoomDetail(rd);


                                }
                            }
                        }
                    }

                }
            }
            foreach (Room rom in newlyAddedRooms)
            {
                if (!rom.SavedObject && rom.RoomType != null)
                {
                    SpaceDTO newCode = null;
                    spc = new SpaceDTO();
                    spc.Id = 0;
                    spc.Description = rom.RoomNo;
                    spc.Type = CNETConstantes.Space_Room;
                    spc.Category = CNETConstantes.Hotel_Halls_pointer;
                    spc.ParentId = Convert.ToInt32(rom.Floor_.ID);
                    spc.ConsigneeUnit = rom.RoomType.Consigneeunit;
                    newCode = UIProcessManager.CreateSpace(spc);
                    isSaved = true;
                    rom.Space = spc;
                    if (newCode != null)
                    {
                        if (rom.RoomType != null)
                        {
                            RoomDetailDTO rd = new RoomDetailDTO();
                            rd.Space = newCode.Id;
                            rd.Description = rom.RoomNo;
                            rd.MaxOccupnancy = null;
                            rd.RoomType = rom.RoomType.Id;
                            rd.PhoneNumber = rom.RoomNo;
                            rd.IsActive = true;
                            rd.Area = null;
                            rom.RoomDetail = rd;
                            if (adDirty != null)
                            {
                                rd.LastState = adDirty.Id;
                            }
                            else
                            {
                                if (newAdCode != null)
                                {
                                    rd.LastState = newAdCode.Id;
                                }
                            }
                            UIProcessManager.CreateRoomDetail(rd);

                        }
                    }
                }
            }



            building.Clear();
            if (isSaved)
            {
                SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");
                RefreshProperty();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Nothing Saved", "MESSAGE");
            }
        }

        private void cbeSelectUnselect_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cbeSelectUnselect_TextChanged(object sender, EventArgs e)
        {

        }

        private void cdeFeature_Load(object sender, EventArgs e)
        {

        }
        private void bbiAddBuildingRoom_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            spaceGenerator = new frmSpaceGenerator();
            spaceGenerator.ExistBuildings = GetBuildings();

            if (spaceGenerator.ShowDialog(this.SubForm) == System.Windows.Forms.DialogResult.OK)
            {
                BuildBuildings();
            }
        }
        private void cbeSelectUnselect_SelectedValueChanged(object sender, EventArgs e)
        {


            if (cbeSelectUnselect.EditValue.ToString().Equals("Select All"))
            {
                if (ISSelectedBuilding)
                {
                    foreach (Floor floor in SelectedFloor.Building_.floors)
                    {
                        foreach (Room room in floor.Rooms)
                        {
                            room.Select();

                            RoomCheckedChanged(new TileItemEventArgs() { Item = room.TileItem_ });
                        }
                    }

                }

                else
                {

                    foreach (Room room in SelectedFloor.Rooms)
                    {
                        room.Select();

                        RoomCheckedChanged(new TileItemEventArgs() { Item = room.TileItem_ });


                    }

                }

            }


            else if (cbeSelectUnselect.EditValue.ToString().Equals("Unselect"))
            {

                Unselect();

            }

        }

        private void bbiEditRoomType_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmRoomTypeEditor roomTypeEditor = new frmRoomTypeEditor();

            roomTypeEditor.SelectedHotelcode = SelectedHotelcode;
            roomTypeEditor.EditedRoomType = (RoomTypeDTO)gv_roomTypeMain.GetFocusedRow();
            roomTypeEditor.Tag = this;
            // roomTypeEditor.ShowDialog(this.SubForm);
            if (roomTypeEditor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                RefreshRoomTypeGrid();
                SystemMessage.ShowModalInfoMessage("Edited Successfully", "MESSAGE");
            }
        }

        private void bbiRemoveRoomType_ItemClick(object sender, ItemClickEventArgs e)
        {

            //   OnDelete();

        }
        List<RoomFeatureDTO> RoomfeatureList { get; set; }
        private void bbiSaveRoomType_ItemClick(object sender, ItemClickEventArgs e)
        {
            gvRoomAmenities.PostEditor();

            RoomFeatureDTO rFeature = new RoomFeatureDTO();
            RoomTypeDTO rt = (RoomTypeDTO)gv_roomTypeMain.GetFocusedRow();
            RoomFeatureDTO saveRoomFeatureDTO = null;

            if (RoomfeatureList != null)
                RoomfeatureList.ForEach(x => UIProcessManager.DeleteRoomFeatureById(x.Id));

            List<AmenitiesDTO> HotelAme = (List<AmenitiesDTO>)gcRoomAmenities.DataSource;
            List<AmenitiesDTO> selectedHotelAme = HotelAme.Where(x => x.select).ToList();

            foreach (AmenitiesDTO checkedItem in selectedHotelAme)
            {
                rFeature = new RoomFeatureDTO();
                rFeature.Id = 0;
                rFeature.Pointer = CNETConstantes.TABLE_ROOM_TYPE;
                rFeature.Reference = rt.Id;
                rFeature.Feature = checkedItem.Id;
                rFeature.Note = checkedItem.Description;
                saveRoomFeatureDTO = UIProcessManager.CreateRoomFeature(rFeature);
            }
            if (saveRoomFeatureDTO != null)
            {
                SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");
            }
            gv_roomTypeMain_FocusedRowChanged(null, null);
            //RoomfeatureList = UIProcessManager.SelectAllRoomFeature().Where(f => f.reference == rt.code).ToList();
            //     
        }

        void Logic_ItemClicked(object sender, EventArgs e)
        {
            RoomTypeVM dto = sender as RoomTypeVM;
            selectedRoomType = UIProcessManager.GetRoomTypeById(dto.Id);

            //Home.ShowStatusBarMessage(this, "RoomType: " + selectedRoomType.description + "=" + selectedRoomType.numberOfRooms + "/" + GetRoomTypeCount(selectedRoomType.description));
            ShowStatusBarMessage("RoomType: " + selectedRoomType.Description + "=" + selectedRoomType.NumberOfRooms + "/" + GetRoomTypeCount(selectedRoomType.Description));

        }

        void sbEditRoom_Click(object sender, EventArgs e)
        {
            //if (SelectedSingleRoom == null) return;Ffrmproperty
            frmRoomDetailForProperty roomDetail = new frmRoomDetailForProperty(this);
            roomDetail.prop = this;

            if (SelectedRooms.Count() == 1)
            {
                roomDetail.RoomToEdit = SelectedRooms.FirstOrDefault();
                roomDetail.SelectedHotelcode = SelectedHotelcode;
                if (roomDetail.ShowDialog(this.SubForm) == System.Windows.Forms.DialogResult.OK)
                {

                    foreach (Room room in SelectedRooms)
                    {

                        room.container = this;
                        room.RoomType = roomDetail.RoomToEdit.RoomType;
                        room.RoomNo = roomDetail.RoomToEdit.RoomNo;
                        room.RoomDetail.PhoneNumber = roomDetail.RoomToEdit.RoomDetail.PhoneNumber;
                        room.Measurement = roomDetail.RoomToEdit.RoomDetail.Area;
                        room.RoomDetail.Remark = roomDetail.RoomToEdit.RoomDetail.Remark;
                    }

                    SystemMessage.ShowModalInfoMessage("Edited Successfully", "MESSAGE");
                    RefreshTileViewer();
                    //RefreshProperty();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Editing cancelled or not Successful", "ERROR");
                }
                if (ISSelectedBuilding)
                {
                    foreach (Floor floor in SelectedFloor.Building_.floors)
                    {
                        foreach (Room room in floor.Rooms)
                        {
                            room.UnSelect();

                            RoomCheckedChanged(new TileItemEventArgs() { Item = room.TileItem_ });
                        }
                    }
                }
                else
                {

                    foreach (Room room in SelectedFloor.Rooms)
                    {
                        room.UnSelect();

                        RoomCheckedChanged(new TileItemEventArgs() { Item = room.TileItem_ });


                    }

                }
            }
            List<RoomTypeDTO> roomTList = GetRoomTypeData(SelectedHotelcode);
            PopulateCacRoomType(roomTList);
        }


        private void tcRooms_Click(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            e.Item.Checked = !e.Item.Checked;


            RoomCheckedChanged(e);

            ChangeTileCheckedStatus(e);

        }

        private void RoomCheckedChanged(DevExpress.XtraEditors.TileItemEventArgs e)
        {
            RefreshHeaderButtonsEnabledState();


        }

        public void LoadData(UILogicBase requesterForm, object args)
        {
            throw new NotImplementedException();
        }


        private void bbiAddRoomType_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmRoomTypeEditor roomTypeEditor = new frmRoomTypeEditor();
            roomTypeEditor.Tag = this;
            roomTypeEditor.SelectedHotelcode = SelectedHotelcode;
            //   roomTypeEditor.ShowDialog(this.SubForm);
            if (roomTypeEditor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

                RefreshRoomTypeGrid();

                SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");
            }


        }

        private void frmProperty_Load(object sender, EventArgs e)
        {
            //// LoadAttachment(); 

            tlOrganizationUnit.ParentFieldName = "parentId";
            tlOrganizationUnit.KeyFieldName = "Id";
            tlOrganizationUnit.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.ParentId, Hotel = x.Name }).ToList();
            tlOrganizationUnit_FocusedNodeChanged(null, null);
            if (CheckIfTheUserHasHotelProfileMaintenanceAccess())
            {
                xtpHotelAmenities.PageEnabled = true;
                xtpHotelAttachment.PageEnabled = true;
                xtpHotelInfo.PageEnabled = true;
            }
        }

        #endregion


        class FeatureDataHolder
        {
            public String Grouper { get; set; }
            public String Name { get; set; }
            public Boolean check { get; set; }
        }

        private void tList_rooms_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            TreeList view = sender as TreeList;
            SpaceVM spVM = (SpaceVM)view.GetDataRecordByNode(e.Node);
            if (spVM == null) return;

            if (spVM.ParentCode == null)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("LightSeaGreen");
                e.Appearance.ForeColor = ColorTranslator.FromHtml("#FFFFFFFF");
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
            }
        }

        public int SelectedHotelcode { get; set; }
        private void tlOrganizationUnit_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (tlOrganizationUnit.FocusedNode != null)
            {
                int code =
                   Convert.ToInt32(tlOrganizationUnit.FocusedNode.GetValue("Id"));

                if (code > 0)
                {
                    SelectedHotelcode = code;
                    List<RoomTypeDTO> rTypeList = GetRoomTypeData(SelectedHotelcode);

                    if (rTypeList != null)
                    {
                        CreateRoomTypeColors(rTypeList);
                    }

                    int totalRooms = 0;
                    foreach (RoomTypeDTO rt in rTypeList)
                    {
                        if (rt.NumberOfRooms != null)
                        {
                            totalRooms += Convert.ToInt32(rt.NumberOfRooms);
                        }

                    }
                    gc_roomTypeMain.DataSource = rTypeList;
                    gv_roomTypeMain_FocusedRowChanged(null, null);
                    if (tcProperty.SelectedTabPage.Name == "rpRoomType")
                    {
                        //  Home.ShowStatusBarMessage(this, "Total Number of Rooms = " + totalRooms.ToString());
                        ShowStatusBarMessage("Total Number of Rooms = " + totalRooms.ToString());

                    }
                    if (tcProperty.SelectedTabPage.Name == "tpRooms")
                    {
                        PopulateCacRoomType(rTypeList);
                        _spaceVMList.Clear();
                        RefreshBuildingTree();
                        LoadBuildingData();
                    }
                    //LoadBuildingData();
                    LoadHotelFeatures();
                    // if (attachementData != null)
                    ConsigneeUnitDTO BranchOrgUnitDef = LocalBuffer.LocalBuffer.HotelBranchBufferList.FirstOrDefault(x => x.Id == SelectedHotelcode);
                    LoadHotelInfo(BranchOrgUnitDef);
                    LoadHotelAddress();
                    LoadHotelRating(BranchOrgUnitDef);
                    txtHotelInfo.Text = BranchOrgUnitDef.Description;
                }

            }
        }

        private void LoadHotelRating(ConsigneeUnitDTO BranchOrgUnitDef)
        {
            leHotelStars.EditValue = null;
            SystemConstantDTO HotelRatingState = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id.ToString() == BranchOrgUnitDef.AddressLine3);
            if (HotelRatingState != null)
            {
                leHotelStars.EditValue = HotelRatingState.Id;
            }
            else
                leHotelStars.EditValue = null;
            txtLongitude.EditValue = BranchOrgUnitDef.Longitude;
            txtlatitude.EditValue = BranchOrgUnitDef.Latitude;
            txtcontact.EditValue = BranchOrgUnitDef.Contact;
            txtphone1.EditValue = BranchOrgUnitDef.Phone1;
            txtphone2.EditValue = BranchOrgUnitDef.Phone2;
            txtemail.EditValue = BranchOrgUnitDef.Email;
            txtwebsite.EditValue = BranchOrgUnitDef.Website;
            txtpobox.EditValue = BranchOrgUnitDef.PoBox;
            sleRegion.EditValue = BranchOrgUnitDef.Region;
            sleCity.EditValue = BranchOrgUnitDef.City;
            sleSubCity.EditValue = BranchOrgUnitDef.Subcity;
            txtwereda.EditValue = BranchOrgUnitDef.Wereda;
            txtkebele.EditValue = BranchOrgUnitDef.Kebele;
            txtstreet.EditValue = BranchOrgUnitDef.Street;
            txtaddress1.EditValue = BranchOrgUnitDef.AddressLine1;
            txtaddress2.EditValue = BranchOrgUnitDef.AddressLine2;



        }

        //  public List<ListOfOrganizationAddress> OrgAdress;
        private void LoadHotelAddress()
        {
            //OrgAdress = UIProcessManager.GetListOfOrganizationAddresses(SelectedHotelcode).ToList();//Authentication.AddressBufferList.Where(a => a.reference == orgval).ToList();
            //cacAddressOrgUnit.ParentFieldName = "parent";
            //cacAddressOrgUnit.KeyFieldName = "prefCode";
            //cacAddressOrgUnit.DataSource = (OrgAdress);
            //cacAddressOrgUnit.ExpandAll();
        }
        private void LoadHotelInfo(ConsigneeUnitDTO BranchOrgUnitDef)
        {
            if (BranchOrgUnitDef != null)
            {
                txtHotelInfo.Text = BranchOrgUnitDef.Description;
            }
        }
        private void btnSaveHotelFeatures_ItemClick(object sender, ItemClickEventArgs e)
        {
            gvHotelAmenities.PostEditor();
            RoomFeatureDTO rFeature = new RoomFeatureDTO();
            RoomFeatureDTO saveRoomFeatureDTO = null;
            if (HotelFeatureList != null)
                HotelFeatureList.ForEach(x => UIProcessManager.DeleteRoomFeatureById(x.Id));

            List<AmenitiesDTO> selectedHotelAme = HotelAminitiesData.Where(x => x.select).ToList();




            foreach (AmenitiesDTO checkedItem in selectedHotelAme)
            {
                rFeature = new RoomFeatureDTO();
                rFeature.Id = 0;
                rFeature.Pointer = CNETConstantes.TABLE_ROOM_TYPE;
                rFeature.Reference = SelectedHotelcode;
                rFeature.Feature = checkedItem.Id;
                rFeature.Note = checkedItem.Description;
                saveRoomFeatureDTO = UIProcessManager.CreateRoomFeature(rFeature);


            }
            if (saveRoomFeatureDTO != null)
            {
                SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Saved Fail!!! ", "MESSAGE");
            }
            LoadHotelFeatures();
            //HotelFeatureList = UIProcessManager.SelectAllRoomFeature().Where(f => f.reference == SelectedHotelcode).ToList();
        }


        private void btnUpdateHotelInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SelectedHotelcode > 0)
            {
                ConsigneeUnitDTO BranchOrgUnitDef = LocalBuffer.LocalBuffer.HotelBranchBufferList.FirstOrDefault(x => x.Id == SelectedHotelcode);
                if (BranchOrgUnitDef != null)
                {
                    BranchOrgUnitDef.Description = txtHotelInfo.Text;

                    if (leHotelStars.EditValue != null && !string.IsNullOrEmpty(leHotelStars.EditValue.ToString()))
                    {
                        BranchOrgUnitDef.AddressLine3 = leHotelStars.EditValue.ToString();
                    }
                    else
                        BranchOrgUnitDef.AddressLine3 = "";


                    decimal? Longitudevalue = null;
                    if (txtLongitude.EditValue != null && !string.IsNullOrEmpty(txtLongitude.EditValue.ToString()))
                    {
                        try { Longitudevalue = decimal.Parse(txtLongitude.EditValue.ToString()); } catch { Longitudevalue = null; }
                    }

                    decimal? Latitudevalue = null;
                    if (txtlatitude.EditValue != null && !string.IsNullOrEmpty(txtLongitude.EditValue.ToString()))
                    {
                        try { Latitudevalue = decimal.Parse(txtlatitude.EditValue.ToString()); } catch { Latitudevalue = null; }
                    }

                    BranchOrgUnitDef.Longitude = Longitudevalue;
                    BranchOrgUnitDef.Latitude = Latitudevalue;
                    BranchOrgUnitDef.Contact = txtcontact.EditValue == null ? null : txtcontact.EditValue.ToString();
                    BranchOrgUnitDef.Phone1 = txtphone1.EditValue == null ? null : txtphone1.EditValue.ToString();
                    BranchOrgUnitDef.Phone2 = txtphone2.EditValue == null ? null : txtphone2.EditValue.ToString();
                    BranchOrgUnitDef.Email = txtemail.EditValue == null ? null : txtemail.EditValue.ToString();
                    BranchOrgUnitDef.Website = txtwebsite.EditValue == null ? null : txtwebsite.EditValue.ToString();
                    BranchOrgUnitDef.PoBox = txtpobox.EditValue == null ? null : txtpobox.EditValue.ToString();
                    BranchOrgUnitDef.Region = sleRegion.EditValue == null ? null : Convert.ToInt32(sleRegion.EditValue.ToString());
                    BranchOrgUnitDef.City = sleCity.EditValue == null ? null : Convert.ToInt32(sleCity.EditValue.ToString());
                    BranchOrgUnitDef.Subcity = sleSubCity.EditValue == null ? null : Convert.ToInt32(sleSubCity.EditValue.ToString());
                    BranchOrgUnitDef.Wereda = txtwereda.EditValue == null ? null : txtwereda.EditValue.ToString();
                    BranchOrgUnitDef.Kebele = txtkebele.EditValue == null ? null : txtkebele.EditValue.ToString();
                    BranchOrgUnitDef.Street = txtstreet.EditValue == null ? null : txtstreet.EditValue.ToString();
                    BranchOrgUnitDef.AddressLine1 = txtaddress1.EditValue == null ? null : txtaddress1.EditValue.ToString();
                    BranchOrgUnitDef.AddressLine2 = txtaddress2.EditValue == null ? null : txtaddress2.EditValue.ToString();

                    ConsigneeUnitDTO saved = UIProcessManager.UpdateConsigneeUnit(BranchOrgUnitDef);


                    if (saved != null)
                        SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");
                    else
                        SystemMessage.ShowModalInfoMessage("Saving Failed", "MESSAGE");
                }
            }
        }



        private void cacAddressOrgUnit_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {

        }




        private void gvRoomAmenities_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gvRoomAmenities.PostEditor();
            if ((sender as GridView).FocusedColumn.FieldName == "Description")
            {
                AmenitiesDTO Aminities = (AmenitiesDTO)gvRoomAmenities.GetFocusedRow();
                if (Aminities != null && !Aminities.select)
                { e.Cancel = true; }

            }
        }

        private void gvHotelAmenities_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gvRoomAmenities.PostEditor();
            if ((sender as GridView).FocusedColumn.FieldName == "Description")
            {
                AmenitiesDTO Aminities = (AmenitiesDTO)gvHotelAmenities.GetFocusedRow();
                if (Aminities != null && !Aminities.select)
                { e.Cancel = true; }

            }
        }
    }


    public class AmenitiesDTO
    {
        public bool select { get; set; }
        public int Id { get; set; }
        public string amenities { get; set; }
        public string Description { get; set; }
    }
}
