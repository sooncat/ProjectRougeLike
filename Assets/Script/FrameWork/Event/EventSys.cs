
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void EventSysCallBack(object param1, object param2);
//public delegate void EventSysCallBack_(uint eventId, object param1, object param2);

// event parameter data define
public class EventParamData
{
    public int EventId { private set; get; }
    public object Param1 { private set; get; }
    public object Param2 { private set; get; }
    public string Msg { private set; get; }

    public EventParamData(int eventId, object param1, object param2, string msg)
    {
        EventId = eventId;
        Param1 = param1;
        Param2 = param2;
        Msg = msg;
    }
}

// Event system
public class EventSys : ISystem
{
    private const String Lock = "lock";
    private List<EventSysCallBack> _allHander;

    private CDealerMap<int, CDealerCB> _mapDealer;

    private Queue _eventQueue;

    public static EventSys Instance;

    private bool _isInited;

    private List<int> _recordStackList;
    private bool _isLogStack;

    private bool _isRecordAll;

    public override void Init()
    {
        if (_isInited)
        {
            return;
        }

        _allHander = new List<EventSysCallBack>();
        _mapDealer = new CDealerMap<int, CDealerCB>();
        _eventQueue = new Queue();
        _recordStackList = new List<int>();

        _isInited = true;
        Instance = this;

        //SetRecordStack(int.ChangeGameState, true);
        //RecordAll(true);
    }

    public void SetRecordStack(int eType, bool isRecord)
    {
        bool isContains = _recordStackList.Contains(eType);
        if (isRecord && !isContains)
        {
            _recordStackList.Add(eType);
        }
        else if (!isRecord && isContains)
        {
            _recordStackList.Remove(eType);
        }

        _isLogStack = _recordStackList.Count > 0;
    }

    public void RecordAll(bool isRecorAll)
    {
        _isRecordAll = isRecorAll;
    }

    public override void SysUpdate()
    {
        lock (Lock)
        {
            while (_eventQueue.Count > 0)
            {
                EventParamData data = _eventQueue.Dequeue() as EventParamData;
                if (data != null)
                {
                    SendEvent(data.EventId, data.Param1, data.Param2, data.Msg);
                }
            }
        }
    }

    public void AddAllHander(EventSysCallBack callBack)
    {
        lock (Lock)
        {
            _allHander.Add(callBack);
        }
    }

    /// <summary>
    /// 如果重复加入同样的回调是无效的，仍然只计一次
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="callBack"></param>
    public void AddHander(int eventId, EventSysCallBack callBack)
    {
        lock (Lock)
        {
            _mapDealer.AddHandle(eventId, new CDealerCB(callBack));
        }
    }

    public void AddHander(Enum e, EventSysCallBack callBack)
    {
        int eId = Convert.ToInt32(e);
        AddHander(eId, callBack);
    }

    /// <summary>
    /// 注意大多数系统 *State 都没有RemoveHander
    /// 这意味着在使用事件时要区分清楚，不同系统之间最好不要共用同一个事件，否则会有混乱的调用
    /// </summary>
    /// <param name="target"></param>
    public void RemoveHander(object target)
    {
        lock (Lock)
        {
            _mapDealer.RemoveHandleByTarget(target);
        }
    }

    public void AddEvent(Enum e, object param1 = null, object param2 = null)
    {
        int eId = Convert.ToInt32(e);
        AddEvent( eId , param1, param2);
    }

    public void AddEvent(int eventId, object param1 = null, object param2 = null)
    {
        lock (Lock)
        {
            string stackMsg = string.Empty;
            if (_recordStackList.Contains(eventId) || _isRecordAll)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                stackMsg = st.ToString();
            }
            _eventQueue.Enqueue(new EventParamData(eventId, param1, param2, stackMsg));
        }
    }

    public void RemoveEvent(int eventId)
    {
        lock (Lock)
        {
            _mapDealer.RemoveHandleById(eventId);
        }
    }

    public void RemoveEvent(int eventId, object target)
    {
        //todo: add funcs
        throw new Exception("Not Support Yet");
    }

    //此函数只能在主线程调用
    public void AddEventNow(int eventId, object param1 = null, object param2 = null)
    {
        //Debug.Log("AddEventNow = " + eventId);
        SendEvent(eventId, param1, param2);
    }

    public void AddEventNow(Enum e, object param1 = null, object param2 = null)
    {
        int eventId = Convert.ToInt32(e);
        //Debug.Log("AddEventNow = " + eventId);
        SendEvent(eventId, param1, param2);
    }

    private void SendEvent(int eventId, object param1, object param2, string msg = "")
    {
        foreach (EventSysCallBack callBack in _allHander)
        {
            callBack(param1, param2);
        }

        var list = _mapDealer.GetDealer(eventId);
        if (list != null)
        {
            foreach (CDealerCB dealerCb in list)
            {
                var cb = dealerCb.cb as EventSysCallBack;
                if (!string.IsNullOrEmpty(msg) && _isLogStack)
                {
                    Debug.Log("stack = " + msg);
                }
                if (cb != null) cb(param1, param2);
            }
        }
    }
}