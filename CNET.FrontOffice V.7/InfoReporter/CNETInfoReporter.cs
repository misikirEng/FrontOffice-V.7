 
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.DataProcessing.InMemoryDataProcessor.AddSurrogateOperationAlgorithm; 

namespace CNET.FrontOffice_V._7
{
 public  class CNETInfoReporter
    {
      
      static SplashScreenManager _splashScreen;

     static UserControl userControlWaitScreen;


     public static void SetUserControlWaitScreen(UserControl usercontrol)
     {
         userControlWaitScreen = usercontrol;
     }

     static Form formWaitScreen;


     public static void SetWaitScreenScreenParentForm(Form parentForm)
     {
         formWaitScreen = parentForm;
     }

    private  static CNETWaitScreenType WaitScreenType;

     public static void SetWaitScreenType(CNETWaitScreenType typex)
     {
         WaitScreenType = typex;

     }

     private static Color waitScreenColor = Color.Transparent;
     public static void SetWaitScreenColor(Color color)
     {
         waitScreenColor = color;
     }

     static int counter = 0;
     static int currentMax = -1;


        //public void DisplayProgress(string Info, int count, int totalcount)
        //{

        //    statusStrip1.Update();
        //    toolStripProgressBar1.Value = ((count / totalcount) * 100);
        //    toolStripProgressBar1.ToolTipText = Info;
        //    statusStrip1.BackColor = Color.FromArgb(79, 208, 154);
        //} 
        public static void WaitForm(String description, String caption = "Please Wait..", int currentValue = -1, int maxValue = -1)
        {  
              
         Type spltype = typeof(CNETWaitForm);

         if (splashScreenType == null)
         {
             spltype = typeof(CNETWaitForm);

         }

         if (_splashScreen == null)
         {
             if(userControlWaitScreen!=null)
                 _splashScreen = new SplashScreenManager(userControlWaitScreen, spltype, true, true,ParentType.Form);
             else
             _splashScreen = new SplashScreenManager(formWaitScreen, spltype, true, true);

             try
             {
                 _splashScreen.ShowWaitForm();
             }
             catch { }
         }
     

         _splashScreen.SetWaitFormCaption(caption);

         _splashScreen.SetWaitFormDescription(description);

         if (waitScreenColor != Color.Transparent)
             _splashScreen.SendCommand(CNETWaitForm.WaitFormCommand.Color, waitScreenColor);
         
         int progressValue = CalculatePercent(currentValue, maxValue);

         if (progressValue != 0)
             _splashScreen.SendCommand( CNETWaitForm.WaitFormCommand.ProgressValue, progressValue);

         if (progressValue > 99)
             Hide();

     }


     #region splash screen

     static UserControl userControlSplash;


     public static void SetUserControlSplash(UserControl usercontrol)
     {
         userControlSplash = usercontrol;
     }

     static Form formSplash;


     public static void SetSplashScreenParentForm(Form parentForm)
     {
         formSplash = parentForm;
     }

     static CNETSplashScreenType splashScreenType;

     public static void SetSplashScreenType(CNETSplashScreenType type)
     {
         splashScreenType = type;

     }

     public static Boolean FullScreenSplashScreen { get; set; }

     public static void SplashScreen(String description, String caption = "Please Wait..", int currentValue = -1, int maxValue = -1)
     {
         //if (FullScreenSplashScreen)
         //    CNETSplSplashScreenT1.FullScreenSplashScreen = true;

         //Type type = typeof(CNETSplSplashScreenT1);

         //if (splashScreenType == null)
         //{
         //    type = typeof(CNETSplSplashScreenT1);

         //}



         ////else if ...
         //if(userControlSplash!=null)
         //    SplashScreenManager.ShowForm(userControlSplash, type, true, true);
            
         //else
         //SplashScreenManager.ShowForm(formSplash, type, true, true, false);



         //SplashScreenManager.Default.SendCommand(CNETSplSplashScreenT1.SplashScreenCommand.Caption, caption);
         //SplashScreenManager.Default.SendCommand(CNETSplSplashScreenT1.SplashScreenCommand.Description, description);

         //int progressValue = CalculatePercent(currentValue, maxValue);


         //if (progressValue != 0)
         //    SplashScreenManager.Default.SendCommand(CNETSplSplashScreenT1.SplashScreenCommand.ProgressValue, progressValue);

         //if (progressValue > 99)
         //    Hide();


     }


     #endregion


     static int MainTaskTotalCount = -1;
     static int MainTaskCurrent = -1;

     static int ChildTaskTotalCount = -1;
     static int ChildTaskCurrent = -1;

     public static void WaitForm(String description, int tasksTotalCount)
     {

         if (MainTaskCurrent == 0) MainTaskCurrent = -1;

         if (MainTaskTotalCount == -1)
         {
             MainTaskTotalCount = tasksTotalCount;

             MainTaskCurrent = 0;

         }

         if (MainTaskTotalCount != -1)
             if (MainTaskTotalCount != tasksTotalCount)
             {
                 MainTaskCurrent = -1;
                 MainTaskTotalCount = tasksTotalCount;

             }
   
         WaitForm(description, "Please Wait..", ++MainTaskCurrent, tasksTotalCount);


     }

     public static void WaitFormChild(String description, int tasksTotalCount, int parentTaskTotalCount)
     {
         if (_splashScreen == null) return;

         if (ChildTaskTotalCount == -1)
         {
             ChildTaskTotalCount = tasksTotalCount;
             ChildTaskCurrent = 0;
         }

         if (ChildTaskTotalCount != -1)
             if (ChildTaskTotalCount != tasksTotalCount)
             {
                 ChildTaskCurrent = -1;
                 ChildTaskTotalCount = tasksTotalCount;
             }

         if (parentTaskTotalCount != MainTaskTotalCount)
         {
             _splashScreen.SendCommand(CNETWaitForm.WaitFormCommand.HideChildProgressPanel, null);

         }

         if (ChildTaskCurrent > ChildTaskTotalCount)
         {
             _splashScreen.SendCommand(CNETWaitForm.WaitFormCommand.HideChildProgressPanel, null);

             return;

         }

         int progressValue = CalculatePercent(++ChildTaskCurrent, ChildTaskTotalCount);

         if (progressValue != 0)
         {

           
             _splashScreen.SendCommand(CNETWaitForm.WaitFormCommand.ProgressValueChild, progressValue);

             _splashScreen.SendCommand(CNETWaitForm.WaitFormCommand.ProgressValueChildDescription, description);
         

         }

         if (ChildTaskCurrent == tasksTotalCount)
         {
             _splashScreen.SendCommand(CNETWaitForm.WaitFormCommand.HideChildProgressPanel, null);

             ChildTaskCurrent = -1;
             ChildTaskTotalCount = -1;
         }






     }

     /// <summary>  Gets or sets a value indicating whether to allways show the progress bar disabling hide calls.  </summary>
     /// <value>    true if to disable calls to hide the control, false if to allow Hiding </value>

     public static Boolean AllwaysShow { get; set; }


        #region Helpers
     public static void Hide()
        {
         if (AllwaysShow == true)
             return;

            if (SplashScreenManager.Default!=null)
        if(SplashScreenManager.Default.IsSplashFormVisible)
         SplashScreenManager.CloseForm(false);

         if (_splashScreen != null)
         {
             _splashScreen.CloseWaitForm();
             _splashScreen = null;

         }
         counter = 0;
         currentMax = -1;

         MainTaskCurrent = -1;
         MainTaskTotalCount = -1;

         ChildTaskTotalCount = -1;
         ChildTaskCurrent = -1;


     }

      


     private static int CalculatePercent(int currentValue, int maxValue)
     {
         int progressValue = 0;
         if (currentValue != -1 && maxValue != -1)


             if (currentValue != -1 && maxValue != -1 && currentValue > 0 && maxValue > 0)
             {
                 progressValue = (currentValue * 100) / maxValue;
             }
         return progressValue;
     }

     #endregion



    }


    public enum CNETSplashScreenType
    {
        POSGlobal,
        ERPGlobal,
        POSRMS,
        POSSMS


    }

    public enum CNETWaitScreenType
    {
        t1
    }
}
