using System;
using System.Collections.Generic;
using System.Linq; 
using CNET.FrontOffice_V._7.Validation;

namespace CNET.FrontOffice_V._7.PMS.Contracts
{
    public interface ICanCNETValidate
    {
        bool IsFormValid();

        List<ValidationInfo> GetInvalidFormControls();
    }
}
