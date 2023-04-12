using System;
using MySqlConnector;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;
using Redage.SDK.Models;


namespace Redage.SDK
{
    /// <summary>
    /// 
    /// </summary>
    public static class MySQL
    {
        private static MysqlSettings config = Settings.ReadAsync("mainDB", new MysqlSettings());
        private static nLog Log = new nLog("MySQL");

        private static string Connection = null;
        private static string LogsConnection = null;
        /// <summary>
        /// 
        /// </summary>
        public static string LogDB = config.DataBase + "logs";
        /// <summary>
        /// 
        /// </summary>
        public static bool Debug = false;

        //private static StreamWriter qlog = new StreamWriter("QueryLog.txt", true, Encoding.UTF8);
        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            if (Connection is string) return;
            Connection =
                $"Host={config.Server};" +
                $"User={config.User};" +
                $"Password={config.Password};" +
                $"Database={config.DataBase};" +
                $"SslMode=None;";

            if (LogsConnection is string) return;
            LogsConnection =
                $"Host={config.Server};" +
                $"User={config.User};" +
                $"Password={config.Password};" +
                $"Database={config.DataBase}logs;" +
                $"SslMode=None;";
        }

        /// <summary>
        /// Тест соединения с базой
        /// </summary>
        /// <returns>True - если все хорошо</returns>
        public static bool Test()
        {
            Log.Debug("Testing connection...");
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Connection))
                {
                    conn.Open();
                    Log.Debug("Connection is successful!", nLog.Type.Success);
                    conn.Close();
                }
                return true;
            }
            catch (ArgumentException ae)
            {
                Log.Write($"Сonnection string contains an error\n{ae.ToString()}", nLog.Type.Error);
                return false;
            }
            catch (MySqlException me)
            {
                switch (me.Number)
                {
                    case 1042:
                        Log.Write("Unable to connect to any of the specified MySQL hosts", nLog.Type.Error);
                        break;
                    case 0:
                        Log.Write("Access denied", nLog.Type.Error);
                        break;
                    default:
                        Log.Write($"({me.Number}) {me.Message}", nLog.Type.Error);
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Выполнить запрос без ответа
        /// </summary>
        /// <param name="command">Передаем заранее составленную команду</param>
        public static void Query(MySqlCommand command)
        {
            try
            {
                if (command.CommandText.Length < 1) Log.Write($"BAD Query?: '{command.CommandText}'", nLog.Type.Error);
                else
                {
                    if (Debug) Log.Debug("Query to DB:\n" + command.CommandText);
                    //qlog.Write($"{DateTime.Now} | Query: {command.CommandText}\n");
                    using (MySqlConnection connection = new MySqlConnection(Connection))
                    {
                        connection.Open();

                        command.Connection = connection;

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Query({command.CommandText}) Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Выполнить запрос без ответа
        /// </summary>
        /// <param name="command">Передаем заранее составленную команду</param>
        public static async Task LogsQueryAsync(MySqlCommand command)
        {
            try
            {
                if (command.CommandText.Length < 1) Log.Write($"BAD LogsQueryAsync?: '{command.CommandText}'", nLog.Type.Error);
                else
                {
                    if (Debug) Log.Debug("Query to LogsDB:\n" + command.CommandText);
                    //qlog.Write($"{DateTime.Now} | Query: {command.CommandText}\n");
                    using (MySqlConnection connection = new MySqlConnection(LogsConnection))
                    {
                        await connection.OpenAsync();

                        command.Connection = connection;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"LogsQuery({command.CommandText}) Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Выполнить запрос без ответа
        /// </summary>
        /// <param name="command">Передаем команду в виде строки</param>
        public static async void LogsQuery(string command)
        {
            using (MySqlCommand cmd = new MySqlCommand(command))
            {
                await LogsQueryAsync(cmd);
            }
        }

        /// <summary>
        /// Выполнить запрос без ответа
        /// </summary>
        /// <param name="command">Передаем команду в виде строки</param>
        public static void Query(string command)
        {
            using (MySqlCommand cmd = new MySqlCommand(command))
            {
                Query(cmd);
            }
        }


        /// <summary>
        /// Выполнить запрос без ответа
        /// </summary>
        /// <param name="command">Передаем заранее составленную команду</param>
        public static async Task QueryAsync(MySqlCommand command)
        {
            try
            {
                if (command.CommandText.Length < 1) Log.Write($"BAD QueryAsync?: '{command.CommandText}'", nLog.Type.Error);
                else
                {
                    if (Debug) Log.Debug("Query to DB:\n" + command.CommandText);
                    //qlog.Write($"{DateTime.Now} | Query: {command.CommandText}\n");
                    using (MySqlConnection connection = new MySqlConnection(Connection))
                    {
                        await connection.OpenAsync();

                        command.Connection = connection;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"QueryAsync({command.CommandText}) #1 Exception: {e.ToString()}");
            }
        }


        /// <summary>
        /// Отправить запрос и считать ответ
        /// </summary>
        /// <param name="command">Передаем заранее составленную команду</param>
        /// <returns>Ответ базы данных в формате таблицы</returns>
        public static DataTable QueryRead(MySqlCommand command)
        {
            try
            {
                if (command.CommandText.Length < 1)
                {
                    Log.Write($"BAD QueryRead?: '{command.CommandText}'", nLog.Type.Error);
                    return null;
                }
                else
                {
                    if (Debug) Log.Debug("Query to DB:\n" + command.CommandText);
                    //qlog.Write($"{DateTime.Now} | Query: {command.CommandText}\n");
                    using (MySqlConnection connection = new MySqlConnection(Connection))
                    {
                        connection.Open();

                        command.Connection = connection;

                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            using (DataTable result = new DataTable())
                            {
                                result.Load(reader);

                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"QueryRead({command.CommandText}) Exception: {e.ToString()}");
                return null;
            }
        }


        /// <summary>
        /// Отправить запрос и считать ответ
        /// </summary>
        /// <param name="command">Передаем команду в виде строки</param>
        /// <returns>Ответ базы данных в формате таблицы</returns>
        public static DataTable QueryRead(string command)
        {
            using (MySqlCommand cmd = new MySqlCommand(command))
            {
                return QueryRead(cmd);
            }
        }


        /// <summary>
        /// Асинхронная версия Read
        /// </summary>
        /// <param name="command">Передаем заранее составленную команду</param>
        /// <returns>Ответ базы данных в формате таблицы</returns>
        public static async Task<DataTable> QueryReadAsync(MySqlCommand command)
        {
            try
            {
                if (command.CommandText.Length < 1)
                {
                    Log.Write($"BAD QueryReadAsync?: '{command.CommandText}'", nLog.Type.Error);
                    return null;
                }
                else
                {
                    if (Debug) Log.Debug("Query to DB:\n" + command.CommandText);
                    //qlog.Write($"{DateTime.Now} | Query: {command.CommandText}\n");
                    using (MySqlConnection connection = new MySqlConnection(Connection))
                    {
                        await connection.OpenAsync();

                        command.Connection = connection;

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            using (DataTable result = new DataTable())
                            {
                                result.Load(reader);

                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"QueryReadAsync({command.CommandText}) Exception: {e.ToString()}");
                return null;
            }
        }


        /// <summary>
        /// Асинхронная версия Read
        /// </summary>
        /// <param name="command">Передаем заранее составленную команду</param>
        /// <returns>Ответ базы данных в формате таблицы</returns>
        public static async Task<DataTable> QueryReadAsync(string command)
        {
            using (MySqlCommand cmd = new MySqlCommand(command))
            {
                return await QueryReadAsync(cmd);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int Insert(MySqlCommand command)
        {
            try
            {
                if (command.CommandText.Length < 1)
                {
                    Log.Write($"BAD QueryRead?: '{command.CommandText}'", nLog.Type.Error);
                    return 0;
                }
                else
                {
                    if (Debug) Log.Debug("Query to DB:\n" + command.CommandText);
                    //qlog.Write($"{DateTime.Now} | Query: {command.CommandText}\n");
                    using (MySqlConnection connection = new MySqlConnection(Connection))
                    {
                        connection.Open();

                        command.Connection = connection;

                        command.ExecuteReader();
                        //command.ExecuteNonQuery();

                        return Convert.ToInt32(command.LastInsertedId);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Insert({command.CommandText}) Exception: {e.ToString()}");
                return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ConvertTime(DateTime dateTime)
        {
            return dateTime.ToString("s");
        }
    }
}