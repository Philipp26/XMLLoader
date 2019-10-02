using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;
using System.Data;

namespace XMLLoader
{
    class Program
    {
        public static void ShowConnectionInfo(SqlConnection connection)
        {
            connection.Open();
            Console.WriteLine("Свойства подключения:");
            Console.WriteLine("\tСтрока подключения: {0}", connection.ConnectionString);
            Console.WriteLine("\tБаза данных: {0}", connection.Database);
            Console.WriteLine("\tСервер: {0}", connection.DataSource);
            Console.WriteLine("\tВерсия сервера: {0}", connection.ServerVersion);
            Console.WriteLine("\tСостояние: {0}", connection.State);
            Console.WriteLine("\tWorkstationld: {0}", connection.WorkstationId);
            connection.Close();
        }

        public static void UploadXMLPolice(string sqlQuery, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();                    

                    var command = new SqlCommand(sqlQuery, connection);
                                        
                    var xmlReader = command.ExecuteXmlReader();

                    var y = xmlReader.NameTable;
                    var doc = XDocument.Load(xmlReader);

                    var t = doc.LastNode;

                    string path = @"Initial." + DateTime.Now.ToString("yyyy-MM-dd") + ".xml";

                    using (var writer = new StreamWriter(path))
                    {
                        doc.Save(writer);
                    }
                }

                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            var connection = new SqlConnection(connectionString);

            //ShowConnectionInfo(connection);

            Console.WriteLine("Введите ID");

            var databaseLogID = 3;

            var sqlQuery = String.Format(
                @"SELECT XmlEvent FROM AdventureWorksDW2012.dbo.DatabaseLog " +
                @"WHERE DatabaseLogID={0}", 
                databaseLogID);

            UploadXMLPolice(sqlQuery, connectionString);

            Console.WriteLine("Подключение закрыто...");
            Console.ReadKey();
        }
    }
}
