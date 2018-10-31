using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class RewardNodeView : BaseView {

    UINode _rewardNode;
    UINode _models;
    Button _getBtn;
    Button _exitBtn;

    int _heroId, _nodeId;
    

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        _rewardNode = rootNode.GetComponent<UINode>();
        _getBtn = _rewardNode.GetRef("Get").GetComponent<Button>();
        _getBtn.onClick.AddListener(GetReward);
        _getBtn.gameObject.SetActive(false);

        _exitBtn = _rewardNode.GetRef("Confirm").GetComponent<Button>();
        _exitBtn.onClick.AddListener(HideRewardView);
        _exitBtn.gameObject.SetActive(false);

        _models = rootNode.GetNode("Models");

        _rewardNode.gameObject.SetActive(false);

        EventSys.Instance.AddHander(ViewEvent.ShowNodeRewardDetails, ShowNodeReward);
        EventSys.Instance.AddHander(ViewEvent.ShowNodeRewardGet, GetNodeReward);
    }

    void CreateReward(Reward reward)
    {
        UINode itemNode = _models.GetNode("Item_model");
        Transform scTrans = _rewardNode.GetRef("Content");
        reward.ConditionData.Sort();
        foreach (Reward.Condition cItem in reward.ConditionData)
        {
            if (!string.IsNullOrEmpty(cItem.Express))
            {
                //show condition
                UINode conditionNode = _models.GetNode("Condition_model");
                GameObject conditionObj = Instantiate(conditionNode.gameObject, scTrans);

                UINode cNode = conditionObj.GetComponent<UINode>();
                Dictionary<int, BaseDataInfo> res = ConfigDataMgr.Instance.GetData<PropertyDesTableData>();
                string showExpress = "";
                string showVal = "";
                foreach (KeyValuePair<int, BaseDataInfo> pair in res)
                {
                    PDesDataInfo info = (PDesDataInfo)pair.Value;
                    if(info.PropertyName.Equals(cItem.Express))
                    {
                        showExpress = info.Description;
                        for (int i = 0; i < info.Values.Count;i++ )
                        {
                            if(cItem.Val.Equals(info.Values[i]))
                            {
                                showVal = info.ValueDes[i];
                                break;
                            }
                        }
                    }
                }
                cNode.GetRef("Condition").GetComponent<Text>().text = "如果<color=#4040DC>" + showExpress + "=" + showVal+"</color>";
            }
            foreach (Item item in cItem.Rewards)
            {
                GameObject newNodeObj = Instantiate(itemNode.gameObject, scTrans);
                //newNodeObj.transform.SetParent(scTrans);

                UINode newNode = newNodeObj.GetComponent<UINode>();
                Image bg = newNode.GetRef("Bg").GetComponent<Image>();
                bg.sprite = GameResSys.Instance.GetFrame(item.Lv.Value);
                Image icon = newNode.GetRef("Icon").GetComponent<Image>();
                icon.sprite = GameResSys.Instance.GetItem(item.Icon);
                Text itemName = newNode.GetRef("Name").GetComponent<Text>();
                itemName.text = item.Name;
                if (item.Count.Value > 1)
                {
                    itemName.text = item.Name + " * " + item.Count.Value;
                }
                Text itemDes = newNode.GetRef("Des").GetComponent<Text>();
                itemDes.text = item.Description;

                newNodeObj.SetActive(true);    
            }
        }
        itemNode.gameObject.SetActive(false);

        _rewardNode.GetRef("Scroll View").GetComponent<ScrollRect>().ScrollToTop();
        _rewardNode.gameObject.SetActive(true);
    }

    void ShowNodeReward(object p1, object p2)
    {
        Reward reward = (Reward)p1;
        
        CreateReward(reward);
        _exitBtn.gameObject.SetActive(true);

    }

    void GetNodeReward(object p1, object p2)
    {
        Reward reward = (Reward)p1;
        _nodeId = ((int[])p2)[0];
        _heroId = ((int[])p2)[1];

        CreateReward(reward);
        _getBtn.gameObject.SetActive(true);
    }

    void Clear()
    {
        _rewardNode.GetRef("Content").DestroyChildren();
        _rewardNode.gameObject.SetActive(false);
        _getBtn.gameObject.SetActive(false);
        _exitBtn.gameObject.SetActive(false);
    }

    void HideRewardView()
    {
        Clear();

        EventSys.Instance.AddEvent(InputEvent.RewardNodeDetailComfirmed);
    }

    void GetReward()
    {
        Clear();

        EventSys.Instance.AddEvent(InputEvent.RewardNodeGet, _nodeId, _heroId);
    }
    
}
