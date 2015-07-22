using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CLI_DBClient
{
    class Program
    {
        public static bool Terminate { get; set; }
        delegate void Command(string[] arguments);
        static ConsoleColor cmdCol = ConsoleColor.White;
        static ConsoleColor inputCol = ConsoleColor.Yellow;
        static ConsoleColor textCol = ConsoleColor.Gray;
        static bool allowPrompt = false;

        static void Main(string[] args)
        {
            Dictionary<string, Command> commands = new Dictionary<string, Command>();
            commands.Add("connect", new Command(ConnectCommand));
            commands.Add("disconnect", new Command(DisconnectCommand));
            commands.Add("help", new Command(HelpCommand));
            commands.Add("insert", new Command(InsertCommand));
            commands.Add("update", new Command(UpdateCommand));
            commands.Add("delete", new Command(DeleteCommand));
            commands.Add("load", new Command(LoadCommand));
            commands.Add("exit", new Command(ExitCommand));

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.Title = "Database client :: by Roman Hodulák";
            Console.WriteLine("Welcome to the database client. A coursework application by Roman Hodulák.");
            Console.WriteLine("Write HELP for a list of commands");
            commands["connect"](null);

            while (!Terminate)
            {
                string input = GetInput();
                string[] arguments = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (arguments.Length > 0)
                {
                    string cmdName = arguments[0];
                    if (commands.ContainsKey(cmdName))
                        commands[cmdName](arguments);
                    else
                        Console.WriteLine("\"{0}\" cannot be recognized as a command.", cmdName);
                }
            }
        }

        public static string GetInput()
        {
            Console.ForegroundColor = inputCol;
            Console.Write(">>");
            string input = "";
            while (input.Length < 1)
            {
                input = Console.ReadLine();
                if (input.Length < 1)
                    Console.SetCursorPosition(2, Console.CursorTop - 1);
            }
            Console.ForegroundColor = textCol;
            return input.ToLower();
        }

        public static char GetInputChar(params char[] allowed)
        {
            Console.ForegroundColor = inputCol;
            Console.Write(">");
            ConsoleKeyInfo input = Console.ReadKey(true);
            while (!allowed.Contains(input.KeyChar))
            {
                Console.SetCursorPosition(1, Console.CursorTop);
                input = Console.ReadKey(true);
            }
            Console.WriteLine(input.KeyChar);
            Console.ForegroundColor = textCol;
            return input.KeyChar;
        }

        public static void HelpCommand(string[] arguments)
        {
            Console.WriteLine("List of commands:");
            WriteCmd("CONNECT", "Initializes connection to the database");
            WriteCmd("DISCONNECT", "Terminates connection to the database");
            WriteCmd("INSERT", "Inserts new record with specified attributes");
            WriteCmd("UPDATE \"ID\"", "Updates a record with specified attributes");
            WriteCmd("DELETE \"ID\"", "Removes a record by its ID");
            WriteCmd("LOAD \"ID\"", "Loads a specific record from the database");
            WriteCmd("LOAD ALL", "Loads a list of all of the records in the database");
            WriteCmd("EXIT", "Terminates connection and exits the application");
        }

        static void WriteCmd(string cmd, string desc)
        {
            Console.ForegroundColor = cmdCol;
            Console.Write(cmd);
            Console.ForegroundColor = textCol;
            Console.WriteLine(" - " + desc);
        }

        public static void ConnectCommand(string[] arguments)
        {
            WriteDB("Awaiting connection...");
            WriteDB(DB.InitializeConnection());
        }

        public static void DisconnectCommand(string[] arguments)
        {
            WriteDB(DB.CloseConnection());
        }

        public static void ExitCommand(string[] arguments)
        {
            if (DB.Conntection != null)
                WriteDB(DB.CloseConnection());
            Terminate = true;
        }

        public static void DeleteCommand(string[] arguments)
        {
            int id;
            EngineerItem item = new EngineerItem();
            if (IntArgument(arguments, "ID", 1, out id))
            {
                if (allowPrompt)
                {
                    string s = DB.LoadRecord(id, out item);
                    if (item == null)
                    {
                        WriteDB(s);
                        return;
                    }
                    ConsoleTable.WriteReadTable(item);
                    Console.ForegroundColor = textCol;
                    Console.WriteLine("Are you sure? (y/n)", item.Surname, item.Name);
                    char ch = GetInputChar(new[] { 'y', 'n', 'Y', 'N' });
                    if (ch == 'y' | ch == 'Y')
                        WriteDB(DB.DeleteRecord(id));
                }
                else
                {
                    WriteDB(DB.DeleteRecord(id));
                }
            }
        }

        public static void InsertCommand(string[] arguments)
        {
            if (DB.Conntection == null)
            {
                WriteDB("Connection is not initialized");
                return;
            }
            EngineerItem item = ConsoleTable.WriteEditTable();
            WriteDB(DB.InsertRecord(item));
        }

        public static void UpdateCommand(string[] arguments)
        {
            int id;
            EngineerItem item = new EngineerItem();
            if (IntArgument(arguments, "ID", 1, out id))
            {
                WriteDB(DB.LoadRecord(id, out item));
                if (item == null)
                    return;
            }
            else
                return;
            ConsoleTable.WriteEditTable(ref item);
            WriteDB(DB.UpdateRecord(item));
        }

        public static void LoadCommand(string[] arguments)
        {
            if (arguments.Length > 1)
            {
                int length;
                if (arguments[1] == "all")
                {
                    EngineerItem[] items;
                    WriteDB(DB.LoadRecords(out items));
                    if (items != null)
                    {
                        Array.Sort<EngineerItem>(items, new Comparison<EngineerItem>(IDComparer));
                        ConsoleTable.WriteReadTable(items);
                    }
                }
                else
                {
                    bool parsed = int.TryParse(arguments[1], out length);
                    if (!parsed | length < 1)
                    {
                        Console.WriteLine("Argument value \"{0}\" is invalid.", arguments[1]);
                        return;
                    }
                    EngineerItem item;
                    WriteDB(DB.LoadRecord(length, out item));
                    if (item != null)
                    {
                        ConsoleTable.WriteReadTable(item);
                    }
                }
            }
            else
            {
                Console.WriteLine("Argument \"ID\" is missing.");
            }
        }

        static int IDComparer(EngineerItem a, EngineerItem b)
        {
            return (a.ID > b.ID) ? 1 : ((a.ID < b.ID) ? -1 : 0);
        }

        static bool StringArgument(string[] args, string argName, int i, out string value)
        {
            value = null;
            if (args.Length > i)
            {
                value = args[i];
                return true;
            }
            else
            {
                Console.WriteLine("Argument \"" + argName + "\" is missing.");
                return false;
            }
        }

        static bool IntArgument(string[] args, string argName, int i, out int value)
        {
            value = -1;
            if (args.Length > i)
            {
                if (!int.TryParse(args[i], out value))
                {
                    Console.WriteLine("Argument value \"{0}\" is invalid.", args[i]);
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("Argument \"{0}\" is missing.", argName);
                return false;
            }
        }

        public static void WriteDB(string s)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("DB: " + s);
            Console.ForegroundColor = color;
        }
    }
}
