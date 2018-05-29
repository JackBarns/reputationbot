using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Discord;
using MySql;
using MySql.Data.MySqlClient;

namespace new_bot.Modules
{
    public class sqlFunctionsHandler {
        public string Host;
        public string Port;
        public string Password;
        public string Username;
        public string Database;
        public string IntegratedSecurity;


        public static MySqlConnection connection = new MySqlConnection();

        public MySqlCommand command = connection.CreateCommand();
        public MySqlDataReader reader;
       
        public void Initialize() { connection.ConnectionString = $"Data Source={Host};Integrated Security={IntegratedSecurity};port={Port};username={Username};password={Password};database={Database};";  }

       public void testConnection()
        {
            try
            {
                connection.Open();
                Sql_logLine("Connection opened.");
                Sql_logLine($"ServerVersion: {connection.ServerVersion}");
                Sql_logLine($"State: {connection.State}");
            }
            catch (MySqlException)
            {
                throw;
            } finally {
                closeConnection(connection);
            }
        }

        public void closeConnection(MySqlConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public bool IsBotChannel(ulong inputId)
        {
            try
            {
                command.CommandText = $"SELECT COUNT(*) As NumOfRecords FROM `master_bot_channels` WHERE `bot_channels` = '{inputId}'";
                connection.Open();

                int resultNum = Convert.ToInt32(command.ExecuteScalar());
                if (resultNum > 0)
                {
                    return true;
                } else
                {
                    return false;
                }

            } catch (MySqlException)
            {
                throw;
            } finally
            {
                closeConnection(connection);
            }
        }

        public void deleteRep(ulong inputId, ulong reppedById)
        {
            try
            {
                command.CommandText = $"DELETE FROM `rep-bot`.`playreps` WHERE `playerId` = '{inputId}' AND `reppedById` = '{reppedById}'";
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                closeConnection(connection);
            }
        }

        public void wipeRep(ulong inputId)
        {
            try
            {
                command.CommandText = $"DELETE FROM `rep-bot`.`playreps` WHERE `playerId`='{inputId}'";
                connection.Open();
                command.ExecuteNonQuery();

            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                closeConnection(connection);
            }
        }

        public bool isAdmin(IGuildUser user)
        {
            ulong[] adminIds = new ulong[] { 400428838481297410, 446412008103870476 };
            for (int x = 0; x < adminIds.Length - 1; x++)
            {
                if (user.RoleIds.Contains<ulong>(adminIds[x])) { return true; }
            }
            return false;
        }

        public void giveRep(ulong inputId, ulong reppedById, string reason, string rep, string nameAndDesc)
        {
            try
            {
                command.CommandText = $"CALL `rep-bot`.`giveRep`({inputId}, {reppedById}, '{Uri.EscapeDataString(reason)}', '{rep}', '{nameAndDesc}');";
                connection.Open();
                command.ExecuteNonQuery();

            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                closeConnection(connection);
            }
        }

        public int totalReps(ulong playerId, string positiveOrNegative, bool anyRecords = false) // -rep = Negative, +rep = Positive
        {
            try
            {
                if (anyRecords == true)
                {
                    command.CommandText = $"SELECT COUNT(*) As NumRecs FROM `rep-bot`.playreps WHERE `playerId` = '{playerId}'";
                }
                else
                {
                    command.CommandText = $"SELECT COUNT(*) As NumRecs FROM `rep-bot`.playreps WHERE `playerId` = '{playerId}' AND `repGiven` = '{positiveOrNegative}'";
                }
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (MySqlException)
            {
                return 0;
                throw;
            }
            finally
            {
                closeConnection(connection);
            }
        }

        public List<CPlayerData> getRecentRep(ulong playerId)
        {
            List<CPlayerData> playerlist = new List<CPlayerData>();
            try
            {
                command.CommandText = $"SELECT `reppedById`, `reasonGiven`, `timeStamp`, `repGiven` FROM `rep-bot`.`playreps` WHERE `playerId` = '{playerId}' ORDER BY `primaryId` DESC LIMIT 4;";
                connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CPlayerData playerDataItem = new CPlayerData()
                    {
                        mPlayerId = Convert.ToUInt64(reader.GetValue(0)),
                        mReason = Uri.UnescapeDataString(Convert.ToString(reader.GetValue(1))),
                        mTimeStamp = Convert.ToDateTime(reader.GetValue(2)),
                        mpRepPoint = Convert.ToString(reader.GetValue(3))
                    };
                    playerlist.Add(playerDataItem);
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                connection.Close();
            }
            return playerlist;
        }

        public List<CPlayerData> getTopFivePlayers()
        {
            List<CPlayerData> playerlist = new List<CPlayerData>();
            try
            {
                command.CommandText = "SELECT playerId, COUNT(repGiven) As repTotal FROM `rep-bot`.`playreps` WHERE repGiven = '+rep' Group By playerId Order By repTotal Desc LIMIT 5;";
                connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CPlayerData playerDataItem = new CPlayerData()
                    {
                        mPlayerId = Convert.ToUInt64(reader.GetValue(0)),
                        mpRepPoint = Convert.ToString(reader.GetValue(1))
                    };
                    playerlist.Add(playerDataItem);
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                connection.Close();
            }
            return playerlist;
        }

        private void discord_logLine(string input) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} Discord     {input}");
            Console.ForegroundColor = currentColor;
        }

        public void IoSql_logLine(string input)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} SQL-IO\t     {input}");
            Console.ForegroundColor = currentColor;
        }

        private void Sql_logLine(string input) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} SQL\t     {input}");
            Console.ForegroundColor = currentColor;
        }
    }
    public class CPlayerData
    {
        public ulong mPlayerId;
        public string mReason;
        public DateTime mTimeStamp;
        public string mpRepPoint;
    }
}
