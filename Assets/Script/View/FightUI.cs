using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
    
    Dictionary<int, Transform> _nodeUIs;
    Dictionary<int, Transform> _nodeLines;

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
        fightBtn.onClick.AddListener(()=>{});
        Button exitBtn = _fightNode.GetRef("Exit").GetComponent<Button>();
        exitBtn.onClick.AddListener(() => { _fightNode.gameObject.SetActive(false); });

        _fightNode.gameObject.SetActive(false);
    }

    void InitRewardNodeUi(UINode rootNode)
    {
        
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
        Sprite newSprite = ResourceSys.Instance.GetIcon(stageNode.Icon);
        nodeImage.sprite = newSprite;

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

        float deltaY = p2.y - p1.y;
        float deltaX = p2.x - p1.x;
        double arc = System.Math.Atan(deltaY / deltaX);
        double angle = (180 / Mathf.PI) * arc;
        if (deltaX < 0)
        {
            angle += 180;
        }

        float distance = Vector3.Distance(p1, p2);

        mySlider.transform.position = new Vector3(centerX, centerY, 0);
        mySlider.transform.localEulerAngles = new Vector3(0, 0, (float)angle);
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

    public string GetPropertyDescription(Enemy enemy)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("级 - ").AppendLine(enemy.CreatureData.Lv.Value.ToString());
        sb.Append("精 - ").AppendLine(enemy.CreatureData.Hp.Value.ToString());
        sb.Append("气 - ").AppendLine(enemy.CreatureData.Mp.Value.ToString());
        sb.Append("神 - ").AppendLine(enemy.CreatureData.Mp.Value.ToString());
        sb.Append("攻 - ").AppendLine(enemy.CreatureData.Att.Value.ToString());
        sb.Append("防 - ").AppendLine(enemy.CreatureData.Def.Value.ToString());
        return sb.ToString();
    }

    void ShowNodeFight(int id)
    {
        Enemy enemy = FightDataMgr.Instance.GetEnemy(id);
        Image icon = _fightNode.GetRef("Icon").GetComponent<Image>();
        icon.sprite = ResourceSys.Instance.GetIcon(enemy.CreatureData.Icon);

        Text detail = _fightNode.GetRef("Info").GetComponent<Text>();
        detail.text = GetPropertyDescription(enemy);

        Text enemyName = _fightNode.GetRef("Name").GetComponent<Text>();
        enemyName.text = enemy.CreatureData.Name;

        Text des = _fightNode.GetRef("Des").GetComponent<Text>();
        des.text = enemy.CreatureData.Description;

        _fightNode.gameObject.SetActive(true);
    }

    void ShowNodeReward(int id)
    {
        
    }



}
