using UnityEngine;

namespace Framework
{
    public enum LogType
    {
        Info,
        Warning,
        Error
    }
    
    public static class Log
    {
        public static void Dev(string msg , LogType type = LogType.Info)
        {
#if UNITY_EDITOR
            switch (type)
            {
                case LogType.Info:
                    Debug.Log(msg);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(msg);
                    break;
                case LogType.Error:
                    Debug.LogError(msg);
                    break;
            }
        }
#endif
    }
}