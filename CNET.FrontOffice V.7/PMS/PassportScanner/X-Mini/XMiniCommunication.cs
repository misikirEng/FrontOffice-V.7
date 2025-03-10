using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.PassportScanner.X_Mini
{
    public class XMiniCommunication
    {
        static DeviceWrapper.LIBWFXEVENTCB m_CBEvent;
        public static string ImageFullPath { get; set; }
        private static string ImageDirectory { get; set; }
        public static bool checkDevice()
        {
            CloseConnection();
            ENUM_LIBWFX_ERRCODE enErrCode;
            enErrCode = DeviceWrapper.LibWFX_InitEx(ENUM_LIBWFX_INIT_MODE.LIBWFX_INIT_MODE_NOOCR);
            if (enErrCode != ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_SUCCESS)
            {
                MessageBox.Show(@"Status:[LibWFX_InitEx Fail [" + ((int)enErrCode).ToString() + "]] "); //get fail message
                return false;
            }
            IntPtr devicee = IntPtr.Zero;

            enErrCode = DeviceWrapper.LibWFX_GetDeviesList(out devicee);
            if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_DEVICES)
            { 
                return false;
            }
            CloseConnection();
            return true;
        }
        public static Image ScanPassport()
        {

            CloseConnection();

            Image PassportImage = null;
            //ImageFullPath = @"C:\\Users\\RND01\\AppData\\Local\\Temp\\WebFXScan\\New folder\\IMG_276459234_00001.jpg";
            //PassportImage = Image.FromFile(ImageFullPath);

            //return PassportImage;


            List<ENUM_LIBWFX_ERRCODE> errorlisttoignor = new List<ENUM_LIBWFX_ERRCODE>
            {
                ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_OCR,
                ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_AVI_OCR,
                ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_DOC_OCR,
                ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_SUCCESS
            };

            ENUM_LIBWFX_ERRCODE enErrCode;

            string localdirectory = Environment.CurrentDirectory + "\\Passport Scanner Images";
            if (!Directory.Exists(localdirectory))
            {
                Directory.CreateDirectory(localdirectory);
            }
            ImageDirectory = localdirectory;

            IntPtr pScanImageList, pOCRResultList, pExceptionRet, pEventRet;
            string Command = "";// "{\"device-name\":\"A64\",\"source\":\"Camera\",\"recognize-type\":\"passport\,\"mode\":\"color\"}"; 


            CommandObject commandObject = new CommandObject()
            {
                DeviceName = "A64",
                Source = "Camera",
                RecognizeType = "passport",
                Mode = "color",
                SavePath = ImageDirectory
            };

            Command = JsonConvert.SerializeObject(commandObject);

            //get command from bat file "AutoCaptureDemo-CSharp.bat"
            //String[] arguments = Environment.GetCommandLineArgs();
            //if (arguments.Length > 1)
            //    Command = arguments[1];

            enErrCode = DeviceWrapper.LibWFX_InitEx(ENUM_LIBWFX_INIT_MODE.LIBWFX_INIT_MODE_NOOCR);


            if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_PATH_TOO_LONG)
            {
                MessageBox.Show(@"Status:[Path Is Too Long (max limit: 130 bits)]");
                //MessageBox.Show(@"Status:[LibWFX_InitEx Fail]");
            }
            else if (!errorlisttoignor.Contains(enErrCode))
            {

                MessageBox.Show(@"Status:[LibWFX_InitEx Fail [" + ((int)enErrCode).ToString() + "]] "); //get fail message
                return null ;
            }
            enErrCode = DeviceWrapper.LibWFX_SetProperty(Command, m_CBEvent, IntPtr.Zero);
            if (enErrCode != ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_SUCCESS)
            {
                IntPtr pstr;
                DeviceWrapper.LibWFX_GetLastErrorCode(enErrCode, out pstr);
                string szErrorMsg = Marshal.PtrToStringUni(pstr);
                MessageBox.Show(@"Status:[LibWFX_SetProperty Fail [" + ((int)enErrCode).ToString() + "]] " + szErrorMsg.ToString()); //get fail message
            }



            enErrCode = DeviceWrapper.LibWFX_PaperReady();
            if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_PAPER_NOT_READY)
            {
                MessageBox.Show(@"Please Input Passport !!");
                return null;
            }
            //if (DoScan)
            //{
            //    MessageBox.Show(@"The card is continuously detected, please remove the card.");
            //    Thread.Sleep(1000);  //option
            //    continue;
            //}

            enErrCode = DeviceWrapper.LibWFX_SynchronizeScan(Command, out pScanImageList, out pOCRResultList, out pExceptionRet, out pEventRet);

            string szExceptionRet = Marshal.PtrToStringUni(pExceptionRet);
            string szEventRet = Marshal.PtrToStringUni(pEventRet);

            if (enErrCode != ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_SUCCESS && enErrCode != ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_COMMAND_KEY_MISMATCH)
            {
                IntPtr pstr;
                DeviceWrapper.LibWFX_GetLastErrorCode(enErrCode, out pstr);
                string szErrorMsg = Marshal.PtrToStringUni(pstr);
                MessageBox.Show(@"Status:[LibWFX_SynchronizeScan Fail [" + ((int)enErrCode).ToString() + "]] " + szErrorMsg.ToString()); //get fail message
            }
            else if (szEventRet.Length > 1) //event happen
            {
                MessageBox.Show(@"Status:[Device Ready!]");
                MessageBox.Show(szEventRet);  //get event message

                if (szEventRet != "LIBWFX_EVENT_UVSECURITY_DETECTED[0]" && szEventRet != "LIBWFX_EVENT_UVSECURITY_DETECTED[1]")
                {
                    MessageBox.Show(@"Status:[Scan End]\n");
                    return null;
                }

                if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_COMMAND_KEY_MISMATCH)
                    MessageBox.Show(@"Status:[There are some mismatched key in command]");

                string szScanImageList = Marshal.PtrToStringUni(pScanImageList);
                string szOCRResultList = Marshal.PtrToStringUni(pOCRResultList);


                string[] ScanImageWords = szScanImageList.Split(new string[] { "|&|" }, System.StringSplitOptions.None);
                string[] OCRResultWords = szOCRResultList.Split(new string[] { "|&|" }, System.StringSplitOptions.None);

                for (int idx = 0; idx < ScanImageWords.Length - 1; idx++)
                {
                    //MessageBox.Show(ScanImageWords[idx].Trim());  //get each image path
                    PassportImage =Image.FromFile(ScanImageWords[idx].Trim());
                    ImageFullPath = ScanImageWords[idx].Trim();
                    // MessageBox.Show(OCRResultWords[idx].Trim());  //get each ocr result
                }
            }
            else
            {
                //MessageBox.Show(@"Status:[Device Ready!]");

                if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_COMMAND_KEY_MISMATCH)
                    MessageBox.Show(@"Status:[There are some mismatched key in command]");

                if (szExceptionRet.Length > 1) //exception happen
                {
                    MessageBox.Show(@"Status:[Device Ready!]");
                    MessageBox.Show(@szExceptionRet);  //get exception message
                }

                string szScanImageList = Marshal.PtrToStringUni(pScanImageList);
                // string szOCRResultList = Marshal.PtrToStringUni(pOCRResultList);


                string[] ScanImageWords = szScanImageList.Split(new string[] { "|&|" }, System.StringSplitOptions.None);
                // string[] OCRResultWords = szOCRResultList.Split(new string[] { "|&|" }, System.StringSplitOptions.None);

                for (int idx = 0; idx < ScanImageWords.Length - 1; idx++)
                {
                    // MessageBox.Show(ScanImageWords[idx].Trim());  //get each image path
                    PassportImage = Image.FromFile(ScanImageWords[idx].Trim());
                    ImageFullPath = ScanImageWords[idx].Trim();
                    //  MessageBox.Show(OCRResultWords[idx].Trim());  //get each ocr result
                }
            }

            CloseConnection();
            return PassportImage;
        }


        private static void CloseConnection()
        {
            DeviceWrapper.LibWFX_CloseDevice();
            DeviceWrapper.LibWFX_DeInit();
            // Environment.Exit(0);
        }
        public static void CalibratePassPortScanner()
        {
            ENUM_LIBWFX_ERRCODE enErrCode;
            bool DoScan = false;
            IntPtr pScanImageList, pOCRResultList, pExceptionRet, pEventRet;
            string Command = "{\"device-name\":\"A64\",\"source\":\"Camera\",\"recognize-type\":\"passport\"}";

            //get command from bat file "AutoCaptureDemo-CSharp.bat"
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
                Command = arguments[1];

            enErrCode = DeviceWrapper.LibWFX_InitEx(ENUM_LIBWFX_INIT_MODE.LIBWFX_INIT_MODE_NOOCR);

            if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_OCR)
                MessageBox.Show(@"Status:[No Recognize Tool]");
            else if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_AVI_OCR)
                MessageBox.Show(@"Status:[No AVI Recognize Tool]");
            else if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_NO_DOC_OCR)
                MessageBox.Show(@"Status:[No DOC Recognize Tool]");
            else if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_PATH_TOO_LONG)
            {
                MessageBox.Show(@"Status:[Path Is Too Long (max limit: 130 bits)]");
                MessageBox.Show(@"Status:[LibWFX_InitEx Fail]");
            }
            else if (enErrCode != ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_SUCCESS)
            {
                MessageBox.Show(@"Status:[LibWFX_InitEx Fail [" + ((int)enErrCode).ToString() + "]] "); //get fail message
                return;
            }

            enErrCode = DeviceWrapper.LibWFX_SetProperty(Command, m_CBEvent, IntPtr.Zero);
            if (enErrCode != ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_SUCCESS)
            {
                IntPtr pstr;
                DeviceWrapper.LibWFX_GetLastErrorCode(enErrCode, out pstr);
                string szErrorMsg = Marshal.PtrToStringUni(pstr);
                MessageBox.Show(@"Status:[LibWFX_SetProperty Fail [" + ((int)enErrCode).ToString() + "]] " + szErrorMsg.ToString()); //get fail message
            }



            enErrCode = DeviceWrapper.LibWFX_PaperReady();
            if (enErrCode == ENUM_LIBWFX_ERRCODE.LIBWFX_ERRCODE_PAPER_NOT_READY)
            {
                MessageBox.Show(@"Please Input Passport !!");
                return;
            }
            //if (DoScan)
            //{
            //    MessageBox.Show(@"The card is continuously detected, please remove the card.");
            //    Thread.Sleep(1000);  //option
            //    continue;
            //}
            CloseConnection();
        }



        public class CommandObject
        {
            [JsonProperty("device-name")]
            public string DeviceName { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("recognize-type")]
            public string RecognizeType { get; set; }

            [JsonProperty("mode")]
            public string Mode { get; set; }

            [JsonProperty("savepath")]
            public string SavePath { get; set; }
        }
    }
}
