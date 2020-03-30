using NUnit.Framework;
using System;

namespace SqlInMemory.Tests
{
    public class SqlInMemoryDbTests
    {
        private string _connectionString;
        private string _connectionStringMaster;
        private string _databaseName;
        private string _driveLetter;
        private IDisposable _releaser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connectionString = "Data Source=.;Initial Catalog=TestDb;Integrated Security=true";
            _connectionStringMaster = "Data Source=.;Initial Catalog=master;Integrated Security=true";
            _databaseName = "TestDb";
            _driveLetter = "Z:\\";

            void action() => _releaser = SqlInMemoryDb.Create(_connectionString);
            Assert.DoesNotThrow(action);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            void action() => _releaser.Dispose();
            Assert.DoesNotThrow(action);
        }

        [Test, Order(1)]
        public void DatabaseExists_Should_Exists()
        {
            var exists = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsTrue(exists);
        }

        [Test, Order(2)]
        public void DatabaseExists_WithName_Should_Exists()
        {
            var exists = SqlHelper.DatabaseExists(_connectionStringMaster, _databaseName);
            Assert.IsTrue(exists);
        }

        [Test, Order(3)]
        public void DropDatabase_Should_Drops_Database()
        {
            void action() => SqlHelper.DropDatabase(_connectionString);
            Assert.DoesNotThrow(action);

            var exists = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsFalse(exists);
        }

        [Test, Order(4)]
        public void CreateDatabase_Should_Creates_Database()
        {
            void action() => SqlHelper.CreateDatabase(_connectionString, folderPath: _driveLetter);
            Assert.DoesNotThrow(action);

            var exists = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsTrue(exists);
        }

        [Test, Order(5)]
        public void DropDatabase_WithName_Should_Drops_Database()
        {
            void action() => SqlHelper.DropDatabase(_connectionStringMaster, _databaseName, true);
            Assert.DoesNotThrow(action);

            var exists = SqlHelper.DatabaseExists(_connectionStringMaster, _databaseName);
            Assert.IsFalse(exists);
        }

        [Test, Order(6)]
        public void CreateDatabase_WithName_Should_Creates_Database()
        {
            void action() => SqlHelper.CreateDatabase(_connectionStringMaster, _databaseName, _driveLetter);
            Assert.DoesNotThrow(action);

            var exists = SqlHelper.DatabaseExists(_connectionStringMaster, _databaseName);
            Assert.IsTrue(exists);
        }

        [Test, Order(7)]
        public void DropDatabaseAndRecreate_ShouldBe_OK()
        {
            //Database already exists. first drop then recreate it.
            void action() => SqlHelper.DropDatabaseAndRecreate(_connectionString, folderPath: _driveLetter, force: true);
            Assert.DoesNotThrow(action);

            var exists = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsTrue(exists);
        }

        [Test, Order(8)]
        public void DropDatabaseAndRecreate_WithName_ShouldBe_OK()
        {
            //Database already exists. first drop then recreate it.
            void action() => SqlHelper.DropDatabaseAndRecreate(_connectionStringMaster, _databaseName, _driveLetter, true);
            Assert.DoesNotThrow(action);

            var exists = SqlHelper.DatabaseExists(_connectionStringMaster, _databaseName);
            Assert.IsTrue(exists);
        }

        [Test, Order(9)]
        public void CreateDatabaseIfNotExists_ShouldBe_OK()
        {
            //Database already exists. therefore does not create
            void action1() => SqlHelper.CreateDatabaseIfNotExists(_connectionString, folderPath: _driveLetter);
            Assert.DoesNotThrow(action1);

            var exists1 = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsTrue(exists1);

            SqlHelper.DropDatabase(_connectionString, force: true);

            void action2() => SqlHelper.CreateDatabaseIfNotExists(_connectionString, folderPath: _driveLetter);
            Assert.DoesNotThrow(action2);

            var exists2 = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsTrue(exists2);
        }

        [Test, Order(10)]
        public void CreateDatabaseIfNotExists_WithName_ShouldBe_OK()
        {
            //Database already exists. therefore does not create
            void action1() => SqlHelper.CreateDatabaseIfNotExists(_connectionStringMaster, _databaseName, folderPath: _driveLetter);
            Assert.DoesNotThrow(action1);

            var exists1 = SqlHelper.DatabaseExists(_connectionStringMaster, _databaseName);
            Assert.IsTrue(exists1);

            SqlHelper.DropDatabase(_connectionStringMaster, _databaseName, force: true);

            void action2() => SqlHelper.CreateDatabaseIfNotExists(_connectionStringMaster, _databaseName, folderPath: _driveLetter);
            Assert.DoesNotThrow(action2);

            var exists2 = SqlHelper.DatabaseExists(_connectionString);
            Assert.IsTrue(exists2);
        }
    }
}