using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HeroNodeView : BaseView {

    UINode _fightNode;
    int _nodeId;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        _fightNode = rootNode.GetComponent<UINode>();
        
        Button exitBtn = _fightNode.GetRef("Confirm").GetComponent<Button>();
        exitBtn.onClick.AddListener(() =>
        {
            _fightNode.gameObject.SetActive(false);
            EventSys.Instance.AddEvent(InputEvent.HeroNodeDetailComfirmed, _nodeId);
        });

        _fightNode.gameObject.SetActive(false);

        EventSys.Instance.AddHander(ViewEvent.ShowHeroNodeDetails, ShowNodeFight);
    }

    void ShowNodeFight(object p1, object p2)
    {
        HeroData cData = (HeroData)p1;
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

        Text job = _fightNode.GetRef("Job").GetComponent<Text>();
        job.text = GetJobDes(cData.Job);

        Text sex = _fightNode.GetRef("Sex").GetComponent<Text>();
        sex.text = GetSexDes(cData.Sex);

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

    public string GetJobDes(int jobVal)
    {
        StringBuilder sb = new StringBuilder();

        Dictionary<int, BaseDataInfo> res = ConfigDataMgr.Instance.GetData<PropertyDesTableData>();
        foreach (KeyValuePair<int, BaseDataInfo> pair in res)
        {
            PDesDataInfo pDesData = (PDesDataInfo)pair.Value;
            if(pDesData.PropertyName.Equals("Job"))
            {
                sb.Append(pDesData.Description).Append(" ");
                for (int i=0;i<pDesData.Values.Count;i++)
                {
                    if(pDesData.Values[i].Equals(jobVal.ToString()))
                    {
                        sb.Append(pDesData.ValueDes[i]);
                    }
                }
            }
        }
        return sb.ToString();
    }

    public string GetSexDes(int sexVal)
    {
        StringBuilder sb = new StringBuilder();

        Dictionary<int, BaseDataInfo> res = ConfigDataMgr.Instance.GetData<PropertyDesTableData>();
        foreach (KeyValuePair<int, BaseDataInfo> pair in res)
        {
            PDesDataInfo pDesData = (PDesDataInfo)pair.Value;
            if (pDesData.PropertyName.Equals("Sex"))
            {
                sb.Append(pDesData.Description).Append(" ");
                for (int i = 0; i < pDesData.Values.Count; i++)
                {
                    if (pDesData.Values[i].Equals(sexVal.ToString()))
                    {
                        sb.Append(pDesData.ValueDes[i]);
                    }
                }
            }
        }
        return sb.ToString();
    }
}
