using System;
using System.Collections.Generic;
using System.Linq; 

namespace CNET.FrontOffice_V._7.PMS.Contracts
{
    public interface ILogicHelper
    {
        void InitializeUI();
        void InitializeData();

        void LoadData(UILogicBase requesterForm, object args);
    }
}
