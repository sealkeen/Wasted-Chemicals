using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecology
{
    class Data
    {
        public Data()
        {
            Name = "\"\"";
            Area = "";
            SO2 = 0;
            NOx = 0;
            Losnm = 0;
            CO = 0;
            C = 0;
            NH3 = 0;
            CH4 = 0;
            Total = 0;
            Source = "";
            Year = 0;
            TotallyWasted = 0;
        }
        public string Name { get; set; }
        public string Area { get; set; }
        public double SO2 { get; set; }
        public double NOx { get; set; }
        public double Losnm { get; set; }
        public double CO { get; set; }
        public double C { get; set; }
        public double NH3 { get; set; }
        public double CH4 { get; set; }
        public double Total { get; set; }
        public string Source { get; set; }
        public int Year { get; set; }
        public double TotallyWasted { get; set; }


        public void SetField(string value, int index)
        {
            switch (index)
            {
                case 0:
                    Name = value;
                    break;
                case 1:
                    Area = value;
                    break;
                case 2:
                    SO2 = Program.convert(value);
                    break;
                case 3:
                    NOx = Program.convert(value);
                    break;
                case 4:
                    Losnm = Program.convert(value);
                    break;
                case 5:
                    CO = Program.convert(value);
                    break;
                case 6:
                    C = Program.convert(value);
                    break;
                case 7:
                    NH3 = Program.convert(value);
                    break;
                case 8:
                    CH4 = Program.convert(value);
                    break;
                case 9:
                    Total = Program.convert(value);
                    break;
                case 10:
                    Source = value;
                    break;
                case 11:
                    Year = (int)Program.convert(value);
                    break;
                case 12:
                    TotallyWasted = Program.convert(value);
                    break;
            }
        }


        public override string ToString()
        {
            return Name + "\t" +SO2 + "\t" + NOx+ "\t" + Losnm + "\t" + CO;
        }
    }

}
