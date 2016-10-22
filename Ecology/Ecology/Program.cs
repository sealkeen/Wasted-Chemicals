using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ecology
{
    //Читаем Comma Separated Data файл, выводим из него необходимые данные на экран консоли.
    class Program
    {
        const int range = 597; //Размер Массива с <data>ми
        static string inPath = @"data.txt";

        #region Auxiliary Fields
        static int endDouPos; static int ePosition;
        static int strCnt = 0;
        static int zapStrCtr = 0;
        static bool isCntr = false;
        static bool isEfound = false;
        static bool isAlreadyGotten = false;
        static string noxBuff = "";

        #endregion

        static void Main(string[] args)
        {
            if (!CheckFileExisting()) { Console.ReadKey();return; }

            List <data> example = new List<data>();
            InitializeListFields(ref example); //Инициализировать пустые значения example<data>
            InsertAllValues(ref example);      //Вставить значения в example<data>

            Console.WriteLine("\n- - - - - 1. a) - - - - -  \n"); //Первый вопрос
            var q11 = from dat in example
                      where dat.name != "\"Российская федерация\""
                      orderby dat.total descending
                      select dat;
            Console.WriteLine($"{q11.First().name} {q11.First().total}");

            Console.WriteLine("\n- - - - - 1. б) - - - - -  \n"); //Второй вопрос
            var q12 = from datExmpl in example
                      group datExmpl by datExmpl.name 
                      into list
                      select new { Field = list.Count(), Area = list.Key };
            var q12Ordered = from list in q12
                             orderby list.Field descending
                             select list;
            Console.WriteLine($"{q12Ordered.First().Field} областей в : {q12Ordered.First().Area}");

            Console.WriteLine("\n- - - - - 1. в) - - - - -  \n"); //Третий вопрос
            var q13 = from dat in example
                      where dat.year == 2014 && dat.name != "\"Российская федерация\""
                      orderby dat.total descending
                      select new { dat.name, dat.total };
            double onePercent = q13.First().total / 79;
            foreach (var datl in q13.Take(10)) { Console.WriteLine($"{datl.total}: {datl.name}"); WritePercentInChars(datl.total / onePercent); }

            Console.WriteLine("\n- - - - - 1. г) - - - - -  \n"); //Четвёртый вопрос

            var q4 = from dat in example
                     where dat.name != "\"Российская федерация\""
                     group dat by dat.source into GroupBySource
                            select new { Name = GroupBySource.Key, Wasted = GroupBySource.Sum(d => d.total)};
            foreach (var group in q4.Take(2)) { Console.WriteLine(group.ToString()); }


            Console.WriteLine("\n- - - - - 1. д) - - - - -  \n"); //Пятый вопрос
            var q5 = from dat in example
                     where dat.name != "\"Российская федерация\""
                            group dat by dat.year into GroupBySource
                            select new { Name = GroupBySource.Key, Wasted = Math.Round(GroupBySource.Sum(d => d.total), 5) };
            onePercent = q5.First().Wasted/79;
            foreach (var group in q5.Take(2)) { Console.WriteLine(group.ToString()); WritePercentInChars(group.Wasted / onePercent); }

            Console.ReadKey();
        }

        static void doubleDataToStruct(int valuePosition, ref List<data> structura)
        {

            FileStream fs = new FileStream(inPath, FileMode.Open);
            StreamReader inFile = new StreamReader(fs, Encoding.Default);

            //Check for error
            if (!File.Exists(inPath))
            {
                Console.WriteLine("error");
                return;
            }

            double[] nox = new double[10];
            for (int per = 0; per < 10; per++) { nox[per] = 0; }

            string item = inFile.ReadLine();
            strCnt++;

            int dozapyatBuff = 0;
            while (item!=null)
            {
                for (int c = 0; isAlreadyGotten == false && c < item.Length; c++)
                {
                    if (item[c] != ',')
                    {
                        dozapyatBuff++;
                    }
                    else
                    {
                        ++zapStrCtr;
                    }
                    try
                    {
                        if (isCntr == false && (zapStrCtr == valuePosition && strCnt > 1 && item[c + 1] != ','))
                        {
                            for (int l = 1; l < (48 - dozapyatBuff); l++)
                            {
                                if (isEfound) break;
                                if (item[c + l] == '.')
                                {
                                    isCntr = true;
                                    for (int cou = 1; (item[c + l + cou] <= 57 && item[c + l + cou] >= 48); cou++)
                                        endDouPos = c + l + (int)cou;
                                }
                                if (isCntr && (c + l > endDouPos))
                                    break;
                                if (item[c + l] != ',')
                                {
                                    noxBuff += item[c + l];
                                    isAlreadyGotten = true;
                                }
                                else
                                    break;
                                if (((c + l + 5) < item.Length) && (item[c + l + 1] == 69 || item[c + l + 1] == 101))
                                {
                                    isEfound = true;
                                    ePosition = c + l + 1;
                                }
                            }
                        }
                        else
                        { if (zapStrCtr == valuePosition && (c + 1) < item.Length && item[c + 1] == ',' && item[c] == ',') { noxBuff = "0"; break; } }
                    }
                    catch { if (zapStrCtr == valuePosition && (c + 1) < item.Length && item[c + 1] == ',' && item[c] == ',') { noxBuff = "0"; break; } }

                    dozapyatBuff = 0;
                    if (isEfound)
                    {
                        noxBuff += item[ePosition];
                        noxBuff += item[ePosition + 1];
                        noxBuff += item[ePosition + 4];
                        break;
                    }
                    if (zapStrCtr > valuePosition)
                        break;
                }
                if (strCnt > 1)
                {
                    //Console.WriteLine(noxBuff + Environment.NewLine);
                    switch (valuePosition)
                    {
                        case 2:
                            
                            structura[strCnt - 1].SO2 = convert(noxBuff);
                            break;
                        case 3:
                            structura[strCnt - 1].NOx = convert(noxBuff);
                            break;
                        case 4:
                            structura[strCnt - 1].losnm = convert(noxBuff);
                            break;
                        case 5:
                            structura[strCnt - 1].CO = convert(noxBuff);
                            break;
                        case 6:
                            structura[strCnt - 1].C = convert(noxBuff);
                            break;
                        case 7:
                            structura[strCnt - 1].NH3 = convert(noxBuff);
                            break;
                        case 8:
                            structura[strCnt - 1].CH4 = convert(noxBuff);
                            break;
                        case 9:
                            structura[strCnt - 1].total = convert(noxBuff);
                            break;
                    }
                }
                noxBuff = "";
                zapStrCtr = 0;
                isCntr = false;
                isEfound = false;
                isAlreadyGotten = false;
                item = inFile.ReadLine();
                strCnt++;
            }
            inFile.Close();
            strCnt = 0;
        }

        static void stringToStruct(int valuePosition, ref List<data> structura)
        {
            switch (valuePosition)
            {
                case 0:
                    for (int ye = 0; ye < range; ye++)
                    {
                        structura[ye].name = "";
                    }
                    break;
                case 1:
                    for (int ye = 0; ye < range; ye++)
                    {
                        structura[ye].area = "";
                    }
                    break;
                case 2:
                    for (int ye = 0; ye < range; ye++)
                    {
                        structura[ye].source = "";
                    }
                    break;
            }
            bool isTwoCommas = false;
            int strCnt = 0;    // каунтер строчек
            int zapStrCtr = 0; // каунтер запятых
            string item = "";
            string strBuffer = "";
            StreamReader inFile = new StreamReader(inPath, Encoding.Default);
            if (!File.Exists(inPath))
            {
                Console.WriteLine("error");
                return;
            }

            //inFile.clear();
            //inFile.seekg(0, ios::beg);

            while (!inFile.EndOfStream)
            {
                ++zapStrCtr;
                item = inFile.ReadLine();
                strCnt++;

                for (int c = 0; c < item.Length; c++)
                {
                    if (item[c] == ',')
                    {
                        ++zapStrCtr;
                        if (item[c + 1] == ',' && zapStrCtr > 1 && (zapStrCtr - 1) == valuePosition)
                        {
                            isTwoCommas = true;
                        }
                    }
                    if (isTwoCommas)
                    {
                        isTwoCommas = false;
                        strBuffer += "\" \"";
                        continue;
                    }
                    if (((zapStrCtr - 1) == valuePosition) && item[c] != ',' && strCnt > 1)
                    {
                        switch (valuePosition)
                        {
                            case 0:
                                structura[strCnt - 1].name += item[c];
                                strBuffer += item[c];
                                break;
                            case 1:
                                structura[strCnt - 1].area += item[c];
                                strBuffer += item[c];
                                break;
                            case 10:
                                structura[strCnt - 1].source += item[c];
                                strBuffer += item[c];
                                break;
                        }
                    }

                    if (zapStrCtr > (valuePosition + 1))
                        break;
                }

                switch (valuePosition)
                {
                    case 0:
                        structura[strCnt - 1].name = strBuffer;
                        break;
                    case 1:
                        structura[strCnt - 1].area = strBuffer;
                        break;
                    case 10:
                        structura[strCnt - 1].source = strBuffer;
                        break;
                }
                zapStrCtr = 0;
                strBuffer = "";
            }
            inFile.Close();
        }

        static void yearToStruct(int valuePosition, ref List<data> structura)
        {
            StreamReader inFile = new StreamReader(inPath, Encoding.Default);

            //Check for error
            if (!File.Exists(inPath))
            {
                Console.WriteLine("error");
                return;
            }

            string item = ""; string buffer = "";
            int strCnt = 0; 

            while (!inFile.EndOfStream)
            {
                item = inFile.ReadLine();
                strCnt++;
                try
                {
                    for (int c = (item.Length < 2 ? 0 : item.Length - 1); isAnumber(item[c]); c--)
                    {
                        if (strCnt > 1)
                            buffer += item[c];
                    }
                }
                catch { }
                if (buffer != "" && buffer.Length == 4)
                {
                    structura[strCnt - 1].year = 2010 + (buffer[0] - 48);
                }

                buffer = "";
            }
            inFile.Close();

        }

        static void totallyWasted(ref List<data> structura)
        {
            for (int i = 0; i < range; i++)
                structura[i].totallyWasted = (structura[i].SO2 + structura[i].NOx +
                structura[i].losnm + structura[i].CO + structura[i].C + structura[i].NH3
                + structura[i].CH4);
        }

        static void InitializeListFields(ref List<data> example)
        {

            for (int d = 0; d < range; d++) { example.Add(new data()); }

            for (int i = 0; i < range; i++)
            {
                example[i].name = "";
                example[i].area = "";
                example[i].SO2 = 0;
                example[i].NOx = 0;
                example[i].losnm = 0;
                example[i].CO = 0;
                example[i].C = 0;
                example[i].NH3 = 0;
                example[i].CH4 = 0;
                example[i].total = 0;
                example[i].source = "";
                example[i].year = 0;
                example[i].totallyWasted = 0;
            }
        }

        static void noxStringToDouble(ref List<data> exmp)
        {
            bool isMaxFound = false;
            double currMax = 0;
            double currVal = 0;
            double[] values = new double[range];
            List<double> maxValues = new List<double>(10);
            List<string> maxValStrings = new List<string>(10);

            for (int i = 0; i < range; i++)
                values[i] = -1;

            for (int c = 0; c < 10; c++)
                maxValues.Add(-1.0);


            for (int c = 0; c < 10; c++)
                maxValStrings.Add("");

            string line = "";


            StreamReader iFile = new StreamReader(inPath, Encoding.Default);

            //Check for error
            if (!File.Exists(inPath))
            {
                Console.WriteLine("error");
                return;
            }
            int doubleCounter = 0;

            while (!iFile.EndOfStream)
            {
                line = iFile.ReadLine();
                try { values[doubleCounter] = Program.convert(line); }
                catch { Console.Write("It is an error with the array man" + Environment.NewLine); Console.ReadKey(); ;
                }
                doubleCounter++;
                }

                for (int k = 0; k < 10; k++)
                {
                    for (int i = 0; i < range; i++)
                    {
                        switch (k)
                        {
                            case 0:
                                if (currMax < values[i])
                                {
                                    currMax = values[i];
                                    maxValStrings[0] = exmp[i + 1].name;
                                    isMaxFound = true;
                                }
                                break;
                            case 1:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[1] = exmp[i + 1].name;
                                }
                                break;
                            case 2:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[2] = exmp[i + 1].name;
                                }
                                break;
                            case 3:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[3] = exmp[i + 1].name;
                                }
                                break;
                            case 4:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[4] = exmp[i + 1].name;
                                }
                                break;
                            case 5:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[5] = exmp[i + 1].name;
                                }
                                break;
                            case 6:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[6] = exmp[i + 1].name;
                                }
                                break;
                            case 7:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[7] = exmp[i + 1].name;
                                }
                                break;
                            case 8:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[8] = exmp[i + 1].name;
                                }
                                break;
                            case 9:
                                if (currVal < values[i] && values[i] < maxValues[k - 1])
                                {
                                    currVal = values[i];
                                    maxValStrings[9] = exmp[i + 1].name;
                                }
                                break;
                        }
                    }
                    if (isMaxFound)
                    {
                        maxValues[0] = currMax;
                        isMaxFound = false;
                    }
                    else
                        maxValues[k] = currVal;
                    currVal = 0;
                }

                // === 10.04.2016 19:41 правка

                List<int> ratesOfTheMax = new List<int>(maxValues.Count);
                for (int c3 = 0; c3 < maxValues.Count; c3++)
                    { ratesOfTheMax.Add(0); }

                double percent = maxValues[0] / 80;
                ratesOfTheMax[0] = 80;

                for (int i = 1; i < maxValues.Count; i++)
                {
                    for (int see = 0; see < 80; see++)
                    {
                        if (maxValues[i] <= (percent * see))
                        {
                            ratesOfTheMax[i] = see;
                            break;
                        }
                    }
                }


            //for(int ki = 0; ki < 10; ki ++)
            //	Console.Write( ratesOfTheMax[ki] + Environment.NewLine);
            //
            //for(int ki = 0; ki < 10; ki ++)
            //	Console.Write( maxValues[ki] + Environment.NewLine);

            //for (int eye = 0; eye < ratesOfTheMax.Count; eye++)
            //    {
            //        Console.Write(maxValStrings[eye] + Environment.NewLine);
            //        for (int i = 0; i < (int)(ratesOfTheMax[eye] * 0.98); i++)
            //            Console.Write("=");
            //        Console.Write(Environment.NewLine);
            //    }
                iFile.Close();
            }

        static void theDeal(ref List<data> exmp)
        {
            //1st question --- start
            int maxTotal = 0;
            int maxTotalNumber = 0;
            string maxTotalName = "";
            for (int sea = 1; sea < 594; sea++)
            {
                if (maxTotal < exmp[sea].total &&
                    exmp[sea].year == 2014)
                {
                    maxTotal = (int)exmp[sea].total;
                    maxTotalNumber = sea;
                    maxTotalName = exmp[sea].name;
                }
            }

            Console.Write("a) Больше всего отходов " + maxTotal + " в " + maxTotalName + " under number " + maxTotalNumber + Environment.NewLine);

            //1st question --- end
            //2nd question --- start

            bool[] isAlreadyExist = new bool[range];
            List<string> areas = new List<string>();
            List<string> areaOrigin = new List<string>();

            for (int sea = 1; sea < 594; sea++)
            {
                recFunc(ref exmp, ref isAlreadyExist, exmp[sea].name);
            }

            for (int sea = 1; sea < 594; sea++)
            {
                if (isAlreadyExist[sea])
                    areas.Add(exmp[sea].name);
            }
            //for(int sea = 0; sea < areas.Count; sea++)
            //	Console.Write(areas[sea] << Environment.NewLine;

            List<int> eachAreas = new List<int>();
            foreach (string item in areas)
            {
                eachAreas.Add(1);
            }
            for (int sea = 0; sea < areas.Count; sea++)
            {
                for (int si = 1; si < exmp.Count(); si++)
                {
                    try
                    {
                        if (areas[sea] == exmp[si].name)
                            eachAreas[sea] += 1;
                    }
                    catch { continue; }
                }
            }

            int secondQuestionCounter = eachAreas[0];
            string secondQuestionArea = areas[0];

            for (int c = 0; c < eachAreas.Count; c++)
            {
                //Console.Write(areas[c] << " " << eachAreas[c] << Environment.NewLine;
                if (secondQuestionCounter < eachAreas[c])
                {
                    secondQuestionCounter = eachAreas[c];
                    secondQuestionArea = areas[c];
                }
            }

            //Отвечаем на второй вопрос
            Console.Write("\nб) В регионе " + secondQuestionArea + " оценка объема выбросов приведена для наибольшего числа населенных пунктов, " + secondQuestionCounter + Environment.NewLine);

            //2nd question --- end
            //3rd question --- start

            bool[] for2014 = new bool[range];
            bool[] inDeCrease = new bool[range];
            zeroIntoTheArray(ref for2014, 0, range);
            zeroIntoTheArray(ref inDeCrease, 0, range);

            for (int i = 1; i < 594; i++)
            {
                if (exmp[i].year == 2014)
                    for2014[i] = true;
            }
            for (int i = 1; i < 594; i++)
            {
                if (for2014[i] == true)
                {
                    for (int si = 0; si < range; si++)
                    {
                        if (exmp[si].name == exmp[i].name &&
                            exmp[si].area == exmp[si].area &&
                                exmp[si].year != exmp[si].year)
                        {
                            if (exmp[si].total < exmp[i].total)
                                inDeCrease[i] = true;
                        }
                    }
                }
            }

            //for(int i = 1; i < 594;i++)
            //{
            //	if(for2014[i]==true)
            //	{
            //		if(inDeCrease[i] == true)
            //			Console.Write(i << ") " << exmp[i].name << "[ ^ ]" << " ";
            //		else
            //			Console.Write(i << ") " << exmp[i].name << "[ v ]" << " ";
            //		if(i%2==0)
            //			Console.Write(Environment.NewLine;
            //	}
            //}

            //3rd question --- ending
            //4th question --- the beginning
            double total2013 = 0;
            double total2014 = 0;
            for (int see = 1; see < 594; see++)
            {
                if (exmp[see].year == 2014)
                    total2014 += exmp[see].total;
                else
                {
                    if (exmp[see].year == 2013)
                        total2013 += exmp[see].total;
                }
            }

            int equalsCouted = (int)(total2014 / 80);
            equalsCouted = (int)(total2013 / equalsCouted);

            Console.Write("\nд)\n Всего в 2014 " + total2014 + Environment.NewLine);
            double OnePercent = total2014 / 79;
            WritePercentInChars(79);

            Console.Write(" Всего в 2013 " + total2013 + Environment.NewLine);
            WritePercentInChars(total2013/OnePercent);

            Console.Write(Environment.NewLine);

            //4th question --- the end


            //5th question --- the beginning

            //5th question --- ending

            //6th question --- the beginning
            double totalRF = 0;

            for (int ki = 0; ki < 597; ki++)
            {
                if ((exmp[ki].name) == ("\"Российская федерация\""))
                {
                    //Console.Write("total "<< exmp[ki].total << Environment.NewLine;
                    //Console.Write("totallyWasted" << exmp[ki].totallyWasted << Environment.NewLine;
                    totalRF += exmp[ki].total;
                }
            }
            Console.Write("\nг) Доля железнодорожного / автомобильного транспорта в общем объеме РФ = " + totalRF + Environment.NewLine);

            //6th question --- ending

        }

        static void recFunc(ref List<data> ex, ref bool[] isExist, string disintegrated)
        {
            int foundNumber = 0;
            for (int i = 1; i < 594; i++)
            {
                if (ex[i].name == disintegrated)
                {
                    foundNumber = i;
                    break;
                }
            }

            for (int i = 1; i < 594; i++)
            {
                if (ex[i].name == ex[foundNumber].name)
                    isExist[i] = false;
            }
            isExist[foundNumber] = true;
        }

        static void zeroIntoTheArray(ref bool[] array, int start, int end)
        {
            for (int c = start; c < end; c++)
            {
                array[start] = false;
            }
        }

        static bool isAnumber(char ch)
        {
                if (ch <= 57 && ch >= 48)
                    return true;
                return false;
            }

        static double convert(string origin)
        {
            if (origin == "0") { return 0.0; }
            string final = "";
            bool dotExists = false;
            bool Eexists = false;
            bool isEfound = false;
            int dotPosition = 0;
            int k = 0;
            double dVal = 0;
            for (int i = 0; i < origin.Length; i++)
            {
                if (origin[(int)i] != '.')
                {
                    final += origin[(int)i];
                }
                else
                    dotPosition = ((int)i + 1);
            }

            for (int si = 0; si < origin.Length; si++)
            {
                if (origin[si] == '.')
                    dotExists = true;
                if (isE(origin[si]))
                    Eexists = true;
            }
            if (dotExists == false && Eexists == false)
            {
                for (int ass = 0; ass < final.Length; ass++)
                {
                    dVal += CharToDouble(final[ass]) * Math.Pow(0.1, ass);
                }
                dVal *= Math.Pow(10.0, (int)final.Length - 1);
                return dVal;
            }

            while (k < final.Length)
            {
                if (isEfound) break;
                if (k > final.Length /*|| k < final.Length*/) break;
                if (isE(final[k]))
                {
                    isEfound = true;
                    double behindEnumbers = 0;
                    string behindString = "";
                    int countor = 0;
                    bool isDotExists = false;

                    while (!isE(final[countor]))
                    {
                        if (origin[countor] == '.' || origin[countor + 1] == '.')
                        {
                            isDotExists = true;
                            break;
                        }
                        behindEnumbers += CharToDouble(final[countor]) * Math.Pow(0.1, countor);
                        countor++;
                    }

                    if (isDotExists)
                    {
                        for (int huy = 0; !isE(origin[huy]); huy++)
                        {
                            behindString += origin[huy];
                        }
                        behindEnumbers = convert(behindString);
                    }
                    else
                        behindEnumbers *= Math.Pow(10.0, countor - 1);

                    double postEnumbers = 0;
                    double postEnumberPosition = 0.0;
                    int postEnumbersCount = 0;
                    double postEfirstNumber = 0;
                    for (int post = final.Length; post > 0; post--)
                    {
                        if (isE(final[post - 1]))
                        {
                            postEfirstNumber = post + 1;
                            postEnumbersCount = (int)(final.Length - postEfirstNumber);
                        }
                    }
                    for (int cnt = 2; cnt < (2 + postEnumbersCount); cnt++)
                    {
                        try
                        {
                            if ((k + cnt) <= final.Length && final[k + cnt] >= 48 && final[k + cnt] <= 57)
                            {
                                double buffa = CharToDouble(final[k + cnt]);
                                postEnumbers += (buffa * Math.Pow(0.1, postEnumberPosition));
                                postEnumberPosition++;
                            }
                            else
                                break;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    postEnumbers *= Math.Pow(10.0, postEnumberPosition - 1);
                    //Console.WriteLine("behindEnumbers"+ behindEnumbers);
                    //Console.WriteLine("postEnumbers = "+ postEnumbers);
                    double resultingWithDotE = 0;

                    if (final[k + 1] == '+')
                        resultingWithDotE = behindEnumbers * Math.Pow(10.0, postEnumbers);
                    else
                        resultingWithDotE = behindEnumbers * Math.Pow(0.1, postEnumbers);


                    return resultingWithDotE;
                }
                else
                    if (!isEfound && dotPosition != 0)
                    dVal += ((double)((int)final[k] - 48) * Math.Pow(0.1, (int)k));
                k++;
            }
            dVal *= (Math.Pow(10.0, (int)k - 1));
            dVal /= (Math.Pow(10.0, (int)origin.Length - dotPosition));
            return dVal;
        }

        static bool isE(char ch)
        {
            if (ch == 'e' || ch == 'E')
                return true;
            return false;
        }

        static double CharToDouble(char ch)
        {
            int i = ((int)ch - 48);
            return (double)i;
        }

        static void showTheArray(ref List<data> example)
        {
            for (int l = 1; l < 110; l+=10)
            {
                Console.Write("(" + l + ") " + example[l].name + "  ");
                Console.Write("(" + l + ") " + example[l].area + "  ");
                Console.Write("(" + l + ") " + example[l].SO2 + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].NOx + "  ");
                Console.Write("(" + l + ") " + example[l].losnm + "  ");
                Console.Write("(" + l + ") " + example[l].CO + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].C + "  ");
                Console.Write("(" + l + ") " + example[l].NH3 + "  ");
                Console.Write("(" + l + ") " + example[l].CH4 + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].total + "  ");
                Console.Write("(" + l + ") " + example[l].source + "  ");
                Console.Write("(" + l + ") " + example[l].year + "  ");
                Console.WriteLine();
                Console.Write("(" + l + ") " + example[l].totallyWasted + "  ");
                Console.WriteLine();
            }
        }

        static void InsertAllValues(ref List<data> example)
        {

                for (int ot2do9 = 2; ot2do9 < 10; ot2do9++)
                    doubleDataToStruct(ot2do9, ref example);
                stringToStruct(0, ref example);
                stringToStruct(1, ref example);
                stringToStruct(10, ref example);
                yearToStruct(11, ref example);

                //Console.WriteLine("\nв) Диаграмма: \n\n");
                totallyWasted(ref example);
                //noxStringToDouble(ref example);
                //showTheArray(ref example);
                //theDeal(ref example);
                example.RemoveAt(0);

        }

        static void showAreas(ref List<data> example)
        {
            for (int i = 0; i < example.Count; i++)
            {
                Console.Write(i + "." + example[i].area + "\t\t");
                if (i % 4 == 0)
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
        static void writeTxtFile(List<data> example, string fullOutFileName)
        {
            StreamWriter sw = new StreamWriter(fullOutFileName, false, Encoding.Unicode);

            foreach (data ln in example)
            {
                sw.WriteLine($"{ln.name}; {ln.area}; {ln.SO2}; {ln.NOx}; {ln.losnm}; "+
                            $"{ln.CO}; {ln.C}; {ln.NH3}; {ln.CH4}; {ln.total}; {ln.source}; {ln.year}");
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
    }
}

