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
    GameObject _stageNodeModel;
    GameObject _heroNodeModel;
    GameObject _lineModel;
    GameObject _layerNameModel;
    GameObject _heroShowModel;

    

    UINode _stageScrollContent;
    UINode _warning;
    UINode _dialog;
    UINode _winNode;
    UINode _modelNode;

    UINode _stageNode;
    
    Dictionary<int, Transform> _nodeUIs;

    struct DIKey
    {
        public bool Equals(DIKey other)
        {
            return _fromNodeId == other._fromNodeId && _targetNodeId == other._targetNodeId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_fromNodeId*397) ^ _targetNodeId;
            }
        }

        readonly int _fromNodeId;
        readonly int _targetNodeId;

        public DIKey(int fId, int tId)
        {
            _fromNodeId = fId;
            _targetNodeId = tId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DIKey && Equals((DIKey) obj);
        }
    }

    Dictionary<DIKey, Transform> _nodeLines;

    /// <summary>
    /// 选择英雄出战时，界面最下方横排的NodeList
    /// { Key:heroId, Value:nodeTransform }
    /// </summary>
    Dictionary<int, Transform> _heroSelectNodes;

    /// <summary>
    /// 被选中出战的英雄对应的Node.初始显示于地图的startnode上
    /// { Key:heroId, Value:nodeTransform }
    /// </summary>
    Dictionary<int, Transform> _fightHeroNodes;

    /// <summary>
    /// 战斗中，最下方显示的英雄对应的Node.
    /// { Key:heroId, Value:nodeTransform }
    /// </summary>
    Dictionary<int, Transform> _showHeroNodes;

    Canvas _stageCanvas;

    int _dragId;

    private const int BottomGap = 50;
    private const int LayerNameWidth = 200;
    readonly int _layerHeight = 150;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        InitStageUi(rootNode);

        EventSys.Instance.AddHander(ViewEvent.CreateStageView, OnCreatStageView);
        EventSys.Instance.AddHander(ViewEvent.FightSubStateMapping, OnFightStateMapping);
        EventSys.Instance.AddHander(ViewEvent.CreateHeroStartNode, OnCreateHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ReplaceHeroStartNode, OnReplaceHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.RemoveHeroStartNode, OnRemoveHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ResetHeroStartNode, OnResetHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ExchangeHeroStartNode, OnExchangeHeroStartNode);
        EventSys.Instance.AddHander(ViewEvent.ShowTipNodePassed, OnTipNodePassed);
        EventSys.Instance.AddHander(ViewEvent.ShowTipNotNextNode, OnTipNotNextNode);

        EventSys.Instance.AddHander(ViewEvent.FightWinReturnToStage, OnFightWinReturnStage);
        EventSys.Instance.AddHander(ViewEvent.FightLoseReturnToStage, OnFightLoseReturnStage);
        EventSys.Instance.AddHander(ViewEvent.ShowStageFail, OnStageFail);
        EventSys.Instance.AddHander(ViewEvent.ShowStageWin, OnStageWin);
        EventSys.Instance.AddHander(ViewEvent.GetRewardReturnToStage, OnGetRewardReturnToStage);


    }

    void InitStageUi(UINode rootNode)
    {

        _stageNode = rootNode.GetNode("Stage");
        _stageCanvas = _stageNode.GetComponent<Canvas>();

        Button btnExit = _stageNode.GetRef("ButtonExit").GetComponent<Button>();
        btnExit.onClick.AddListener(OnBtnExitClicked);

        _modelNode = _stageNode.GetNode("Models");
        _stageNodeModel = _modelNode.GetNode("Node_model").gameObject;
        _heroNodeModel = _modelNode.GetNode("HeroNode_model").gameObject;
        _lineModel = _modelNode.GetNode("Slider_model").gameObject;
        _layerNameModel = _modelNode.GetRef("LayerName_model").gameObject;
        _heroShowModel = _modelNode.GetNode("HeroShow_model").gameObject;
        
        _stageScrollContent = _stageNode.GetNode("Content");
        _stageNodeRootTrans = _stageScrollContent.GetRef("Stage").transform;
        _stageLineRootTrans = _stageScrollContent.GetRef("Line").transform;
        _stageLayerRootTrans = _stageScrollContent.GetRef("LayerNames").transform;

        Button btnReady = _stageNode.GetRef("ButtonReady").GetComponent<Button>();
        btnReady.onClick.AddListener(OnBtnReadyClicked);


        _stageNode.GetRef("HeroShowtList").gameObject.SetActive(true);
        _stageNode.GetRef("HeroShowtList").gameObject.SetActive(false);

        _warning = _stageNode.GetNode("Warning");
        DelayAction da = _warning.gameObject.AddComponent<DelayAction>();
        da.DelaySecond = 1;
        da.DAction = () => { _warning.gameObject.SetActive(false); };
        _warning.gameObject.SetActive(false);

        _dialog = _stageNode.GetNode("Dialog");
        _dialog.GetRef("Button").GetComponent<Button>().onClick.AddListener(() => { EventSys.Instance.AddEvent(InputEvent.FightExit); });
        _dialog.gameObject.SetActive(false);

        _winNode = _stageNode.GetNode("Win");
        _winNode.GetRef("Button").GetComponent<Button>().onClick.AddListener(() => { EventSys.Instance.AddEvent(InputEvent.FightExit); });
        _winNode.gameObject.SetActive(false);
    }

    

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.FightExit);
    }

    void OnBtnReadyClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.FightReady);
    }

    void OnFightStateMapping(object p1, object p2)
    {
        CatDebug.LogFunc();

        Transform hsl = _stageNode.GetRef("HeroSelectList");
        hsl.gameObject.SetActive(false);

        Transform readyBtn = _stageNode.GetRef("ButtonReady");
        readyBtn.gameObject.SetActive(false);

        foreach (KeyValuePair<int, Transform> pair in _nodeUIs)
        {
            Dropable dropable = pair.Value.GetComponentInChildren<Dropable>();
            dropable.enabled = true;
        }

        Dictionary<int, FightHero> heroes = (Dictionary<int, FightHero>)p1;

        foreach (KeyValuePair<int, Transform> pair in _fightHeroNodes)
        {
            Dragable dragable = pair.Value.GetComponentInChildren<Dragable>();
            HeroData hd = (HeroData)heroes[pair.Key].CreatureData;
            dragable.DragIcon = ResourceSys.Instance.GetSprite(hd.FightIcon);
        }

        //create hero show list
        CreateHeroShowList(heroes);
    }

    void CreateHeroShowList(Dictionary<int, FightHero> heros)
    {
        _showHeroNodes = new Dictionary<int, Transform>();
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            FightHero fightHero = pair.Value;
            GameObject go = Instantiate(_heroShowModel);
            UINode node = go.GetComponent<UINode>();
            node.GetRef("Icon").gameObject.AddComponent<ImageGray>();
            SetShowNodeData(fightHero, node);
            
            go.transform.SetParent(_stageNode.GetRef("HeroShowListContent"));

            _showHeroNodes.Add(fightHero.Id, go.transform);
        }

        _stageNode.GetRef("HeroShowtList").gameObject.SetActive(true);
    }

    /// <summary>
    /// 战斗中，最下方一排的英雄显示赋值
    /// </summary>
    /// <param name="fightHero"></param>
    /// <param name="node"></param>
    void SetShowNodeData(FightHero fightHero, UINode node)
    {
        node.GetRef("Icon").GetComponent<Image>().sprite = ResourceSys.Instance.GetSprite(fightHero.CreatureData.Icon);
        node.GetRef("Name").GetComponent<Text>().text = fightHero.CreatureData.Name;
        node.GetRef("Hp").GetComponent<Text>().text = "血";
        node.GetRef("HpSlider").GetComponent<Slider>().value = fightHero.CreatureData.HpPercent;
        node.GetRef("Mp").GetComponent<Text>().text = "气";
        node.GetRef("MpSlider").GetComponent<Slider>().value = fightHero.CreatureData.MpPercent;

        ImageGray ig = node.GetRef("Icon").GetComponent<ImageGray>();
        ig.Gray = fightHero.CreatureData.HpPercent <= 0;
        
    }

    void OnCreatStageView(object p1, object p2)
    {
        StageConfig stageConfig = (StageConfig)p1;

        _nodeUIs = new Dictionary<int, Transform>();
        _nodeLines = new Dictionary<DIKey, Transform>();

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
                    _nodeLines.Add(new DIKey(node.Id, nextNode), lineModel.transform);
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
            
            _heroSelectNodes.Add(hd.Id, go.transform);
        }

        heroSelectList.gameObject.SetActive(true);
    }

    void CreateLayer(StageLayer stageLayer)
    {
        CatDebug.LogFunc(GetHashCode().ToString());
        float allNodeWidth = _stageScrollContent.GetComponent<RectTransform>().sizeDelta.x - LayerNameWidth;
        float singleNodeWidth = allNodeWidth / stageLayer.Nodes.Count;
        Vector2 modelNodeSize = _stageNodeModel.GetComponent<RectTransform>().sizeDelta;
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
        GameObject go = Instantiate(_stageNodeModel);
        UINode uiNode = go.GetComponent<UINode>();
        go.transform.SetParent(_stageNodeRootTrans);
        go.transform.localPosition = new Vector3(posX, posY, 0);

        Button btn = uiNode.GetRef("Button").GetComponent<Button>();
        btn.onClick.AddListener(() => { OnNodeClicked(stageNode.Id); });
        
        Image nodeImage = uiNode.GetRef("Button").GetComponent<Image>();
        Sprite newSprite = ResourceSys.Instance.GetSprite(stageNode.Icon);
        nodeImage.sprite = newSprite;

        Dropable drop = uiNode.GetRef("Button").gameObject.AddComponent<Dropable>();
        drop.ActionId = stageNode.Id;
        drop.OnDroped = DropOnNode;
        if(!stageNode.NodeType.Equals(typeof(StageNodeStart).Name))
        {
            drop.enabled = false;
        }

        uiNode.GetRef("All").gameObject.SetActive(false);
        
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
        mySlider.GetComponent<Slider>().value = 0f;
    }

    void OnNodeClicked(int id)
    {
        //Debug.Log("OnNodeClicked " + id);
        EventSys.Instance.AddEvent(InputEvent.StageNodeClicked, id);
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

    void OnCreateHeroStartNode(object p1, object p2)
    {
        CatDebug.LogFunc();

        HeroData heroData = (HeroData)p1;
        int targetNodeId = (int)p2;

        if(_heroSelectNodes.ContainsKey(heroData.Id))
        {
            Dragable d = _heroSelectNodes[heroData.Id].GetComponentInChildren<Dragable>();
            d.SetEnable(false);
        }

        GameObject go = InsHeroNode(heroData, false);//这里的英雄还可能与其他英雄交换，故不添加特殊dragicon
        go.transform.SetParent(_stageNodeRootTrans);
        //go.GetComponent<Button>().onClick.AddListener(() => { });

        Dropable drop = go.AddComponent<Dropable>();
        drop.ActionId = heroData.Id;
        drop.OnDroped = DropOnHero;

        Transform target = _nodeUIs[targetNodeId];
        UINode targetNode = target.GetComponent<UINode>();
        go.transform.SetParent(targetNode.GetRef("HeroRoot"));

        _fightHeroNodes.Add(heroData.Id, go.transform);
    }

    void OnReplaceHeroStartNode(object p1, object p2)
    {
        HeroData newHeroData = (HeroData)p1;
        int targetHeroId = ((int[])p2)[0];
        int targetNodeId = ((int[])p2)[1];

        Transform node = _fightHeroNodes[targetHeroId];
        Destroy(node.gameObject);
        _fightHeroNodes.Remove(targetHeroId);
        if (_heroSelectNodes.ContainsKey(targetHeroId))
        {
            Dragable d = _heroSelectNodes[targetHeroId].GetComponentInChildren<Dragable>();
            d.SetEnable(true);
        }

        OnCreateHeroStartNode(newHeroData, targetNodeId);
    }

    void OnRemoveHeroStartNode(object p1, object p2)
    {
        
    }

    void OnResetHeroStartNode(object p1, object p2)
    {
        int heroId = (int)p1;
        int targetNodeId = (int)p2;

        Transform node = _fightHeroNodes[heroId];
        Transform target = _nodeUIs[targetNodeId];
        Transform hRoot = target.GetComponent<UINode>().GetRef("HeroRoot");
        node.transform.SetParent(hRoot);
    }

    /// <summary>
    /// 交换两个英雄在起始点上的位置
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnExchangeHeroStartNode(object p1, object p2)
    {
        int hero1Id = ((int[])p1)[0];
        int node1Id = ((int[])p1)[1];
        int hero2Id = ((int[])p2)[0];
        int node2Id = ((int[])p2)[1];

        Transform heroNode1 = _fightHeroNodes[hero1Id];
        Transform target1 = _nodeUIs[node1Id].GetComponent<UINode>().GetRef("HeroRoot");
        Transform heroNode2 = _fightHeroNodes[hero2Id];
        Transform target2 = _nodeUIs[node2Id].GetComponent<UINode>().GetRef("HeroRoot");

        heroNode1.transform.SetParent(target1);
        heroNode2.transform.SetParent(target2);
    }

    /// <summary>
    /// 地图上的英雄node的实例化
    /// </summary>
    /// <param name="heroData"></param>
    /// <param name="setDragIcon"></param>
    /// <returns></returns>
    GameObject InsHeroNode(HeroData heroData, bool setDragIcon)
    {
        GameObject go = Instantiate(_heroNodeModel);
        UINode uiNode = go.GetComponent<UINode>();
        Image nodeImage = uiNode.GetRef("Image").GetComponent<Image>();
        nodeImage.sprite = ResourceSys.Instance.GetSprite(heroData.Icon);

        Dragable drag = uiNode.GetRef("Image").gameObject.AddComponent<Dragable>();
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

    void OnTipNodePassed(object p1, object p2)
    {
        ShowWarning("此节点已被攻略");
    }

    void OnTipNotNextNode(object p1, object p2)
    {
        ShowWarning("没有通路");
    }

    void ShowWarning(string msg)
    {
        Text t = _warning.GetRef("Text").GetComponent<Text>();
        t.text = msg;
        DelayAction da = _warning.GetComponent<DelayAction>();
        da.StartDelay();
    }

    void OnFightWinReturnStage(object p1, object p2)
    {
        //update Node
        int nodeId = (int)p2;
        ImageGray ig = _nodeUIs[nodeId].gameObject.AddComponent<ImageGray>();
        ig.Gray = true;

        Dictionary<int, FightHero> heros = (Dictionary<int, FightHero>)p1;
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            int hId = pair.Value.Id;
            //change pos
            Transform targetTrans = _nodeUIs[pair.Value.NowNodeId].GetComponent<UINode>().GetRef("HeroRoot");
            _fightHeroNodes[hId].SetParent(targetTrans);
            //update data
            UINode showNode = _showHeroNodes[hId].GetComponent<UINode>();
            SetShowNodeData(pair.Value, showNode);

            Color c = ((HeroData)pair.Value.CreatureData).TheColor;
            SetPathPassed(pair.Value.LastNodeId, pair.Value.NowNodeId, c);
        }
        
    }

    void OnFightLoseReturnStage(object p1, object p2)
    {
        Dictionary<int, FightHero> heros = (Dictionary<int, FightHero>)p1;
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            int hId = pair.Value.Id;
            //update show list
            UINode showNode = _showHeroNodes[hId].GetComponent<UINode>();
            SetShowNodeData(pair.Value, showNode);
            //update stage nodes
            if(pair.Value.CreatureData.Hp.Value <= 0)
            {
                _fightHeroNodes[hId].GetComponentInChildren<Dragable>().SetEnable(false);
            }
        }
    }

    void OnStageFail(object p1, object p2)
    {
        _dialog.GetRef("Text").GetComponent<Text>().text = "胜败乃兵家常事";
        _dialog.gameObject.SetActive(true);
    }

    void OnStageWin(object p1, object p2)
    {
        List<Item> items = (List<Item>)p1;
        Transform content = _winNode.GetRef("Content");
        content.DestroyChildren();
        foreach (Item item in items)
        {
            GameObject newNodeObj = Instantiate(_modelNode.GetNode("RewardItem").gameObject, content);

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

        _winNode.gameObject.SetActive(true);
    }

    void OnGetRewardReturnToStage(object p1, object p2)
    {
        //update Node
        int nodeId = (int)p2;
        Transform nodeTrans = _nodeUIs[nodeId];
        ImageGray ig = nodeTrans.gameObject.AddComponent<ImageGray>();
        ig.Gray = true;

        FightHero hero = (FightHero)p1;
        Transform t = nodeTrans.GetComponent<UINode>().GetRef("HeroRoot");
        _fightHeroNodes[hero.Id].transform.SetParent(t);
        SetPathPassed(hero.LastNodeId, hero.NowNodeId, ((HeroData)hero.CreatureData).TheColor);
    }

    void SetPathPassed(int fromNodeId, int targetNodeId, Color c)
    {
        DIKey id = new DIKey(fromNodeId, targetNodeId);
        UINode node = _nodeLines[id].GetComponent<UINode>();
        node.GetComponent<Slider>().value = 1;
        node.GetRef("Fill").GetComponent<Image>().color = c;
    }
}
