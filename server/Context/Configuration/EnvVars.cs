namespace Context.Configuration;

public static class EnvVars
{
    // Report everything at once, so a fresh setup doesn't fail one variable per restart
    public static void Require(params string[] names)
    {
        var missing = names.Where(name => GetOptional(name) == null).ToList();
        if (missing.Count == 0) return;

        throw new InvalidOperationException(
            $"Missing environment variables: {string.Join(", ", missing)}. " +
            "Set them in .env (see .env.example) or in the environment.");
    }

    public static string Get(string name) =>
        GetOptional(name) ?? throw new InvalidOperationException($"Missing environment variable: {name}.");

    public static string? GetOptional(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        // An empty "KEY=" line in .env means "not set", not an empty value
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static int? GetOptionalInt(string name)
    {
        var value = GetOptional(name);
        if (value == null) return null;

        return int.TryParse(value, out var number)
            ? number
            : throw new InvalidOperationException($"{name} must be a whole number, got '{value}'.");
    }
}
