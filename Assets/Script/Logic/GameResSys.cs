using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResSys : ISystem {

    private static GameResSys _instance;
    public static GameResSys Instance
    {
        get { return _instance; }
    }

    public override void Init()
    {
        base.Init();
        _instance = this;
    }

    public Sprite GetNodeSprite(string spriteName)
    {
        CatDebug.LogFunc("sprite = " + spriteName);
        return ResourceSys.Instance.GetSprite("tex/icon/nodes", spriteName);
    }

    public Sprite GetCard(string spriteName)
    {
        return ResourceSys.Instance.GetSprite("tex/icon/cards", spriteName);
    }

    public Sprite GetItem(string spriteName)
    {
        return ResourceSys.Instance.GetSprite("tex/icon/item", spriteName);
    }

    public Sprite GetSkill(string spriteName)
    {
        return ResourceSys.Instance.GetSprite("tex/icon/skill", spriteName);
    }

    public Sprite GetWeapon(string spriteName)
    {
        return ResourceSys.Instance.GetSprite("tex/icon/weapon", spriteName);
    }

    public Sprite GetBuff(string spriteName)
    {
        return ResourceSys.Instance.GetSprite("tex/icon/buff", spriteName);
    }

    public Sprite GetFrame(int lv)
    {
        string framePath = GameConstants.FramePath + lv;
        return ResourceSys.Instance.GetSprite("tex/frame", framePath);
    }

    public Sprite GetCG(string cgName)
    {
        return ResourceSys.Instance.GetSprite("tex/cg", cgName);
    }

    public Sprite GetMask(string maskName)
    {
        return ResourceSys.Instance.GetSprite("tex/icon/mask", maskName);
    }
}
