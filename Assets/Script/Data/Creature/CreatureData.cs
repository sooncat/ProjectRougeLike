using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureData {

    public int Id;
    public string Name;
    public string Description;

    //base property
    public int Lv;
    public int Sex;
    public int Hp;
    public int Mp;
    public int Def;
    public int Att;

    //advanced property
    public int HpMax;
    public float HpPercent
    {
        get { return (float)Hp / HpMax; }
    }
    public int MpMax;
    public float MpPercent
    {
        get { return (float)Mp / MpMax; }
    }
}
