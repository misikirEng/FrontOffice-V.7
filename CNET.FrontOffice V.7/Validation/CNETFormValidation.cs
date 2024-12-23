using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Validation
{
    public class CNETFormValidation
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public static string IsValid(string value, CNETFormValidation.Rexp type, bool required = true)
        {
            string result = "";
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
                        else if (value.Length > 10000)
                        {
                            result = "It is too long , it is more than 10000 Letter";
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
                    if (value.Length > 10000)
                    {
                        result = "It is too long , it is more than 10000 Letter";
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
                    if (value.Length > 100)
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
                    if (!Regex.IsMatch(value, "^[0-9]{10,12}$"))
                    {
                        result = "TIN must be 10 - 13 digits long.";
                    }
                    break;
            }
            return result;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002200 File Offset: 0x00000400
        public static string IsValid(DateTime minDate, DateTime maxDate, CNETFormValidation.Rexp type, bool required = true)
        {
            return "";
        }

        // Token: 0x02000003 RID: 3
        public enum Rexp
        {
            // Token: 0x04000002 RID: 2
            email,
            // Token: 0x04000003 RID: 3
            PhoneNo,
            // Token: 0x04000004 RID: 4
            Description,
            // Token: 0x04000005 RID: 5
            Remark,
            // Token: 0x04000006 RID: 6
            dropdown,
            // Token: 0x04000007 RID: 7
            Date,
            // Token: 0x04000008 RID: 8
            TIN
        }
    }
}
