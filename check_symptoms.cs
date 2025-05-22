using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Automat;
using System.IO;
using static Automat.Program;

namespace sarasProject
{
    public class check_symptoms
    {
        public static string RunSarasProject(string inputStr)
        {
            string str = "";
            Dictionary<int, string> DiseasesNames = buildNamesDictionary(@"..\..\DiseasesNames.txt");
            n = DiseasesNames.Count();
            Dictionary<int, string> symptomsNames = buildNamesDictionary(@"..\..\symptomsNames.txt");
            m = symptomsNames.Count();
            let[] Auto = DFA.buildAuto();                                                                 //בנית אוטומט
            Console.WriteLine("lets begin");
                List<Symptom> symptoms = read_the_input.readTheInput(Auto, inputStr);                                           //שלב האימות וקביעה עבור כל תסמין רלונטי את הערך שהוא מיצג
                check_symptoms.checkSymptoms(symptoms);                                                               //קריאת הקלט, מעבר באוטומט וקבלת נתוני תסמין    //       
                List<int> illnessDecision = IllnessDecision.illnessDecision(symptoms);                                 //הכרעת המחלה
                str = "";
                foreach (int illness in illnessDecision)                                               //הדפסת המחלות הכבדות ביותר
                {
                    str += DiseasesNames[illness] = " , ";
                    Console.WriteLine("The disease identified is: " + illness + ": " + DiseasesNames[illness]);
                }
                // Console.WriteLine($"The disease identified is: {illnessDecision.ToString()}.");
                //Console.WriteLine("enter another symptom");
            return str;
        }
        public static void checkSymptoms(List<Program.Symptom> symptoms)                                   //פונקציה שמבצעת את שלב האימות ומאתחלת את השדה num
        {
            Dictionary<BigInteger, int> dic = buildDictionary();
            foreach (Program.Symptom s in symptoms)                                                        //עוברים על כל תסמין
            {
                int num1 = -1;
                BigInteger value = s.sum;
                Console.WriteLine("value of sum:  " + value);
                num1 = CheckInDict(dic, s.sum);
                if (s.not % 2 == 1 || s.time != 0)
                {
                    num1 = -1;                                                                     //התסמין לא רלונטי
                }
                s.num = num1;
            }
        }

        public static Dictionary<BigInteger, int> buildDictionary()                              //בנית מילון ובו יצוגי הסכימות של המילים משמשים כמפתחות ויצוגי התסמינים משמשים כערכים
        {
            Dictionary<BigInteger, int> dic = new Dictionary<BigInteger, int>();
            string filePath = @"..\..\symptoms.txt";
            string[] arr = File.ReadAllLines(filePath);
            foreach (string symptom2 in arr)                                                      //מעבר על המערך והמרתו למילון 
            {
                string[] arr2 = symptom2.Split(',');
                int value = int.Parse(arr2[0]);
                for (int i = 1; i < arr2.Length; i++)
                    if (!dic.ContainsKey(BigInteger.Parse(arr2[i])))
                    {
                        dic[BigInteger.Parse(arr2[i])] = value;
                    }
            }
            return dic;
        }
        //BigInteger originalValue = BigInteger.Parse(binaryStr, System.Globalization.NumberStyles.AllowHexSpecifier);
        public static int CheckInDict(Dictionary<BigInteger, int> dict, BigInteger originalValue)
        {
            if (dict.ContainsKey(originalValue))                                                                // ננסה לבדוק את הערך המקורי
                return dict[originalValue];
            for (int i = 0; i < Program.num_of_words; i++)                                                                         // ננסה לכבות כל סיבית בתורה
            {
                BigInteger mask = ~(BigInteger.One << i);                                                              // מסיכה שמכבה את הסיבית במקום i
                byte[] str= mask.ToByteArray();
                BigInteger modifiedValue = originalValue & mask;
                if (dict.ContainsKey(modifiedValue))
                    return dict[modifiedValue];                                                              //החזר את התסמין השמור במילון עבור הערך שהתקבל לאחר התעלמות ממילה, כלומר כיבוי סיבית                }
            }
            return -1;                                                                                            //לא נמצא ערך במילון גם לאחר כיבוי סיביות
        }
       


        public static int CalculateHammingDistance(string str1, string str2)
        {
            if (str1.Length != str2.Length)
            {
                throw new ArgumentException("Strings must be of the same length.");
            }

            int distance = 0;

            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[i])
                {
                    distance++;
                }
            }

            return distance;
        }


        public static int check_in_dict(Dictionary<BigInteger, int> dic, BigInteger user_description)
        {
            string str = Convert.ToString((long)user_description, 2);
            BigInteger current_user_description;
            string stringMask = new string('1',Program.num_of_words - 1) + "0";
            BigInteger mask = BigInteger.Parse(stringMask);
            for (int i = 0; i <Program.num_of_words; i++)
            {
                //Console.WriteLine(stringMask);
                current_user_description = user_description & mask;
                string str3 = current_user_description.ToString();
                //Console.WriteLine(Convert.ToString((long)current_user_discription, 2));
                if (dic.ContainsKey(current_user_description))
                {
                    Console.WriteLine("we find this value in the dictionary:     " + dic[current_user_description]);
                    return dic[current_user_description];
                }
                mask = leftRotate(mask);
                string strMask = mask.ToString();
            }
            Console.WriteLine("there is not sum in the dictionary");
            return -1;
        }
        public static BigInteger leftRotate(BigInteger vector)       //פונקציה שמסובבת את המסיכה על מנת לבדוק את הערך שנשאר לאחר הורדת המילה הבאה
        {
            BigInteger leftBitHigh = BigInteger.One << (Program.num_of_words );                  ///וקטור של אפסים כאשר רק הסיבית במיקום ה על מנת לבודד את הסיבית השמאלית
            BigInteger leftBit = vector & leftBitHigh;
            if (leftBit > 0) leftBit = 1;
            vector = vector << 1;
            BigInteger number = (BigInteger.One << Program.num_of_words) - 1;
            vector = vector & number;
            vector = vector | leftBit;
            return vector;
        }
    }
}
