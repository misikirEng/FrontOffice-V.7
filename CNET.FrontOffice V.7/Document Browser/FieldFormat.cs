using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.POS.DocumentBrowser.Models
{
    public class FieldFormat
    {
        public string columnName { get; set; }
        public int? columnWidth { get; set; }
        public int? index { get; set; }
        public string font { get; set; }
        public string color { get; set; }
        public string columnBindName { get; set; }
        public bool? columnWrap { get; set; }
    }
}
