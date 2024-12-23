using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
using CNETResourceProvider.Resource_Provider;
//

namespace CNET.ERP.ResourceProvider 
{
    public  static class Provider
    {
        private static IconBrowser controlPanel = new IconBrowser();


        public static ImageCollection GetImageCollection(PictureSize pictureSize, ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.APPLICATIONICON:
                    {
                        switch (pictureSize)
                        {
                            case PictureSize.Dimension_8X8:
                                return controlPanel.icIcon8;
                            case PictureSize.Dimension_16X16:
                                return controlPanel.icicon16;
                            case PictureSize.Dimension_20X20:
                                return controlPanel.icIcon20;
                            case PictureSize.Dimension_32X32:
                                return controlPanel.icIcon32;
                            case PictureSize.Dimension_24X24:
                                return controlPanel.icicon24;
                            case PictureSize.Dimension_28X28:
                                return controlPanel.icicon28;
                            case PictureSize.Dimension_40X40:
                                return controlPanel.icIcon40;
                            case PictureSize.Dimension_72X72:
                                return controlPanel.icIcon72;
                        }
                    }
                    break;
                case ProviderType.IMAGE:
                    return controlPanel.icRMSPOSItems;
                case ProviderType.OTHER:
                    return controlPanel.icLogoImages;
            }
            return null;
        }

        public static Image GetSharedImageCollection(String imageName, ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.APPLICATIONICON:
                    return controlPanel.icIcon40.Images[imageName];
                case ProviderType.IMAGE:
                    return controlPanel.icRMSPOSItems.Images[imageName];
                case ProviderType.OTHER:
                    {
                        Image im = controlPanel.icDummyImages.Images[imageName];
                        if (im != null)
                        {
                            return im;
                        }
                        else
                        {
                            im = controlPanel.icLogoImages.Images[imageName];
                            if (im != null)
                            {
                                return im;
                            }
                            else
                            {
                                im = controlPanel.icLogOff.Images[imageName];
                                if (im != null)
                                {
                                    return im;
                                }
                                return null;
                            }
                        }
                    }
            }
            return null;
        }

        public static Image GetImage(String imageName, ProviderType providerType)
        {
            switch (providerType)
            {
                case ProviderType.APPLICATIONICON:
                    return controlPanel.icIcon40.Images[imageName];
                case ProviderType.IMAGE:
                    return controlPanel.icRMSPOSItems.Images[imageName];
                case ProviderType.OTHER:
                    {
                       Image im = controlPanel.icDummyImages.Images[imageName];
                        if (im != null)
                        {
                            return im;
                        }
                        else
                        {
                            im = controlPanel.icLogoImages.Images[imageName];
                            if (im != null)
                            {
                                return im;
                            }
                            else
                            {
                                im = controlPanel.icLogOff.Images[imageName];
                                if (im != null)
                                {
                                    return im;
                                }
                                return null;
                            }
                        }
                    }
            }
            return null;


        }

        public static Image GetImage(String imageName, ProviderType providerType, PictureSize iconSize)
        { 
            Image im;
            switch (providerType)
            {
                case ProviderType.IMAGE:
                    { 
                        return controlPanel.icRMSPOSItems.Images[imageName]; 
                    }
                case ProviderType.APPLICATIONICON:
                    {
                        switch (iconSize)
                        {
                            case PictureSize.Dimension_8X8:
                                return controlPanel.icIcon8.Images[imageName]; 
                            case PictureSize.Dimension_16X16:
                                return controlPanel.icicon16.Images[imageName]; 
                            case PictureSize.Dimension_20X20:
                                return controlPanel.icIcon20.Images[imageName]; 
                            case PictureSize.Dimension_24X24:
                                return controlPanel.icicon24.Images[imageName]; 
                            case PictureSize.Dimension_28X28:
                                return controlPanel.icicon28.Images[imageName];
                            case PictureSize.Dimension_32X32:
                                return controlPanel.icIcon32.Images[imageName]; 
                            case PictureSize.Dimension_40X40:
                                return controlPanel.icIcon40.Images[imageName];
                            case PictureSize.Dimension_72X72:
                                return controlPanel.icIcon72.Images[imageName]; 
                        }
                        return null;
                    }
                case ProviderType.OTHER:
                    {
                        im = controlPanel.icDummyImages.Images[imageName];
                        if (im != null)
                        {
                            return im;
                        }
                        else
                        {
                            im = controlPanel.icLogoImages.Images[imageName];
                            if (im != null)
                            {
                                return im;
                            }
                            else
                            {
                                im = controlPanel.icLogOff.Images[imageName];
                                if (im != null)
                                {
                                    return im;
                                }
                                return null;
                            }
                        }
                    }
            }
            return null;
        }

        public static Image GetImage(int imageId, PictureSize pictureSize)
        {

            switch (pictureSize)
            {
                case PictureSize.Dimension_8X8:
                    return controlPanel.icIcon8.Images[imageId];
                case PictureSize.Dimension_16X16:
                    return controlPanel.icicon16.Images[imageId];
                case PictureSize.Dimension_24X24:
                    return controlPanel.icicon24.Images[imageId];
                case PictureSize.Dimension_28X28:
                    return controlPanel.icicon28.Images[imageId];
                case PictureSize.Dimension_32X32:
                    return controlPanel.icIcon32.Images[imageId];
                case PictureSize.Dimension_40X40:
                    return controlPanel.icIcon32.Images[imageId];
                case PictureSize.Dimension_72X72:
                    return controlPanel.icIcon72.Images[imageId]; 
            }
            return null;
        }

        // CNET.ERP.ResourceProvider.IconAndImageProvider  static Image GetIcon(String iconName, PictureSize iconSize, ProviderType providerType)

        public static Image GetIcon(String iconName, PictureSize iconSize)
        {
            switch (iconSize)
            {
                case PictureSize.Dimension_8X8:
                    return controlPanel.icIcon8.Images[iconName];
                case PictureSize.Dimension_16X16:
                    return controlPanel.icicon16.Images[iconName];
                case PictureSize.Dimension_20X20:
                    return controlPanel.icIcon20.Images[iconName];
                case PictureSize.Dimension_24X24:
                    return controlPanel.icicon24.Images[iconName];
                case PictureSize.Dimension_28X28:
                    return controlPanel.icicon28.Images[iconName];
                case PictureSize.Dimension_32X32:
                    return controlPanel.icicon16.Images[iconName];
                case PictureSize.Dimension_40X40:
                    return controlPanel.icIcon40.Images[iconName];
                case PictureSize.Dimension_72X72:
                    return controlPanel.icIcon72.Images[iconName]; 
            }
            return null;


        }

        public static Image GetIcon(int iconId, PictureSize iconSize, ProviderType providerType)
        {
            Image im;
            switch (providerType)
            {
                case ProviderType.IMAGE:
                    {
                        return controlPanel.icRMSPOSItems.Images[iconId];
                    }
                case ProviderType.APPLICATIONICON:
                    {
                        switch (iconSize)
                        {
                            case PictureSize.Dimension_8X8:
                                return controlPanel.icIcon8.Images[iconId];
                            case PictureSize.Dimension_16X16:
                                return controlPanel.icicon16.Images[iconId];
                            case PictureSize.Dimension_20X20:
                                return controlPanel.icIcon20.Images[iconId];
                            case PictureSize.Dimension_24X24:
                                return controlPanel.icicon24.Images[iconId];
                            case PictureSize.Dimension_28X28:
                                return controlPanel.icicon28.Images[iconId];
                            case PictureSize.Dimension_32X32:
                                return controlPanel.icIcon32.Images[iconId];
                            case PictureSize.Dimension_40X40:
                                return controlPanel.icIcon40.Images[iconId];
                            case PictureSize.Dimension_72X72:
                                return controlPanel.icIcon72.Images[iconId]; 
                        }
                        return null;
                    }
                case ProviderType.OTHER:
                    {
                        im = controlPanel.icDummyImages.Images[iconId];
                        if (im != null)
                        {
                            return im;
                        }
                        else
                        {
                            im = controlPanel.icLogoImages.Images[iconId];
                            if (im != null)
                            {
                                return im;
                            }
                            else
                            {
                                im = controlPanel.icLogOff.Images[iconId];
                                if (im != null)
                                {
                                    return im;
                                }
                                return null;
                            }
                        }
                    }
            }
            return null;





            return null;
        }

        public  static int GetImageIndex(string imageName, ProviderType providerType, PictureSize iconSize)
        {//Gets index of an Image; Images are in a sharedImageCollection 
            Image im;
            switch (providerType)
            {
                case ProviderType.IMAGE:
                    {
                        //im = controlPanel.shrdICImages.ImageSource.Images[imageName];
                        im = controlPanel.icRMSPOSItems.Images[imageName];
                        return controlPanel.icRMSPOSItems.Images.IndexOf(im);
                    }
                case ProviderType.APPLICATIONICON:
                    {
                        switch (iconSize)
                        {
                            case PictureSize.Dimension_8X8:
                                im = controlPanel.icIcon8.Images[imageName];
                                return controlPanel.icIcon8.Images.IndexOf(im);

                            case PictureSize.Dimension_16X16:
                                im = controlPanel.icicon16.Images[imageName];
                                return controlPanel.icicon16.Images.IndexOf(im);

                            case PictureSize.Dimension_20X20:
                                im = controlPanel.icIcon20.Images[imageName];
                                return controlPanel.icIcon20.Images.IndexOf(im);

                            case PictureSize.Dimension_24X24:
                                im = controlPanel.icicon24.Images[imageName];
                                return controlPanel.icicon24.Images.IndexOf(im);

                            case PictureSize.Dimension_28X28:
                                im = controlPanel.icicon28.Images[imageName];
                                return controlPanel.icicon28.Images.IndexOf(im);


                            case PictureSize.Dimension_32X32:
                                im = controlPanel.icIcon32.Images[imageName];
                                return controlPanel.icIcon32.Images.IndexOf(im);

                            case PictureSize.Dimension_40X40:
                                im = controlPanel.icIcon40.Images[imageName];
                                return controlPanel.icIcon40.Images.IndexOf(im);
                            case PictureSize.Dimension_72X72:
                                im = controlPanel.icIcon72.Images[imageName];
                                return controlPanel.icIcon72.Images.IndexOf(im);

                        }
                        return 0;
                    }
                case ProviderType.OTHER:
                    {
                        im = controlPanel.icDummyImages.Images[imageName];
                        if (im != null)
                        {
                            return controlPanel.icDummyImages.Images.IndexOf(im);
                        }
                        else
                        {
                            im = controlPanel.icLogoImages.Images[imageName];
                            if (im != null)
                            {
                                return controlPanel.icDummyImages.Images.IndexOf(im);
                            }
                            else
                            {
                                im = controlPanel.icLogOff.Images[imageName];
                                if (im != null)
                                {
                                    return controlPanel.icDummyImages.Images.IndexOf(im);
                                }
                                return 0;
                            }
                        }
                    }


            }

            return 0;

        }

        public  static string GetImageName(int imageIndex, ProviderType providerType, PictureSize iconSize)
        {
            //Gets index of an Image; Images are in a sharedImageCollection 
            string name;
            switch (providerType)
            {
                case ProviderType.IMAGE:
                    {
                        return controlPanel.icRMSPOSItems.Images.Keys[imageIndex];
                    }
                case ProviderType.APPLICATIONICON:
                    {
                        switch (iconSize)
                        {
                            case PictureSize.Dimension_8X8:
                                return controlPanel.icIcon8.Images.Keys[imageIndex];

                            case PictureSize.Dimension_16X16:
                                return controlPanel.icicon16.Images.Keys[imageIndex];

                            case PictureSize.Dimension_20X20:
                                return controlPanel.icIcon20.Images.Keys[imageIndex];


                            case PictureSize.Dimension_24X24:
                                return controlPanel.icicon24.Images.Keys[imageIndex];

                            case PictureSize.Dimension_28X28:
                                return controlPanel.icicon28.Images.Keys[imageIndex];

                            case PictureSize.Dimension_32X32:
                                return controlPanel.icIcon32.Images.Keys[imageIndex];

                            case PictureSize.Dimension_40X40:
                                return controlPanel.icIcon40.Images.Keys[imageIndex];
                            case PictureSize.Dimension_72X72:
                                return controlPanel.icIcon72.Images.Keys[imageIndex];
                        }
                        return null;

                    }
                case ProviderType.OTHER:
                    {
                        name = controlPanel.icDummyImages.Images.Keys[imageIndex];
                        if (string.IsNullOrEmpty(name))
                        {
                            return name; 
                        }
                        else
                        {
                            name = controlPanel.icDummyImages.Images.Keys[imageIndex];
                            if (string.IsNullOrEmpty(name))
                            {
                                return name;
                            }
                            else
                            {
                                name = controlPanel.icDummyImages.Images.Keys[imageIndex];
                                if (string.IsNullOrEmpty(name))
                                {
                                    return name;
                                }
                                return null;
                            }
                        }
                    }


            }

            return null;

        }

        public  static void ApplyIconsForRibbon(RibbonControl ribbon)
        {
            ribbon.LargeImages = Provider.GetImageCollection(PictureSize.Dimension_40X40, ProviderType.APPLICATIONICON);
            string caption;

            foreach (BarItem item in ribbon.Items)
            {

                item.LargeImageIndex = Provider.GetImageIndex(item.Caption, ProviderType.APPLICATIONICON, PictureSize.Dimension_40X40);

            }

            foreach (RibbonPage page in ribbon.Pages)
            {

                page.ImageIndex = Provider.GetImageIndex(page.Text, ProviderType.APPLICATIONICON, PictureSize.Dimension_40X40);


            }


        }
        public  static void ApplyIconsForRibbon(RibbonControl ribbon, PictureSize iconSize)
        {
            string caption;
            ribbon.LargeImages = Provider.GetImageCollection(iconSize, ProviderType.APPLICATIONICON);

            ribbon.Images = Provider.GetImageCollection(iconSize, ProviderType.APPLICATIONICON);

            foreach (BarItem item in ribbon.Items)
            {
                item.LargeImageIndex = Provider.GetImageIndex(item.Caption, ProviderType.APPLICATIONICON, iconSize);

                BarButtonItem bbi = item as BarButtonItem;

                if (item is BarSubItem)
                {

                    item.LargeImageIndex = Provider.GetImageIndex(item.Caption, ProviderType.APPLICATIONICON, iconSize);
                    BarSubItem subitem = item as BarSubItem;

                    foreach (BarButtonItemLink bsil in subitem.ItemLinks)
                    {

                        // caption = item.Caption.Replace(" ", "_");
                        bsil.Visible = true;
                        bsil.ImageIndex = Provider.GetImageIndex(bsil.Caption, ProviderType.APPLICATIONICON, iconSize);
                        //sil.Links[0]. PaintStyle = BarItemPaintStyle.CaptionGlyph;
                        bsil.UserRibbonStyle = RibbonItemStyles.Large;
                    }

                }

                foreach (RibbonPage page in ribbon.Pages)
                {

                    page.ImageIndex = Provider.GetImageIndex(page.Text, ProviderType.APPLICATIONICON, iconSize);


                }


            }
        }

        public static void ApplyIconsForImageListBoxControls(ImageListBoxControl imageBoxControl)
        {
            imageBoxControl.ImageList = Provider.GetImageCollection(PictureSize.Dimension_24X24, ProviderType.APPLICATIONICON);
            string caption;
            foreach (ImageListBoxItem item in imageBoxControl.Items)
            {

                caption = item.Value.ToString();
                if (caption.Contains(" "))
                    caption.Replace(" ", "_");


                item.ImageIndex = Provider.GetImageIndex(caption, ProviderType.APPLICATIONICON, PictureSize.Dimension_24X24);

            }






        }
        public  static void ApplyIconsForImageListBoxControls(ImageListBoxControl imageBoxControl, PictureSize pictureDimention)
        {
            imageBoxControl.ImageList = Provider.GetImageCollection(pictureDimention, ProviderType.APPLICATIONICON);
            string caption;
            foreach (ImageListBoxItem item in imageBoxControl.Items)
            {

                caption = item.Value.ToString().Trim();



                item.ImageIndex = Provider.GetImageIndex(caption, ProviderType.APPLICATIONICON, pictureDimention);

            }






        }
         
    }
    public enum PictureSize
    {
        // ICON SIZES
        Dimension_8X8,//for mobile devices
        Dimension_16X16,
        Dimension_20X20,
        Dimension_24X24,
        Dimension_28X28,
        Dimension_32X32,
        Dimension_40X40,
        Dimension_72X72,


        //IMAGE SIZEs
        //Dimension_640X480,
        //Dimension_800X600,
        //Dimension_1024X768,
        //Dimension_VARIABLE,
        //Dimension_FIXED

    }

    public enum ProviderType
    {
        APPLICATIONICON,
        IMAGE,
        OTHER
    } 
}
