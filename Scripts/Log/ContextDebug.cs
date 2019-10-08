using UnityEngine;


namespace RedHoney.Log
{
    /// <summary>
    /// This class provides a custom Debug logger object with pretty prefix.
    /// You can use import it like this:
    /// using Debug = ContextDebug<MyClass>;
    /// </summary>
    /// <typeparam name="T">The name of this type will be used as prefix in the logs</typeparam>
    public class ContextDebug<T>
    {
        private static readonly string prefix = $"<b>[{typeof(T).Name}]</b>";
        private static string logTemplate(object message) => $"{prefix} {message}";

        public static void Log(object message) => Debug.Log(logTemplate(message));
        public static void LogWarning(object message) => Debug.LogWarning(logTemplate(message));
        public static void LogError(object message) => Debug.LogError(logTemplate(message));

    }
}