using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        foreach (Item item in reward.Data)
        {
            GameObject newNodeObj = Instantiate(itemNode.gameObject, scTrans);
            //newNodeObj.transform.SetParent(scTrans);

            UINode newNode = newNodeObj.GetComponent<UINode>();
            Image bg = newNode.GetRef("Bg").GetComponent<Image>();
            bg.sprite = ResourceSys.Instance.GetFrame(item.Lv.Value);
            Image icon = newNode.GetRef("Icon").GetComponent<Image>();
            icon.sprite = ResourceSys.Instance.GetSprite(item.Icon);
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
        itemNode.gameObject.SetActive(false);
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
