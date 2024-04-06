using System;
using System.IO;
using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        new MenuConsoleService(
            new CryptoService(),
            new RepositoryKeys( @"Data Source=./data/mydatabase.db;Version=3;"),
            new FileManager("./files/")
        );
    }
}