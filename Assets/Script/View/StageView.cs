using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class StageView : BaseView
{

    //UINode _stageDetail;
    Transform _stageNodeRootTrans;
    Transform _stageLineRootTrans;
    Transform _stageLayerRootTrans;
    GameObject _nodeModel;
    GameObject _lineModel;
    GameObject _layerNameModel;
    GameObject _heroShowModel;

    UINode _stageScrollContent;

    UINode _stageNode;
    UINode _fightNode;
    UINode _rewardNode;
    
    Dictionary<int, Transform> _nodeUIs;
    Dictionary<int, Transform> _nodeLines;

    /// <summary>
    /// 选择英雄出战时，界面最下方横排的NodeList
    /// { Key:heroId, Value:nodeTransform }
    /// </summary>
    Dictionary<int, Transform> _heroSelectNodes;

    /// <summary>
    /// 被选中出战的英雄对应的Node.
    /// { Key:heroId, Value:nodeTransform }
    /// </summary>
    Dictionary<int, Transform> _fightHeroNodes;
    Canvas _stageCanvas;

    int _dragId;

    private const int BottomGap = 50;
    private const int LayerNameWidth = 200;
    readonly int _layerHeight = 150;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        InitStageUi(rootNode);
        InitFightNodeUi(rootNode);
        InitRewardNodeUi(rootNode);

        EventSys.Instance.AddHander(ViewEvent.CreateStageView, OnCreatStageView);
        EventSys.Instance.AddHander(ViewEvent.FightSubStateMapping, OnFightStateMapping);
        EventSys.Instance.AddHander(ViewEvent.CreateHeroStartNode, OnCreateHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ReplaceHeroStartNode, OnReplaceHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.RemoveHeroStartNode, OnRemoveHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ResetHeroStartNode, OnResetHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ExchangeHeroStartNode, OnExchangeHeroStartNode);
        
    }

    void InitStageUi(UINode rootNode)
    {

        _stageNode = rootNode.GetNode("Stage");
        _stageCanvas = _stageNode.GetComponent<Canvas>();

        Button btnExit = _stageNode.GetRef("ButtonExit").GetComponent<Button>();
        btnExit.onClick.AddListener(OnBtnExitClicked);

        UINode modelNode = _stageNode.GetNode("Models");
        _nodeModel = modelNode.GetRef("Node_model").gameObject;
        _lineModel = modelNode.GetRef("Slider_model").gameObject;
        _layerNameModel = modelNode.GetRef("LayerName_model").gameObject;
        _heroShowModel = modelNode.GetNode("HeroShow_model").gameObject;

        _stageScrollContent = _stageNode.GetNode("Content");
        _stageNodeRootTrans = _stageScrollContent.GetRef("Stage").transform;
        _stageLineRootTrans = _stageScrollContent.GetRef("Line").transform;
        _stageLayerRootTrans = _stageScrollContent.GetRef("LayerNames").transform;

        Button btnReady = _stageNode.GetRef("ButtonReady").GetComponent<Button>();
        btnReady.onClick.AddListener(OnBtnReadyClicked);


        _stageNode.GetRef("HeroShowtList").gameObject.SetActive(true);
        _stageNode.GetRef("HeroShowtList").gameObject.SetActive(false);
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
        fightBtn.onClick.AddListener(() => { EventSys.Instance.AddEvent(InputEvent.FightReady); });
        Button exitBtn = _rewardNode.GetRef("Exit").GetComponent<Button>();
        exitBtn.onClick.AddListener(HideRewardView);

        _rewardNode.gameObject.SetActive(false);
    }

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.FightExit);
    }

    void OnBtnReadyClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.FightReady);
    }

    void OnFightStateMapping(int id, object p1, object p2)
    {
        CatDebug.LogFunc();

        Transform hsl = _stageNode.GetRef("HeroSelectList");
        hsl.gameObject.SetActive(false);

        Transform readyBtn = _stageNode.GetRef("ButtonReady");
        readyBtn.gameObject.SetActive(false);

        foreach (KeyValuePair<int, Transform> pair in _nodeUIs)
        {
            Dropable dropable = pair.Value.GetComponent<Dropable>();
            dropable.enabled = true;
        }

        Dictionary<int, FightHero> heroes = (Dictionary<int, FightHero>)p1;

        foreach (KeyValuePair<int, Transform> pair in _fightHeroNodes)
        {
            Dragable dragable = pair.Value.GetComponent<Dragable>();
            HeroData hd = (HeroData)heroes[pair.Key].CreatureData;
            dragable.DragIcon = ResourceSys.Instance.GetSprite(hd.FightIcon);
        }

        //create hero show list
        CreateHeroShowList(heroes);
    }

    void CreateHeroShowList(Dictionary<int, FightHero> heros)
    {
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            FightHero fightHero = pair.Value;
            GameObject go = Instantiate(_heroShowModel);
            UINode node = go.GetComponent<UINode>();
            node.GetRef("Icon").GetComponent<Image>().sprite = ResourceSys.Instance.GetSprite(fightHero.CreatureData.Icon);
            node.GetRef("Name").GetComponent<Text>().text = fightHero.CreatureData.Name;
            node.GetRef("Hp").GetComponent<Text>().text = "血";
            node.GetRef("HpSlider").GetComponent<Slider>().value = fightHero.CreatureData.HpPercent;
            node.GetRef("Mp").GetComponent<Text>().text = "气";
            node.GetRef("MpSlider").GetComponent<Slider>().value = fightHero.CreatureData.MpPercent;
            go.transform.SetParent(_stageNode.GetRef("HeroShowListContent"));
        }

        _stageNode.GetRef("HeroShowtList").gameObject.SetActive(true);
    }

    void OnCreatStageView(int id, object p1, object p2)
    {
        StageConfig stageConfig = (StageConfig)p1;

        _nodeUIs = new Dictionary<int, Transform>();
        _nodeLines = new Dictionary<int, Transform>();

        //_layerHeight = (Screen.height - LayerGap * 2) / stageConfig.Layers.Count;

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
                    lineModel.transform.SetParent(_stageLineRootTrans);
                    SetLineUI(t.position, nextT.position, lineModel.GetComponent<Slider>());
                    _nodeLines.Add(node.Id * 100 + nextNode, lineModel.transform);
                }
            }
        }

        //set scroll view Height
        //float AllHeight = LayerHeight * stageConfig.Layers.Count + BottomGap;
        RectTransform scrollRect = _stageScrollContent.GetComponent<RectTransform>();
        scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, BottomGap);
        //scroll to bottom
        Transform scrollView = _stageNode.GetRef("StageScrollView");
        scrollView.GetComponent<ScrollRect>().ScrollToBottom();

        //CreateHeroSelectView
        CreateHeroSelectView();
    }

    /// <summary>
    /// 选择英雄UI
    /// </summary>
    void CreateHeroSelectView()
    {
        _heroSelectNodes = new Dictionary<int, Transform>();
        _fightHeroNodes = new Dictionary<int, Transform>();

        Transform heroSelectList = _stageNode.GetRef("HeroSelectList");
        Transform heroSelectListContent = _stageNode.GetRef("HeroSelectListContent");

        foreach (Hero hero in PlayerDataMgr.Instance.Heros)
        {
            HeroData hd = (HeroData)hero.CreatureData;

            GameObject go = InsHeroNode(hd, false);
            go.transform.SetParent(heroSelectListContent);
            go.GetComponent<Button>().onClick.AddListener(() => {  });
            
            _heroSelectNodes.Add(hd.Id, go.transform);
        }

        heroSelectList.gameObject.SetActive(true);
    }

    void CreateLayer(StageLayer stageLayer)
    {
        CatDebug.LogFunc(GetHashCode());
        float allNodeWidth = _stageScrollContent.GetComponent<RectTransform>().sizeDelta.x - LayerNameWidth;
        float singleNodeWidth = allNodeWidth / stageLayer.Nodes.Count;
        Vector2 modelNodeSize = _nodeModel.GetComponent<RectTransform>().sizeDelta;
        float nodeXLeft = LayerNameWidth + (singleNodeWidth - modelNodeSize.x) / 2;

        float posY = stageLayer.Layer * _layerHeight + BottomGap;
        posY += (_layerHeight - modelNodeSize.y) / 2.0f;

        foreach (BaseStageNode stageNode in stageLayer.Nodes)
        {
            float posX = nodeXLeft + singleNodeWidth * stageNode.Index;
            CreateNode(stageNode, posX, posY);
        }

        GameObject layerNameObj = Instantiate(_layerNameModel);
        layerNameObj.GetComponent<Text>().text = stageLayer.Name;
        layerNameObj.transform.SetParent(_stageLayerRootTrans);
        layerNameObj.transform.localPosition = new Vector3(50, posY, 0);
    }

    void CreateNode(BaseStageNode stageNode, float posX, float posY)
    {
        GameObject go = Instantiate(_nodeModel);
        go.transform.SetParent(_stageNodeRootTrans);
        go.GetComponent<Button>().onClick.AddListener(() => { OnNodeClicked(stageNode.Id, stageNode.GetType()); });
        go.transform.localPosition = new Vector3(posX, posY, 0);

        Image nodeImage = go.GetComponent<Image>();
        Sprite newSprite = ResourceSys.Instance.GetSprite(stageNode.Icon);
        nodeImage.sprite = newSprite;

        Dropable drop = go.AddComponent<Dropable>();
        drop.ActionId = stageNode.Id;
        drop.OnDroped = DropOnNode;
        if(!stageNode.NodeType.Equals(typeof(StageNodeStart).Name))
        {
            drop.enabled = false;
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
        //Debug.Log("OnNodeClicked " + id);
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
            GameObject newNodeObj = Instantiate(itemNode.gameObject, scTrans);
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

    void OnDrag(int nodeId)
    {
        //CatDebug.LogFunc(nodeId.ToString());
        _dragId = nodeId;
    }

    void DropOnNode(int nodeId)
    {
        //CatDebug.LogFunc(nodeId.ToString());
        EventSys.Instance.AddEvent(InputEvent.FightDragOnNode, _dragId, nodeId);
    }

    void DropOnHero(int heroId)
    {
        EventSys.Instance.AddEvent(InputEvent.FightDragOnHero, _dragId, heroId);
    }

    void OnCreateHeroStartNode(int id, object p1, object p2)
    {
        CatDebug.LogFunc();

        HeroData heroData = (HeroData)p1;
        int targetNodeId = (int)p2;

        if(_heroSelectNodes.ContainsKey(heroData.Id))
        {
            Dragable d = _heroSelectNodes[heroData.Id].GetComponent<Dragable>();
            d.SetEnable(false);
        }

        GameObject go = InsHeroNode(heroData, false);//这里的英雄还可能与其他英雄交换，故不添加特殊dragicon
        go.transform.SetParent(_stageNodeRootTrans);
        go.GetComponent<Button>().onClick.AddListener(() => { });

        Dropable drop = go.AddComponent<Dropable>();
        drop.ActionId = heroData.Id;
        drop.OnDroped = DropOnHero;

        Transform target = _nodeUIs[targetNodeId];
        go.transform.position = target.position;

        _fightHeroNodes.Add(heroData.Id, go.transform);
    }

    void OnReplaceHeroStartNode(int id, object p1, object p2)
    {
        HeroData newHeroData = (HeroData)p1;
        int targetHeroId = ((int[])p2)[0];
        int targetNodeId = ((int[])p2)[1];

        Transform node = _fightHeroNodes[targetHeroId];
        Destroy(node.gameObject);
        _fightHeroNodes.Remove(targetHeroId);
        if (_heroSelectNodes.ContainsKey(targetHeroId))
        {
            Dragable d = _heroSelectNodes[targetHeroId].GetComponent<Dragable>();
            d.SetEnable(true);
        }

        OnCreateHeroStartNode(0, newHeroData, targetNodeId);
    }

    void OnRemoveHeroStartNode(int id, object p1, object p2)
    {
        
    }

    void OnResetHeroStartNode(int id, object p1, object p2)
    {
        int heroId = (int)p1;
        int targetNodeId = (int)p2;

        Transform node = _fightHeroNodes[heroId];
        Transform target = _nodeUIs[targetNodeId];
        node.transform.position = target.position;
    }

    void OnExchangeHeroStartNode(int id, object p1, object p2)
    {
        int hero1Id = ((int[])p1)[0];
        int node1Id = ((int[])p1)[1];
        int hero2Id = ((int[])p2)[0];
        int node2Id = ((int[])p2)[1];

        Transform heroNode1 = _fightHeroNodes[hero1Id];
        Transform target1 = _nodeUIs[node1Id];
        Transform heroNode2 = _fightHeroNodes[hero2Id];
        Transform target2 = _nodeUIs[node2Id];

        heroNode1.transform.position = target1.position;
        heroNode2.transform.position = target2.position;
    }

    GameObject InsHeroNode(HeroData heroData, bool setDragIcon)
    {
        GameObject go = Instantiate(_nodeModel);
        Image nodeImage = go.GetComponent<Image>();
        nodeImage.sprite = ResourceSys.Instance.GetSprite(heroData.Icon);

        Dragable drag = go.AddComponent<Dragable>();
        drag.ActionId = heroData.Id;
        drag.OnDragStart = OnDrag;
        drag.Canv = _stageCanvas;
        if (setDragIcon)
        {
            drag.DragIcon = ResourceSys.Instance.GetSprite(heroData.FightIcon);
        }
        drag.HasTail = true;
        drag.TailSprite = ResourceSys.Instance.GetSprite(GameConstants.CommonDragTail);
        drag.TailColor = heroData.TheColor;
        drag.TailWidth = 20;
        drag.IsDisableGray = true;
        return go;
    }
}
