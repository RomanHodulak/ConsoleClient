using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CLI_DBClient
{
    public class ConsoleTable
    {
        public static ConsoleColor TextColor
        {
            get { return ConsoleTable.textColor; }
            set { ConsoleTable.textColor = value; }
        }
        public static ConsoleColor BorderColor
        {
            get { return ConsoleTable.borderColor; }
            set { ConsoleTable.borderColor = value; }
        }

        static ConsoleColor borderColor = ConsoleColor.DarkCyan;
        static ConsoleColor textColor = ConsoleColor.White;

        public static EngineerItem WriteEditTable()
        {
            EngineerItem item = new EngineerItem();
            int wageWidth = 7;
            int dateWidth1 = "Birth (yyyy-mm-dd)".Length;
            int dateWidth2 = "Employed (yyyy-mm-dd)".Length;
            int nameWidth = (Console.BufferWidth - dateWidth1 - dateWidth2 - wageWidth - 6) / 2;
            int[] columnWidths = new[] { nameWidth, nameWidth, dateWidth1, dateWidth2, wageWidth };

            WriteSplitLine('╔', '╦', '╗', columnWidths);
            WriteColumn("Name", nameWidth);
            WriteColumn("Surname", nameWidth);
            WriteColumn("Birth (yyyy-mm-dd)", dateWidth1);
            WriteColumn("Employed (yyyy-mm-dd)", dateWidth2);
            WriteColumn("Wage", wageWidth);
            WriteSplitLine('╠', '╬', '╣', columnWidths);
            Console.CursorTop += 1;
            WriteSplitLine('╚', '╩', '╝', columnWidths);
            Console.CursorTop -= 2;
            WriteSplitLine('║', '║', '║', ' ', columnWidths);
            Console.CursorTop -= 1;

            Console.ForegroundColor = textColor;

            Console.CursorLeft = 1;
            item.Name = EditValue(item.Name);
            Console.CursorTop -= 1;

            Console.CursorLeft = 2 + nameWidth;
            item.Surname = EditValue(item.Surname);
            Console.CursorTop -= 1;

            Console.CursorLeft = 3 + (nameWidth * 2);
            item.Date_of_birth = EditValue(item.Date_of_birth);
            Console.CursorTop -= 1;

            Console.CursorLeft = 4 + (nameWidth * 2) + dateWidth1;
            item.Employed_since = EditValue(item.Employed_since);
            Console.CursorTop -= 1;

            Console.CursorLeft = 5 + (nameWidth * 2) + dateWidth1 + dateWidth2;
            item.Wage = EditValue(item.Wage);

            Console.CursorTop += 1;
            return item;
        }

        public static void WriteEditTable(ref EngineerItem item)
        {
            int wageWidth = 7;
            int dateWidth1 = "Birth (yyyy-mm-dd)".Length;
            int dateWidth2 = "Employed (yyyy-mm-dd)".Length;
            int nameWidth = (Console.BufferWidth - dateWidth1 - dateWidth2 - wageWidth - 6) / 2;
            int[] columnWidths = new[] { nameWidth, nameWidth, dateWidth1, dateWidth2, wageWidth };

            WriteSplitLine('╔', '╦', '╗', columnWidths);
            WriteColumn("Name", nameWidth);
            WriteColumn("Surname", nameWidth);
            WriteColumn("Birth (yyyy-mm-dd)", dateWidth1);
            WriteColumn("Employed (yyyy-mm-dd)", dateWidth2);
            WriteColumn("Wage", wageWidth);
            WriteSplitLine('╠', '╬', '╣', columnWidths);
            Console.CursorTop += 1;
            WriteSplitLine('╚', '╩', '╝', columnWidths);
            Console.CursorTop -= 2;
            WriteSplitLine('║', '║', '║', ' ', columnWidths);
            Console.CursorTop -= 1;
            
            Console.ForegroundColor = textColor;

            Console.CursorLeft = 1;
            item.Name = EditValue(item.Name);
            Console.CursorTop -= 1;

            Console.CursorLeft = 2 + nameWidth;
            item.Surname = EditValue(item.Surname);
            Console.CursorTop -= 1;

            Console.CursorLeft = 3 + (nameWidth * 2);
            item.Date_of_birth = EditValue(item.Date_of_birth);
            Console.CursorTop -= 1;

            Console.CursorLeft = 4 + (nameWidth * 2) + dateWidth1;
            item.Employed_since = EditValue(item.Employed_since);
            Console.CursorTop -= 1;

            Console.CursorLeft = 5 + (nameWidth * 2) + dateWidth1 + dateWidth2;
            item.Wage = EditValue(item.Wage);

            Console.CursorTop += 1;
        }

        public static void WriteReadTable(params EngineerItem[] items)
        {
            int idWidth = MaxID(items).ToString().Length;
            if (idWidth < 2)
                idWidth = 2;
            int wageWidth = LargestWage(items).ToString().Length;
            if (wageWidth < 4)
                wageWidth = 4;
            int dateWidth = "Employed since".Length;
            int nameWidth = (Console.BufferWidth - idWidth - dateWidth - dateWidth - wageWidth - 7);
            if (nameWidth % 2 != 0)
                wageWidth += 1;
            nameWidth /= 2;
            int[] columnWidths = new[] { idWidth, nameWidth, nameWidth, dateWidth, dateWidth, wageWidth };

            WriteSplitLine('╔', '╦', '╗', columnWidths);
            WriteColumn("ID", idWidth);
            WriteColumn("Name", nameWidth);
            WriteColumn("Surname", nameWidth);
            WriteColumn("Date of birth", dateWidth);
            WriteColumn("Employed since", dateWidth);
            WriteColumn("Wage", wageWidth);
            WriteSplitLine('╠', '╬', '╣', columnWidths);
            foreach (EngineerItem i in items)
            {
                WriteColumn(i.ID.ToString(), idWidth, true);
                WriteColumn(i.Name, nameWidth);
                WriteColumn(i.Surname, nameWidth);
                WriteColumn(i.Date_of_birth.ToShortDateString(), dateWidth, true);
                WriteColumn(i.Employed_since.ToShortDateString(), dateWidth, true);
                WriteColumn(i.Wage.ToString(), wageWidth, true);
            }
            WriteSplitLine('╚', '╩', '╝', columnWidths);
        }

        static void WriteSplitLine(char start, char split, char end, int[] columnWidths)
        {
            WriteSplitLine(start, split, end, '═', columnWidths);
        }

        static void WriteSplitLine(char start, char split, char end, char fillchar, int[] columnWidths)
        {
            Console.ForegroundColor = borderColor;
            Console.Write(start);
            for (int i = 0; i < Console.BufferWidth - 2; i++)
            {
                int w = 0;
                bool b = false;
                for (int j = 0; j < columnWidths.Length; j++)
                {
                    w += columnWidths[j];
                    if (i == j + w)
                    {
                        Console.Write(split);
                        b = true;
                        break;
                    }
                }
                if (!b)
                    Console.Write(fillchar);
            }
            Console.Write(end);
        }

        static void WriteColumn(string str, int columnWidth)
        {
            WriteColumn(str, columnWidth, false);
        }

        static void WriteColumn(string str, int columnWidth, bool rightAlign)
        {
            Console.ForegroundColor = borderColor;
            if (Console.CursorLeft == 0)
                Console.Write("║");
            Console.ForegroundColor = textColor;
            if (rightAlign)
            {
                for (int j = 0; j < columnWidth - str.Length; j++)
                    Console.Write(" ");
                Console.Write(str);
            }
            else
            {
                Console.Write(str);
                for (int j = 0; j < columnWidth - str.Length; j++)
                    Console.Write(" ");
            }
            Console.ForegroundColor = borderColor;
            if (Console.CursorLeft == Console.BufferWidth - 1)
                Console.Write("║");
            else
                Console.Write("║");
        }

        static int MaxID(EngineerItem[] items)
        {
            int max = int.MinValue;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].ID > max)
                    max = items[i].ID;
            }
            return max;
        }

        static int LargestWage(EngineerItem[] items)
        {
            int max = int.MinValue;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Wage > max)
                    max = items[i].Wage;
            }
            return max;
        }

        static string EditValue(string value)
        {
            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;
            SendKeys.SendWait(value);
            string s = Console.ReadLine();
            while (s.Length < 1)
            {
                Console.SetCursorPosition(cursorX, cursorY);
                SendKeys.SendWait(value);
                s = Console.ReadLine();
            }
            return s;
        }

        static DateTime EditValue(DateTime date)
        {
            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;
            if (date.Year > 1900)
                SendKeys.SendWait(DB.ToSQLDate(date));
            DateTime val = new DateTime();
            string s = Console.ReadLine();
            for (; ; )
            {
                try
                {
                    val = DB.ToDate(s);
                    break;
                }
                catch
                {
                    Console.SetCursorPosition(cursorX, cursorY);
                    for (int i = 0; i < s.Length; i++)
                        Console.Write(' ');
                    Console.SetCursorPosition(cursorX, cursorY);
                    if (date.Year > 1900)
                        SendKeys.SendWait(DB.ToSQLDate(date));
                    s = Console.ReadLine();
                }
            }
            return val;
        }

        static int EditValue(int value)
        {
            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;
            if (value > 0)
                SendKeys.SendWait(value.ToString());
            int num = 0;
            string s = Console.ReadLine();
            while (!int.TryParse(s, out num))
            {
                Console.SetCursorPosition(cursorX, cursorY);
                for (int i = 0; i < s.Length; i++)
                    Console.Write(' ');
                Console.SetCursorPosition(cursorX, cursorY);
                if (value > 0)
                    SendKeys.SendWait(value.ToString());
                s = Console.ReadLine();
            }
            return num;
        }
    }
}
