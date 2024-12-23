 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.PmsSchema;

namespace CNET.FrontOffice_V._7
{
    class PackageVM
    {
        public int Id { get; set; }
        public Boolean value { get; set; }
        public String pkgHeader { get; set; }
        public Boolean? isIncluded { get; set; }
        public decimal? quantity { get; set; }
        public PackageHeaderDTO AttachedObject { get; set; }
    }
}
