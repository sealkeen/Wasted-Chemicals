using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology
{
    class data
    {

        public string name { get; set; }
        public string area { get; set; }
        public double SO2 { get; set; }
        public double NOx { get; set; }
        public double losnm { get; set; }
        public double CO { get; set; }
        public double C { get; set; }
        public double NH3 { get; set; }
        public double CH4 { get; set; }
        public double total { get; set; }
        public string source { get; set; }
        public int year { get; set; }
        public double totallyWasted { get; set; }

        public override string ToString()
        {
            return name + "\t" +SO2 + "\t" + NOx+ "\t" + losnm + "\t" + CO;
        }
    }

}
