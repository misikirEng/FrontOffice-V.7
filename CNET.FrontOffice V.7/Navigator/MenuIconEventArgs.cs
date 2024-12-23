using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CNET.ERP.Client.Common.UI.Library.DLLLibrary.Navigator
{
    public class MenuIconEventArgs:EventArgs
    {
        public MenuIconEventArgs(Image iconImage,String iconName)
        {
            IconImage = iconImage;
            IconName = iconName;
        }

        public MenuIconEventArgs(String iconName)
        {
            IconName = iconName;
        }



        public Image IconImage { get; set; }
        public String IconName { get; set; }

    }
}
