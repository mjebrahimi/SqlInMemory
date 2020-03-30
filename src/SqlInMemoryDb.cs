using RamDisk;
using System;

namespace SqlInMemory
{
    /// <summary>
    /// SqlInMemoryDb
    /// </summary>
    public static class SqlInMemoryDb
    {
        /// <summary>
        /// Mounts a drive on system memory and creates SQL database there. (drops database if exists first)
        /// </summary>
        /// <param name="connectionString">Connection string to SQL database</param>
        /// <param name="sizeMegaByte">Size of ram drive in mega bytes</param>
        /// <param name="drive"></param>
        /// <returns></returns>
        public static IDisposable Create(string connectionString, int sizeMegaByte = 256, char drive = 'Z')
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            RamDrive.Mount(sizeMegaByte, RamDisk.FileSystem.NTFS, drive, "SqlInMemory");
            SqlHelper.DropDatabaseAndRecreate(connectionString, null, $"{drive}:\\", force: true);

            return new Releaser(connectionString, drive);
        }

        private readonly struct Releaser : IDisposable
        {
            private readonly string _connectionString;
            private readonly char _drive;

            public Releaser(string connectionString, char drive)
            {
                _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
                _drive = drive;
            }

            public void Dispose()
            {
                SqlHelper.DropDatabase(_connectionString, force: true);
                RamDrive.Unmount(_drive);
            }
        }
    }
}
