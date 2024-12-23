using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous.Revenue_Management_Modals
{
   public  class AvailabilityControlObject
    {
       public AvailabilityControlObject()
       {
           Day1 = new DayCO(this);
           Day1.ValueChanged += Day_ValueChanged;
           Day1.DayNumber = 1;
           Day2 = new DayCO(this);
           Day2.ValueChanged += Day_ValueChanged;
           Day2.DayNumber = 2;
           Day3 = new DayCO(this);
           Day3.DayNumber = 3;
           Day3.ValueChanged += Day_ValueChanged;
           Day4 = new DayCO(this);
           Day4.DayNumber = 4;
           Day4.ValueChanged += Day_ValueChanged;
           Day5 = new DayCO(this);
           Day5.DayNumber = 5;
           Day5.ValueChanged += Day_ValueChanged;
           Day6 = new DayCO(this);
           Day6.DayNumber = 6;
           Day6.ValueChanged += Day_ValueChanged;
           Day7 = new DayCO(this);
           Day7.DayNumber = 7;
           Day7.ValueChanged += Day_ValueChanged;
           Day8 = new DayCO(this);
           Day8.DayNumber = 8;
           Day8.ValueChanged += Day_ValueChanged;
           Day9 = new DayCO(this);
           Day9.DayNumber = 9;
           Day9.ValueChanged += Day_ValueChanged;
           Day10 = new DayCO(this);
           Day10.DayNumber = 10;
           Day10.ValueChanged += Day_ValueChanged;
           Day11 = new DayCO(this);
           Day11.DayNumber = 11;
           Day11.ValueChanged += Day_ValueChanged;
           Day12 = new DayCO(this);
           Day12.DayNumber = 12;
           Day12.ValueChanged += Day_ValueChanged;
           Day13 = new DayCO(this);
           Day13.DayNumber = 13;
           Day13.ValueChanged += Day_ValueChanged;
           Day14 = new DayCO(this);
           Day14.DayNumber = 14;
           Day14.ValueChanged += Day_ValueChanged;
           Day15 = new DayCO(this);
           Day15.DayNumber = 15;
           Day15.ValueChanged += Day_ValueChanged;
           Day16 = new DayCO(this);
           Day16.DayNumber = 16;
           Day16.ValueChanged += Day_ValueChanged;
           Day17 = new DayCO(this);
           Day17.DayNumber = 17;
           Day17.ValueChanged += Day_ValueChanged;
           Day18 = new DayCO(this);
           Day18.DayNumber = 18;
           Day18.ValueChanged += Day_ValueChanged;
           Day19 = new DayCO(this);
           Day19.DayNumber = 19;
           Day19.ValueChanged += Day_ValueChanged;
           Day20 = new DayCO(this);
           Day20.DayNumber = 20;
           Day20.ValueChanged += Day_ValueChanged;
           Day21 = new DayCO(this);
           Day21.DayNumber = 21;
           Day21.ValueChanged += Day_ValueChanged;
           Day22 = new DayCO(this);
           Day22.DayNumber = 22;
           Day22.ValueChanged += Day_ValueChanged;
           Day23 = new DayCO(this);
           Day23.DayNumber = 23;
           Day23.ValueChanged += Day_ValueChanged;
           Day24 = new DayCO(this);
           Day24.DayNumber = 24;
           Day24.ValueChanged += Day_ValueChanged;
           Day25 = new DayCO(this);
           Day25.DayNumber = 25;
           Day25.ValueChanged += Day_ValueChanged;
           Day26 = new DayCO(this);
           Day26.DayNumber = 26;
           Day26.ValueChanged += Day_ValueChanged;
           Day27 = new DayCO(this); 
           Day27.DayNumber = 27;
           Day27.ValueChanged += Day_ValueChanged;
           Day28 = new DayCO(this);
           Day28.DayNumber = 28;
           Day28.ValueChanged += Day_ValueChanged;
           Day29 = new DayCO(this);
           Day29.DayNumber = 29;
           Day29.ValueChanged += Day_ValueChanged;
           Day30 = new DayCO(this);
           Day30.DayNumber = 30;
           Day30.ValueChanged += Day_ValueChanged;
           Day31 = new DayCO(this);
           Day31.DayNumber = 31;
           Day31.ValueChanged += Day_ValueChanged;

       }

       public static String OPEN_INDICATOR = "O";
       public static String CLOSED_INDICATOR = "C";
       public static String NOSTATUS_INDICATOR = "";

            public static String GetStatusIndicator(AvailabilityStatus status)
       {

           if (status == AvailabilityStatus.OPEN)
               return AvailabilityControlObject.OPEN_INDICATOR;
           if (status == AvailabilityStatus.CLOSED)
               return AvailabilityControlObject.CLOSED_INDICATOR;
           if (status == AvailabilityStatus.NONE)
               return AvailabilityControlObject.NOSTATUS_INDICATOR;          

                return null;
                }



            public static AvailabilityStatus GetAvailabilityStatus(string statusIndicator)
            {

                if (statusIndicator == AvailabilityControlObject.OPEN_INDICATOR)
                    return AvailabilityStatus.OPEN;
                if (statusIndicator == AvailabilityControlObject.CLOSED_INDICATOR)
                    return AvailabilityStatus.CLOSED;
                else
                    return AvailabilityStatus.NONE;

            }


       private void Day_ValueChanged(object sender, DayStatusChangedEventArgs e)
       {
  
           if (DayChanged != null)
               DayChanged.Invoke(sender, e);
       }



       public String Name { get; set; }
       public int Month { get; set; }
       public int Year{ get; set; }
       public event EventHandler<DayStatusChangedEventArgs> DayChanged;


       public DayCO Day1 { get; set; }
       public DayCO Day2 { get; set; }
       public DayCO Day3 { get; set; }
       public DayCO Day4 { get; set; }
       public DayCO Day5 { get; set; }
       public DayCO Day6 { get; set; }
       public DayCO Day7 { get; set; }
       public DayCO Day8 { get; set; }
       public DayCO Day9 { get; set; }
       public DayCO Day10 { get; set; }
       public DayCO Day11 { get; set; }
       public DayCO Day12 { get; set; }
       public DayCO Day13 { get; set; }
       public DayCO Day14 { get; set; }
       public DayCO Day15 { get; set; }
       public DayCO Day16 { get; set; }
       public DayCO Day17 { get; set; }
       public DayCO Day18 { get; set; }
       public DayCO Day19 { get; set; }
       public DayCO Day20 { get; set; }
       public DayCO Day21 { get; set; }
       public DayCO Day22 { get; set; }
       public DayCO Day23 { get; set; }
       public DayCO Day24 { get; set; }
       public DayCO Day25 { get; set; }
       public DayCO Day26 { get; set; }
       public DayCO Day27 { get; set; }
       public DayCO Day28 { get; set; }
       public DayCO Day29 { get; set; }
       public DayCO Day30 { get; set; }
       public DayCO Day31 { get; set; }


    }

    public class DayStatusChangedEventArgs:EventArgs
    {
            public DayCO Day { get; set; }
            public AvailabilityStatus Status { get; set; }

    }

    public enum DayCOStatus
    {
        Status1,
        Status2

    }

   public class DayCO
   {

       public DayCO(AvailabilityControlObject controller)
       {
           RepoItem = new RepositoryItemTextEdit();
           isEnabled = true;
           DayColor = DayCO.DefaultColor;
           Value = String.Empty;
           this.controller = controller;

           RepoItem.Click += RepoItem_Click;
           RepoItem.EditValueChanging += RepoItem_EditValueChanging;
       }

       void RepoItem_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
       {
           e.NewValue = this.value;
       }

     

       void RepoItem_Click(object sender, EventArgs e)
       {
           ChangeStatus();   
           
       }

       private void ChangeStatus()
       {
           String indicator = AvailabilityControlObject.GetStatusIndicator(frmRevenueManagement.CurrentAvailaibityStatus);
           if (value == "X") return;
           if (value ==  AvailabilityControlObject.NOSTATUS_INDICATOR)
           {
               Value = indicator;
           }
           else Value = AvailabilityControlObject.NOSTATUS_INDICATOR;
       
       }

  

       private AvailabilityControlObject controller;
       public RepositoryItemTextEdit RepoItem { get; set; }
       public DateTime DayDate { get; set; }
       public Boolean IsSelected { get; set; }
       public Boolean IsHoliday { get; set; }
       private String value;
       public AvailabilityStatus Status;
       public int DayNumber { get; set; }

       public DateTime GetDate()
       {
           try
           {
               var daysInMonth = DateTime.DaysInMonth(controller.Year, controller.Month);

           if (this.DayNumber > daysInMonth) return new DateTime();

               DateTime date = new DateTime(controller.Year, controller.Month, this.DayNumber);

          


               return date;
           }
           catch {
           
           }
           return new DateTime();
       }

       public String Value
       {
           get { return this.value; }
           set { 


               this.value = value;
               try
               {
                   DayStatusChangedEventArgs changeEvArg = new DayStatusChangedEventArgs();
                   changeEvArg.Day = this;
                   changeEvArg.Status = AvailabilityControlObject.GetAvailabilityStatus(value);

                   if (ValueChanged != null)
                       ValueChanged.Invoke(this, changeEvArg);
               }
               catch { }
               
           }
       }
       public event EventHandler<DayStatusChangedEventArgs> ValueChanged;
       private bool isEnabled;

       public bool IsEnabled
       {
           get { return isEnabled; }
           set { isEnabled = value;
           }
       }

       public Color DayColor { get; set; }

       public event EventHandler<DaySelectedEventArgs> Selected ;

       public static Color DefaultColor = Color.FromArgb(0x66, 0x99, 0xFF);

   }

   public class DaySelectedEventArgs : EventArgs
   {
       public Boolean IsSelected { get; set; }

       public DayCO DayControlObject { get; set; }

   }
    

}
