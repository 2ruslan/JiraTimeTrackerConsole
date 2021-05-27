using System;
using System.Configuration;

namespace JiraTimeTracker
{
    class Program
    {
        private static string BaseUrl = ConfigurationManager.AppSettings["baseUrl"];
        private static string User = ConfigurationManager.AppSettings["user"];
        private static string Pass = ConfigurationManager.AppSettings["password"];

        private static DateTime DateFrom;
        private static DateTime DateTo => DateFrom.AddDays(6);

        private static Repository repo;

        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            Console.SetWindowSize(Console.BufferWidth, (int)(Console.LargestWindowHeight * 0.75));

            initDate();

            repo = new Repository(BaseUrl, User, Pass);

            bool continueWork = true;

            ShowData();

            do
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        continueWork = false;
                        break;
                    case ConsoleKey.LeftArrow:
                        ShowPrev();
                        break;
                    case ConsoleKey.RightArrow:
                        ShowNext();
                        break;
                    case ConsoleKey.Spacebar:
                        ShowData();
                        break;

                }
              
            } while (continueWork);
        
        }

        private static void ShowData()
        {
            Console.Clear();
            repo.LoadData(DateFrom, DateTo);

            new Presenter().ShowData(repo, DateFrom, DateTo);

            ShowPromt();
        }

        private static void ShowPrev()
        {
            DateFrom = DateFrom.AddDays(-7);
            ShowData();
        }

        private static void ShowNext()
        {
            DateFrom = DateFrom.AddDays(7);
            ShowData();
        }

        private static void ShowPromt()
        {
            Console.WriteLine();
            Console.WriteLine("← = prew week");
            Console.WriteLine("→ = next week");
            Console.WriteLine("space = reload data");
            Console.WriteLine("esc = exit");
        }

        private static void initDate()
            => DateFrom = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1).Date;

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
