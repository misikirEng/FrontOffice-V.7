using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7.CustomEventArgs
{
    public class TabChangedEventArgs : EventArgs
    {
        public TabChangedEventArgs(XtraTabPage prevuiousPage, XtraTabPage selectedPage, String selectedPageName)
        {
            PreviousPage = prevuiousPage;
            SelectedPage = selectedPage;
            SelectedPageName = selectedPageName;
        }
        public XtraTabPage PreviousPage;
        public XtraTabPage SelectedPage;
        public String SelectedPageName;
    }
}
