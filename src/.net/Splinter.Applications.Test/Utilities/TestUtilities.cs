using System;
using System.Text;
using System.Threading.Tasks;

namespace Splinter.Applications.Test.Utilities
{
    public static class TestUtilities
    {
        private const string SectionLine = "=";

        private const ConsoleColor FinalSuccessColor = ConsoleColor.Magenta;
        private const ConsoleColor SectionTextColor = ConsoleColor.Blue;
        private const ConsoleColor TestTextColor = ConsoleColor.Cyan;
        private const ConsoleColor DoneColor = ConsoleColor.Green;

        public static void Reset()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SectionText(string text)
        {
            var sectionLine = GetSectionLine(text);
            var print = $"\r\n{sectionLine}\r\n{text}\r\n{sectionLine}\r\n";

            Console.ForegroundColor = SectionTextColor;
            Console.WriteLine(print);
        }

        public static async Task ExecuteTest(string text, Func<Task> action)
        {
            Console.ForegroundColor = TestTextColor;
            Console.Write($"{text}... ");

            await action();

            Console.ForegroundColor = DoneColor;
            Console.WriteLine("DONE!");
        }

        public static void FinalSuccess()
        {
            Console.ForegroundColor = FinalSuccessColor;
            Console.WriteLine("\r\nAll Splinter integration tests succeeded!\r\n");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static string GetSectionLine(string section)
        {
            var output = new StringBuilder(section.Length);

            for (var i = 0; i < section.Length; i++)
            {
                output.Append(SectionLine);
            }

            return output.ToString();
        }
    }
}
