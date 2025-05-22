using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Automat.Program;
using System.IO;

namespace sarasProject
{
    internal class DFA
    {
        public static let[] buildAuto()
        {
            string filePath = @"..\..\words.txt";
            string[] words = File.ReadAllLines(filePath);
            num_of_words = words.Length / 3;
            let[] l, Auto = new let[26];
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
                            ptr[index] = new let();
                        ptr = ptr[index].next;                                                                                // התקדמות למצב הבא
                    }
                }
                int endIndex = (int)words[i][words[i].Length - 1] - (int)'a';
                if (endIndex >= 0 && endIndex < 26 && ptr[endIndex] == null || !(ptr[endIndex] is endLet))
                    if (ptr[endIndex] != null)
                    {
                        l = ptr[endIndex].next;
                        ptr[endIndex] = new endLet(int.Parse(words[i + 1]), int.Parse(words[i + 2]));
                        ptr[endIndex].next = l;
                    }
                    else
                        ptr[endIndex] = new endLet(int.Parse(words[i + 1]), int.Parse(words[i + 2]));
            }
            return Auto;
        }
    }
}
