using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class RouteVM
    {
        public int Id { get; set; }
        public string FlightCode { get; set; }
        public string Carrier { get; set; }
        public string TransportType { get; set; }
        public string From { get; set; }
        public string To { get; set; }

    }
}
