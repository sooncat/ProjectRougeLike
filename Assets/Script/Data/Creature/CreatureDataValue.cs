using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    BuffEffect,
    WeaponValue,
    Equip,
    Total,
    TotalScale,
}

public class CreatureDataValue<T>
{

        //public string Name;
		
        //public T TotalValue 
        //{
        //    get
        //    {
        //        return GetValue(CreatureAttributeValueType.Total).Value;
        //    }
        //}
        
        //Dictionary<int, ENumber.ENum<T>> _datas;

        //public CreatureDataValue(T originalValue)
        //{
        //    SetValue(CreatureAttributeValueType.Original, originalValue);
        //    RefreshValue();
        //    Init();
        //}

        //public CreatureDataValue(string name)
        //{
        //    Name = name;
        //    Init();
        //}
        
        //public void Init()
        //{
        //    _datas = new Dictionary<int, EvalueData<T>>();
        //    _datas.Add((int)CreatureAttributeValueType.Original, new EvalueData<T>());
        //    _datas.Add((int)CreatureAttributeValueType.Debug, new EvalueData<T>());
        //    _datas.Add((int)CreatureAttributeValueType.BuffEffect, new EvalueData<T>());
        //    _datas.Add((int)CreatureAttributeValueType.WeaponValue, new EvalueData<T>());
        //    _datas.Add((int)CreatureAttributeValueType.Equip, new EvalueData<T>());
        //    _datas.Add((int)CreatureAttributeValueType.Total, new EvalueData<T>());
        //    _datas.Add((int)CreatureAttributeValueType.TotalScale, new EvalueData<T>());
        //}

        //public void SetValue(CreatureAttributeValueType type, T value)
        //{
        //    var v = new EZFunNumber<T>(value);
        //    SetValue(type, v);
        //}

        //public void SetValue(CreatureAttributeValueType type, EvalueData<T> value)
        //{
        //    SetValue(type, value.m_value);
        //}

        //public void SetValue(CreatureAttributeValueType type, EZFunNumber<T> value)
        //{
        //    if (type == CreatureAttributeValueType.Total)
        //    {
        //        throw new ArgumentException("Don't Support Set TotalValue");
        //    }
        //    //double beforeValue = _datas[(int)type].m_value.ToDouble();
        //    _datas[(int)type].m_value = value;
        //    //double curValue = _datas[(int)type].m_value.ToDouble();
        //    RefreshValue();
        //}

        //public EZFunNumber<T> GetValue(CreatureAttributeValueType type)
        //{
        //    return _datas[(int)type].m_value;
        //}

        //public void AddValue(CreatureAttributeValueType type, T value)
        //{
        //    if (type == CreatureAttributeValueType.Total)
        //    {
        //        throw new ArgumentException("Don't Support Add TotalValue");
        //    }
        //    //double beforeValue = _datas[(int)type].m_value.ToDouble();
        //    var v = new EZFunNumber<T>(value);
        //    _datas[(int)type].m_value += v;
        //    //double curvalue = _datas[(int)type].m_value.ToDouble();
            
        //    RefreshValue();
            
        //}

        //public void MinusValue(CreatureAttributeValueType type, T value)
        //{
        //    if (type == CreatureAttributeValueType.Total)
        //    {
        //        throw new ArgumentException("Don't Support Add TotalValue");
        //    }
        //    //double beforeValue = _datas[(int)type].m_value.ToDouble();
        //    var v = new EZFunNumber<T>(value);
        //    _datas[(int)type].m_value -= v;
        //    //double curvalue = _datas[(int)type].m_value.ToDouble();
            
        //    RefreshValue();
            
        //}

        //public EvalueData<T> GetCValue(CreatureAttributeValueType type)
        //{
        //    return _datas[(int)type];
        //}

        //public void RefreshValue()
        //{
        //    var v =
        //        (
        //        GetValue(CreatureAttributeValueType.Original) +
        //        GetValue(CreatureAttributeValueType.Equip) +
        //        GetValue(CreatureAttributeValueType.WeaponValue) +
        //        GetValue(CreatureAttributeValueType.BuffEffect) +
        //        GetValue(CreatureAttributeValueType.Debug)
        //        ) *
        //        GetValue(CreatureAttributeValueType.TotalScale);
            
        //    SetValue(CreatureAttributeValueType.Total, v);   
        //}
}
