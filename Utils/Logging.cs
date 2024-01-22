using BepInEx.Logging;

namespace LethalMDK.Logging;

public static class Log
{
    private static readonly ManualLogSource LOGSource = Logger.CreateLogSource(LethalMDK.ModGuid);
    
    public static void Print(object message)
    {
        LOGSource.LogInfo(message ?? "Null");
    }

    public static void Warning(object message)
    {
        LOGSource.LogWarning(message ?? "Null");
    }

    public static void Error(object message)
    {
        LOGSource.LogError(message ?? "Null");
    }

    public static void Exception(Exception exception)
    {
        LOGSource.LogError(exception?.Message ?? "Null");
    }
}