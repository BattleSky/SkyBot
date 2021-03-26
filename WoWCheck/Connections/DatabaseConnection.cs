using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace WoWCheck.Connections
{
    class DatabaseConnection
    {
        //Constructor
        public DatabaseConnection()
        {
            Initialize();
        }
        public MySqlConnection Connection { get; private set; }

#if DEBUG
        private const string Sql = @"Connections/sql_test.tkey";
#else
        private const string Sql = @"Connections/sql.tkey";
#endif


        //Initialize values
        private void Initialize()
        {
            string connectionString;
            using (var sr = new StreamReader(Sql))
               connectionString = sr.ReadToEnd();
            Connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        public void CloseConnection()
        {
            try
            {
                Connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        /// <summary>
        /// Выполняет INSERT INTO
        /// </summary>
        /// <param name="query">Полностью подготовленный запрос</param>
        public void Insert(string query)
        {
            //open connection
            if (this.OpenConnection() != true) return;
            //create command and assign the query and connection from the constructor
            var cmd = new MySqlCommand(query, Connection);

            //Execute command
            cmd.ExecuteNonQuery();

            //close connection
            this.CloseConnection();
        }

        /// <summary>
        /// Выполняет UPDATE
        /// </summary>
        /// <param name="query">Полностью подготовленный запрос</param>
        public void Update(string query)
        {

            //Open connection
            if (this.OpenConnection() != true) return;
            //create mysql command
            var cmd = new MySqlCommand {CommandText = query, Connection = Connection};

            //Execute query
            cmd.ExecuteNonQuery();

            //close connection
            this.CloseConnection();
        }

        /// <summary>
        /// Выполняет DELETE FROM
        /// </summary>
        /// <param name="query">Полностью подготовленный запрос</param>
        public void Delete(string query)
        {
            if (this.OpenConnection() != true) return;
            var cmd = new MySqlCommand(query, Connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }

        /// <summary>
        /// Запрос SELECT FROM WHERE
        /// </summary>
        /// <param name="tableName">Имя таблицы, требуется обрамить в ``</param>
        /// <param name="whereCondition">Полный запрос, содержащий WHERE, "WHERE id = 1". null, если WHERE отсутствует</param>
        /// <param name="columns">Список получаемых столбцов</param>
        /// <returns></returns>
        public Dictionary<string, List<string>> Select(string tableName, string whereCondition, params string[] columns)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT ");
            var dictionaries = new Dictionary<string, List<string>>();
            //Создаем поля в словаре и формируем запрос
            for (var i = 0; i < columns.Length; i++)
            {
                dictionaries.Add(columns[i], new List<string>());

                queryBuilder.Append("`" + columns[i] + "`");
                if (i + 1 != columns.Length)
                    queryBuilder.Append(", ");
            }
            queryBuilder.Append(" FROM " + tableName);

            if (whereCondition != null)
                queryBuilder.Append(" " + whereCondition + ";");

            var query = queryBuilder.ToString();

            //Open connection
            if (this.OpenConnection() != true) return dictionaries;
            //Create Command
            var cmd = new MySqlCommand(query, Connection);
            //Create a data reader and Execute the command
            var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                foreach (var t in columns)
                {
                    dictionaries[t].Add(dataReader[t] + "");
                }
            }
            //close Data Reader
            dataReader.Close();
            //close Connection
            this.CloseConnection();
            //return dictionary to be displayed
            return dictionaries;

        }
        /// <summary>
        /// Запрос SELECT с полным указанием кода запроса
        /// </summary>
        /// <param name="straightQuery">Прямой полный запрос, требует форматирования заранее.</param>
        /// <param name="columns">Список получаемых столбцов (копия после ключевого слова "SELECT")</param>
        /// <returns></returns>
        public Dictionary<string, List<string>> SelectWithQuery(string straightQuery, params string[] columns)
        {
            var dictionaries = new Dictionary<string, List<string>>();

            //Open connection
            if (this.OpenConnection() != true) return dictionaries;
            //Create Command
            var cmd = new MySqlCommand(straightQuery, Connection);
            //Create a data reader and Execute the command
            var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                foreach (var t in columns)
                {
                    dictionaries[t].Add(dataReader[t] + "");
                }
            }
            //close Data Reader
            dataReader.Close();
            //close Connection
            this.CloseConnection();
            //return dictionary to be displayed
            return dictionaries;

        }

    }
}
