using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Text;


public class CatDebug {

    public enum LogLevel
    {
        Log,
        Warning,
        Error,
    }

    public static int Enabletype = 0;
    public static bool Enable = false;
    private static bool _isInited;
    private static DateTime _startTime;
    private static Dictionary<int, string> _logTypeLable; 

    private static void Init()
    {
        if (_isInited) return;
        _isInited = true;
        _startTime = DateTime.Now;
        _logTypeLable = new Dictionary<int, string>(); 
    }

    private static string GetTime()
    {
        return (DateTime.Now - _startTime).TotalSeconds.ToString("F");
    }

    public static void RegistLogTypeLabel(int id, string label)
    {
        Init();
        if (!_logTypeLable.ContainsKey(id))
        {
            _logTypeLable.Add(id, string.Empty);
        }
        _logTypeLable[id] = label;
    }
    static string GetTypeLabel(int id)
    {
        string result = id.ToString();
        if(_logTypeLable.ContainsKey(id))
            result = _logTypeLable[id];
        return result;
    }

    /// <summary>
    /// 打印调堆栈上的方法的信息
    /// </summary>
    /// <param name="msg">额外添加的输出信息</param>
    /// <param name="stackLayer">代表向上回退的堆栈层数</param>
    /// <returns></returns>
    static string LogFunction(string msg, int stackLayer)
    {
        StackTrace st = new StackTrace(true);
        //得到当前的所以堆栈  
        StackFrame[] stackFrames = st.GetFrames();
        if (stackFrames != null && stackFrames.Length > stackLayer)
        {
            string fullName = "null";
            StackFrame stackFrame = stackFrames[stackLayer];
            Type declaringType = stackFrame.GetMethod().DeclaringType;
            if (declaringType != null)
            {
                fullName = declaringType.FullName;
            }
            string info = fullName + "::" + stackFrame.GetMethod().Name + ", L" + stackFrame.GetFileLineNumber();
            if(!string.IsNullOrEmpty(msg))
            {
                info += ",   (" + msg + ")";
            }
            UnityEngine.Debug.Log(info);
            return info;
        }
        return "";
    }

    public static void LogFunc()
    {
        LogFunction(String.Empty, 2);
    }
    public static void LogFuncInStack(int stackLayer)
    {
        LogFunction(String.Empty, 2 + stackLayer);
    }
    public static void LogFunc(int msg)
    {
        LogFunc(msg.ToString());
    }
    public static void LogFunc(string msg)
    {
        LogFunction(msg, 2);
    }
    public static void LogFunc(string msg, int stackLayer)
    {
        LogFunction(msg, 2 + stackLayer);
    }

    [Conditional("CATDEBUG")]
    public static void Log(string msg, int type)
    {
        Init();
        if (Enable && (type & Enabletype) > 0)
        {
            UnityEngine.Debug.Log("Cat: " + GetTypeLabel(type) + " : " + msg + ",  T=" + GetTime());
        }
    }

    [Conditional("CATDEBUG")]
    public static void Warning(string msg, int type)
    {
        Init();
        if (Enable && (type & Enabletype) > 0)
        {
            UnityEngine.Debug.LogWarning("Cat: " + GetTypeLabel(type) + " : " + msg + ",  T=" + GetTime());
        }
    }

    [Conditional("CATDEBUG")]
    public static void Error(string msg, int type)
    {
        Init();
        if (Enable && (type & Enabletype) > 0)
        {
            UnityEngine.Debug.LogError("Cat: " + GetTypeLabel(type) + " : " + msg + ",  T=" + GetTime());
        }
    }

    /// <summary>
    /// ConditionLog 条件存在
    /// 非宏编译方法，一直存在，一般只有在真机调试中使用
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="type"></param>
    public static void CLog(string msg, int type)
    {
        Init();
        if (Enable && (type & Enabletype) > 0)
            UnityEngine.Debug.Log("Cat: " + GetTypeLabel(type) + " : " + msg + ",  T=" + GetTime());
    }

    /// <summary>
    /// 非宏编译方法，一直存在，一般只有在真机调试中使用
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="type"></param>
    public static void CWarning(string msg, int type)
    {
        Init();
        if (Enable && (type & Enabletype) > 0)
            UnityEngine.Debug.LogWarning("Cat: " + GetTypeLabel(type) + " : " + msg + ",  T=" + GetTime());
    }

    /// <summary>
    /// 非宏编译方法，一直存在，一般用于严重报错
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="type"></param>
    public static void CError(string msg, int type)
    {
        Init();
        if (Enable && (type & Enabletype) > 0)
            UnityEngine.Debug.LogError("Cat: " + GetTypeLabel(type) + " : " + msg + ",  T=" + GetTime());
    }

    /// <summary>
    /// 用于S级错误，单独设定关键字，方便后台提取
    /// </summary>
    /// <param name="msg"></param>
    public static void FatalError(string msg)
    {
        UnityEngine.Debug.LogError("FatalError: " + msg);
    }

#region time_records

    static Dictionary<int, List<string>> _arrayRecords;

    public static void RecordMsg(int id, string msg, bool withStackTrack = false)
    {
        if (_arrayRecords == null)
        {
            _arrayRecords = new Dictionary<int, List<string>>();
        }
        if (!_arrayRecords.ContainsKey(id))
        {
            _arrayRecords.Add(id, new List<string>());
        }
        _arrayRecords[id].Add(msg);
        if (withStackTrack)
        {
            _arrayRecords[id].Add(GetStackTrace());
        }
    }

    public static void ClearMsg(int id)
    {
        if (_arrayRecords != null && _arrayRecords.ContainsKey(id))
        {
            _arrayRecords.Remove(id);
        }
    }

    public static void PrintMsg(int id, LogLevel level)
    {
        if (_arrayRecords != null && _arrayRecords.ContainsKey(id))
        {
            List<string> msgs = _arrayRecords[id];
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Cat: ArrayMsg : T = ");
            foreach (string msg in msgs)
            {
                sb.AppendLine(msg);
            }
            switch (level)
            {
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(sb.ToString());
                    break;
                case LogLevel.Log:
                    UnityEngine.Debug.Log(sb.ToString());
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(sb.ToString());
                    break;
            }
        }
    }

    static String GetStackTrace()
    {
        string info = "null";
        //设置为true，这样才能捕获到文件路径名和当前行数，当前行数为GetFrames代码的函数，也可以设置其他参数  
        StackTrace st = new StackTrace(true);
        //得到当前的所以堆栈  
        StackFrame[] stackFrames = st.GetFrames();
        if (stackFrames != null)
        {
            foreach (StackFrame sf in stackFrames)
            {
                string fullName = "null";
                Type declaringType = sf.GetMethod().DeclaringType;

                if (declaringType != null)
                {
                    fullName = declaringType.FullName;
                }
                info = info + "\r\n" + " FileName=" + sf.GetFileName() + " fullname=" + fullName + " function=" + sf.GetMethod().Name + " FileLineNumber=" + sf.GetFileLineNumber();
            }
        }
        return info;
    }
#endregion
}
