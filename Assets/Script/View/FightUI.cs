using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class FightUI : BaseUI
{

    UINode _stageDetail;
    Transform _stageNodeTrans;
    Transform _stageLineTrans;
    Transform _stageLayerTrans;
    GameObject _nodeModel;
    GameObject _lineModel;
    GameObject _layerNameModel;

    UINode _fightNode;
    UINode _rewardNode;
    
    Dictionary<int, Transform> _nodeUIs;
    Dictionary<int, Transform> _nodeLines;

    Canvas _stageCanvas;

    private const int LayerGap = 150;
    private const int LayerNameWidth = 200;
    int _layerHeight;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        InitStageUi(rootNode);
        InitFightNodeUi(rootNode);
        InitRewardNodeUi(rootNode);

        EventSys.Instance.AddHander(LogicEvent.DrawFightStageUi, OnDrawFightUi);
    }

    void InitStageUi(UINode rootNode)
    {

        UINode sRoot = rootNode.GetNode("Stage");
        _stageCanvas = sRoot.GetComponent<Canvas>();

        Button btnExit = sRoot.GetRef("ButtonExit").GetComponent<Button>();
        btnExit.onClick.AddListener(OnBtnExitClicked);

        _stageDetail = sRoot.GetNode("StageDetail");
        _stageNodeTrans = _stageDetail.GetRef("Stage").transform;
        _stageLineTrans = _stageDetail.GetRef("Line").transform;
        _stageLayerTrans = _stageDetail.GetRef("LayerNames").transform;
        _nodeModel = _stageDetail.GetRef("Node_model").gameObject;
        _lineModel = _stageDetail.GetRef("Slider_model").gameObject;
        _layerNameModel = _stageDetail.GetRef("LayerName_model").gameObject;
    }

    void InitFightNodeUi(UINode rootNode)
    {
        _fightNode = rootNode.GetNode("FightNodeDetail");
        Button fightBtn = _fightNode.GetRef("Fight").GetComponent<Button>();
        fightBtn.onClick.AddListener(()=> { });
        Button exitBtn = _fightNode.GetRef("Exit").GetComponent<Button>();
        exitBtn.onClick.AddListener(() => { _fightNode.gameObject.SetActive(false); });

        _fightNode.gameObject.SetActive(false);
    }

    void InitRewardNodeUi(UINode rootNode)
    {
        _rewardNode = rootNode.GetNode("RewardNodeDetail");
        Button fightBtn = _rewardNode.GetRef("Go").GetComponent<Button>();
        fightBtn.onClick.AddListener(() => { });
        Button exitBtn = _rewardNode.GetRef("Exit").GetComponent<Button>();
        exitBtn.onClick.AddListener(HideRewardView);

        _rewardNode.gameObject.SetActive(false);
    }

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(ViewEvent.ChangeState, typeof(CityState));
    }

    void OnDrawFightUi(int id, object p1, object p2)
    {
        StageConfig stageConfig = (StageConfig)p1;

        _nodeUIs = new Dictionary<int, Transform>();
        _nodeLines = new Dictionary<int, Transform>();

        _layerHeight = (Screen.height - LayerGap * 2) / stageConfig.Layers.Count;

        foreach (StageLayer stageLayer in stageConfig.Layers)
        {
            CreateLayer(stageLayer);
        }

        foreach (StageLayer stageLayer in stageConfig.Layers)
        {
            foreach (BaseStageNode node in stageLayer.Nodes)
            {
                Transform t = _nodeUIs[node.Id];
                foreach (int nextNode in node.NextNodes)
                {
                    Transform nextT = _nodeUIs[nextNode];
                    GameObject lineModel = Instantiate(_lineModel);
                    lineModel.transform.SetParent(_stageLineTrans);
                    SetLineUI(t.position, nextT.position, lineModel.GetComponent<Slider>());
                    _nodeLines.Add(node.Id * 100 + nextNode, lineModel.transform);
                }
            }
        }
    }

    void CreateLayer(StageLayer stageLayer)
    {
        CatDebug.LogFunc(GetHashCode());
        float fullWidth = Screen.width - LayerNameWidth;
        float nodeFullWidth = fullWidth / stageLayer.Nodes.Count;
        Vector2 nodeSize = _nodeModel.GetComponent<RectTransform>().sizeDelta;
        float nodeXLeft = LayerNameWidth + (nodeFullWidth - nodeSize.x) / 2;

        float posY = stageLayer.Layer * _layerHeight + LayerGap;
        posY += (_layerHeight - nodeSize.y) / 2.0f;

        foreach (BaseStageNode stageNode in stageLayer.Nodes)
        {
            float posX = nodeXLeft + nodeFullWidth * stageNode.Index;
            CreateNode(stageNode, posX, posY);
        }

        GameObject layerNameObj = Instantiate(_layerNameModel);
        layerNameObj.GetComponent<Text>().text = stageLayer.Name;
        layerNameObj.transform.SetParent(_stageLayerTrans);
        layerNameObj.transform.localPosition = new Vector3(50, posY, 0);
    }

    void CreateNode(BaseStageNode stageNode, float posX, float posY)
    {
        GameObject go = Instantiate(_nodeModel);
        go.transform.SetParent(_stageNodeTrans);
        go.GetComponent<Button>().onClick.AddListener(() => { OnNodeClicked(stageNode.Id, stageNode.GetType()); });
        go.transform.localPosition = new Vector3(posX, posY, 0);

        Image nodeImage = go.GetComponent<Image>();
        Sprite newSprite = ResourceSys.Instance.GetSprite(stageNode.Icon);
        nodeImage.sprite = newSprite;

        if(stageNode.NodeType.Equals(typeof(StageNodeStart).Name))
        {
            
            Dragable drag = go.AddComponent<Dragable>();
            drag.ActionId = stageNode.Id;
            drag.OnDragStart = OnDrag;
            drag.Canv = _stageCanvas;
            //drag.DragIcon = ResourceSys.Instance.GetSprite();
            //drag.HasTail = true;
            //drag.TailSprite = ResourceSys.Instance.GetSprite();
            //drag.TailColor = ;
            //drag.TailWidth = 20;
        }
        else
        {
            Dropable drop = go.AddComponent<Dropable>();
            drop.ActionId = stageNode.Id;
            drop.OnDroped = OnDrop;
        }
        
        _nodeUIs.Add(stageNode.Id, go.transform);
    }

    /// <summary>
    /// slider 由p1指向p2
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="mySlider"></param>
    void SetLineUI(Vector3 p1, Vector3 p2, Slider mySlider)
    {
        float centerX = (p1.x + p2.x) / 2;
        float centerY = (p1.y + p2.y) / 2;

        float distance = Vector3.Distance(p1, p2);

        mySlider.transform.position = new Vector3(centerX, centerY, 0);
        mySlider.transform.localEulerAngles = UIUtils.GetEulerAngle(p1, p2);
        mySlider.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, 20);
        mySlider.GetComponent<Slider>().value = 0.3f;
    }

    void OnNodeClicked(int id, System.Type type)
    {
        Debug.Log("OnNodeClicked " + id);
        //EventSys.Instance.AddEvent(ViewEvent.ClickFightNode, id);
        if (type == typeof(StageNodeFight))
        {
            ShowNodeFight(id);
        }
        else if(type == typeof(StageNodeReward))
        {
            ShowNodeReward(id);
        }
    }

    public string GetPropertyDescription1(Enemy enemy)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("精 - ").AppendLine(enemy.CreatureData.Hp.Value.ToString());
        sb.Append("气 - ").AppendLine(enemy.CreatureData.Mp.Value.ToString());
        sb.Append("神 - ").AppendLine(enemy.CreatureData.Mp.Value.ToString());
        
        return sb.ToString();
    }

    public string GetPropertyDescription2(Enemy enemy)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("攻 - ").AppendLine(enemy.CreatureData.Att.Value.ToString());
        sb.Append("防 - ").AppendLine(enemy.CreatureData.Def.Value.ToString());
        
        return sb.ToString();
    }

    void ShowNodeFight(int id)
    {
        Enemy enemy = FightDataMgr.Instance.GetEnemy(id);
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

    void ShowNodeReward(int id)
    {
        Reward reward = FightDataMgr.Instance.GetReward(id);

        UINode itemNode = _rewardNode.GetNode("Item_model");
        Transform scTrans = _rewardNode.GetRef("Content");
        foreach (Item item in reward.Data)
        {
            GameObject newNodeObj = GameObject.Instantiate(itemNode.gameObject, scTrans);
            //newNodeObj.transform.SetParent(scTrans);
            
            UINode newNode = newNodeObj.GetComponent<UINode>();
            Image bg = newNode.GetRef("Bg").GetComponent<Image>();
            bg.sprite = ResourceSys.Instance.GetFrame(item.Lv.Value);
            Image icon = newNode.GetRef("Icon").GetComponent<Image>();
            icon.sprite = ResourceSys.Instance.GetSprite(item.Icon);
            Text itemName = newNode.GetRef("Name").GetComponent<Text>();
            itemName.text = item.Name;
            if(item.Count.Value > 1)
            {
                itemName.text = item.Name + " * " + item.Count.Value;
            }
            Text itemDes = newNode.GetRef("Des").GetComponent<Text>();
            itemDes.text = item.Description;

            newNodeObj.SetActive(true);
        }
        
        //set scroll view height
        //float itemHeight = itemNode.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        //float height = reward.Data.Count * itemHeight;
        //Transform scrollViewRef = _rewardNode.GetRef("Scroll View");
        //RectTransform rt = scrollViewRef.GetComponent<RectTransform>();
        //rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);

        itemNode.gameObject.SetActive(false);
        _rewardNode.gameObject.SetActive(true);
    }

    void HideRewardView()
    {
        Transform scTrans = _rewardNode.GetRef("Content");
        List<Transform> tobeDel = new List<Transform>();
        foreach (Transform subTrans in scTrans)
        {
            if(subTrans.name.Equals("Item_model"))
            {
                continue;
            }
            tobeDel.Add(subTrans);
        }
        for (int i = 0; i < tobeDel.Count;i++ )
        {
            Destroy(tobeDel[i].gameObject);
        }

        _rewardNode.gameObject.SetActive(false);
    }

    void OnDrag(int nodeID)
    {
        Debug.Log("OnDrag = " + nodeID);
    }

    void OnDrop(int nodeID)
    {
        Debug.Log("OnDrop = " + nodeID);
    }


}
