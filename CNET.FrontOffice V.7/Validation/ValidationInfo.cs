using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Validation
{
    public class ValidationInfo
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600000E RID: 14 RVA: 0x00002890 File Offset: 0x00000A90
        // (set) Token: 0x0600000F RID: 15 RVA: 0x000028A7 File Offset: 0x00000AA7
        public Control Control { get; set; }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000010 RID: 16 RVA: 0x000028B0 File Offset: 0x00000AB0
        // (set) Token: 0x06000011 RID: 17 RVA: 0x000028C7 File Offset: 0x00000AC7
        public Control ComparedControl { get; set; }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000012 RID: 18 RVA: 0x000028D0 File Offset: 0x00000AD0
        // (set) Token: 0x06000013 RID: 19 RVA: 0x000028E7 File Offset: 0x00000AE7
        public object DefaultValue { get; set; }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000014 RID: 20 RVA: 0x000028F0 File Offset: 0x00000AF0
        // (set) Token: 0x06000015 RID: 21 RVA: 0x00002907 File Offset: 0x00000B07
        public string InvalidMessage { get; set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000016 RID: 22 RVA: 0x00002910 File Offset: 0x00000B10
        // (set) Token: 0x06000017 RID: 23 RVA: 0x00002927 File Offset: 0x00000B27
        public object ControlLocationInfo { get; set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000018 RID: 24 RVA: 0x00002930 File Offset: 0x00000B30
        // (set) Token: 0x06000019 RID: 25 RVA: 0x00002947 File Offset: 0x00000B47
        public ValidationRule ValidationRule { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600001A RID: 26 RVA: 0x00002950 File Offset: 0x00000B50
        // (set) Token: 0x0600001B RID: 27 RVA: 0x00002967 File Offset: 0x00000B67
        public ConditionOperator ConditionOperator { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600001C RID: 28 RVA: 0x00002970 File Offset: 0x00000B70
        // (set) Token: 0x0600001D RID: 29 RVA: 0x00002987 File Offset: 0x00000B87
        public string GroupName { get; set; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600001E RID: 30 RVA: 0x00002990 File Offset: 0x00000B90
        // (set) Token: 0x0600001F RID: 31 RVA: 0x000029A7 File Offset: 0x00000BA7
        public bool IsValidated { get; set; }

        // Token: 0x06000020 RID: 32 RVA: 0x000029B0 File Offset: 0x00000BB0
        public ValidationInfo(Control comparedControl, CompareControlOperator? compareControlOperator, ConditionOperator? conditionOperator)
        {
            string errorText = string.Empty;
            if (conditionOperator != null)
            {
                if (conditionOperator.Value.HasFlag(ConditionOperator.NotEquals))
                {
                    errorText = "Value cannot be empty.";
                }
                if (conditionOperator.Value.HasFlag(ConditionOperator.Between))
                {
                    errorText = "Please enter a value that between to the first and third value.";
                }
            }
            if (compareControlOperator != null)
            {
                if (compareControlOperator.Value.HasFlag(CompareControlOperator.LessOrEqual))
                {
                    errorText = "Please enter a value that equals or less to the second value.";
                }
                else if (compareControlOperator.Value.HasFlag(CompareControlOperator.Less))
                {
                    errorText = "Please enter a value that is less than the second value.";
                }
                else if (compareControlOperator.Value.HasFlag(CompareControlOperator.GreaterOrEqual))
                {
                    errorText = "Please enter a value that equals or greater to the first value.";
                }
                else if (compareControlOperator.Value.HasFlag(CompareControlOperator.Greater))
                {
                    errorText = "Please enter a value that is greater than the first value.";
                }
            }
            if (compareControlOperator != null)
            {
                CompareAgainstControlValidationRule validationRule = new CompareAgainstControlValidationRule
                {
                    Control = comparedControl,
                    CompareControlOperator = compareControlOperator.Value,
                    ErrorText = errorText,
                    CaseSensitive = true
                };
                this.ValidationRule = validationRule;
            }
        }
    }
}
