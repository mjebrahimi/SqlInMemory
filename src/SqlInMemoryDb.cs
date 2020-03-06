using System;

namespace SqlInMemory
{
    public static class SqlInMemoryDb
    {
        public static IDisposable Create(string connectionString, int sizeMegaByte = 256, char drive = 'Z')
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            RamDisk.RamDisk.Mount(sizeMegaByte, RamDisk.FileSystem.NTFS, drive, "SqlInMemory");
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
                RamDisk.RamDisk.Unmount(_drive);
            }
        }
    }
}
