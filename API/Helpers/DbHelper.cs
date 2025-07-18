using System;
using Npgsql;

namespace API.Helpers;

public static class DbHelper
{
    private static string _connectionString = "";

    public static NpgsqlDataSource DataSource { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        _connectionString = connectionString;
        DataSource = NpgsqlDataSource.Create(_connectionString);
    }
}
