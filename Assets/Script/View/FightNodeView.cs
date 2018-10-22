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
        Enemy enemy = (Enemy)p1;
        _nodeId = (int)p2;
        Image icon = _fightNode.GetRef("Icon").GetComponent<Image>();
        icon.sprite = ResourceSys.Instance.GetSprite(enemy.CreatureData.Cg);

        Text detail1 = _fightNode.GetRef("Info1").GetComponent<Text>();
        detail1.text = GetPropertyDescription1(enemy);
        Text detail2 = _fightNode.GetRef("Info2").GetComponent<Text>();
        detail2.text = GetPropertyDescription2(enemy);


        Text enemyName = _fightNode.GetRef("Name").GetComponent<Text>();
        enemyName.text = enemy.CreatureData.Name + " Lv" + enemy.CreatureData.Lv.Value;

        Text des = _fightNode.GetRef("Des").GetComponent<Text>();
        des.text = enemy.CreatureData.Description;

        _fightNode.gameObject.SetActive(true);
    }

    public string GetPropertyDescription1(Enemy enemy)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("血 - ").AppendLine(enemy.CreatureData.Hp.Value.ToString());
        sb.Append("气 - ").AppendLine(enemy.CreatureData.Mp.Value.ToString());

        return sb.ToString();
    }

    public string GetPropertyDescription2(Enemy enemy)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("攻 - ").AppendLine(enemy.CreatureData.Att.Value.ToString());
        sb.Append("防 - ").AppendLine(enemy.CreatureData.Def.Value.ToString());

        return sb.ToString();
    }
}
