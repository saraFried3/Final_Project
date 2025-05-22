using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Automat.Program;
using System.IO;

namespace sarasProject
{
    public class IllnessDecision
    {
        public static double[][] buildWeightMatrix()                                               //בנית מטריצת משקלים
        {
            string filePath = @"..\..\weights.txt";
            string[] arr = File.ReadAllLines(filePath);
            double[][] weightMatrix = new double[arr.Length][];
            for (int i = 0; i < arr.Length; i++)
            {
                string[] numbers = arr[i].Split('|');
                weightMatrix[i] = new double[numbers.Length];                                      // הגדרת אורך השורה
                for (int j = 0; j < numbers.Length; j++)
                {
                    weightMatrix[i][j] = double.Parse(numbers[j].ToString());
                }
            }
            return weightMatrix;
        }
        public static List<int> illnessDecision(List<Symptom> symptoms)                            //הכרעת מחלה
        {
            int[] arr = new int[m];
            int IllnessDecision = -1;
            List<int> maxIllnesses = new List<int>();
            double maxValue = 0;
            double[][] weightMatrix = buildWeightMatrix();
            double[] diseases = new double[weightMatrix.Length];
            foreach (Symptom symptom in symptoms)                                                  //סכימת משקלי התסמינים שהופיעו עבור כל מחלה
            {
                if (symptom.num != -1 && arr[symptom.num] == 0)
                {
                    arr[symptom.num]++;
                    for (int i = 0; i < diseases.Length; i++)
                    {
                        diseases[i] += weightMatrix[i][symptom.num];
                        Console.WriteLine("i: " + i + " = " + diseases[i]);
                    }
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
            if (maxValue == 0)
                Console.WriteLine("there is no deasess with this symptom");
            else
                Console.WriteLine("the max value in the deases array is:  " + maxValue);
            return maxIllnesses;
        }
    }
}
