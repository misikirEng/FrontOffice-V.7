 
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms.Setting_and_Miscellaneous.DTO
{

    public class SpaceVM
    {
        public string Id { get; set; }
        public string FakeCode { get; set; }
        public string FakeParentCode { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public SpaceDTO Parent { get; set; }
        public Floor Floor { get; set; }
        public List<Floor> ChildFloors { get; set; }
    }

    public class Building
    {
        public string ID { get; set; }
        public Boolean SavedObject;

        public String Name { get; set; }
        public int Code { get; set; }
        public int organazationUnitDefn { get; set; }
        public List<Floor> floors = new List<Floor>();
        public SpaceDTO Space { get; set; }

      public void AddFloor(Floor floor)
      {
          floor.Building_ = this;
          
          floors.Add(floor);
      }


      public override bool Equals(object obj)
      {
          return Code.Equals(((Building)obj).Code);
      }

    }


    public class Floor
    {
        public string ID { get; set; }
        public Boolean SavedObject;
        private string name;
        public String Name
        {
            get { return name;
            }
            set
            {
                name = value;
        if (String.IsNullOrEmpty(FullName)) FullName = value;
        } }

        public String FullName { get; set; }
        public String Code { get; set; }
        public Building Building_ { get; set; }
        public List<Room> Rooms = new List<Room>();
        public int Number;
        public SpaceDTO Space { get; set; }

        public void AddRoom(Room room)
        {
            room.Floor_ = this;

            Rooms.Add(room);

            this.FullName = this.Name + " ( " + Rooms.Count.ToString() + " Rooms )";

        }

        public void DeleteRoom(Room room)
        {
            if(Rooms.Contains(room))
            Rooms.Remove(room);

            this.FullName = this.Name + " ( " + Rooms.Count.ToString() + " Rooms )";

        }

        public string BuildingName { get; set; }


        public override bool Equals(object obj)
        {
            if (Code != null) return Code.Equals(((Floor)obj).Code);
            else return false;
        }

        public int Digit { get; set; }
    }

    public class Room
    {
        public Room()
        {
            RoomDetail = new RoomDetailDTO();
        }
        public string ID { get; set; }
        public Boolean SavedObject;
        public String Code { get; set; }
        public String RoomNo { get; set; }
        public String Name { get; set; }
        public decimal? Measurement { get; set; }
        public Boolean SmockingAllowed{ get; set; }
        public Boolean isActive { get; set; }
        private RoomTypeDTO roomType;
        public SpaceDTO Space { get; set; }
        public frmProperty container { get; set; }

        public RoomTypeDTO RoomType
        {
            get { return roomType; }
            set { 
                
                roomType = value;

                if (value != null)

                    if (String.IsNullOrEmpty(value.Description))
                    {
                        tileItem_.Elements[0].Text = "";
                        tileItem_.AppearanceItem.Normal.BackColor = frmProperty.DefaultTileColor;

                    }
                    else
                    {

                        if (tileItem_ == null && container != null) tileItem_ = container.CreateTileRoom(this);
                      tileItem_.Elements[0].Text = value.Description;//       . Aggregate((u, y) => u + "," + y).ToString();
                      tileItem_.AppearanceItem.Normal.BackColor = container.GetRoomTypeColor(value.Description);
                    }

                tileItem_.AppearanceItem.Normal.Options.UseBackColor = true;

            }
        }




        RoomDetailDTO _RoomDetail;

        public RoomDetailDTO RoomDetail
        {
            get { return _RoomDetail; }
            set
            {
                _RoomDetail = value;
   

            } 
        }
        private String feature;

        public String Feature
        { 
            get { return feature; } 
            set {
                if (tileItem_ != null)
                    tileItem_.Elements[0].Text = value;


                feature = value; }
        }


        public Floor Floor_ { get; set; }
        private TileItem tileItem_;
        public Boolean IsSelected { get; set; }

        public void Select()
        {
            if (tileItem_ != null)
                tileItem_.Checked = true;
        }


        public void UnSelect()
        {
            if (tileItem_ != null)
                tileItem_.Checked = false;
        }

        public TileItem TileItem_
        {
            get { return tileItem_; }
            set {
                value.CheckedChanged += value_CheckedChanged;
                
                tileItem_ = value; }
        }

        

        void value_CheckedChanged(object sender, TileItemEventArgs e)
        {

            this.IsSelected = e.Item.Checked;
            
        }
      

        public void AddFloor(Floor floor)
        {

            Floor_ = floor;

           floor.AddRoom(this);

        }
         


    }


}
