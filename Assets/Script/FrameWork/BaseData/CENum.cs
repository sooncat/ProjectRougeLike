using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;



/// <summary>
    /// Complex Encrypt Number 复杂加密数值
/// </summary>
/// <typeparam name="T"></typeparam>
public class CENum<T>
{
    public string Name
    {
        get;
        private set;
    }

    public T Value
    {
        get
        {
            return _total.Value;
        }
    }

    Dictionary<int, ENum<T>> _data;
    Dictionary<int, ENum<float>> _scaleData;
    ENum<T> _dataTotal;
    ENum<float> _scaleTotal;
    ENum<T> _total;

    //public CreatureDataValue(T originalValue) : 
    //    this(0, originalValue, string.Empty)
    //{}

    public CENum(): this(string.Empty)
    {
    }

    public CENum(string name)
    {
        Name = name;
        Init();
    }

    public CENum(int type, T originalValue) : this(type, originalValue, string.Empty)
    {
    }

    public CENum(int type, T originalValue, string name)
    {
        Name = name;
        Init();
        SetValue(type, originalValue);
    }

    public void Init()
    {
        _data = new Dictionary<int, ENum<T>>();
        _scaleData = new Dictionary<int, ENum<float>>();
        _scaleTotal = new ENum<float>(1); //默认值
        _dataTotal = new ENum<T>();
    }

    public void SetValue(int type, T value)
    {
        SetValue(type, new ENum<T>(value));
    }

    public void SetValue(int type, ENum<T> value)
    {
        //double beforeValue = _datas[(int)type].m_value.ToDouble();
        if(!_data.ContainsKey(type))
        {
            _data.Add(type, value);
        }
        else
        {
            _data[type] = value;    
        }
        //double curValue = _datas[(int)type].m_value.ToDouble();
        RefreshValue(true, false);
    }

    public void SetScaleValue(int type, float value)
    {
        
    }

    public void SetScaleValue(int type, ENum<float> value)
    {
        RefreshValue(false, true);
    }

    public void RemoveValue(int type)
    {
        if (_data.ContainsKey(type))
        {
            _data.Remove(type);
        }
        RefreshValue(true, false);
    }

    public void RemoveScaleValue(int type)
    {
        if (_scaleData.ContainsKey(type))
        {
            _scaleData.Remove(type);
        }
        RefreshValue(false, true);
    }

    public ENum<T> GetValue(int type)
    {
        if(_data.ContainsKey(type))
            return _data[type];
        return new ENum<T>();
    }

    //public void AddValue(int type, T value)
    //{
    //    //double beforeValue = _datas[(int)type].m_value.ToDouble();
    //    var v = new ENum<T>(value);
    //    _data[type].Value += v.Value;
    //    //double curvalue = _datas[(int)type].m_value.ToDouble();
    //    RefreshValue();
    //}

    //public void MinusValue(int type, T value)
    //{
    //    //double beforeValue = _datas[(int)type].m_value.ToDouble();
    //    var v = new ENum<T>(value);
    //    _data[type].Value -= v.Value;
    //    //double curvalue = _datas[(int)type].m_value.ToDouble();
    //    RefreshValue();
    //}

    public void RefreshValue(bool refreshData, bool refreshScale)
    {
        if(refreshData)
        {
            _dataTotal.Value = CalSum(_data).Value;
        }
        if(refreshScale)
        {
            _scaleTotal.Value = 1;
            if(_scaleData.Count > 0)
            {
                _scaleTotal.Value = 0;
                foreach (KeyValuePair<int, ENum<float>> pair in _scaleData)
                {
                    _scaleTotal.Value += pair.Value.Value;
                }
            }
        }

        if(typeof(T) == typeof(int) || typeof(T) == typeof(long))
        {
            long temp = (long)Convert.ChangeType(_dataTotal.Value, typeof(long));
            _total.Value = (T)Convert.ChangeType(_scaleTotal.Value * temp, typeof(T)); 
        }
        else if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
        {
            double temp = (double)Convert.ChangeType(_dataTotal.Value, typeof(double));
            _total.Value = (T)Convert.ChangeType(_scaleTotal.Value * temp, typeof(T)); 
        }
    }

    static ENum<T> CalSum(Dictionary<int, ENum<T>> data)
    {
        ENum<T> result = new ENum<T>();

        long tempLong = 0;
        double tempDouble = 0;

        bool isLong = (typeof(T) == typeof(int) || typeof(T) == typeof(long));
        bool isDouble = (typeof(T) == typeof(float) || typeof(T) == typeof(double));

        foreach (KeyValuePair<int, ENum<T>> keyValuePair in data)
        {
            ENum<T> eNum = keyValuePair.Value;
            if (isLong)
            {
                long longNum = (long)Convert.ChangeType(eNum.Value, typeof(long));
                tempLong += longNum;
            }
            else if (isDouble)
            {
                double doubleNum = (double)Convert.ChangeType(eNum.Value, typeof(double));
                tempDouble += doubleNum;
            }
        }
        if (isLong)
        {
            result.Value = (T)Convert.ChangeType(tempLong, typeof(T));
        }
        else if (isDouble)
        {
            result.Value = (T)Convert.ChangeType(tempDouble, typeof(T));
        }

        return result;
    }
}
