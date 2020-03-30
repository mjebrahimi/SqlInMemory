#if NETSTANDARD2_0 || NET461
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System;
using System.IO;

namespace SqlInMemory
{
    /// <summary>
    /// SqlHelper
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Create database if not exists
        /// Example 1 : databaseName: null(optional) and connectionString: "Data Source=.;Initial Catalog=MyDatabaseName;Integrated Security=true"
        /// Example 1 : databaseName: "MyDatabaseName" and connectionString: "Data Source=.;Initial Catalog=master;Integrated Security=true"
        /// </summary>
        /// <param name="connectionString">Connection string to connect</param>
        /// <param name="databaseName">Database name to check or create</param>
        /// <param name="folderPath">Folder path to create database</param>
        public static void CreateDatabaseIfNotExists(string connectionString, string databaseName = null, string folderPath = null)
        {
            if (!DatabaseExists(connectionString, databaseName))
                CreateDatabase(connectionString, databaseName, folderPath);
        }

        /// <summary>
        /// Drop database and recreate
        /// Example 1 : databaseName: null(optional) and connectionString: "Data Source=.;Initial Catalog=MyDatabaseName;Integrated Security=true"
        /// Example 1 : databaseName: "MyDatabaseName" and connectionString: "Data Source=.;Initial Catalog=master;Integrated Security=true"
        /// </summary>
        /// <param name="connectionString">Connection string to connect</param>
        /// <param name="databaseName">Database name to check or create</param>
        /// <param name="folderPath">Folder path to create database</param>
        /// <param name="force">Force to close existing connections</param>
        public static void DropDatabaseAndRecreate(string connectionString, string databaseName = null, string folderPath = null, bool force = false)
        {
            if (DatabaseExists(connectionString, databaseName))
                DropDatabase(connectionString, databaseName, force);
            CreateDatabase(connectionString, databaseName, folderPath);
        }

        /// <summary>
        /// Drop database
        /// Example 1 : databaseName: null(optional) and connectionString: "Data Source=.;Initial Catalog=MyDatabaseName;Integrated Security=true"
        /// Example 1 : databaseName: "MyDatabaseName" and connectionString: "Data Source=.;Initial Catalog=master;Integrated Security=true"
        /// </summary>
        /// <param name="connectionString">Connection string to connect</param>
        /// <param name="databaseName">Database name to check</param>
        /// <param name="force">Force to close existing connections</param>
        public static void DropDatabase(string connectionString, string databaseName = null, bool force = false)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            var isMaster = builder.InitialCatalog.Equals("master", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                if (isMaster)
                    throw new InvalidOperationException($"If {nameof(databaseName)} hasn't value then current InitialCatalog shouldn't be 'master'");

                databaseName = builder.InitialCatalog;
                builder.InitialCatalog = "master";
            }
            else
            {
                if (!isMaster)
                    throw new InvalidOperationException($"If {nameof(databaseName)} has value ({databaseName}) then current InitialCatalog should be 'master'");
            }

            var command = $"DROP DATABASE {databaseName};";
            if (force)
                command = $"ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " + command;

            using (var sqlConnection = new SqlConnection(builder.ConnectionString))
            {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                using (var sqlCommand = new SqlCommand(command, sqlConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks the existence of a database
        /// Example 1 : databaseName: null(optional) and connectionString: "Data Source=.;Initial Catalog=MyDatabaseName;Integrated Security=true"
        /// Example 1 : databaseName: "MyDatabaseName" and connectionString: "Data Source=.;Initial Catalog=master;Integrated Security=true"
        /// </summary>
        /// <param name="connectionString">Connection string to connect</param>
        /// <param name="databaseName">Database name to check</param>
        /// <returns>Whether or not the database is exist</returns>
        public static bool DatabaseExists(string connectionString, string databaseName = null)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            var isMaster = builder.InitialCatalog.Equals("master", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                if (isMaster)
                    throw new InvalidOperationException($"If {nameof(databaseName)} hasn't value then current InitialCatalog shouldn't be 'master'");

                databaseName = builder.InitialCatalog;
                builder.InitialCatalog = "master";
            }
            else
            {
                if (!isMaster)
                    throw new InvalidOperationException($"If {nameof(databaseName)} has value ({databaseName}) then current InitialCatalog should be 'master'");
            }

            var command = "select count(*) from master.dbo.sysdatabases where name=@database";
            using (var sqlConnection = new SqlConnection(builder.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(command, sqlConnection))
                {
                    sqlCommand.Parameters.Add("@database", System.Data.SqlDbType.NVarChar).Value = databaseName;
                    sqlConnection.Open();
                    return Convert.ToInt32(sqlCommand.ExecuteScalar()) == 1;
                }
            }
        }

        /// <summary>
        /// Create database
        /// Example 1 : databaseName: null(optional) and connectionString: "Data Source=.;Initial Catalog=MyDatabaseName;Integrated Security=true"
        /// Example 1 : databaseName: "MyDatabaseName" and connectionString: "Data Source=.;Initial Catalog=master;Integrated Security=true"
        /// </summary>
        /// <param name="connectionString">Connection string to connect</param>
        /// <param name="databaseName">Database name to create</param>
        /// <param name="folderPath">Folder path to create database</param>
        public static void CreateDatabase(string connectionString, string databaseName = null, string folderPath = null)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            var isMaster = builder.InitialCatalog.Equals("master", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                if (isMaster)
                    throw new InvalidOperationException($"If {nameof(databaseName)} hasn't value then current InitialCatalog shouldn't be 'master'");

                databaseName = builder.InitialCatalog;
                builder.InitialCatalog = "master";
            }
            else
            {
                if (!isMaster)
                    throw new InvalidOperationException($"If {nameof(databaseName)} has value ({databaseName}) then current InitialCatalog should be 'master'");
            }

            var command = $"CREATE DATABASE {databaseName}";
            if (string.IsNullOrWhiteSpace(folderPath) == false && Directory.Exists(folderPath))
                command += $" ON PRIMARY ( NAME = {databaseName}, FILENAME = '{Path.Combine(folderPath, databaseName + ".mdf")}' )";

            #region More info
            //https://stackoverflow.com/questions/39499810/how-to-create-database-if-not-exist-in-c-sharp-winforms
            //https://www.codeproject.com/Questions/666651/How-to-create-a-database-using-csharp-code-in-net
            //https://support.microsoft.com/en-us/help/307283/how-to-create-a-sql-server-database-programmatically-by-using-ado-net

            // ConnectionString examples
            //"server=(local)\\SQLEXPRESS;Trusted_Connection=yes"
            //"Server=localhost;Integrated security=SSPI;database=master"

            //var command = $"CREATE DATABASE {"MyDatabase"} ON PRIMARY " +
            //    $"(NAME = {"MyDatabase_Data"}, " +
            //    $"FILENAME = '{"C:\\MyDatabaseData.mdf"}', " +
            //    $"SIZE = {2}MB, " +
            //    $"MAXSIZE = {10}MB, " +
            //    $"FILEGROWTH = {10}%) " +
            //    $"LOG ON (NAME = {"MyDatabase_Log"}, " +
            //    $"FILENAME = '{"C:\\MyDatabaseLog.ldf"}', " +
            //    $"SIZE = {1}MB, " +
            //    $"MAXSIZE = {5}MB, " +
            //    $"FILEGROWTH = {10}%)";
            #endregion

            using (var sqlConnection = new SqlConnection(builder.ConnectionString))
            {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                using (var sqlCommand = new SqlCommand(command, sqlConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
