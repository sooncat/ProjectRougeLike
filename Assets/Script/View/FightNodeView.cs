using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FightNodeView : BaseView {

    UINode _fightNode;
    int _nodeId;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        _fightNode = rootNode.GetComponent<UINode>();
        //Button fightBtn = _fightNode.GetRef("Fight").GetComponent<Button>();
        //fightBtn.onClick.AddListener(() => { });
        Button exitBtn = _fightNode.GetRef("Confirm").GetComponent<Button>();
        exitBtn.onClick.AddListener(() =>
        {
            _fightNode.gameObject.SetActive(false);
            EventSys.Instance.AddEvent(InputEvent.FightNodeDetailComfirmed, _nodeId);
        });

        _fightNode.gameObject.SetActive(false);

        EventSys.Instance.AddHander(ViewEvent.ShowNodeFightDetails, ShowNodeFight);
    }

    void ShowNodeFight(object p1, object p2)
    {
        CreatureData cData = (CreatureData)p1;
        _nodeId = (int)p2;
        Image icon = _fightNode.GetRef("Icon").GetComponent<Image>();
        icon.sprite = ResourceSys.Instance.GetSprite(cData.Cg);

        Text detail1 = _fightNode.GetRef("Info1").GetComponent<Text>();
        detail1.text = GetPropertyDescription1(cData);
        Text detail2 = _fightNode.GetRef("Info2").GetComponent<Text>();
        detail2.text = GetPropertyDescription2(cData);


        Text enemyName = _fightNode.GetRef("Name").GetComponent<Text>();
        enemyName.text = cData.Name + " Lv" + cData.Lv.Value;

        Text des = _fightNode.GetRef("Des").GetComponent<Text>();
        des.text = cData.Description;

        _fightNode.gameObject.SetActive(true);
    }

    public string GetPropertyDescription1(CreatureData cData)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("血 - ").AppendLine(cData.Hp.Value.ToString());
        sb.Append("气 - ").AppendLine(cData.Mp.Value.ToString());

        return sb.ToString();
    }

    public string GetPropertyDescription2(CreatureData cData)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("攻 - ").AppendLine(cData.Att.Value.ToString());
        sb.Append("防 - ").AppendLine(cData.Def.Value.ToString());

        return sb.ToString();
    }
}
