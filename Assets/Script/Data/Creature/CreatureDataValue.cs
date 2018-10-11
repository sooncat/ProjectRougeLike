using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public enum CreatureAttributeType
{
    MaxHP,
    CurrentHP,
    PhysicalAttack,
    PhysicalDefense,
    MagicDefense,
    Hit,
    Dodge,
    Crit,
    Parry,
    BuffDamageAdd,
    BuffAttackedDamageAdd,
    CritRate,
    DodgeRate,
    ShieldValue,
    BuffAddHitRate,
	BuffTenacity,
	CritValueRate,
}

public enum CreatureAttributeValueType
{
    Original,
    Debug,
    Scale,
    Total,
}

public class CreatureDataValue<T>
{

    const int scaleType = -999;

    public string Name;

    public T TotalValue
    {
        get
        {
            return _total.Value;
        }
    }

    Dictionary<int, ENum<T>> _data;
    ENum<T> _total;
    ENum<float> _scale; 

    public CreatureDataValue(T originalValue) : 
        this(0, originalValue, string.Empty)
    {}

    public CreatureDataValue(T originalValue, string name):
        this(0, originalValue, string.Empty)
    {}

    public CreatureDataValue(int type, T originalValue, string name)
    {
        Name = name;
        Init();
        SetValue(type, originalValue);
        RefreshValue();
    }

    public void Init()
    {
        _data = new Dictionary<int, ENum<T>>();
        _data.Add((int)CreatureAttributeValueType.Original, new ENum<T>());
        _data.Add((int)CreatureAttributeValueType.Total, new ENum<T>());
        _scale = new ENum<float>(1);
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
        RefreshValue();
    }

    public ENum<T> GetValue(int type)
    {
        if(_data.ContainsKey(type))
            return _data[type];
        return new ENum<T>();
    }

    public void AddValue(int type, T value)
    {
        //double beforeValue = _datas[(int)type].m_value.ToDouble();
        var v = new ENum<T>(value);
        _data[type].Value += v.Value;
        //double curvalue = _datas[(int)type].m_value.ToDouble();
        RefreshValue();

    }

    public void MinusValue(int type, T value)
    {
        //double beforeValue = _datas[(int)type].m_value.ToDouble();
        var v = new ENum<T>(value);
        _data[type].Value -= v.Value;
        //double curvalue = _datas[(int)type].m_value.ToDouble();
        RefreshValue();
    }

    public void RefreshValue()
    {
        var v =
            (
            GetValue(CreatureAttributeValueType.Original) +
            GetValue(CreatureAttributeValueType.Equip) +
            GetValue(CreatureAttributeValueType.WeaponValue) +
            GetValue(CreatureAttributeValueType.BuffEffect) +
            GetValue(CreatureAttributeValueType.Debug)
            ) *
            _scale.Value;

        _total.Value = v;
    }
}
