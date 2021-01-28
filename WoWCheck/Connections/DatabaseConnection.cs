using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace WoWCheck
{
    class DatabaseConnection
    {
        //Constructor
        public DatabaseConnection()
        {
            Initialize();
        }
        private MySqlConnection connection;

#if DEBUG
        private readonly string sql = @"Connections/sql_test.tkey";
#else
        private readonly string sql = @"sql.tkey";
#endif


        //Initialize values
        private void Initialize()
        {
            string connectionString;
            using (StreamReader sr = new StreamReader(sql))
               connectionString = sr.ReadToEnd();
            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
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
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Выполняет INSERT INTO
        /// </summary>
        /// <param name="query">Полностью подготовленный запрос</param>
        public void Insert(string query)
        {
            //string query = "INSERT INTO wowcheck.translations (`default_name`, `ru`) VALUES('Halls of Atonement', 'Чертоги покаяния')";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        /// <summary>
        /// Выполняет UPDATE
        /// </summary>
        /// <param name="query">Полностью подготовленный запрос</param>
        public void Update(string query)
        {

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        /// <summary>
        /// Выполняет DELETE FROM
        /// </summary>
        /// <param name="query">Полностью подготовленный запрос</param>
        public void Delete(string query)
        {
           //"DELETE FROM tableinfo WHERE name='John Smith'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
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
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Чтение полученной информации
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
            else
            {
                return dictionaries;
            }
        }

        ////Count statement
        //public int Count()
        //{
        //}

    }
}
