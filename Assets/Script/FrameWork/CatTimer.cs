using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Timers;

public class CatTimer {

    //const int DefaultStartRecordSize = 10;
    //const int DefaultEndRecordSzie = 256;
    //const int DefaultTypeSize = 10;

    private static List<string> _timerStartIdRecords = new List<string>();
    private static List<DateTime> _timerStartTimeRecords = new List<DateTime>();

    private static List<string> _timerEndIdRecords = new List<string>();
    private static List<double> _timerEndTimeRecords = new List<double>();

    private static Dictionary<int, double> _timerTypeRecords = new Dictionary<int, double>();
    private static Dictionary<int, List<int>> _timerTypeIdxRecords = new Dictionary<int, List<int>>();  

    private static string _lastLineTimerId = string.Empty;
    private static int _lastLineTimerType = 0;

    private static bool _isStart = false;

    private static bool _isShowDetail;

    private static int _typeSize;

    /// <summary>
    /// 注意此句话会清除部分数据，请在开始记录前使用
    /// </summary>
    /// <param name="typeSize"></param>
    /// <param name="recordSize"></param>
    public static void PrepareRecordSize(int typeSize, int recordSize = 0)
    {
        if (recordSize > 0)
        {
            _timerEndIdRecords = new List<string>(recordSize);
            _timerEndTimeRecords = new List<double>(recordSize);
        }
        _typeSize = typeSize;
        ResetTypeRecord();
        
    }

    static void ResetTypeRecord()
    {
        _timerTypeRecords.Clear();
        _timerTypeIdxRecords.Clear();
        for (int i = 0; i < _typeSize; i++)
        {
            _timerTypeRecords.Add(i, 0);
            _timerTypeIdxRecords.Add(i, new List<int>());
        }
    }

    public static void Start()
    {
        if(_isShowDetail)
        {
            CatDebug.CLog("CatTimer:Start()", CatLogType.CatTimer);
        }
        _isStart = true;
    }

    /// <summary>
    /// 同时结束Line计时
    /// </summary>
    public static void Stop()
    {
        if (_isShowDetail)
        {
            CatDebug.CLog("CatTimer:Stop()", CatLogType.CatTimer);
        }
        StopLineRecord();
        _isStart = false;
    }

    public static void ShowDetail(bool isShow)
    {
        _isShowDetail = isShow;
        if (_isShowDetail)
        {
            CatDebug.CLog("CatTimer:ShowDetail(" + isShow + ")", CatLogType.CatTimer);
        }
    }

    public static void StartRecord(string timerId, int type)
    {
        if (!_isStart) return;
        if (_timerStartIdRecords.Contains(timerId))
        {
            CatDebug.CWarning("StartRecord :Repeat timerID = " + timerId, CatLogType.CatTimer);
            return;
        }

        if (string.IsNullOrEmpty(timerId))
        {
            CatDebug.CError("StartRecord : Null/Empty timerID.", CatLogType.CatTimer);
            return;
        }

        if (_isShowDetail)
        {
            CatDebug.CLog("StartRecord : " + timerId + "," + type, CatLogType.CatTimer);
        }

        _lastLineTimerId = timerId;
        _lastLineTimerType = type;

        _timerStartIdRecords.Add(timerId);
        _timerStartTimeRecords.Add(DateTime.Now);
    }

    public static void EndRecord(string timerId)
    {
        if (!_isStart) return;
        int index = _timerStartIdRecords.IndexOf(timerId);
        if (index < 0)
        {
            CatDebug.CError("EndRecord : Null timerID = " + timerId, CatLogType.CatTimer);
            return;
        }
        TimeSpan ts = DateTime.Now - _timerStartTimeRecords[index];
        _timerStartIdRecords.RemoveAt(index);
        _timerStartTimeRecords.RemoveAt(index);
        if(_isShowDetail)
        {
            CatDebug.CLog("EndRecord : " + timerId + " Cost " + ts.TotalSeconds, CatLogType.CatTimer);
        }
        //_timerEndRecords.Add(timerId, ts.TotalMilliseconds);
        AddEndRecord(timerId, ts.TotalSeconds);
        if(timerId.Equals(_lastLineTimerId))
        {
            _lastLineTimerId = string.Empty;
        }
    }

    /// <summary>
    /// 单线记录，当第二次调用该方法时，将自动停止上次的计时。
    /// </summary>
    /// <param name="timerId"></param>
    /// <param name="type">分类</param>
    public static void StartLineRecord(string timerId, int type = 0)
    {
        if (!_isStart) return;
        if(!string.IsNullOrEmpty(_lastLineTimerId))
        {
            EndRecord(_lastLineTimerId);
        }
        StartRecord(timerId, type);
    }

    public static void  StopLineRecord()
    {
        if (!_isStart) return;
        if (!string.IsNullOrEmpty(_lastLineTimerId))
        {
            EndRecord(_lastLineTimerId);
        }
    }

    private static void AddEndRecord(string id, double second)
    {
        _timerEndIdRecords.Add(id);
        _timerEndTimeRecords.Add(second);
        if (_timerTypeRecords.ContainsKey(_lastLineTimerType))
        {
            _timerTypeRecords[_lastLineTimerType] += second;
            _timerTypeIdxRecords[_lastLineTimerType].Add(_timerEndIdRecords.Count - 1);
        }
        else
        {
            _timerTypeRecords.Add(_lastLineTimerType, second);
            List<int> data = new List<int> {_timerEndIdRecords.Count - 1};
            _timerTypeIdxRecords.Add(_lastLineTimerType, data);
        }
        
    }

    static StringBuilder GetTypeDetail(int type)
    {
        StringBuilder sb = new StringBuilder();
        if (_timerTypeIdxRecords.ContainsKey(type))
        {
            //sb.Append("PrintType("+type+")").AppendLine();

            List<int> ids = _timerTypeIdxRecords[type];
            foreach (int id in ids)
            {
                sb.Append(type).Append(",").Append(_timerEndIdRecords[id]).Append(",").Append(_timerEndTimeRecords[id]).AppendLine();
            }

            //CatDebug.CLog(sb.ToString(), CatLogType.CatTimer);
        }
        return sb;
    }

    public static void PrintType(int type)
    {
        CatDebug.CLog(GetTypeDetail(type).ToString(), CatLogType.CatTimer);   
    }

    public static void PrintTypes()
    {
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        foreach (KeyValuePair<int, double> pair in _timerTypeRecords)
        {
            sb1.Append(pair.Key).Append(",");
            sb2.Append(pair.Value).Append(",");
        }

        sb1.AppendLine().Append(sb2);
        Debug.Log(sb1.ToString());
    }

    public static void PrintTypeDetails()
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<int, double> pair in _timerTypeRecords)
        {
            sb.Append(GetTypeDetail(pair.Key));
        }
        Debug.Log(sb.ToString());
    }
    
    public static void ClearRecords()
    {
        _timerStartIdRecords.Clear();
        _timerStartTimeRecords.Clear();

        _timerEndIdRecords.Clear();
        _timerEndTimeRecords.Clear();

        ResetTypeRecord();

        _lastLineTimerId = string.Empty;
        _lastLineTimerType = 0;
    }
}