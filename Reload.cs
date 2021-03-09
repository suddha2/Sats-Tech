using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Variedades
{
    public class Reload
    {
        public int ID { get; set; }
        public int customer { get; set; }
        public DateTime txDate { get; set; }
        public string expDate { get; set; }
        public string provider { get; set; }
        public decimal total { get; set; }
        public string packDesc { get; set; }
        public decimal packAmount { get; set; }
        public string addOnDesc { get; set; }
        public decimal addOnAmount { get; set; }
        public string extrachargeDesc { get; set; }
        public decimal extrachargeAmount { get; set; }
    }
}
