using DotNetEnv;

namespace Context.Configuration;

public static class EnvFile
{
    private const string FileName = ".env";

    public static string? Load()
    {
        var path = Find();
        if (path == null)
        {
            Console.WriteLine("No .env file found, using the process environment.");
            return null;
        }

        // systemd and docker compose set real variables; those must win over the file
        Env.Load(path, new LoadOptions(clobberExistingVars: false));
        // Old .env copies can sit in bin/ and publish/; the log shows which one was used
        Console.WriteLine($"Loaded environment from {path}");
        return path;
    }

    private static string? Find()
    {
        var explicitPath = Environment.GetEnvironmentVariable("ENV_FILE");
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            var fullPath = Path.GetFullPath(explicitPath);
            // A typo in ENV_FILE must not silently start the app without its config
            return File.Exists(fullPath)
                ? fullPath
                : throw new FileNotFoundException($"ENV_FILE points to {fullPath}, but that file does not exist.");
        }

        // dotnet run and dotnet ef start in the source tree; prod starts the dll from another folder
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var dir = new DirectoryInfo(start); dir != null; dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, FileName);
                if (File.Exists(candidate)) return candidate;

                // Stop at the repository root, so an unrelated ~/.env is never loaded
                if (!Path.Exists(Path.Combine(dir.FullName, ".git"))) continue;

                // Older setups keep the file in server/API, which tests and dotnet ef never walk through
                var legacy = Path.Combine(dir.FullName, "server", "API", FileName);
                if (File.Exists(legacy)) return legacy;
                break;
            }
        }

        return null;
    }
}
