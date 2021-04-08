using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace TestTask_1
{           
    class Program
    {
        // По умолчанию комбинация клавиш ctrl+s забиндена в работе консоли по умолчанию, поэтому необходимо сменить данную настройку
 //<-----------Импорт библиотеки dll, а из нее -  функции SetConsoleMode    
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [Flags]
        private enum ConsoleInputModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100
        }

        [Flags]
        private enum ConsoleOutputModes : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        static int STD_INPUT_HANDLE = -10;

 //-------------------------------------------------------------->

        static void ClearLine(int line)
        {
            Console.MoveBufferArea(0, line, Console.BufferWidth, 1, Console.BufferWidth, line, ' ', Console.ForegroundColor, Console.BackgroundColor);
        }
      

        static void Main(string[] args)
        {
                SetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), (uint)(         // Вызов импортированной функции
                 ConsoleInputModes.ENABLE_WINDOW_INPUT |
                 ConsoleInputModes.ENABLE_MOUSE_INPUT |
                 ConsoleInputModes.ENABLE_EXTENDED_FLAGS                 
                 ));

            string userInputFirstValue = "";
            bool isSaved = false;
            List<string> userInputStrings = new List<string>();
            ConsoleKeyInfo keyInpuInfo;        
                  
            Console.WriteLine("Enter text line and press Enter for new line. Press Ctrl + S to Save file");

            while (true) 
            {               
                keyInpuInfo = Console.ReadKey();
                if (isSaved == true) 
                {
                    ClearLine(2);
                    isSaved = false;
                }
                if (keyInpuInfo.Key == ConsoleKey.S && keyInpuInfo.Modifiers == ConsoleModifiers.Control)
                {
                    DateTime currentDateAndTime = new DateTime();
                    currentDateAndTime = DateTime.Now;
                    string dateAndTime = currentDateAndTime.ToString();
                    dateAndTime = dateAndTime.Replace(":", "-");
                    dateAndTime = dateAndTime.Replace(".", "-");
                    dateAndTime = dateAndTime.Replace(" ", "-");

                    string path = Environment.CurrentDirectory + @"\" + dateAndTime + ".txt";
                    FileStream file = new FileStream(path, FileMode.Create);
                    StreamWriter writer = new StreamWriter(file);
                    FileInfo fileInfo = new FileInfo(path);
                    foreach (string value in userInputStrings)
                    {
                        writer.WriteLine(value);                      
                    }                    
                    writer.Close();
                    file.Close();
                    long fileSize = fileInfo.Length;

                    ClearLine(1);
                    Console.SetCursorPosition(0, 2);
                    Console.WriteLine("File successfully saved. " + fileSize.ToString() + " bytes");
                    Console.SetCursorPosition(0, 1);
                    isSaved = true;
                }
                else
                {
                    if (keyInpuInfo.Key != ConsoleKey.Enter)
                    {
                        userInputFirstValue += keyInpuInfo.KeyChar;                       
                    }
                    else 
                    {
                        Console.SetCursorPosition(0, 1);
                        userInputStrings.Add(userInputFirstValue);
                        userInputFirstValue = "";
                        ClearLine(1);
                    }
                }                                                            
            }
        }
    }    
}
