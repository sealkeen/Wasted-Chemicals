using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace Ecology
{
    
    //Читаем Comma Separated Data файл, выводим из него необходимые данные на экран консоли.
    partial class Program
    {
        const int range = 597; //Размер Массива с <data>ми
        static string inPath = @"data.txt";

        public static FileStream fS = new FileStream(inPath, FileMode.Open);
        public static StreamReader lineReader = new StreamReader(fS, Encoding.Default);

        static void Main(string[] args)
        {
            if (!CheckFileExisting()) { Console.ReadKey(); return; }
            Console.ReadKey();
            
            List<Data> example = new List<Data>();
            ReadLines(ref example);
            totallyWasted(ref example);

            Console.WriteLine("\n- - - - - 1. a) - - - - -  \n"); //Первый вопрос
            var q11 = from dat in example
                      where dat.Name != "\"Российская федерация\""
                      orderby dat.Total descending
                      select dat;
            Console.WriteLine($"{q11.First().Name} {q11.First().Total}");

            Console.WriteLine("\n- - - - - 1. б) - - - - -  \n"); //Второй вопрос
            var q12 = from datExmpl in example
                      group datExmpl by datExmpl.Name 
                      into list
                      select new { Field = list.Count(), Area = list.Key };
            var q12Ordered = from list in q12
                             orderby list.Field descending
                             select list;
            Console.WriteLine($"{q12Ordered.First().Field} областей в : {q12Ordered.First().Area}");

            Console.WriteLine("\n- - - - - 1. в) - - - - -  \n"); //Третий вопрос
            var q13 = from dat in example
                      where dat.Year == 2014 && dat.Name != "\"Российская федерация\""
                      orderby dat.Total descending
                      select new { dat.Name, dat.Total };
            double onePercent = q13.First().Total / 79;
            foreach (var datl in q13.Take(10)) { Console.WriteLine($"{datl.Total}: {datl.Name}"); WritePercentInChars(datl.Total / onePercent); }

            Console.WriteLine("\n- - - - - 1. г) - - - - -  \n"); //Четвёртый вопрос

            var q4 = from dat in example
                     where dat.Name != "\"Российская федерация\""
                     group dat by dat.Source into GroupBySource
                            select new { Name = GroupBySource.Key, Wasted = GroupBySource.Sum(d => d.Total)};
            foreach (var group in q4.Take(2)) { Console.WriteLine(group.ToString()); }


            Console.WriteLine("\n- - - - - 1. д) - - - - -  \n"); //Пятый вопрос
            var q5 = from dat in example
                     where dat.Name != "\"Российская федерация\""
                            group dat by dat.Year into GroupBySource
                            select new { Name = GroupBySource.Key, Wasted = Math.Round(GroupBySource.Sum(d => d.Total), 5) };
           
            onePercent = q5.First().Wasted/79;
            foreach (var group in q5.Take(2)) { Console.WriteLine(group.ToString()); WritePercentInChars(group.Wasted / onePercent); }

            Console.ReadKey();
        }

        private static void ReadLines(ref List<Data> example)
        {
            lineReader.ReadLine();
            while (true)
            {
                Data dee = new Data();
                if (!ReadLine(ref dee))
                    break;
                example.Add(dee);
            }
        }
         
        private static bool ReadLine(ref Data dataSet)
        {
            string line;
            line = lineReader.ReadLine();

            if (line == null)
                return false;

            string[] values = line.Split(',');

            for (int i = 0; i < values.Length; ++i)
            {
                dataSet.SetField(values[i], i);
            }

            return true;
        }

        static public double convert(string origin)
        {
            string newLine = origin;
            if (origin.Contains('.'))
            {
                newLine = origin.Replace('.', ',');
            }
            try
            {
                return Convert.ToDouble(newLine);
            }
            catch { return 0; }
       } 

        static void PrintLines(ref List<Data> example)
        {
            for (int l = 1; l < example.Count; l+=10)
            {
                Console.Write("(" + l + ") " + example[l].Name + "  ");
                Console.Write("(" + l + ") " + example[l].Area + "  ");
                Console.Write("(" + l + ") " + example[l].SO2 + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].NOx + "  ");
                Console.Write("(" + l + ") " + example[l].Losnm + "  ");
                Console.Write("(" + l + ") " + example[l].CO + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].C + "  ");
                Console.Write("(" + l + ") " + example[l].NH3 + "  ");
                Console.Write("(" + l + ") " + example[l].CH4 + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].Total + "  ");
                Console.Write("(" + l + ") " + example[l].Source + "  ");
                Console.Write("(" + l + ") " + example[l].Year + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].TotallyWasted + "  ");
                Console.WriteLine();
            }
        }

        static string replaceCharacters(string line, char oldChar, char newChar)
        {
            string newLine = "";
            foreach (char ch in line)
            {
                if (ch == oldChar)
                    newLine += newChar;
                else
                    newLine += ch;
            }
            return newLine;
        }

        static void txtIntoExcel(string inputPath, string outputPath)
        {
            List<string> dat = new List<string>();
            StreamReader sr = new StreamReader(inputPath, Encoding.Default);
            string line = sr.ReadLine();
            while (line != null)
            {
                dat.Add(replaceCharacters(line, ',', ';'));
                line = sr.ReadLine();
            }

            for (int k = 0; k < dat.Count; k++)
            {
                dat[k] = replaceCharacters(dat[k], '.', ',');
            }
            StreamWriter sw = new StreamWriter(outputPath, false, Encoding.Unicode);

            foreach (string ln in dat)
            {
                sw.WriteLine(ln);
            }
            sr.Close(); sw.Close();
            Console.ReadKey();
        }

        static void writeTxtFile(List<Data> example, string fullOutFileName)
        {
            StreamWriter sw = new StreamWriter(fullOutFileName, false, Encoding.Unicode);

            foreach (Data ln in example)
            {
                sw.WriteLine($"{ln.Name}; {ln.Area}; {ln.SO2}; {ln.NOx}; {ln.Losnm}; "+
                            $"{ln.CO}; {ln.C}; {ln.NH3}; {ln.CH4}; {ln.Total}; {ln.Source}; {ln.Year}");
            }
            sw.Close();
            Console.ReadKey();
        }

        static void WritePercentInChars(double percent)
        {
            for (double i = 1; i < percent; i++)
            {
                Console.Write("=");
            }
            Console.Write("\n");
        }

        static bool CheckFileExisting()
        {
            if (!File.Exists(inPath))
            {
                Console.WriteLine($"File data.txt not found in the directory: \"{Directory.GetCurrentDirectory()}\"");
                return false;
            }
            return true;
        }

        static void RenewTextReader()
        {
            Console.WriteLine(lineReader.ReadLine()+Environment.NewLine);
            lineReader = new StreamReader(fS, Encoding.Default);
            Console.WriteLine(lineReader.ReadLine());
            
        }

        ~Program()
        {
            lineReader.Close();
        }


    }
}

