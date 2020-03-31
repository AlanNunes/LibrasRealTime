using PIC.Classes;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace PIC.DataBase
{
    public class SQLDataBase
    {
        private static SQLiteConnection SQLiteConnection;

        public static SQLiteConnection DBConnection()
        {
            string projectPath = System.IO.Directory.GetCurrentDirectory();
            SQLiteConnection = new SQLiteConnection(@"Data Source=C:\Users\alann\Videos\PIC\SQL\PIC.sqlite; Version=3;");
            SQLiteConnection.Open();
            //SQLiteConnection.EnableExtensions(true);
            //SQLiteConnection.LoadExtension(@"C:\Users\alann\Videos\PIC\SQL\levenshtein.dll", "SQLITE_EXTENSION_INIT1");
            return SQLiteConnection;
        }

        public static void CreateDataBase()
        {
            try
            {
                SQLiteConnection.CreateFile(@"C:\Users\alann\Videos\PIC\SQL\PIC.sqlite");
            }
            catch
            {
                throw;
            }
        }

        public static void CreateTable()
        {
            try
            {
                using (var cmd = DBConnection().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Utterances(Phrase VARCHAR(255), VideoPath VARCHAR(255))";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RestoreUtterancesData()
        {
            try
            {
                using (var cmd = DBConnection().CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Utterances;";
                    cmd.ExecuteNonQuery();
                    FileInfo fileInfo = new FileInfo(@"C:\Users\alann\Videos\PIC\SQL\InsertUtterances.sql");
                    string script = fileInfo.OpenText().ReadToEnd();
                    cmd.CommandText = script;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetVideoPath(string text)
        {
            try
            {
                using (var cmd = DBConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM 'Utterances' WHERE Phrase = '{text}'";
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            return r["VideoPath"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
