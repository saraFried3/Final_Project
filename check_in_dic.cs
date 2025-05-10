using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Automat;

namespace sarasProject
{
    public class check_in_dic
    {
        //BigInteger originalValue = BigInteger.Parse(binaryStr, System.Globalization.NumberStyles.AllowHexSpecifier);
        public static int CheckInDict(Dictionary<BigInteger, int> dict, BigInteger originalValue)
        {
            if (dict.ContainsKey(originalValue))  // ננסה לבדוק את הערך המקורי
            {
                Console.WriteLine("   : מצאנו את הערך גם ללא הורדת סיבית" + dict[originalValue]);
                return dict[originalValue];
            }
            for (int i = 0; i < Program.num_of_words; i++)                    // ננסה לכבות כל סיבית בתורה
            {
                BigInteger mask = ~(BigInteger.One << i); // מסיכה שמכבה את הסיבית במקום i
                BigInteger modifiedValue = originalValue & mask;
                if (dict.ContainsKey(modifiedValue))
                {
                    Console.WriteLine("מצאנו ערך לאחר כיבוי סיבית: ❤️" + dict[modifiedValue]);
                    return dict[modifiedValue];
                }
            }
            Console.WriteLine("😢לא נמצא ערך גם לאחר כיבוי סיבית");
            return -1;
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
            //Console.WriteLine(vector.ToString());
            //Console.WriteLine(num_of_words.ToString()+ "  :num_of_words");
            //Console.WriteLine( leftBitHigh.ToString()+ "  leftBitHigh:  " );
            vector = vector << 1;
            //Console.WriteLine(  vector.ToString()+ "  vector << 1:  ");
            BigInteger number = (BigInteger.One << Program.num_of_words) - 1;
            //Console.WriteLine( number+ "str:  ");
            vector = vector & number;
            //Console.WriteLine( vector.ToString()+"  vector & number from str:  ");
            vector = vector | leftBit;
            //Console.WriteLine( vector.ToString()+ "  vector | leftBit :  ");
            //Console.WriteLine( vector.ToString()+ "  final vector :  " );
            return vector;
        }
    }
}
