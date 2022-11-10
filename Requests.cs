using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleWEBD
{
    public class Requests
    {
        private static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);

        #region Return
        public static string Return(string sql)
        {
            connection.Open();
            string str;
            SqlCommand command = new SqlCommand(sql, connection);

            if (command.ExecuteScalar() != null)
            {
                str = command.ExecuteScalar().ToString();
                connection.Close();
                return str;
            }
            else
            {
                str = "";
                connection.Close();
                return str;
            }
        }

        public static string ReturnForTask(string sql)
        {
            string str = "";
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);

                str = command.ExecuteScalar().ToString();

            }
            catch
            {

            }
            return str;
        }

        public static void ReturnMyTasks(long TID, long owner)
        {
            connection.Open();

            List<string> tasks = new List<string>() { };

            string sql = $"SELECT Hash FROM Task WHERE Owner = {owner} AND Doer = {TID}";

            SqlCommand command = new SqlCommand(sql, connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(reader.GetString(0));
            }

            reader.Close();

            connection.Close();

            SortTasks(tasks, TID);
        }

        #endregion

        #region Update
        public static void Update(string sql)
        {
            connection.Open();

            SqlCommand command = new SqlCommand(sql, connection);

            command.ExecuteScalar();

            connection.Close();
        }
        #endregion

        #region AddItems
        public static string AddTask(string title, string body, string company, long owner, long doer, int rating, int status)
        {
            connection.Open();

            string date = DateTime.Now.ToString("d");

            string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(8);

            SqlCommand command = new SqlCommand();

            command.CommandText = $"INSERT INTO Task(Hash, Title, Body, Company, Owner, Doer, Rating, Status, Date) VALUES('{hash}', '{title}', '{body}', '{company}', '{owner}', '{doer}', '{rating}', {status}, '{date}')";
            command.Connection = connection;
            command.ExecuteScalar();
            connection.Close();
            Update($"UPDATE Users SET Tasks = Tasks + '{hash + ";"}' WHERE TID = {owner}");
            return hash;
        }
        public static void AddUser(long TID, string fname, string lname, string tasks, string company, string rang, int ended)
        {
            connection.Open();

            string date = DateTime.Now.ToString("d");

            string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(8);

            SqlCommand command = new SqlCommand();

            command.CommandText = $"INSERT INTO Users(Hash, TID, FName, LName, Tasks, Company, Rang, Ended, Date) VALUES('{hash}', {TID}, '{fname}', '{lname}', '{tasks}', '{company}', '{rang}', {ended}, '{date}')";

            command.Connection = connection;

            command.ExecuteNonQueryAsync();

            connection.Close();
        }
        //public static void AddCompany(string name, string tasks, string partners, int ended, string rsoy, string rroo, string rsta)
        //{
        //    connection.Open();

        //    string date = DateTime.Now.ToString("d");
        //    string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(8);

        //    SqlCommand command = new SqlCommand();

        //    command.CommandText = $"INSERT INTO Registry(Hash, Name, Tasks, Partners, Ended, Date) VALUES('{hash}', '{name}', '{tasks}', '{partners}', {ended}, '{date}')";

        //    command.Connection = connection;

        //    command.ExecuteNonQueryAsync();

        //    connection.Close();

        //}

        public static void AddCompany(string hash, string name, string tasks, string partners, int ended)
        {
            connection.Open();

            string date = DateTime.Now.ToString("d");

            SqlCommand command = new SqlCommand();

            command.CommandText = $"INSERT INTO Company(Hash, Name, Tasks, Partners, Ended, Date) VALUES('{hash}', '{name}', '{tasks}', '{partners}', {ended}, '{date}')";
            
            command.Connection = connection;
            
            command.ExecuteScalar();
            
            connection.Close();
        }
        #endregion

        #region Checking

        public static bool Check(string waht, long TID)
        {
            try
            {
                string sql = $"SELECT {waht} FROM Users WHERE TID = {TID}";
                string str = "";
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                if (command.ExecuteScalar() != null)
                {
                    str = command.ExecuteScalar().ToString();
                }
                else
                {
                    connection.Close();
                    return false;
                }

                connection.Close();

                if (str == "0")
                {
                    return false;
                }

                return true;
            }
            catch
            {
                connection.Close();
                return false;
            }
        }
        #endregion

        #region Backend
        private static void SortTasks(List<string> tasks, long TID)
        {
            for (int i = 0; i <= tasks.Count - 1; i++)
            {
                InlineKeyboardMarkup InlineKeyboardTask = new[] { new[] { InlineKeyboardButton.WithCallbackData(text: "Выполнено", callbackData: $"{tasks[i]}"), InlineKeyboardButton.WithCallbackData(text: "Коментарий", callbackData: $"{tasks[i]}1") } };
                TelegramDriver.bot.SendTextMessageAsync(TID, CreateTasks(tasks[i]), replyMarkup: InlineKeyboardTask, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
        private static string CreateTasks(string hash)
        {
            string endline, sqlTitle = $"SELECT Title FROM Task WHERE Hash = '{hash}'";
            string title = Return(sqlTitle);

            if (title.Length > 25)
            {
                endline = "";

                int i = 25;

                while (title != "")
                {
                    string buf = "";

                    if (title.Length > i)
                    {
                        buf = title.Remove(i);
                    }
                    else
                    {
                        buf = title;
                    }

                    endline = endline + buf + "\n";
                    
                    title = title.Replace(buf, "");
                }
            }
            else
            {
                endline = title;
            }

            //невидимый символ "ㅤ"

            string sqlRating = $"SELECT Rating FROM Task WHERE Hash = '{hash}' AND Status = '0'";

            string mes = "";

            //string blankline = "ㅤㅤㅤㅤㅤㅤㅤㅤㅤㅤㅤㅤㅤ"; 

            if (Return(sqlRating) == "1")
            {
                mes = $"ㅤ ㅤㅤㅤ ㅤЗАДАЧА ⭐️ㅤ ㅤ ㅤ ㅤ\n\n" +
                      $"Описание:\n● <b>{endline}</b>\n";
            }
            if (Return(sqlRating) == "2")
            {
                mes = $"ㅤㅤㅤ ㅤㅤЗАДАЧА ⭐️ ⭐️ㅤㅤㅤㅤㅤ\n\n" +
                      $"Описание:\n● <b>{endline}</b>\n";
            }
            if (Return(sqlRating) == "3")
            {
                mes = $"ㅤㅤㅤㅤЗАДАЧА ⭐️ ⭐️ ⭐️ㅤ   ㅤ\n\n" +
                      $"Описание:\n● <b>{endline}</b>\n";
            }

            return mes;
        }
        public static void CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }
        public static void MyPartners(string hash, long TID)
        {
            List<string> partners = new List<string>();
            string hashs = Return($"SELECT Partners FROM Company WHERE Hash = '{hash}'");
            while (hashs != "")
            {
                string buf = Regex.Match(hashs, "[0-9]*;").Value;
                hashs = hashs.Replace(buf, "");
                buf = buf.Replace(";", "");
                partners.Add(buf);
            }
            SortPartners(partners, TID);
        }
        private static void SortPartners(List<string> strings, long TID)
        {
            for (int i = 0; i <= strings.Count - 1; i++)
            {
                InlineKeyboardMarkup InlineKeyboardTask = new[] { new[] { InlineKeyboardButton.WithCallbackData(text: "Дать задачу", callbackData: $"p{strings[i]}") } };
                TelegramDriver.bot.SendTextMessageAsync(TID, CreatePartners(strings[i]), replyMarkup: InlineKeyboardTask, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
        public static string CreatePartners(string ID)
        {
            string mes = "";
            mes = $"{Return($"SELECT FName FROM Users WHERE TID = '{ID}'")}";
            return mes;
        }
        public static void MyCompany(string hash, long TID)
        {
            List<string> partners = new List<string>();
            string hashs = Return($"SELECT Company FROM Users WHERE Hash = '{hash}'");
            while (hashs != "")
            {
                string buf = Regex.Match(hashs, "[A-z0-9]*;").Value;
                hashs = hashs.Replace(buf, "");
                buf = buf.Replace(";", "");
                partners.Add(buf);
            }
            SortCompany(partners, TID);
        }
        private static void SortCompany(List<string> strings, long TID)
        {
            for (int i = 0; i <= strings.Count - 1; i++)
            {
                InlineKeyboardMarkup InlineKeyboardTask = new[] { new[] { InlineKeyboardButton.WithCallbackData(text: "Просмтореть", callbackData: $"v{strings[i]}") } };
                TelegramDriver.bot.SendTextMessageAsync(TID, CreateCompany(strings[i]), replyMarkup: InlineKeyboardTask, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
        public static string CreateCompany(string hash)
        {
            string mes = "";
            mes =
                $"ㅤㅤㅤㅤ ㅤКОМПАНИЯ ㅤ ㅤㅤㅤㅤ \n\n" +
                $"Название: <b>{Return($"SELECT Name FROM Company WHERE Hash = '{hash}'")}</b>" +
                $"Моих задач: ";
            return mes;
        }
        #endregion

        #region  Test
        public static TaskWeBd TakeTask(string hash)
        {
            TaskWeBd taskWeBd = new TaskWeBd();

            string sql1 = $"SELECT Title FROM Task WHERE Hash = '{hash}'";
            string sql2 = $"SELECT Body FROM Task WHERE Hash = '{hash}'";
            string sql3 = $"SELECT Company FROM Task WHERE Hash = '{hash}'";
            string sql4 = $"SELECT Owner FROM Task WHERE Hash = '{hash}'";
            string sql5 = $"SELECT Doer FROM Task WHERE Hash = '{hash}'";
            string sql6 = $"SELECT Rating FROM Task WHERE Hash = '{hash}'";
            string sql7 = $"SELECT Status FROM Task WHERE Hash = '{hash}'";
            string sql8 = $"SELECT Date FROM Task WHERE Hash = '{hash}'";

            connection.Open();
            taskWeBd.Title = ReturnForTask(sql1);
            taskWeBd.Body = ReturnForTask(sql2);
            taskWeBd.Company = ReturnForTask(sql3);
            taskWeBd.Owner = ReturnForTask(sql4);
            taskWeBd.Doer = ReturnForTask(sql5);
            taskWeBd.Rating = Convert.ToInt32(ReturnForTask(sql6));
            taskWeBd.Status = Convert.ToInt32(ReturnForTask(sql7));
            taskWeBd.Date = ReturnForTask(sql8);
            connection.Close();

            return taskWeBd;
        }
        #endregion
    }
}