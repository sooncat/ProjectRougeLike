// /************************************************************
//     File      : CDealerMap.cs
//     brief     :   
//     author    : JanusLiu   janusliu@ezfun.cn
//     version   : 1.0
//     date      : 2016/3/31 16:53:29
//     copyright : Copyright 2014 EZFun Inc.
// **************************************************************/


using System.Collections.Generic;
using System;

public class CDealerCB
{
    public System.Delegate cb;
    public CDealerCB(System.Delegate c)
    {
        cb = c;
    }
    public bool EqualsTarget(object target)
    {
        return cb.Target == target;
    }
}

public class CDealerMap<TID, TCB>
    where TCB : CDealerCB
{
    public Dictionary<TID, List<TCB>> m_dealerMap = new Dictionary<TID, List<TCB>>();
    private Dictionary<object, List<TID>> m_targetMap = new Dictionary<object, List<TID>>();

    public void AddHandle(TID id, TCB _cb)
    {
        /*
        if (!typeof(TCB).IsSubclassOf(typeof(Delegate)))
        {
            throw new InvalidOperationException(typeof(TCB).Name + " is not a delegate type");
        }
        */

        if (!m_dealerMap.ContainsKey(id))
        {
            m_dealerMap[id] = new List<TCB>();
        }

        var list = m_dealerMap[id];
        TCB ocb = list.Find((TCB mcb) =>
        {
            return mcb.cb.Equals(_cb.cb);
        });
        if (ocb != null)
        {
            return;
        }

        list.Add(_cb);

        System.Delegate cb = _cb.cb;
        if (cb != null && cb.Target != null)
        {
            if (!m_targetMap.ContainsKey(cb.Target))
            {
                m_targetMap[cb.Target] = new List<TID>();
            }
            var tlist = m_targetMap[cb.Target];
            tlist.Add(id);
        }
    }
    public void RemoveHandleByTarget(object target)
    {
        if (m_targetMap.ContainsKey(target))
        {
            var tlist = m_targetMap[target];
            for (int i = tlist.Count - 1; i >= 0; --i)
            {
                TID id = tlist[i];
                if (m_dealerMap.ContainsKey(id))
                {
                    var clist = m_dealerMap[id];
                    clist.RemoveAll((TCB cb) =>
                    {
                        return cb.EqualsTarget(target);
                    });
                    if (clist.Count == 0)
                    {
                        m_dealerMap.Remove(id);
                    }
                }
            }
            m_targetMap.Remove(target);
        }
    }

    public void RemoveHandleById(TID id)
    {
        if (m_dealerMap.ContainsKey(id))
        {
            var list = m_dealerMap[id];
            foreach (var _cb in list)
            {
                System.Delegate cb = _cb.cb;
                if (cb != null && cb.Target != null && m_targetMap.ContainsKey(cb.Target))
                {
                    var idlist = m_targetMap[cb.Target];
                    idlist.RemoveAll((TID obj) =>
                    {
                        return id.Equals(obj);
                    });
                    if (idlist.Count == 0)
                    {
                        m_targetMap.Remove(cb.Target);
                    }
                }
            }
        }
        m_dealerMap.Remove(id);
    }

    public List<TCB> GetDealer(TID id)
    {
        List<TCB> list = null;
        m_dealerMap.TryGetValue(id, out list);
        return list;
        //以下方式需要去取两次，一次72b，换成这样的把
        //if (m_dealerMap.ContainsKey(id)) 
        //{
        //    return m_dealerMap[id];
        //} 
        //else 
        //{
        //    return null;
        //}
    }
}
