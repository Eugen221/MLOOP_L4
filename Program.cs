using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions; // Для utf-8

namespace MLOOP_L4
{
    internal class Program
    {
        // Кольори
        static string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
        static string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
        static string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
        static string UNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[4m";
        static string NOUNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[24m";
        static string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
        static string YELLOW = Console.IsOutputRedirected ? "" : "\x1b[93m";

        // Змінні для зміни розміра екрану
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_NOSIZE = 0x0001;
        const int HWND_TOP = 0;

        // Змінні для маніпулюцій із розміром вікна
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_SIZEBOX = 0x40000;

        static IntPtr handle = GetConsoleWindow();

        static Random rnd = new Random();

        private static Regex hexPattern = new Regex(@"^\d{6}$", RegexOptions.Compiled);
        static int hexIntVar;
        static string input;
        const int OUTPUTSTRING_MAX_LENGTH = 1024;
        static StringBuilder outputString = new StringBuilder(OUTPUTSTRING_MAX_LENGTH);

        static void PressAnyKeyToContinue()
        {
            Console.WriteLine(" Натисніть на будь-яку клавішу для продовження ");
            Console.ReadKey();
        }

        static void PrintTextFile(string FileName) // Просто вивід текстового файлу
        {
            using (StreamReader readtext = new StreamReader(FileName))
            {
                while (!readtext.EndOfStream)
                {
                    Console.WriteLine(readtext.ReadLine());
                }
            }
        }

        public static void DrawTextImage(int x, int y, string fileName) // Вивід текстового файлу по кооридантам
        {
            if (x >= Console.BufferWidth || y >= Console.BufferHeight)
            {
                return; // Навіщо малювати те, що повністю за межами екрану?
            }

            int currentY = y;
            int lineOffset = 0;

            if (y < 0)
            {
                lineOffset = -y;
                currentY = 0;
            }

            int lineCount = 0;

            using (StreamReader strReader = new StreamReader(fileName))
            {
                string line;

                while (lineCount < lineOffset && (line = strReader.ReadLine()) != null)
                {
                    lineCount++;
                }

                lineCount = 0;

                while ((line = strReader.ReadLine()) != null && currentY < Console.BufferHeight)
                {
                    int charOffset = 0;
                    int displayX = x;

                    if (x < 0)
                    {
                        charOffset = -x;
                        displayX = 0;
                    }

                    if (charOffset < line.Length)
                    {
                        int availableWidth = Console.BufferWidth - displayX;
                        string displayLine = line.Substring(charOffset);

                        if (displayLine.Length > availableWidth)
                        {
                            displayLine = displayLine.Substring(0, availableWidth);
                        }

                        Console.SetCursorPosition(displayX, currentY);
                        Console.Write(displayLine);
                    }

                    currentY++;
                    lineCount++;
                }
            }
        }

        static bool IsHex(string input)
        {
            if (!hexPattern.IsMatch(input)) return false;
            return int.TryParse(input, out hexIntVar) ? hexIntVar.ToString().Length == 6 : false;
        }

        static void Task1()
        {
            Console.WriteLine();
            bool isHex;
            while (true)
            {
                Console.Write(" Введіть шістнадцяткове число (0 - вихід)\n > ");
                input = Console.ReadLine();
                if (input == "0") break;

                isHex = IsHex(input);
                Console.WriteLine(isHex ? $" Число {input} є шістнадцятковим" : $" Число {input} не є шістнадцятковим");
            }
        }

        static int GetNumOfPositiveElements(int[] inArray)
        {
            int numOfPositiveElements = 0;
            foreach (int element in inArray)
            {
                if (element > 0)
                {
                    numOfPositiveElements++;
                }
            }
            return numOfPositiveElements;
        }

        static int GetNumOfNegativeElements(int[] inArray)
        {
            int numOfNEgativeElements = 0;
            foreach (int element in inArray)
            {
                if (element < 0)
                {
                    numOfNEgativeElements++;
                }
            }
            return numOfNEgativeElements;
        }

        static void PrintMatrix(int[][] inMatrix)
        {
            int i, j;
            Console.Write(GREEN);
            for (i = 0; i < inMatrix.Length; i++)
            {
                Console.Write(" ");
                for (j = 0; j < inMatrix[0].Length; j++)
                {
                    Console.Write($"\t{inMatrix[i][j]}");
                }
                Console.WriteLine("");
            }
            Console.WriteLine(NORMAL);
        }

        static string pasreCharsToLatin(string input)
        {
            // Перетворюємо українську і в латинську i
            string outputString = String.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '?') outputString += 'i';
                else outputString += input[i];
            }
            return outputString;
        }

        static void Task2()
        {
            Console.WriteLine();
            string[] inputSplitted;
            while (true)
            {
                Console.WriteLine();
                outputString.Clear();
                Console.Write($" Введіть строку (0 - вихід, максимальна дожвина - {OUTPUTSTRING_MAX_LENGTH})\n > ");
                input = Console.ReadLine();
                if (input == "0") break;
                if (input.Length > OUTPUTSTRING_MAX_LENGTH || input == null) continue;

                inputSplitted = input.Split();
                foreach (string word in inputSplitted)
                {
                    if(word.Length <= 5) outputString.Append(word + " ТАК ");
                    else outputString.Append(word + $" {word.Length} ");
                }

                Console.WriteLine(" " + pasreCharsToLatin(outputString.ToString()));
            }
        }

        static void WriteTextAt(int x, int y, string text) // Виводить текст на конкретних координатах
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(lines[i]);
            }
        }

        static void WriteTextAt(int x, int y, string text, ref int currentY) // Не тільки виводить текст на конкретних координатах, а й збільшує лічільник стрічок
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                currentY += 1;
                Console.SetCursorPosition(x, y + i);
                Console.Write(lines[i]);
            }
        }

        static void Setup()
        {
            Console.BackgroundColor = ConsoleColor.Black;

            Console.OutputEncoding = Encoding.UTF8;
            Console.Write(NORMAL);

            SetWindowPos(handle, (IntPtr)HWND_TOP, 10, 120, 1900, 800, SWP_NOZORDER);
            Console.CursorVisible = false;
            int style = GetWindowLong(handle, GWL_STYLE);
            style &= ~(WS_MAXIMIZEBOX | WS_SIZEBOX);
            SetWindowLong(handle, GWL_STYLE, style);
        }
        static void Main(string[] args)
        {
            Setup();
            PrintTitle($"02.04.2025", 4, "Створення й обробка рядків");

            bool isRunning = true;
            while (isRunning)
            {
                Console.Write($"\n Введіть відповідний номер:\n {UNDERLINE}0){NOUNDERLINE} Вихід;\n " +
                    $"{UNDERLINE}1){NOUNDERLINE} Завдання №4.1.10;\n " +
                    $"{UNDERLINE}2){NOUNDERLINE} Завдання №4.1.23;\n > ");
                int userChoice;
                if (!int.TryParse(Console.ReadLine(), out userChoice)) { break; }
                Console.Clear();

                switch (userChoice)
                {
                    case 0:
                        isRunning = false;
                        break;
                    case 1:
                        Task1();
                        break;
                    case 2:
                        Task2();
                        break;
                    default:
                        Console.WriteLine(" Введено некоректне число.");
                        break;
                }
                Console.Clear();
            }
        }
    }
}
