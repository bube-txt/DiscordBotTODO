using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBotLibrary
{
    public class DiscordBotCommands
    {
        SqlClass db = new SqlClass();

        // Собирает Help из полученных аргументов
        private string HelpConstructor(string command, string[] args, string description)
        {
            string arguments = "";
            foreach(string arg in args)
            {
                if (!String.IsNullOrEmpty(arg)) 
                {
                    arguments += $"[{arg}] ";
                }
            }
            return $"{command} {arguments}— {description}\n";
        }

        // Вернуть список команд
        public void Help(SocketMessage msg)
        {
            // HelpConstructor(string command, string[] args, string description)
            string commands =
            HelpConstructor("Prefix",       new string[] { "" },            "!") +
            HelpConstructor("uncompleted",  new string[] { "" },            "возвращает незавершенные задачи") +
            HelpConstructor("emergency",    new string[] { "" },            "возвращает срочные задачи") +
            HelpConstructor("info",         new string[] { "Name" },        "возвращает незавершенные задачи") +
            HelpConstructor("wip",          new string[] { "" },            "возвращает задачи в процессе") +
            HelpConstructor("complete",     new string[] { "TaskName" },    "завершить задачу") +
            HelpConstructor("drop",         new string[] { "TaskName" },    "бросить задачу") +
            HelpConstructor("take",         new string[] { "TaskName" },    "взять задачу");

            msg.Channel.SendMessageAsync(commands);
        }

        // Вернуть список незавершенных задач
        public void Uncompleted(SocketMessage msg)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM `Tasks` WHERE NOT `Completed` AND `Taken` IS NULL", db.getConnection());
            //command.Parameters.Add("@uMessage", MySqlDbType.VarChar).Value = message[1];

            db.openConnection();

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string taskName = reader["Name"] + "";
                string taskDisc = reader["Description"] + "";
                string taskAuthor = reader["Author"] + "";
                string taskTaken = reader["Taken"] + "";
                string taskEmergency = reader["Emergency"] + "";
                string taskCompleted = reader["Completed"] + "";

                string read =
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Автор: " + taskAuthor + "\n"
                    + "\n" + "\n";
                while (reader.Read())
                {
                    taskName = reader["Name"] + "";
                    taskDisc = reader["Description"] + "";
                    taskAuthor = reader["Author"] + "";
                    taskTaken = reader["Taken"] + "";
                    taskEmergency = reader["Emergency"] + "";
                    taskCompleted = reader["Completed"] + "";

                    read +=
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Автор: " + taskAuthor + "\n"
                    + "\n" + "\n";
                }
                msg.Channel.SendMessageAsync(read);
            }
            else
            {
                msg.Channel.SendMessageAsync("No data found!");
            }
            db.closeConnection();
        }

        // Вернуть список срочных задач
        public void Emergency(SocketMessage msg)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM `Tasks` WHERE `Emergency` AND `Completed` = 0", db.getConnection());
            //command.Parameters.Add("@uMessage", MySqlDbType.VarChar).Value = message[1];

            db.openConnection();

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string taskName = reader["Name"] + "";
                string taskDisc = reader["Description"] + "";
                string taskAuthor = reader["Author"] + "";
                string taskTaken = reader["Taken"] + "";
                string taskEmergency = reader["Emergency"] + "";
                string taskCompleted = reader["Completed"] + "";

                string read =
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Взял: " + (taskTaken.ToString() == "" ? "[Никто]" : taskTaken.ToString()) + "\n" +
                    "Автор: " + taskAuthor + "\n" +
                    "\n" + "\n";
                while (reader.Read())
                {
                    taskName = reader["Name"] + "";
                    taskDisc = reader["Description"] + "";
                    taskAuthor = reader["Author"] + "";
                    taskTaken = reader["Taken"] + "";
                    taskEmergency = reader["Emergency"] + "";
                    taskCompleted = reader["Completed"] + "";

                    read +=
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Взял: " + taskTaken + "\n" +
                    "Автор: " + taskAuthor + "\n" +
                    "\n" + "\n";
                }
                msg.Channel.SendMessageAsync(read);
            }
            else
            {
                msg.Channel.SendMessageAsync("No data found!");
            }
            db.closeConnection();
        }

        // Вернуть всю информацию о задаче
        public void Info(SocketMessage msg, string[] message)
        {
            if (message.Length < 2)
            {
                throw new Exception("Exception: Недостаточно аргументов!");
            }
            MySqlCommand command = new MySqlCommand($"SELECT * FROM `Tasks` WHERE `Name` = '{message[1]}'", db.getConnection());
            //command.Parameters.Add("@uMessage", MySqlDbType.VarChar).Value = message[1];

            db.openConnection();

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string taskName = reader["Name"] + "";
                string taskDisc = reader["Description"] + "";
                string taskAuthor = reader["Author"] + "";
                string taskTaken = reader["Taken"] + "";
                string taskEmergency = reader["Emergency"] + "";
                string taskCompleted = reader["Completed"] + "";

                string read =
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Взял: " + (taskTaken.ToString() == "" ? "[Никто]" : taskTaken.ToString()) + "\n" +
                    "Автор: " + taskAuthor + "\n" +
                    "\n" + "\n";
                while (reader.Read())
                {
                    taskName = reader["Name"] + "";
                    taskDisc = reader["Description"] + "";
                    taskAuthor = reader["Author"] + "";
                    taskTaken = reader["Taken"] + "";
                    taskEmergency = reader["Emergency"] + "";
                    taskCompleted = reader["Completed"] + "";

                    read +=
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Взял: " + taskTaken + "\n" +
                    "Автор: " + taskAuthor + "\n" +
                    "\n" + "\n";
                }
                msg.Channel.SendMessageAsync(read);
            }
            else
            {
                msg.Channel.SendMessageAsync("No data found!");
            }
            db.closeConnection();
        }

        // Вернуть задачи в процессе
        public void WIP(SocketMessage msg)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM `Tasks` WHERE `Taken` IS NOT NULL AND `Completed` = 0", db.getConnection());
            //command.Parameters.Add("@uMessage", MySqlDbType.VarChar).Value = message[1];

            db.openConnection();

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string taskName = reader["Name"] + "";
                string taskDisc = reader["Description"] + "";
                string taskAuthor = reader["Author"] + "";
                string taskTaken = reader["Taken"] + "";
                string taskEmergency = reader["Emergency"] + "";
                string taskCompleted = reader["Completed"] + "";

                string read =
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Взял: " + taskTaken + "\n" +
                    "Автор: " + taskAuthor + "\n" +
                    "\n" + "\n";
                while (reader.Read())
                {
                    taskName = reader["Name"] + "";
                    taskDisc = reader["Description"] + "";
                    taskAuthor = reader["Author"] + "";
                    taskTaken = reader["Taken"] + "";
                    taskEmergency = reader["Emergency"] + "";
                    taskCompleted = reader["Completed"] + "";

                    read +=
                    "Заголовок: " + taskName + "\n" +
                    "Описание: " + taskDisc + "\n" +
                    "Статус: " + (taskCompleted == "True" ? ":white_check_mark:" : (taskEmergency == "True" ? ":fire:" : taskTaken != "" ? ":tools:" : ":x:")) + "\n" +
                    "Взял: " + taskTaken + "\n" +
                    "Автор: " + taskAuthor + "\n" +
                    "\n" + "\n";
                }
                msg.Channel.SendMessageAsync(read);
            }
            else
            {
                msg.Channel.SendMessageAsync("No data found!");
            }
            db.closeConnection();
        }

        // Завершить задачу
        public void Complete(SocketMessage msg, string[] message)
        {
            try
            {
                if (message.Length < 2)
                {
                    throw new Exception("Exception: Недостаточно аргументов!");
                }
                MySqlCommand command = new MySqlCommand("SELECT `Completed`, `Taken` FROM `tasks` WHERE `Name` = @TaskName", db.getConnection());
                db.openConnection();
                command.Parameters.AddWithValue("@TaskName", message[1]);

                MySqlDataReader reader;
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (reader["Taken"].ToString() != msg.Author.Username)
                    {
                        throw new Exception("Exception: Это не ваша задача");
                    }
                    if(reader["Completed"].ToString() == "True")
                    { 
                        throw new Exception("Exception: Эта задача уже завершена!"); 
                    }

                }
                db.closeConnection();



                command = new MySqlCommand("UPDATE `tasks` SET `Completed` = 1 WHERE `Name` = @TaskName", db.getConnection());

                db.openConnection();

                // command.Parameters.AddWithValue("@TaskName", message[1]); Дубликат
                command.ExecuteNonQuery();
                msg.Channel.SendMessageAsync("Task completed successfully!");

                db.closeConnection();
            }
            catch(Exception ex)
            {
                msg.Channel.SendMessageAsync(ex.Message);
            }
        }
        
        // Взять задачу
        public void Take(SocketMessage msg, string[] message)
        {
            try
            {
                if (message.Length < 2)
                {
                    throw new Exception("Exception: Недостаточно аргументов!");
                }
                MySqlCommand command = new MySqlCommand("SELECT `Completed`, `TakenID` FROM `tasks` WHERE `Name` = @TaskName", db.getConnection());
                db.openConnection();
                command.Parameters.AddWithValue("@TaskName", message[1]);

                MySqlDataReader reader;
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (!String.IsNullOrEmpty(reader["TakenID"].ToString()))
                    {
                        throw new Exception("Exception: Это задача уже занята");
                    }

                }
                db.closeConnection();



                command = new MySqlCommand("UPDATE `tasks` SET `Taken` = @TakenName, `TakenID` = @TakenID, `TakenDate` = @TakenDate WHERE `Name` = @TaskName", db.getConnection());

                db.openConnection();

                command.Parameters.AddWithValue("@TaskName", message[1]);
                command.Parameters.AddWithValue("@TakenName", msg.Author.Username);
                command.Parameters.AddWithValue("@TakenID", msg.Author.Id);
                command.Parameters.AddWithValue("@TakenDate", DateTime.Now.ToString("yyyy.MM.dd - HH:mm:ss"));
                command.ExecuteNonQuery();
                msg.Channel.SendMessageAsync("The task has been successfully taken! :ballot_box_with_check:");

                db.closeConnection();
            }
            catch (Exception ex)
            {
                msg.Channel.SendMessageAsync(ex.Message);
            }
        }

        // Бросить задачу
        public void Drop(SocketMessage msg, string[] message)
        {
            try
            {
                if (message.Length < 2)
                {
                    throw new Exception("Exception: Недостаточно аргументов!");
                }
                MySqlCommand command = new MySqlCommand("SELECT `Completed`, `TakenID` FROM `tasks` WHERE `Name` = @TaskName", db.getConnection());
                db.openConnection();
                command.Parameters.AddWithValue("@TaskName", message[1]);

                MySqlDataReader reader;
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (Convert.ToUInt64(reader["TakenID"]) != msg.Author.Id)
                    {
                        throw new Exception("Exception: Вы не брали эту задачу.");
                    }
                    if (reader["Completed"].ToString() == "True")
                    {
                        throw new Exception("Exception: Задача уже завершена.");
                    }

                }
                db.closeConnection();



                command = new MySqlCommand("UPDATE `tasks` SET `Taken` = NULL, `TakenID` = NULL, `TakenDate` = NULL WHERE `Name` = @TaskName", db.getConnection());

                db.openConnection();

                command.Parameters.AddWithValue("@TaskName", message[1]);
                command.ExecuteNonQuery();
                msg.Channel.SendMessageAsync("The task has been successfully dropped! :ballot_box_with_check:");

                db.closeConnection();
            }
            catch (Exception ex)
            {
                msg.Channel.SendMessageAsync(ex.Message);
            }
        }
        
        // Добавить задачу
        public void Add(SocketMessage msg, string[] message)
        {
            try
            {
                if (message.Length < 3)
                {
                    throw new Exception("Exception: Недостаточно аргументов!");
                }


                MySqlCommand command = new MySqlCommand("SELECT * FROM `tasks` WHERE `Name` = @TaskName AND `Completed` = 0", db.getConnection());

                db.openConnection();

                command.Parameters.AddWithValue("@TaskName", message[1]);

                MySqlDataReader reader;
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    throw new Exception("Exception: Не завершенная задача с таким именем уже существует!");
                }

                db.closeConnection();

                command = new MySqlCommand(
                    "INSERT INTO tasks(`Name`, `Description`, `Author`, `AuthorDate`, `AuthorID`, `Taken`, `TakenDate`, `TakenID`, `Emergency`, `Completed`)" +
                    "VALUES (@TaskName, @TaskDescription, @AuthorName, @AuthorDate, @AuthorID, NULL, NULL, NULL, @Emergency, 0)", db.getConnection());

                db.openConnection();

                // Добавляем информацию о задаче
                command.Parameters.AddWithValue("@TaskName", message[1]);
                command.Parameters.AddWithValue("@TaskDescription", message[2]);
                command.Parameters.AddWithValue("@Emergency", message[3]);

                // Добавляем информацию об авторе задачи
                command.Parameters.AddWithValue("@AuthorName", msg.Author.Username);
                command.Parameters.AddWithValue("@AuthorDate", DateTime.Now.ToString("yyyy.MM.dd - HH:mm:ss"));
                command.Parameters.AddWithValue("@AuthorID", msg.Author.Id);

                command.ExecuteNonQuery();
                msg.Channel.SendMessageAsync("The task has been successfully created! :ballot_box_with_check:");

                db.closeConnection();
            }
            catch (Exception ex)
            {
                msg.Channel.SendMessageAsync(ex.Message);
            }
        }

        // Удалить задачу
        public void Del(SocketMessage msg, string[] message)
        {
            try
            {
                if (message.Length < 2)
                {
                    throw new Exception("Exception: Недостаточно аргументов!");
                }


                MySqlCommand command = new MySqlCommand("SELECT `AuthorID` FROM `tasks` WHERE `Name` = @TaskName AND `Completed` = 0", db.getConnection());

                db.openConnection();

                command.Parameters.AddWithValue("@TaskName", message[1]);

                MySqlDataReader reader;
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (Convert.ToUInt64(reader["AuthorID"]) != msg.Author.Id)
                    {
                        throw new Exception("Exception: Вы не автор этой задачи!");
                    }
                }

                db.closeConnection();

                command = new MySqlCommand("DELETE FROM `tasks` WHERE `Name` = @TaskName", db.getConnection());

                db.openConnection();

                // Добавляем информацию о задаче
                command.Parameters.AddWithValue("@TaskName", message[1]);

                command.ExecuteNonQuery();
                msg.Channel.SendMessageAsync("The task has been successfully deleted! :ballot_box_with_check:");

                db.closeConnection();
            }
            catch (Exception ex)
            {
                msg.Channel.SendMessageAsync(ex.Message);
            }
        }
    }
}
