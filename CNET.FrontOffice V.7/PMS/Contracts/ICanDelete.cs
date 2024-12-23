using System;
using System.Collections.Generic;
using System.Linq; 

namespace CNET.FrontOffice_V._7.PMS.Contracts
{
    public interface ICanDelete
    {
        DeleteClickedResult OnDelete();
    }
}
