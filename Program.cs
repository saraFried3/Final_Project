using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Diagnostics.Eventing.Reader;
using sarasProject;
//using Porter2Stemmer;
//using Porter2StemmerStandard;
using System.Diagnostics;
using System.Threading;
using System.Text;


//  "C:\Users\WIN 11\Desktop\clientConnection\bin\Debug\net8.0\sarasProject.exe"




namespace Automat
{
    public class Program
    {
        public static int n = 0;          //מספר המחלות
        public static int m = 0;          //מספר התסמינים
        public static int num_of_words = 0;
        public static void Main(string[] args)
        {
            string str = "";
            string inputFromApi = "";
            if (args != null && args.Length > 0)
            {
                inputFromApi = args[0];
                //Console.WriteLine($"sarasProject עיבד: {inputFromApi}");
                // כאן תהיה הלוגיקה העיקרית של sarasProject שכותבת לקונסול
            }
            else
            {
                Console.WriteLine("לא התקבל קלט מ-API.");
            }
            Dictionary<int, string> DiseasesNames = buildNamesDictionary(@"..\..\DiseasesNames.txt");
            n = DiseasesNames.Count();
            Dictionary<int, string> symptomsNames = buildNamesDictionary(@"..\..\symptomsNames.txt");
            m = symptomsNames.Count();
            let[] Auto = DFA.buildAuto();                                                                 //בנית אוטומט
            //Console.WriteLine("lets begin");
            //while (true)
            //{
                List<Symptom> symptoms = read_the_input.readTheInput(Auto, inputFromApi);                                           //שלב האימות וקביעה עבור כל תסמין רלונטי את הערך שהוא מיצג
                check_symptoms.checkSymptoms(symptoms);                                                               //קריאת הקלט, מעבר באוטומט וקבלת נתוני תסמין    //       
                List<int> illnessDecision = IllnessDecision.illnessDecision(symptoms);                                 //הכרעת המחלה
                foreach (int illness in illnessDecision)                                               //הדפסת המחלות הכבדות ביותר
                {
                    str += DiseasesNames[illness] = " , ";
                    //Console.WriteLine("The disease identified is: " + illness + ": " + DiseasesNames[illness]);
                }
                //Console.WriteLine($"The disease identified is: {illnessDecision.ToString()}.");
                //Console.WriteLine("enter another symptom");
            //}
            Console.WriteLine(str);
        }
        public static string runSarasProject(string inputStr)
        {
            string str = "";
            Dictionary<int, string> DiseasesNames = buildNamesDictionary(@"..\..\DiseasesNames.txt");
            n = DiseasesNames.Count();
            Dictionary<int, string> symptomsNames = buildNamesDictionary(@"..\..\symptomsNames.txt");
            m = symptomsNames.Count();
            let[] Auto = DFA.buildAuto();                                                                 //בנית אוטומט
            Console.WriteLine("lets begin");
            while (true)
            {
                List<Symptom> symptoms = read_the_input.readTheInput(Auto, inputStr);                                           //שלב האימות וקביעה עבור כל תסמין רלונטי את הערך שהוא מיצג
                check_symptoms.checkSymptoms(symptoms);                                                               //קריאת הקלט, מעבר באוטומט וקבלת נתוני תסמין    //       
                List<int> illnessDecision = IllnessDecision.illnessDecision(symptoms);                                 //הכרעת המחלה
                str = "";
                foreach (int illness in illnessDecision)                                               //הדפסת המחלות הכבדות ביותר
                {
                    str += DiseasesNames[illness] = " , ";
                    Console.WriteLine("The disease identified is: " + illness + ": " + DiseasesNames[illness]);
                }
                Console.WriteLine($"The disease identified is: {illnessDecision.ToString()}.");
                Console.WriteLine("enter another symptom");
            }
            return str;
        }
        public class let
        {
            public let[] next { get; set; }
            public let()
            {
                this.next = new let[26];
            }
        }
        public class endLet : let
        {
            public int num { get; set; }
            public int collection { get; set; }
            public endLet(int num, int collection)
            {
                this.num = num;
                this.collection = collection;
            }
            public endLet()
            {
                this.num = -1;
                this.collection = 0;
            }
        }
        public class Symptom
        {
            public int num { get; set; }
            public string str { get; set; }                                                                     //מחרוזת תיוג
            public int not { get; set; }                                                                        //מונה למילות שלילה
            public int time { get; set; }                                                                        //מונה למילות שלילה
            public BigInteger sum { get; set; }                                                                 //סכום היצוגים של כל המילים
            public List<endLet> listOfWords { get; set; }                                                       //לבדוק האם צריך - שמירת המילים 
            public Symptom()
            {
                this.time = 0;
                this.num = -1;
                this.str = string.Empty;
                this.not = 0;
                this.sum = 0;
            }
            public string ToString()
            {
                return " num: " + this.num + " not: " + this.not + " sum: " + this.sum + " .";
            }
            public void addWord(endLet word)
            {
                this.str += word.collection;
                switch (word.collection)
                {
                    case 1:
                        this.sum += (BigInteger)Math.Pow(2, word.num);
                        break;
                    case 2:
                        this.not++;
                        break;
                    case 4:
                        this.time = word.num;
                        break;
                }
                Console.WriteLine("the word added succesfully!  ");
                Console.WriteLine(word.num);
            }
        }
        public static Dictionary<int, string> buildNamesDictionary(string filePath)
        {
            Dictionary<int, string> names = new Dictionary<int, string>();
            string[] arr = File.ReadAllLines(filePath);
            for (int i = 0; i < arr.Length; i++)
            {
                names.Add(i, arr[i]);
            }
            return names;
        }                                                                                        //בנית מילון ובו יצוגי הסכימות של המילים משמשים כמפתחות ויצוגי התסמינים משמשים כערכים
    }
}