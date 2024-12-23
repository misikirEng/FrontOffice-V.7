using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    public class LoadEventArgs : EventArgs
    {
        public LoadEventArgs()
        {
        }
        public LoadEventArgs(UILogicBase sender, Object args)
        {
            Sender = sender;
            Args = args;
        }

        public UILogicBase Sender { get; set; }
        public Object Args { get; set; }
    }
}
