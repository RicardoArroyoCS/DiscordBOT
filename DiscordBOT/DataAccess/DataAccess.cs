using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DiscordBOT.DataAccess
{
    public class DataAccess
    {
        protected static string ConnectionString
        {
            get
            {
                return BotConfiguration.DatabaseConnectionString;
            }
        }

        protected bool PerformSqlInsertOrUpdate(Func<SqlConnection, SqlCommand> setupSqlConnection)
        {
            bool successfullyAdded = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = setupSqlConnection(connection);
                    try
                    {
                        connection.Open();
                        successfullyAdded = command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Error occured Performing SQL Insert/Update");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured connecting to SQL: {ex.ToString()}");
            }
            return successfullyAdded;
        }

        protected T PerformSqlSingleRead<T>(Func<SqlConnection, SqlCommand> setupSqlConnection, Func<SqlDataReader, T> readData)
            where T : class
        {
            T container = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = setupSqlConnection(connection);
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            container = readData(reader);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Error occured getting repboard");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured connecting to SQL: {ex.ToString()}");
            }

            return container;
        }
    }
}
