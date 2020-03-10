[![NuGet](https://img.shields.io/nuget/v/SqlInMemory.svg)](https://www.nuget.org/packages/SqlInMemory/)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/licenses/MIT)

# SqlInMemory

**SqlInMemory** is a library for create SqlServer database on **Memory** instead of hard disk, at last **Drop and Dispose** database when you're done with it. This is useful for **Integration Testing**.

This library uses [RamDisk](https://github.com/mjebrahimi/RamDisk) (which also uses [ImDisk](http://www.ltr-data.se/opencode.html/#ImDisk)) for creating virtual disk drive.

## Get Started

### 1. Install Package

```
PM> Install-Package SqlInMemory
```

### 2. Use it

Pass your connection string and it will create (mount) a virtual disk drive 'Z' and create database there finaly when disposed, drop database and unmount drive.

```csharp
using (SqlInMemoryDb.Create("Data Source=.;Initial Catalog=TestDb;Integrated Security=true"))
{
    //Use database using ADO.NET or ORM
}
```

## Contributing

Create an [issue](https://github.com/mjebrahimi/SqlInMemory/issues/new) if you find a BUG or have a Suggestion or Question. If you want to develop this project, Fork on GitHub and Develop it and send Pull Request.

A **HUGE THANKS** for your help.

## License

SqlInMemory is Copyright © 2020 [Mohammd Javad Ebrahimi](https://github.com/mjebrahimi) under the [MIT License](https://github.com/mjebrahimi/SqlInMemory/LICENSE).
