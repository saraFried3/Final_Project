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

namespace Automat
{
    internal class Program
    {
        public static int n=0;
        public static int m=0;
        public static int num_of_words=0;
        static void Main(string[] args)
        {
            
            Dictionary<int, string> DiseasesNames = buildNamesDictionary(@"..\..\DiseasesNames.txt");
            n = DiseasesNames.Count();
            Dictionary<int, string> symptomsNames = buildNamesDictionary(@"..\..\symptomsNames.txt");
            m = symptomsNames.Count();
            let[] Auto = buildAuto();                                                                 //בנית אוטומט
            Console.WriteLine("lets begin");
            while (true)
            {
                List<Symptom> symptoms = readTheInput(Auto);                                           //שלב האימות וקביעה עבור כל תסמין רלונטי את הערך שהוא מיצג
                checkSymptoms(symptoms);                                                               //קריאת הקלט, מעבר באוטומט וקבלת נתוני תסמין    //       
                List<int> illnessDecision = IllnessDecision(symptoms);                                 //הכרעת המחלה
                foreach (int illness in illnessDecision)                                               //הדפסת המחלות הכבדות ביותר
                {
                    Console.WriteLine("The disease identified is: "+illness+": " + DiseasesNames[illness]);
                }   
                Console.WriteLine($"The disease identified is: {illnessDecision.ToString()}.");
                Console.WriteLine("enter another symptom");
            }
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
        }
        public static let[] buildAuto()
        {
            string filePath = @"..\..\words.txt";
            string[] words = File.ReadAllLines(filePath);
            num_of_words = words.Length/3;
            let[] Auto = new let[26];
            let[] l;
            for (int i = 0; i + 2 < words.Length; i += 3)                                                                     // מעבר על כל מילה
            {
                let[] ptr = Auto;
                for (int j = 0; j < words[i].Length - 1; j++)                                                                 // מעבר על כל אות
                {
                    char key = words[i][j];
                    int index = key - 'a';                                                                                    // המרת תו לאינדקס
                    if (index >= 0 && index < 26)                                                                             // בדוק אם האינדקס בטווח
                    {
                        if (ptr[index] == null)
                        {
                            ptr[index] = new let();
                        }
                        ptr = ptr[index].next;                                                                                // התקדמות למצב הבא
                    }
                }
                int endIndex = (int)words[i][words[i].Length - 1] - (int)'a';
                if (endIndex >= 0 && endIndex < 26)
                    if (ptr[endIndex] == null || !(ptr[endIndex] is endLet))
                    {
                        if (ptr[endIndex] != null)
                        {
                            l = ptr[endIndex].next;
                            ptr[endIndex] = new endLet(int.Parse(words[i + 1]), int.Parse(words[i + 2]));
                            ptr[endIndex].next = l;
                        }
                        else
                        {
                            ptr[endIndex] = new endLet(int.Parse(words[i + 1]), int.Parse(words[i + 2]));
                        }
                    }
            }
            return Auto;
        }
        public static List<Symptom> readTheInput(let[] Auto)
        {
            bool isExpression2;
            Symptom symptom;
            string pattern = @"[^a-zA-Z\s]";
            endLet word = new endLet();
            List<Symptom> symptoms = new List<Symptom>();
            let[] ptr;
            int countWord;
            bool isValidWord;
            Console.WriteLine("Tell me how you feel, what symptoms you have.");
            string str1 = Console.ReadLine();                                                                           //קבלת קלט בכתב או מהקלטה
            str1 = str1.Trim();
            str1 = str1.ToLower();
            string[] arr = str1.Split('.', '!', '?');
            for (int i = 0; i < arr.Length; i++)
                arr[i] = Regex.Replace(arr[i], pattern, "");
            for (int i = 0; i < arr.Length; i++)
                arr[i] += ' ';
            foreach (string str2 in arr)                                                                                //מעבר על כל תסמין-כל משפט מהקלט
            {
                isValidWord = true;
                isExpression2 = false;
                int x = -1;
                countWord = 0;
                symptom = new Symptom();                                                                                //יצירת עצם חדש עבור התסמין הנוכחי
                ptr = Auto;
                for (int j = 0; j < str2.Length - 1; j++)                                                               //מעבר על כל אות בקלט
                {

                    if (str2[j] - 'a' < 26 && str2[j] - 'a' >= 0 && str2[j + 1] != ' ')                                 //אם התו הוא אכן אות אנגלית קטנה
                    {                                                                                                   //&& !(ptr[(int)str2[j] - (int)'a'] is endLet)) //אם האות מופיעה באוטומט וגם היא לא האות האחרונה במילה
                        if (ptr[(int)str2[j] - (int)'a'] != null)                                                       //אם המקום הזה לא ריק
                        {
                            ptr = ptr[(int)str2[j] - (int)'a'].next;                                                    //התקדם למצב של האות הבאה
                        }
                        else
                        {
                            ptr = Auto;
                            isValidWord = false;
                        }
                    }
                    if (str2[j + 1] == ' ' && (str2[j] - 'a' >= 0 && str2[j] - 'a' < ptr.Length))                       //אם הגעתי לאות האחרונה במילה- סוף מילה
                    {
                        if (j + 2 < str2.Length)
                            if (isExpression(ptr, str2, j, ref word) != -1)
                                isExpression2 = true;
                        if (!isExpression2)
                            if (ptr[str2[j] - 'a'] is endLet)
                            //if (j + 2 < str2.Length)
                            {
                                word = (endLet)ptr[str2[j] - 'a'];
                                ptr = Auto;
                            }
                        if (!isExpression2)
                        {
                            countWord++;
                            Console.WriteLine($"word.num: {word.num}");
                            if (word.collection == 1)
                                symptom.sum += (BigInteger)Math.Pow(2, word.num);
                            if (word.collection == 2)                                            //אם המילה היא מילת שלילה
                                symptom.not++;                                                                         //תעלה את מונה מילות השלילה                                                                                  
                            if (word.collection == 4)
                                symptom.time = word.num;
                            symptom.str = symptom.str + word.collection.ToString();                                    //שרשור יצוג האוסף למחרוזת היצוגים      
                            word = new endLet();
                            ptr = Auto;
                        }
                        else                                                                                           //במקרה והגענו לסוף המילה והיא לא מופיעה באוטומט נבדוק האם היא מוכלת בביטוי מורכב          
                        {
                            isExpression2 = false;
                            if (ptr[str2[j] - 'a'] != null)
                                ptr = ptr[str2[j] - 'a'].next;
                        }

                    }
                }
                symptoms.Add(symptom);
                Console.WriteLine($"Symptom_str: {symptom.str}, Not: {symptom.not}, Sum: {symptom.sum}, countWord:{countWord}");
                Console.WriteLine($"print the symptoms:{str2}");
            }
            return symptoms;
        }

        public static List<Symptom> readTheInputk2(let[] Auto)
        {
            bool isExpression2;
            Symptom symptom;
            string pattern = @"[^a-zA-Z\s]";
            endLet word = new endLet();
            List<Symptom> symptoms = new List<Symptom>();
            let[] ptr;
            int countWord;
            bool isValidWord;
            Console.WriteLine("Tell me how you feel, what symptoms you have.");
            string str1 = Console.ReadLine();                                                                           //קבלת קלט בכתב או מהקלטה
            str1 = str1.Trim();
            str1 = str1.ToLower();
            string[] arr = str1.Split('.', '!', '?');
            for (int i = 0; i < arr.Length; i++)
                arr[i] = Regex.Replace(arr[i], pattern, "");
            for (int i = 0; i < arr.Length; i++)
                arr[i] += ' ';
            foreach (string str2 in arr)                                                                                //מעבר על כל תסמין-כל משפט מהקלט
            {
                isValidWord = true;
                isExpression2 = false;
                int x = -1;
                countWord = 0;
                symptom = new Symptom();                                                                                //יצירת עצם חדש עבור התסמין הנוכחי
                ptr = Auto;
                for (int j = 0; j < str2.Length - 1; j++)                                                               //מעבר על כל אות בקלט
                {
                    if (str2[j] - 'a' < 26 && str2[j] - 'a' >= 0 && str2[j + 1] != ' ')                                 //אם התו הוא אכן אות אנגלית קטנה
                    {                                                                                                   //&& !(ptr[(int)str2[j] - (int)'a'] is endLet)) //אם האות מופיעה באוטומט וגם היא לא האות האחרונה במילה
                        if (ptr[(int)str2[j] - (int)'a'] != null)                                                       //אם המקום הזה לא ריק
                        {
                            ptr = ptr[(int)str2[j] - (int)'a'].next;                                                    //התקדם למצב של האות הבאה
                        }
                        else                                                                                             //לא קיים במילון כלומר המילה לא נמצאת במאגר
                        {
                            string newString = str2.Substring(j);
                            int end_of_word = newString.IndexOf(' ');
                            j += end_of_word;
                            ptr = Auto;
                            isValidWord = false;

                        }
                    }
                    if (j + 1 == str2.Length || str2[j + 1] == ' ')  //הגעתי לסוף מילה
                        if (j + 1 >= 0 && j + 1 < str2.Length && (str2[j] - 'a' >= 0 && str2[j] - 'a' < ptr.Length))                       //אם הגעתי לאות האחרונה במילה- סוף מילה
                        {
                            word = new endLet();
                            if (j + 2 < str2.Length)//&& isValidWord)
                                if (isExpression(ptr, str2, j, ref word) != -1)
                                    isExpression2 = true;
                            if (!isExpression2)
                                if (ptr[str2[j] - 'a'] is endLet)
                                //if (j + 2 < str2.Length)
                                {
                                    word = (endLet)ptr[str2[j] - 'a'];
                                    ptr = Auto;
                                }
                                else                                                                                           //במקרה והגענו לסוף המילה והיא לא מופיעה באוטומט נבדוק האם היא מוכלת בביטוי מורכב          
                                {
                                    //if (!isExpression2)
                                    //{
                                    //  symptom.str += 0;
                                    //  ptr = Auto;
                                    //}
                                    //else
                                    //{

                                    //}
                                }
                            if (!isExpression2)
                            {
                                countWord++;
                                //if (word == null)
                                //  word = (endLet)ptr[str2[j] - 'a'];
                                //Console.WriteLine($"word.num: {Math.Log(word.num) / Math.Log(2)}");
                                Console.WriteLine($"word.num: {word.num}");
                                Console.WriteLine(j + "   j  ");
                                //symptom.listOfWords.Add(word);                                                           // listOfWords -יש למחוק שורה זו אם מבטלים את השדה 
                                switch (word.collection)
                                {
                                    case 1:
                                        symptom.sum += (BigInteger)Math.Pow(2, word.num);
                                        break;
                                    case 2:
                                        symptom.not++;                                                                         //תעלה את מונה מילות השלילה                                                                                  
                                        break;
                                    case 4:
                                        symptom.time = word.num;
                                        break;
                                }
                                //if (word.collection == 1)
                                //    symptom.sum += (BigInteger)Math.Pow(2, word.num);
                                //if (word.collection == 2 || word.collection==4)                                            //אם המילה היא מילת שלילה
                                //    symptom.not++;                                                                         //תעלה את מונה מילות השלילה                                                                                  
                                symptom.str = symptom.str + word.collection.ToString();                                    //שרשור יצוג האוסף למחרוזת היצוגים      
                                word = new endLet();
                                ptr = Auto;
                            }
                            isExpression2 = false;
                            if (ptr[str2[j] - 'a'] != null)
                                ptr = ptr[str2[j] - 'a'].next;
                        }
                    symptoms.Add(symptom);
                    Console.WriteLine($"Symptom_str: {symptom.str}, Not: {symptom.not}, Time: {symptom.time}, Sum: {symptom.sum}, countWord:{countWord}");
                    Console.WriteLine($"print the symptoms:{str2}");
                }
            }
            return symptoms;
        }
        private static void addWord(int x, Symptom s)
        {
            switch (x)
            {
                case 1:
                    s.sum += x;
                    break;
                case 2:
                    s.not++;
                    break;
                case -1:
                    x = 0;
                    break;
            }
            s.str += x;
        }
        private static int isExpression(let[] ptr, string str1, int j,ref endLet word2)
        {
            if (ptr[str1[j]-'a']!=null)
                ptr = ptr[str1[j] - 'a'].next;
            for (int i = j + 2; i < str1.Length; i++)
            {
                int index = str1[i] - 'a';
                if (index >= 0 && index < 26)
                {
                    if (ptr[index] is endLet)
                    {
                        word2 = (endLet)ptr[index];
                        return word2.num;
                    }
                    if (ptr[index] != null)
                        ptr = ptr[index].next;
                }
                else
                {
                    return -1;                                                                       //לא קיים ביטוי מורכב עם המילה הזו
                }
            }
            return -1;
        }
        public static Dictionary<int, string> buildNamesDictionary(string filePath )
        {
            Dictionary<int,string> names = new Dictionary<int,string>();
            string[] arr = File.ReadAllLines(filePath);
            for (int i = 0; i < arr.Length; i++)
            {
                names.Add(i, arr[i]);
            }
            return names;
        }                                  //בנית מילון ובו יצוגי הסכימות של המילים משמשים כמפתחות ויצוגי התסמינים משמשים כערכים


        public static Dictionary<BigInteger, int> buildDictionary()                                  //בנית מילון ובו יצוגי הסכימות של המילים משמשים כמפתחות ויצוגי התסמינים משמשים כערכים
        {
            Dictionary<BigInteger, int> dic = new Dictionary<BigInteger, int>();
            string filePath = @"..\..\symptoms.txt";
            string[] arr = File.ReadAllLines(filePath);
            foreach (string symptom2 in arr)                                                         //מעבר על המערך והמרתו למילון 
            {
                string[] arr2 = symptom2.Split(',');
                int value = int.Parse(arr2[0]);
                for (int i = 1; i < arr2.Length; i++)
                    if (!dic.ContainsKey(BigInteger.Parse(arr2[i])))
                    {
                        dic[BigInteger.Parse(arr2[i])] = value;
                        //dic[value]= int.Parse(arr2[i]);
                    }
            }
            return dic;
        }
        public static void checkSymptoms(List<Symptom> symptoms)                             //פונקציה שמבצעת את שלב האימות ומאתחלת את השדה num
        {
            int t;
            Dictionary<BigInteger, int> dic = buildDictionary();
            foreach (Symptom s in symptoms)                                                  //עוברים על כל תסמין
            {
                int num1 = -1;
                //Console.WriteLine("(int)Math.Floor(Math.Log(s.sum, 2)):  " + (int)Math.Floor(Math.Log(s.sum, 2)));
                BigInteger value = s.sum;
                //int value=(int)Math.Floor(Math.Log(s.sum, 2));
                Console.WriteLine("value of sum:  " + value);
                string str = value.ToString();
                if (dic.ContainsKey(value))
                    num1 = dic[value];
                else
                {
                    t = check_in_dic.CheckInDict(dic, s.sum);
                    if (t != -1 && s.num == -1)
                    {
                        num1 = t;
                    }
                }
                //for (int i = 0; i < s.str.Length; i++)
                //if (s.str[i] == 2 && i<s.str.Length-1 && s.str[i + 1] == 2)             //אם מופיעות 2 מילות שלילה רצופות
                if (s.not % 2 == 1 || s.time!=0)
                {
                    num1 = -1;                                                             //התסמין לא רלונטי
                }
                s.num = num1;
            }
        }
        public static double[][] buildWeightMatrix()                                         //בנית מטריצת משקלים
        {
            string filePath = @"..\..\weights.txt";
            string[] arr = File.ReadAllLines(filePath);
            double[][] weightMatrix = new double[arr.Length][];
            for (int i = 0; i < arr.Length; i++)
            {
                string[] numbers = arr[i].Split('|');
                weightMatrix[i] = new double[numbers.Length];                               // הגדרת אורך השורה
                for (int j = 0; j < numbers.Length; j++)
                {
                    weightMatrix[i][j] = double.Parse(numbers[j].ToString());
                }
            }
            return weightMatrix;
        }
        public static List<int> IllnessDecision(List<Symptom> symptoms)                            //הכרעת מחלה
        {
            int IllnessDecision = -1;
            List<int> maxIllnesses = new List<int>();
            double maxValue = 0;
            double[][] weightMatrix = buildWeightMatrix();
            double[] diseases = new double[weightMatrix.Length];
            foreach (Symptom symptom in symptoms)                                                //סכימת משקלי התסמינים שהופיעו עבור כל מחלה
            {
                if (symptom.num != -1)
                    for (int i = 0; i < diseases.Length; i++)
                    {
                        //diseases[i] += weightMatrix[i][int.Parse(Math.Log(symptom.num)/Math.Log(2))];
                        //Console.WriteLine((int)Math.Floor(Math.Log(symptom.sum, 2)) - 1);
                        //diseases[i] += weightMatrix[i][(int)Math.Floor(Math.Log(symptom.sum, 2))-1];
                        diseases[i] += weightMatrix[i][symptom.num];
                        Console.WriteLine("i: "+i +" = "+ diseases[i]);
                    }
            }
            for (int i = 0; i < diseases.Length; i++)                                         //בדיקת המחלות הכבדות ביותר
                if (!(maxValue > diseases[i]))
                {
                    if (maxValue == diseases[i])
                        maxIllnesses.Add(i);                                                 //הוספת מחלה עם הערך הצקסימלי
                    else                                                                 
                    {                                                                    
                        maxValue = diseases[i];                                              //שמירת הערך המקסימלי המעודכן 
                        maxIllnesses.Clear();                                                //ריקון הרשימה מהמחלות עם הערך הקודם
                        maxIllnesses.Add(i);                                                 //הוספת המחלה עם הערך המקסימלי המעודכן
                    }
                }
            if (maxValue== 0) 
                Console.WriteLine("there is no deasess with this symptom");
            else
                Console.WriteLine("the max value in the deases array is:  "+maxValue);
                return maxIllnesses;
        }

        public static int check_in_dic2(Dictionary<BigInteger,int> dic, BigInteger user_discription)
        {
            string str = Convert.ToString((long)user_discription, 2);
            BigInteger current_user_discription;
            string stringMask = new string('1', num_of_words-1) + "0";
            BigInteger mask = BigInteger.Parse(stringMask);
            for (int i = 0; i < num_of_words; i++)
            {
                //Console.WriteLine(stringMask);
                current_user_discription = user_discription & mask;
                string str3 = Convert.ToString((long)current_user_discription, 2);
                //Console.WriteLine(Convert.ToString((long)current_user_discription, 2));
                if(dic.ContainsKey(current_user_discription))
                { 
                    Console.WriteLine("we find this value in the dictionary:     "+dic[current_user_discription]); 
                    return dic[current_user_discription];
                }
                mask = check_in_dic.leftRotate(mask);
                string strMask = Convert.ToString((long)mask, 2);
            }
            Console.WriteLine("there is not sum in the dictionary");
            return -1;
        }

    }
}