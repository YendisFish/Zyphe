namespace Zyphe;

public class Logger
{
    public static void Log(object? obj)
    {
        if (obj != null)
        {
            if (obj.GetType() == typeof(string) && ((string)obj).Trim() == "")
            {
                Console.WriteLine("\"\"");
            } else {
                Console.WriteLine(obj);
            }
        }
    }
}