﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 
using DevExpress.XtraEditors;
using System.IO; 
using DevExpress.XtraRichEdit.Model.History;
using ERP.Attachement;

namespace CNETDocumentScanner
{
   
    public partial class ScannerMain: UserControl
    {
        public static string comboval;
        public ScannerMain()
        {
            InitializeComponent();
        }
        
        Control Container;
        //public void SetControlsContainer(Control container)
        //{
        //    Container = container;
        //    container.Controls.Clear();

        //    container.Controls.Add(lcMain);

        //}
        
        
       

        
        //public void scaner(PictureEdit picturebox, System.Windows.Forms.ComboBox lbDevices)
        //{

        //    if (lbDevices.SelectedItem != null)
        //    {
        //        //get images from scanner
        //        List<Image> images = Scan((string)lbDevices.SelectedItem);
        //        foreach (Image image in images)
        //        {

        //            //  pictureBoxDisplay.Image = image;
        //            //undocroppicbox.Image = image;

        //            picturebox.Image = image;
        //           // sacnnedimage = image;
        //            // pictureBoxDisplay.Show();
        //            // pictureBoxDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
        //            //save scanned image into specific folder
        //            //    image.Save(@"E:\" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".jpeg", ImageFormat.Jpeg);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Select a Device");
        //    }
        //}

        //scanner property controls
        #region
        const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";


        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
        }
        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }
        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        }
        /// <summary>
        /// Use scanner to scan an image (with user selecting the scanner from a dialog).
        /// </summary>
        /// <returns>Scanned images.</returns>
      /*  public static List<Image> Scan()
        {
            WIA.ICommonDialog dialog = new WIA.CommonDialog();
            WIA.Device device = dialog.ShowSelectDevice(WIA.WiaDeviceType.UnspecifiedDeviceType, true, false);
            if (device != null)
            {
                return Scan(device.DeviceID);
            }
            else
            {
                throw new Exception("You must select a device for scanning.");
            }
        }*/

        /// <summary>
        /// Use scanner to scan an image (scanner is selected by its unique id).
        /// </summary>
        /// <param name="scannerName"></param>
        /// <returns>Scanned images.</returns>
        public List<Image> Scan(string scannerId)
        {
            List<Image> images = new List<Image>();
            bool hasMorePages = true;
            while (hasMorePages)
            {
                // select the correct scanner using the provided scannerId parameter
                WIA.DeviceManager manager = new WIA.DeviceManager();
                WIA.Device device = null;
                foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                {
                    if (info.DeviceID == scannerId)
                    {
                        // connect to scanner
                        try
                        {
                            device = info.Connect();
                            break;
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show(ex.Message);
                        }
                       
                    }
                }
                try
                {
                    // device was not found
                    if (device == null)
                    {
                        // enumerate available devices
                        string availableDevices = "";
                        foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                        {
                            availableDevices += info.DeviceID + "\n";
                        }

                        // show error with available devices
                        throw new Exception("The device with provided ID could not be found. Available Devices:\n" + availableDevices);
                    }
                }
                catch (Exception exx)
                {
                    XtraMessageBox.Show(exx.Message);
                }
              
                WIA.Item item = device.Items[1] as WIA.Item;
                try
                {
                    // scan image
                    WIA.ICommonDialog wiaCommonDialog = new WIA.CommonDialog();
                    // ImageFile image = ImageFile wiaCommonDialog.ShowTransfer(item, wiaFormatBMP, false);
                    WIA.ImageFile image = (WIA.ImageFile)wiaCommonDialog.ShowTransfer(item, wiaFormatBMP, false);

                    // save to temp file
                    string fileName = Path.GetTempFileName();
                    File.Delete(fileName);
                    image.SaveFile(fileName);
                    image = null;
                    // add file to output list
                    images.Add(Image.FromFile(fileName));
                    
                }
                catch (Exception exc)
                {
                    XtraMessageBox.Show(exc.Message);
                }
                finally
                {
                    item = null;
                    //determine if there are any more pages waiting
                    WIA.Property documentHandlingSelect = null;
                    WIA.Property documentHandlingStatus = null;
                    foreach (WIA.Property prop in device.Properties)
                    {
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                            documentHandlingSelect = prop;
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                            documentHandlingStatus = prop;
                    }
                    // assume there are no more pages
                    hasMorePages = false;
                    // may not exist on flatbed scanner but required for feeder
                    if (documentHandlingSelect != null)
                    {
                        // check for document feeder
                        if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                        {
                            hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                        }
                    }
                }
            }
            return images;
        }

        /// <summary>
        /// Gets the list of available WIA devices.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDevices()
        {
            List<string> devices = new List<string>();
            WIA.DeviceManager manager = new WIA.DeviceManager();
            foreach (WIA.DeviceInfo info in manager.DeviceInfos)
            {
                devices.Add(info.DeviceID);
            }
            return devices;
        }

        #endregion

       
        public static string ComboValue(string ComboVal)
        {
            comboval = ComboVal;
            return comboval;

        }

        private void ScannerMain_Load(object sender, EventArgs e)
        {
           
        }
        public void Run(Image sImage)
        {

            pictureEdit1.Image = sImage;
        }

        public void run()
        {
            pictureEdit1.Image = modalNewAttachment.imm;
        }
        public event ScannerEventHandler SaveClicked;

          public delegate void ScannerEventHandler  (List<Image> images);

          private void pictureEdit1_MouseHover(object sender, EventArgs e)
          {
              pictureEdit1.Focus();
          }

       

       
    }
    
}
