using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityEngine
{
    public class Message
    {
        public static void Log(object message)
        {
    #if UNITY_EDITOR
            Debug.Log(message);
    #endif
        }
        public static void Log(object message, UnityEngine.Object context)
        {
    #if UNITY_EDITOR
            Debug.Log(message, context);
    #endif
        }
        public static void LogWarning(object message)
        {
    #if UNITY_EDITOR
            Debug.LogWarning(message);
    #endif
        }
        public static void LogWarning(object message, UnityEngine.Object context)
        {
    #if UNITY_EDITOR
            Debug.LogWarning(message, context);
    #endif
        }
        public static void LogError(object message)
        {
    #if UNITY_EDITOR
            Debug.LogError(message);
    #endif
        }
        public static void LogError(object message, UnityEngine.Object context)
        {
    #if UNITY_EDITOR
            Debug.LogError(message, context);
    #endif
        }
        //static void LogStackTrace()
        //{
        //    StringBuilder sb = new();
        //    System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
        //    for (int i = 0; i < stackTrace.FrameCount; i++)
        //    {
        //        System.Diagnostics.StackFrame frame = stackTrace.GetFrame(i);
        //        string fileName = frame.GetFileName();
        //        int lineNumber = frame.GetFileLineNumber();
        //        string methodName = frame.GetMethod().Name;
        //        string declaringType = frame.GetMethod().DeclaringType.FullName;
    
        //        if (!string.IsNullOrEmpty(fileName))
        //        {
        //            sb.Append($"{declaringType}.{methodName} (at {fileName}:{lineNumber}) \n");
        //        }
        //    }
        //    Debug.Log(sb.ToString());
        //}
    }
}

