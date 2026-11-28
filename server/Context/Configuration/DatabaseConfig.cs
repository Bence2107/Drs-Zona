using Npgsql;

namespace Context.Configuration;

public static class DatabaseConfig
{
    // DB_PORT and DB_PASSWORD stay optional: Npgsql defaults to 5432, and local trust auth needs no password
    public static readonly string[] RequiredVariables = ["DB_HOST", "DB_NAME", "DB_USER"];

    public static string BuildConnectionString()
    {
        EnvVars.Require(RequiredVariables);

        // The builder escapes ';' and quotes in passwords, which string concatenation broke
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = EnvVars.Get("DB_HOST"),
            Database = EnvVars.Get("DB_NAME"),
            Username = EnvVars.Get("DB_USER"),
            Password = EnvVars.GetOptional("DB_PASSWORD")
        };

        var port = EnvVars.GetOptionalInt("DB_PORT");
        if (port != null) builder.Port = port.Value;

        return builder.ConnectionString;
    }
}
