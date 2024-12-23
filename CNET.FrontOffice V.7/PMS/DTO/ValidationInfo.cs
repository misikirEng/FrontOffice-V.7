using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace CNET.FrontOffice_V._7
{
    public class ValidationInfo
    {
        public Control control { get; set; }
        public String InvalidMessage { get; set; }
        public Object ControlLocationInfo { get; set; }
    }
}
