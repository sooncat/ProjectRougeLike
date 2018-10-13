using System.Collections;
using System.Collections.Generic;
using com.initialworld.framework;
using UnityEngine;

public class CreatureData {

    public int Id;
    public string Name;
    public string Description;

    //base property
    public ENum<int> Lv;
    public ENum<int> Sex;
    public CENum<int> Hp;
    public CENum<int> Mp;
    public CENum<int> Def;
    public CENum<int> Att;

    //advanced property
    public CENum<int> HpMax;
    public float HpPercent
    {
        get { return (float)Hp.Value / HpMax.Value; }
    }
    public CENum<int> MpMax;
    public float MpPercent
    {
        get { return (float)Mp.Value / MpMax.Value; }
    }

    public CreatureData()
    {
        
    }
}
