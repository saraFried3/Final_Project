using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sarasProject
{
    internal class audio_from_python
    {

        public static void audio()
        {
            string pythonScriptPath = @"C:\Users\WIN 11\Desktop\Final_Project\speech_to_text_project\speechToText.py";
            string pythonExecutablePath = "python"; // נסי גם עם נתיב מלא אם יש בעיה

            Console.WriteLine($"נתיב קובץ הפייתון שנקבע: {pythonScriptPath}");
            Console.WriteLine($"נתיב מפרש הפייתון שנקבע: {pythonExecutablePath}");

            ProcessStartInfo psi = new ProcessStartInfo(pythonExecutablePath, pythonScriptPath);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.CreateNoWindow = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.StandardErrorEncoding = Encoding.UTF8;
            //psi.StandardInputEncoding = Encoding.UTF8;

            try
            {
                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        // ניתוח פלט
                        process.OutputDataReceived += (sender, eventArgs) =>
                        {
                            if (!string.IsNullOrEmpty(eventArgs.Data))
                            {
                                Console.WriteLine($"[Python Output]: {eventArgs.Data}");
                            }
                        };
                        process.BeginOutputReadLine();

                        // ניתוח שגיאות
                        process.ErrorDataReceived += (sender, eventArgs) =>
                        {
                            if (!string.IsNullOrEmpty(eventArgs.Data))
                            {
                                Console.Error.WriteLine($"[Python Error]: {eventArgs.Data}");
                            }
                        };
                        process.BeginErrorReadLine();

                        Console.WriteLine("תהליך הפיתון התחיל. לחץ על Enter כדי לעצור את ההקלטה...");
                        Console.ReadKey();

                        // שליחת פקודת עצירה
                        using (StreamWriter sw = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8))
                        {
                            sw.WriteLine("STOP_RECORDING");
                            sw.Flush();
                        }

                        process.WaitForExit();
                        int exitCode = process.ExitCode;
                        Console.WriteLine($"תהליך הפיתון הסתיים עם קוד יציאה: {exitCode}");
                    }
                    else
                    {
                        Console.WriteLine("שגיאה בהפעלת תהליך הפיתון (התהליך היה null).");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה כללית בהפעלת הפיתון: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  שגיאה פנימית: {ex.InnerException.Message}");
                }
            }
        }

        public static void audio3()
        {

            Console.WriteLine("Hello, World!"); // הדפס פלט לבדוק שהקוד רץ

            string pythonScriptPath = @"C:\Users\WIN 11\Desktop\Final_Project\speech_to_text_project\speechToText.py";
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // או "python3" בהתאם להתקנה שלך
            start.Arguments = $"\"{pythonScriptPath}\""; // העבר את נתיב הסקריפט
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true; // כדי לתפוס שגיאות
            start.CreateNoWindow = true;

            using (Process process = Process.Start(start))
            {
                // קריאה לפלט מהקונסול של פייתון
                Thread outputThread = new Thread(() =>
                {
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string result;
                        while ((result = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(result); // הדפס את הפלט מהפייתון
                        }
                    }
                });
                outputThread.Start();
                // המתן ללחיצה על Enter כדי לסיים את ההקלטה
                Console.WriteLine("Press Enter to stop recording...");
                Console.ReadLine();
                // סיים את התהליך של פייתון
                if (!process.HasExited)
                {
                    process.Kill();
                }
                outputThread.Join(); // המתן לסיום של thread הפלט
            }
        }

        public static void audio4()
        {
            string pythonScriptPath = @"C:\Users\WIN 11\Desktop\Final_Project\speech_to_text_project\speechToText.py";
            string pythonExecutablePath = "python";
            Console.WriteLine($"נתיב קובץ הפייתון שנקבע: {pythonScriptPath}");
            ProcessStartInfo psi = new ProcessStartInfo(pythonExecutablePath, pythonScriptPath);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true; // מאפשר כתיבה לקלט של פיתון
            psi.CreateNoWindow = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.StandardErrorEncoding = Encoding.UTF8;
            //psi.StandardInputEncoding = Encoding.UTF8;

            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    // ניתוח פלט (כמו בדוגמה הקודמת)
                    process.OutputDataReceived += (sender, eventArgs) =>
                    {
                        if (!string.IsNullOrEmpty(eventArgs.Data))
                        {
                            Console.WriteLine($"[Python Output]: {eventArgs.Data}");
                        }
                    };
                    process.BeginOutputReadLine();
                    process.ErrorDataReceived += (sender, eventArgs) =>
                    {
                        if (!string.IsNullOrEmpty(eventArgs.Data))
                        {
                            Console.Error.WriteLine($"[Python Error]: {eventArgs.Data}");
                        }
                    };
                    process.BeginErrorReadLine();

                    Console.WriteLine("תהליך הפיתון התחיל. לחץ על Enter כדי לעצור את ההקלטה...");
                    Console.ReadKey(); // המתנה ללחיצה על Enter

                    // שליחת פקודה לפיתון דרך הקלט הסטנדרטי
                    process.StandardInput.WriteLine("STOP_RECORDING");
                    process.StandardInput.Flush(); // חשוב לשטוף את הbuffer כדי שהפיתון יקבל את הפקודה

                    process.WaitForExit();
                    int exitCode = process.ExitCode;
                    Console.WriteLine($"תהליך הפיתון הסתיים עם קוד יציאה: {exitCode}");
                }
                else
                {
                    Console.WriteLine("שגיאה בהפעלת תהליך הפיתון.");
                }
            }
        }
        public static void audio2()
        {
            string pythonScriptPath = @"C:\Users\WIN 11\Desktop\Final_Project\speech_to _text_project\speechToText.py";
            //string inputValue = "World"; // ערך לדוגמה
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // או "python3" בהתאם להתקנה שלך
            start.Arguments = $"\"{pythonScriptPath}\" ";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;

            using (Process process = Process.Start(start))
            {
                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result); // הדפס את הפלט מהפייתון
                }
            }
        }
    }
}
