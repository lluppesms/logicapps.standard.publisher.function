namespace LogicPublisher;
public static class Common
{
    public static int ParseIntegerFromRequest(HttpRequest req, string keyName, int defaultValue = 0)
    {
        var value = req.Query[keyName];
        if (string.IsNullOrEmpty(value))
            return defaultValue;
        try
        {
            int result = Int32.Parse(value);
            return result;
        }
        catch (FormatException)
        {
            return defaultValue;
        }
    }

    public static async Task<string> ParseRequestBodyAsync(HttpRequest req)
    {
        var value = await req.ReadAsStringAsync();
        return value;
    }

    public static string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }

    public static string GetEnvironmentVariable(string name, string defaultValue)
    {
        var value = GetEnvironmentVariable(name);
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }
}