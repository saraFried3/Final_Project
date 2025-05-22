using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Automat.Program;
using System.Numerics;

namespace sarasProject
{
    internal class read_the_input
    {
        static string GetStemsFromSentence(string sentence)
        {
            // מפצל את המחרוזת למילים
            string[] words = sentence.Split(new char[] { ' ', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            // הגדרת השורש מתוך ה-Porter2StemmerStandard
            var stemmer = new Porter2StemmerStandard.EnglishPorter2Stemmer();  // הגדרה מפורשת של ה-namespace
            var stems = new string[words.Length];
            for (int i = 0; i < words.Length; i++)
            {
                stems[i] = stemmer.Stem(words[i]).Value; // השתמש ב-.Value כדי לקבל מחרוזת
            }
            return string.Join(" ", stems);
        }
        public static List<Symptom> readTheInput3(let[] Auto)
        {
            string str2, pattern = @"[^a-zA-Z\s]";
            bool isExpression2, isValidWord;
            Symptom symptom;
            endLet word = new endLet();
            List<Symptom> symptoms = new List<Symptom>();
            int countWord;
            Console.WriteLine("Tell me how you feel, what symptoms you have.");
            string str1 = Console.ReadLine().Trim().ToLower();                                                 //קבלת קלט בכתב או מהקלטה
            Console.WriteLine(str1);
            string[] arr = str1.Split('.', '!', '?');
            for (int i = 0; i < arr.Length - 1; i++)// כל משפט- תסמין
            {
                arr[i] = Regex.Replace(arr[i], pattern, "");
                arr[i] = GetStemsFromSentence(arr[i]);
                //arr[i] = arr[i].Trim();
                endLet w = new endLet();
                isValidWord = true;
                isExpression2 = false;
                countWord = 0;
                symptom = new Symptom();                                                                                //יצירת עצם חדש עבור התסמין הנוכחי
                string[] wordsArr = arr[i].Split(' ');
                for (int j = 0; j < wordsArr.Length; j++) //כל מילה
                {
                    // if (j + 1 < wordsArr.Length)
                    //    w = get_word_details(wordsArr[j] + wordsArr[++j], Auto);
                    //if (w.collection==0)
                    w = get_word_details(wordsArr[j], Auto, j);
                    symptom.addWord(w);
                }

            }
            return symptoms;
        }
        private static endLet get_word_details(string input, let[] Auto, int j)
        {
            let[] ptr = Auto;
            int i = 0;
            while (i < input.Length - 1 && ptr[input[i] - 'a'] != null)
            {
                ptr = ptr[input[i] - 'a'].next;
                i++;
            }
            if (ptr[input[i] - 'a'] is endLet)
            {
                return (endLet)ptr[input[i] - 'a'];
            }
            return new endLet();
        }
        public static List<Symptom> readTheInput(let[] Auto,string str1)
        {
            string str2;
            bool isExpression2, isValidWord;
            Symptom symptom;
            string pattern = @"[^a-zA-Z\s]";
            endLet word = new endLet();
            List<Symptom> symptoms = new List<Symptom>();
            let[] ptr;
            int countWord;
            Console.WriteLine("Tell me how you feel, what symptoms you have.");
            str1=str1.Trim().ToLower();
            //string str1 = Console.ReadLine().Trim().ToLower();                                                 //קבלת קלט בכתב 
            Console.WriteLine(str1);
            string[] arr = str1.Split('.', '!', '?');
            for (int i = 0; i < arr.Length; i++)
            {
                str2 = Regex.Replace(arr[i], pattern, "");
                str2 = GetStemsFromSentence(str2);
                str2 += ' ';
                isValidWord = true;
                isExpression2 = false;
                countWord = 0;
                symptom = new Symptom();                                                                                //יצירת עצם חדש עבור התסמין הנוכחי
                ptr = Auto;
                for (int j = 0; j < str2.Length - 1; j++)                                                               //מעבר על כל אות בקלט
                {
                    if (str2[j] - 'a' < 26 && str2[j] - 'a' >= 0 && str2[j + 1] != ' ')                                 //אם התו הוא אכן אות אנגלית קטנה
                    {                                                                                                   //&& !(ptr[(int)str2[j] - (int)'a'] is endLet)) //אם האות מופיעה באוטומט וגם היא לא האות האחרונה במילה
                        if (ptr[(int)str2[j] - (int)'a'] != null && isValidWord)                                                       //אם המקום הזה לא ריק
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
                            {
                                word = (endLet)ptr[str2[j] - 'a'];
                                ptr = Auto;
                            }
                        if (!isExpression2)
                        {
                            isValidWord = true;
                            countWord++;
                            symptom.addWord(word);
                            word = new endLet();
                            ptr = Auto;
                        }
                        else                                                                                          //במקרה והגענו לסוף המילה והיא לא מופיעה באוטומט נבדוק האם היא מוכלת בביטוי מורכב          
                        {
                            isExpression2 = false;
                            if (ptr[str2[j] - 'a'] != null && isValidWord)
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
        public static void wordAdd(endLet word, ref Symptom symptom)
        {
            Console.WriteLine($"word.num: {word.num}");
            if (word.collection == 1)
                symptom.sum += (BigInteger)Math.Pow(2, word.num);
            if (word.collection == 2)                                            //אם המילה היא מילת שלילה
                symptom.not++;                                                                         //תעלה את מונה מילות השלילה                                                                                  
            if (word.collection == 4)
                symptom.time = word.num;
            symptom.str = symptom.str + word.collection.ToString();
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
        private static int isExpression(let[] ptr, string str1, int j, ref endLet word2)
        {
            if (ptr[str1[j] - 'a'] != null)
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
                    return -1;                                                        //לא קיים ביטוי מורכב עם המילה הזו
                }
            }
            return -1;
        }
    }
}


//Console.WriteLine($"word.num: {word.num}");
//if (word.collection == 1)
//    symptom.sum += (BigInteger)Math.Pow(2, word.num);
//if (word.collection == 2)                                                              //אם המילה היא מילת שלילה
//    symptom.not++;                                                                     //תעלה את מונה מילות השלילה                                                                                  
//if (word.collection == 4)
//    symptom.time = word.num;
//symptom.str = symptom.str + word.collection.ToString();                                //שרשור יצוג האוסף למחרוזת היצוגים      
