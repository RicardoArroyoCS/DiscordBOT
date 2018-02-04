﻿using Discord;
using DiscordBOT.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DiscordBOT.DataAccess
{
    public static class ReputationDataAccess
    {
        private const int _availableRep = 50;
        private static UserRecordExistenceSet _userRecordExistenceSet = null;
        private static DateTime _defaultRepAccumulationTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 1, DateTime.MinValue.Minute, DateTime.MinValue.Second);

        #region Properties
        private static string ConnectionString
        {
            get
            {
                return BotConfiguration.DatabaseConnectionString;
            }
        }

        private static UserRecordExistenceSet UserRecordExistenceSet
        {
            get
            {
                if (_userRecordExistenceSet == null)
                {
                    _userRecordExistenceSet = new UserRecordExistenceSet();
                }

                return _userRecordExistenceSet;
            }
        }
        #endregion

        #region Get Data Access Methods
        public static string GetUserReputation(int userId)
        {
            string reputation = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "Select [TotalReputation] " +
                                    "FROM [ReputationBot].[dbo].[Reputation_View]" +
                                    "WHERE UserID = @userId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reputation = reader["TotalReputation"].ToString();

                                if(string.IsNullOrEmpty(reputation))
                                {
                                    // User has no rep history because it was just added to DB. Start REP at 0
                                    reputation = "0";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured getting user reputation: {ex.ToString()}");
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
            

            return reputation;
        }

        public static bool CheckUserRecordExists(int userId)
        {
            bool userExists = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "Select [UserID] " +
                                    "FROM [ReputationBot].[dbo].[User]" +
                                    "WHERE UserID = @userId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userExists = (int)reader["UserID"] > 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured getting user ID: {ex.ToString()}");
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

            return userExists;
        }

        public static int? GetUserReputationAvailability(int userId)
        {
            int? userReputation = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT [AvailableReputation] " +
                        "FROM [ReputationBot].[dbo].[User_Reputation_Settings] " +
                        "WHERE UserID = @userId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userReputation = (int)reader["AvailableReputation"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured getting user reputation: {ex.ToString()}");
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

            return userReputation;
        }
        #endregion

        #region Set Data Access Methods
        public static bool CreateNewUserRecord(int userId, string userName)
        {
            bool successfullyCreated = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO [User] (UserId, UserName)" +
                                    "VALUES(@userId, @userName)" +
                                    "INSERT INTO [User_Reputation_Settings] (UserID, RepAccumulationTime, AvailableReputation) " +
                                    "VALUES(@userId, @repAccumulationTime, @availableRep);";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@userName", userName);
                    command.Parameters.AddWithValue("@repAccumulationTime", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@availableRep", _availableRep);                    
                    try
                    {
                        connection.Open();
                        successfullyCreated = command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured creating user record: {ex.ToString()}");
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

            return successfullyCreated;
        }

        public static bool AddReputation(int toUser, int fromUser, int repValue)
        {
            bool successfullyAdded = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO [User_Reputation_History] " +
                                    "(UserID, AffectedUserID, ReputationValue, DateGiven) " +
                                    "VALUES (@fromUserId, @toUserId, @reputationValue, @date);" +
                                    "UPDATE [ReputationBot].[dbo].[User_Reputation_Settings] " +
                                    "SET AvailableReputation = (AvailableReputation - 1) " +
                                    "WHERE UserID = @fromUserId;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@toUserId", toUser);
                    command.Parameters.AddWithValue("@fromUserId", fromUser);
                    command.Parameters.AddWithValue("@reputationValue", repValue);
                    command.Parameters.AddWithValue("@date", DateTime.UtcNow);
                    try
                    {
                        connection.Open();
                        successfullyAdded = command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured creating user record: {ex.ToString()}");
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

        public static bool UpdateAvailableReputation(int userId)
        {
            bool successfullyExecuted = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("UpdateUserAvailableReputation", connection) { CommandType = System.Data.CommandType.StoredProcedure } )
                    {
                        try
                        {
                            connection.Open();
                            command.Parameters.Add(new SqlParameter("@userID", userId));
                            successfullyExecuted = command.ExecuteNonQuery() > 0;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error occured creating user record: {ex.ToString()}");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error occured connecting to SQL: {ex.ToString()}");
            }
            return successfullyExecuted;
        }
        #endregion

        #region Helpers        
        public static bool CheckOrCreateUserRecord(IUser user)
        {
            bool isSuccess = false;
            bool existsInCache = false;
            string userName = user.Username;
            int userId = user.DiscriminatorValue;

            if ((existsInCache = IsUserRecordExistCache(userId)) 
                    || CheckUserRecordExists(userId) 
                    || (CreateNewUserRecord(userId, userName))
                    )
            {
                isSuccess = true;
            }

            if(!existsInCache && isSuccess)
            {
                UserRecordExistenceSet.Add(userId);
            }

            return isSuccess;
        }

        private static bool IsUserRecordExistCache(int userId)
        {
            return UserRecordExistenceSet.Contains(userId);
        }

        public static bool IsUserAbleToRep(int userId)
        {
            int? userRepAvailable = GetUserReputationAvailability(userId);
            return userRepAvailable.HasValue ? userRepAvailable.Value > 0 : false;
        }
        #endregion
    }

}
