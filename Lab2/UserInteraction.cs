using System;
using static System.Console;

namespace FileWatcherForFun
{
    public static class UserInteraction
    {
        public delegate void PrintNotification(string message);

        public static PrintNotification printNotification;              

        // User can inputs only int number.
        public static int GetCorrectPositiveNumber()
        {
            int ourInt;

            // User inputs value while it isn't correct.
            do
            {
                try
                {
                    ourInt = Convert.ToInt32(ReadLine());
                }
                catch (Exception)
                {
                    InvalidInputMessage();
                    continue;
                }

                if (ourInt >= 0)
                {
                    break;
                }


            } while (true);

            return ourInt;
        }

        public static int GetCorrectPositiveNumber(int numberTo)
        {
            int ourInt;

            // User inputs value while it isn't correct.
            do
            {
                try
                {
                    ourInt = Convert.ToInt32(ReadLine());
                }
                catch (Exception)
                {
                    InvalidInputMessage();
                    continue;
                }

                if (ourInt <= numberTo && ourInt >= 0)
                {
                    break;
                }

            } while (true);

            return ourInt;
        }

        public static int GetCorrectNumber(int numberFrom, int numberTo)
        {
            int ourInt;

            // User inputs value while it isn't correct.
            do
            {
                try
                {
                    ourInt = Convert.ToInt32(ReadLine());

                    if (ourInt <= numberTo && ourInt >= numberFrom)
                    {
                        break;
                    }
                    else
                    {
                        InvalidInputMessage();
                    }

                }
                catch (Exception)
                {
                    InvalidInputMessage();
                    continue;
                }                

            } while (true);

            return ourInt;
        }

        public static int GetCorrectNumber(int numberTo)
        {
            int ourInt;

            // User inputs value while it isn't correct.
            do
            {
                try
                {
                    ourInt = Convert.ToInt32(ReadLine());
                }
                catch (Exception)
                {
                    InvalidInputMessage();
                    continue;
                }

                if (ourInt <= numberTo)
                {
                    break;
                }

            } while (true);

            return ourInt;
        }

        public static int GetCorrectNumber()
        {
            int ourInt;

            // User inputs value while it isn't correct.
            do
            {
                try
                {
                    ourInt = Convert.ToInt32(ReadLine());
                }
                catch (Exception)
                {
                    InvalidInputMessage();
                    continue;
                }

                break;

            } while (true);

            return ourInt;
        }

        public static void InvalidInputMessage()
        {
            Write("You enter smth wrong ! \n" +
                  "Try again : ");
        }

        public static void PrintExceptionInfo(Exception ex)
        {
            WriteLine($"There is an {ex.GetType().Name} ! ");

            WriteLine($"Exception : {ex.Message}");
            WriteLine($"In Method : {ex.TargetSite}");
        }

        public static string errorMsg = "Error ! ";
        public static string successMsg = "Success ! ";
        public static string fileDoesNotExistsMsg = String.Empty;

        public static void FileDoesNotExistsMsg (string filePath, ref string fileDoesNotExistsMsg)
        {
            fileDoesNotExistsMsg = $"File {filePath} doesn't exists !";
        }

        public static void ErrorMsg()
        {
            WriteLine("Error ! ");
        }

        public static void SuccessMsg()
        {
            WriteLine("Success ! ");
        }

        public static void FileNotExistsMsg(string filePath)
        {
            WriteLine($"File {filePath} doesn't exists !");
        }        

        public static void DirectoryNotExistsMsg(string dirPath)
        {
            WriteLine($"Directory {dirPath} doesn't exists !");
        }



    }
}
