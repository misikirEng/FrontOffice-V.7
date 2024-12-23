 
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Validation
{
    public class CustomValidationRule : ConditionValidationRule
    {
        // Token: 0x06000004 RID: 4 RVA: 0x00002220 File Offset: 0x00000420
        public static string IsValid(string value, CNETFormValidation.Rexp type, bool required = true)
        {
            string result = string.Empty;
            switch (type)
            {
                case CNETFormValidation.Rexp.email:
                    {
                        if (required)
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                result = "This value can n't be null";
                                break;
                            }
                        }
                        Regex regex = new Regex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$");
                        Match match = regex.Match(value);
                        if (!match.Success)
                        {
                            result = " incorrect Format , example@.net";
                        }
                        else if (value.Length > 40)
                        {
                            result = "It is too long , it is more than 40 Letter";
                        }
                        break;
                    }
                case CNETFormValidation.Rexp.Description:
                    if (required)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            result = "This value can n't be null";
                        }
                    }
                    if (value != null && value.Length > 40)
                    {
                        result = "It is too long , it is more than 40 Letter";
                    }
                    break;
                case CNETFormValidation.Rexp.Remark:
                    if (required)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            result = "This value can n't be null";
                        }
                    }
                    if (value != null && value.Length > 100)
                    {
                        result = "It is too long , it is more than 100 Letter";
                    }
                    break;
                case CNETFormValidation.Rexp.dropdown:
                    if (string.IsNullOrEmpty(value))
                    {
                        result = "This value can n't be null";
                    }
                    break;
                case CNETFormValidation.Rexp.Date:
                    if (required)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            result = "This value can n't be null";
                        }
                    }
                    else if (Convert.ToDateTime(value) > DateTime.Now)
                    {
                        result = "Date is in the future";
                    }
                    break;
                case CNETFormValidation.Rexp.TIN:
                    if (!Regex.IsMatch(value, "\\^[0-9]{10,13}\\$"))
                    {
                        result = "TIN must be 10 - 13 digits long.";
                    }
                    break;
            }
            return result;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x000023D8 File Offset: 0x000005D8
        public static IList<Control> Validate(List<Control> controlsList, bool required = true)
        {
            CustomValidationRule._dxValidationProvider = new DXValidationProvider
            {
                ValidationMode = ValidationMode.Auto
            };
            foreach (Control control in controlsList)
            {
                if (required)
                {
                    ConditionValidationRule rule = new ConditionValidationRule
                    {
                        ConditionOperator = ConditionOperator.IsNotBlank,
                        ErrorText = "Value cannot be empty."
                    };
                    CustomValidationRule._dxValidationProvider.SetValidationRule(control, rule);
                    CustomValidationRule._dxValidationProvider.Validate();
                }
            }
            return CustomValidationRule._dxValidationProvider.GetInvalidControls();
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002490 File Offset: 0x00000690
        public static IList<Control> Validate2(IList<ValidationInfo> validationInfos)
        {
            CustomValidationRule._dxValidationProvider = new DXValidationProvider
            {
                ValidationMode = ValidationMode.Auto
            };
            foreach (ValidationInfo validationInfo in validationInfos)
            {
                if (validationInfo.IsValidated)
                {
                    CustomValidationRule._dxValidationProvider.SetValidationRule(validationInfo.Control, validationInfo.ValidationRule);
                    CustomValidationRule._dxValidationProvider.Validate();
                }
            }
            return CustomValidationRule._dxValidationProvider.GetInvalidControls();
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002538 File Offset: 0x00000738
        public static IList<Control> Validate(List<ValidationInfo> controlsList)
        {
            CustomValidationRule._dxValidationProvider = new DXValidationProvider
            {
                ValidationMode = ValidationMode.Auto
            };
            foreach (ValidationInfo validationInfo in controlsList)
            {
                if (validationInfo.IsValidated)
                {
                    ConditionValidationRule rule = new ConditionValidationRule
                    {
                        ConditionOperator = validationInfo.ConditionOperator,
                        ErrorText = validationInfo.InvalidMessage
                    };
                    CustomValidationRule._dxValidationProvider.SetValidationRule(validationInfo.Control, rule);
                    CustomValidationRule._dxValidationProvider.Validate();
                    CustomValidationRule._dxValidationProvider.SetIconAlignment(validationInfo.Control, ErrorIconAlignment.MiddleLeft);
                }
            }
            return CustomValidationRule._dxValidationProvider.GetInvalidControls();
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002610 File Offset: 0x00000810
        public static void RemoveInvalidatedControls(List<Control> controls)
        {
            if (!object.Equals(CustomValidationRule._dxValidationProvider, null))
            {
                ConditionValidationRule rule = new ConditionValidationRule();
                foreach (Control control in controls)
                {
                    CustomValidationRule._dxValidationProvider.SetValidationRule(control, rule);
                    CustomValidationRule._dxValidationProvider.Validate(control);
                    CustomValidationRule._dxValidationProvider.RemoveControlError(control);
                }
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000026A0 File Offset: 0x000008A0
        public static void RemoveInvalidatedControls(List<ValidationInfo> validationInfos)
        {
            if (!object.Equals(CustomValidationRule._dxValidationProvider, null))
            {
                foreach (ValidationInfo validationInfo in validationInfos)
                {
                    CustomValidationRule._dxValidationProvider.SetValidationRule(validationInfo.Control, null);
                    CustomValidationRule._dxValidationProvider.Validate(validationInfo.Control);
                    CustomValidationRule._dxValidationProvider.RemoveControlError(validationInfo.Control);
                }
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002738 File Offset: 0x00000938
        public static void RemoveInvalidatedControl(Control control)
        {
            try
            {
                if (!object.Equals(CustomValidationRule._dxValidationProvider, null))
                {
                    CustomValidationRule._dxValidationProvider.RemoveControlError(control);
                    CustomValidationRule._dxValidationProvider.SetValidationRule(control, null);
                    CustomValidationRule._dxValidationProvider.Validate(control);
                }
            }
            catch (Exception)
            {
            }
        }

        // Token: 0x0600000B RID: 11 RVA: 0x0000279C File Offset: 0x0000099C
        public static bool Validate(Control control, bool required = true)
        {
            CustomValidationRule._dxValidationProvider = new DXValidationProvider
            {
                ValidationMode = ValidationMode.Auto
            };
            if (required)
            {
                ConditionValidationRule rule = new ConditionValidationRule
                {
                    ConditionOperator = ConditionOperator.IsNotBlank,
                    ErrorText = "Value cannot be empty."
                };
                CustomValidationRule._dxValidationProvider.SetValidationRule(control, rule);
            }
            return CustomValidationRule._dxValidationProvider.Validate();
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002804 File Offset: 0x00000A04
        public static bool Validate(Control control, string value, string validationType)
        {
            bool result = false;
            if (validationType != null)
            {
                if (!(validationType == "Email"))
                {
                    if (!(validationType == "Mobile phone"))
                    {
                        if (!(validationType == "Website"))
                        {
                            if (!(validationType == "Telephone"))
                            {
                            }
                        }
                    }
                }
                else
                {
                    Regex regex = new Regex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        // Token: 0x04000009 RID: 9
        private static DXValidationProvider _dxValidationProvider;
    }
}
