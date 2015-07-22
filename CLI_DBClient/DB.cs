using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CLI_DBClient
{
    public static class DB
    {
        public static SqlConnection Conntection { get { return con; } }
        static SqlConnection con;
        public static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "lsd.spsejecna.net";
                builder.UserID = "hodular";
                builder.Password = "databaze";
                builder.InitialCatalog = "hodular";
                return builder.ConnectionString;
            }
        }

        public static string InitializeConnection()
        {
            string ret = "Connection initialized";
            if (con != null)
                ret = "Connection reinitialized";
            CloseConnection();
            con = new SqlConnection(ConnectionString);
            try
            {
                con.Open();
                return ret;
            }
            catch (Exception e)
            {
                if (con != null)
                {
                    con.Close();
                    con = null;
                }
                return e.ToString();
            }
        }

        public static string CloseConnection()
        {
            if (con != null)
            {
                con.Close();
                con = null;
            }
            else
                return "Connection is not initialized";
            return "Connection terminated";
        }

        public static string ResetDatabase()
        {
            try
            {
                SqlCommand command0 = new SqlCommand("IF OBJECT_ID('EngineerTable', 'U') IS NOT NULL \nDROP TABLE EngineerTable", con);
                ExecuteCommand(command0);

                SqlCommand command1 = new SqlCommand("Create table EngineerTable (engineer_id int IDENTITY(1,1) PRIMARY KEY, name nvarchar(32), surname nvarchar(32), date_of_birth date, wage int, employed_since date);", con);
                ExecuteCommand(command1);

                return "Database has been reset successfully";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string LoadRecord(int id, out EngineerItem item)
        {
            List<EngineerItem> itemsList = new List<EngineerItem>();
            item = null;
            if (con == null)
                return "Connection is not initialized";
            try
            {
                SqlCommand command2 = new SqlCommand("Select * from EngineerTable where engineer_id='" + id + "';", con);
                SqlDataReader sdr1 = command2.ExecuteReader();
                while (sdr1.Read())
                {
                    item = new EngineerItem();
                    item.ID = (int)sdr1["engineer_id"];
                    item.Name = sdr1["name"].ToString();
                    item.Surname = sdr1["surname"].ToString();
                    item.Date_of_birth = ToReaderDate(sdr1["birthdate"].ToString());
                    item.Wage = (int)sdr1["wage"];
                    item.Employed_since = ToReaderDate(sdr1["Employed_since"].ToString());
                }
                sdr1.Close();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            if (item != null)
                return "Record loaded";
            else
                return "Record " + id + " not found";
        }

        public static string LoadRecords(out EngineerItem[] items)
        {
            return LoadRecords(out items, 0);
        }

        public static string LoadRecords(out EngineerItem[] items, int count)
        {
            List<EngineerItem> itemsList = new List<EngineerItem>();
            items = null;
            if (con == null)
                return "Connection is not initialized";
            try
            {
                SqlCommand command2 = null;
                if (count < 1)
                    command2 = new SqlCommand("Select * from EngineerTable", con);
                else
                    command2 = new SqlCommand("Select TOP " + count + " * from EngineerTable", con);
                SqlDataReader sdr1 = command2.ExecuteReader();
                while (sdr1.Read())
                {
                    EngineerItem item = new EngineerItem();
                    item.ID = (int)sdr1["engineer_id"];
                    item.Name = sdr1["name"].ToString();
                    item.Surname = sdr1["surname"].ToString();
                    item.Date_of_birth = ToReaderDate(sdr1["birthdate"].ToString());
                    item.Wage = (int)sdr1["wage"];
                    item.Employed_since = ToReaderDate(sdr1["Employed_since"].ToString());
                    itemsList.Add(item);
                }
                sdr1.Close();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            items = itemsList.ToArray();
            if (items.Length != 1)
                return items.Length + " records loaded";
            else
                return items.Length + " record loaded";
        }

        public static string InsertRecord(EngineerItem item)
        {
            SqlCommand command2 = new SqlCommand("Insert into EngineerTable (name, surname, birthdate, wage, employed_since) values (@a, @b, @c, @d, @e);", con);
            command2.Parameters.AddWithValue("@a", item.Name);
            command2.Parameters.AddWithValue("@b", item.Surname);
            command2.Parameters.AddWithValue("@c", ToSQLDate(item.Date_of_birth));
            command2.Parameters.AddWithValue("@d", item.Wage);
            command2.Parameters.AddWithValue("@e", ToSQLDate(item.Employed_since));
            return ExecuteCommand(command2, "Record saved");
        }

        public static string UpdateRecord(EngineerItem item)
        {
            SqlCommand command = new SqlCommand("Update EngineerTable Set name ='" + item.Name + "', surname = '" + item.Surname +
                "', birthdate = '" + ToSQLDate(item.Date_of_birth) + "', wage = '" + item.Wage +
                "', Employed_since = '" + ToSQLDate(item.Employed_since) + "' where engineer_id = " + item.ID + ";", con);
            return ExecuteCommand(command);
        }

        public static string DeleteRecord(EngineerItem item)
        {
            return DeleteRecord(item.ID);
        }

        public static string DeleteRecord(int id)
        {
            EngineerItem item = new EngineerItem();
            string str = LoadRecord(id, out item);
            if (item == null)
                return str;
            SqlCommand command = new SqlCommand("Delete from EngineerTable where engineer_id = '" + id + "';", con);
            return ExecuteCommand(command, "Record " + id + " deleted.");
        }

        public static string ExecuteCommand(SqlCommand cmd)
        {
            try
            {
                int rows2 = cmd.ExecuteNonQuery();
                return "Rows affected: " + rows2;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string ExecuteCommand(SqlCommand cmd, string msg)
        {
            try
            {
                int rows2 = cmd.ExecuteNonQuery();
                return msg;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string ToSQLDate(DateTime date)
        {
            return date.Year + "-" + date.Month + "-" + date.Day;
        }

        public static DateTime ToDate(string sqldate)
        {
            int i1 = sqldate.IndexOf('-');
            int i2 = sqldate.LastIndexOf('-');
            string s = sqldate.Substring(0, i1);
            string s1 = sqldate.Substring(i1 + 1, i2 - i1 - 1);
            string s2 = sqldate.Substring(i2 + 1);
            return new DateTime(int.Parse(sqldate.Substring(0, i1)),
                int.Parse(sqldate.Substring(i1 + 1, i2 - i1 - 1)),
                int.Parse(sqldate.Substring(i2 + 1)));
        }

        static DateTime ToReaderDate(string sqldate)
        {
            string s = sqldate.Substring(0, sqldate.IndexOf(' '));
            int i1 = s.IndexOf('.');
            int i2 = s.LastIndexOf('.');
            return new DateTime(int.Parse(s.Substring(i2 + 1)), int.Parse(s.Substring(i1 + 1, i2 - i1 - 1)), int.Parse(s.Substring(0, i1)));
        }
    }

    public class EngineerItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Date_of_birth { get; set; }
        public int Wage { get; set; }
        public DateTime Employed_since { get; set; }

        public EngineerItem()
        {
        }

        public EngineerItem(string name, string surname, DateTime date_of_birth, int wage, DateTime employed_since)
        {
            this.Name = name;
            this.Surname = surname;
            this.Date_of_birth = date_of_birth;
            this.Wage = wage;
            this.Employed_since = employed_since;
        }

        public override string ToString()
        {
            string idstring = ID.ToString();
            if (idstring.Length < 2)
                idstring = "0" + idstring;
            string[] s = 
            { 
                idstring + ": " + Name, 
                " " + Surname + "\n", 
                Spaces(idstring.Length + 2) + "Date of birth: " + Date_of_birth.ToShortDateString() + " | ", 
                "Employed since: " + Employed_since.ToShortDateString() + " | ",
                "Wage: " + Wage.ToString()
            };
            string ret = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (ret.Length + s[i].Length - ret.LastIndexOf('\n') >= Console.BufferWidth)
                {
                    ret += "\n";
                }
                ret += s[i];
            }
            return ret;
        }

        static string Spaces(int count)
        {
            string s = "";
            for (int i = 0; i < count; i++)
            {
                s += " ";
            }
            return s;
        }
    }
}
