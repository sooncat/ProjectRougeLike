﻿using System.Collections;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CreateUIs : MonoBehaviour {

    public Transform StageNodeTrans;
    public Transform StageLineTrans;
    public Transform StageLayerTrans;
    public GameObject NodeModel;
    public GameObject LineModel;
    public GameObject LayerNameModel;

    StageConfig stageConfig;

    Dictionary<int, Transform> nodeUIs;
    Dictionary<int, Transform> nodeLines;

	// Use this for initialization
	void Start () {
        CreateStageConfig();
		CreateUi();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int LayerGap = 150;
    public int LayerNameWidth = 200;
    int _layerHeight;
    
    void CreateStageConfig()
    {
        StageNodeStart nodeStart1 = new StageNodeStart(1, 0, "P1", "", "Assets/Res/Icon/cards/ico_head_aoyou");
        nodeStart1.NextNodes.Add(10); nodeStart1.NextNodes.Add(11);
        StageNodeStart nodeStart2 = new StageNodeStart(2, 1, "P2", "", "Assets/Res/Icon/cards/ico_head_aoyou");
        nodeStart2.NextNodes.Add(10); nodeStart2.NextNodes.Add(11);

        StageNodeFight nodeFight1 = new StageNodeFight(10, 0, "Fight", "", "Assets/Res/Icon/cards/ico_head_anduxie");
        nodeFight1.SetData(100, 1, 1);
        nodeFight1.NextNodes.Add(20); nodeFight1.NextNodes.Add(21);
        StageNodeFight nodeFight2 = new StageNodeFight(20, 1, "Fight", "", "Assets/Res/Icon/cards/ico_head_anduxie");
        nodeFight2.SetData(100, 1, 1);
        nodeFight2.NextNodes.Add(40);

        StageNodeSafe nodeSafe = new StageNodeSafe(40, 0, "House", "", "Assets/Res/Icon/Item/ico_common_mianzhan");
        nodeSafe.NextNodes.Add(30);


        StageNodeFight nodeFightBoss = new StageNodeFight(30, 0, "FighBoss", "", "Assets/Res/Icon/cards/ico_head_baoshi");
        nodeFightBoss.SetData(200, 1, 1);

        StageNodeReward nodeReward1 = new StageNodeReward(11, 1, "Reward", "", "Assets/Res/Icon/cards/ico_head_guidiao");
        Dictionary<int,int> cRewards = new Dictionary<int, int>();
        cRewards.Add(10000, 10);
        cRewards.Add(10001, 20);
        nodeReward1.AddData("Job", "2", cRewards);
        nodeReward1.AddData(string.Empty, "", cRewards);

        nodeReward1.NextNodes.Add(20); nodeReward1.NextNodes.Add(21);
        StageNodeReward nodeReward2 = new StageNodeReward(21, 0, "Reward", "", "Assets/Res/Icon/cards/ico_head_guidiao");

        Dictionary<int, int> cRewards2 = new Dictionary<int, int>();
        cRewards2.Add(10000, 11);
        cRewards2.Add(10001, 21);
        nodeReward2.AddData("Job", "2", cRewards2);
        nodeReward2.AddData(string.Empty, "", cRewards2);
        nodeReward2.NextNodes.Add(40);

        StageLayer layer0 = new StageLayer(0, "Start", "");
        layer0.Nodes.Add(nodeStart1);
        layer0.Nodes.Add(nodeStart2);
        StageLayer layer1 = new StageLayer(1, "L1", "");
        layer1.Nodes.Add(nodeFight1);
        layer1.Nodes.Add(nodeReward1);
        StageLayer layer2 = new StageLayer(2, "L2", "");
        layer2.Nodes.Add(nodeFight2);
        layer2.Nodes.Add(nodeReward2);
        StageLayer layer3 = new StageLayer(3, "L3", "");
        layer3.Nodes.Add(nodeSafe);
        StageLayer layer4 = new StageLayer(4, "Boss", "");
        layer4.Nodes.Add(nodeFightBoss);

        stageConfig = new StageConfig(0, "Stage First", "");
        stageConfig.Layers.Add(layer0);
        stageConfig.Layers.Add(layer1);
        stageConfig.Layers.Add(layer2);
        stageConfig.Layers.Add(layer3);
        stageConfig.Layers.Add(layer4);

        stageConfig.ItemIds.Add(20000);

        stageConfig.Sort();
    }

    void CreateUi()
    {
        nodeUIs = new Dictionary<int, Transform>();
        nodeLines = new Dictionary<int, Transform>();

        _layerHeight = (Screen.height - LayerGap * 2) / stageConfig.Layers.Count;

        foreach (StageLayer stageLayer in stageConfig.Layers)
        {
            CreateLayer(stageLayer);
        }

        foreach (StageLayer stageLayer in stageConfig.Layers)
        {
            foreach (BaseStageNode node in stageLayer.Nodes)
            {
                Transform t = nodeUIs[node.Id];
                foreach (int nextNode in node.NextNodes)
                {
                    Transform nextT = nodeUIs[nextNode];
                    GameObject lineModel = Instantiate(LineModel);
                    lineModel.transform.SetParent(StageLineTrans);
                    SetLineUI(t.position, nextT.position, lineModel.GetComponent<Slider>());
                    nodeLines.Add(node.Id*100+nextNode, lineModel.transform);
                }
            }
        }
    }

    void CreateLayer(StageLayer stageLayer)
    {
        
        float fullWidth = Screen.width - LayerNameWidth;
        float nodeFullWidth = fullWidth / stageLayer.Nodes.Count;
        Vector2 nodeSize = NodeModel.GetComponent<RectTransform>().sizeDelta;
        float nodeXLeft = LayerNameWidth + (nodeFullWidth - nodeSize.x) / 2;

        float posY = stageLayer.Layer * _layerHeight + LayerGap;
        posY += (_layerHeight - nodeSize.y) / 2.0f;

        foreach (BaseStageNode stageNode in stageLayer.Nodes)
        {
            float posX = nodeXLeft + nodeFullWidth * stageNode.Index;
            CreateNode(stageNode, posX, posY);
        }

        GameObject layerNameObj = Instantiate(LayerNameModel);
        layerNameObj.GetComponent<Text>().text = stageLayer.Name;
        layerNameObj.transform.SetParent(StageLayerTrans);
        layerNameObj.transform.localPosition = new Vector3(50, posY, 0);
    }

    void CreateNode(BaseStageNode stageNode, float posX, float posY)
    {
        GameObject go = Instantiate(NodeModel);
        go.transform.SetParent(StageNodeTrans);
        go.GetComponent<Button>().onClick.AddListener(() => { OnNodeClicked(stageNode.Id); });
        go.transform.localPosition = new Vector3(posX, posY, 0);

        Image nodeImage = go.GetComponent<Image>();
        Sprite newSprite;
        if(stageNode.NodeType.Equals(typeof(StageNodeFight).Name))
        {
            newSprite = GameResSys.Instance.GetCard(stageNode.Icon);
        }
        else
        {
            newSprite = GameResSys.Instance.GetNodeSprite(stageNode.Icon);    
        }
        
        nodeImage.sprite = newSprite;

        nodeUIs.Add(stageNode.Id, go.transform);
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

    void OnNodeClicked(int id)
    {
        Debug.Log("OnNodeClicked " + id);
        if (id == 1)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            string temp = JsonConvert.SerializeObject(stageConfig, settings);
            IOUtils.SaveFile(temp, "Assets/sc10.json");
        }
        else if (id == 2)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            byte[] b = IOUtils.ReadFileFromStreamingAssets("Assets/sc10.json");
            string str = System.Text.Encoding.UTF8.GetString(b);
            StageConfig sc = JsonConvert.DeserializeObject<StageConfig>(str, settings);
        }
        else if (id == 10)
        {
            PropertyDesTableData d = new PropertyDesTableData();
            d.Data = new List<PDesDataInfo>();
            PDesDataInfo info = new PDesDataInfo();
            info.PropertyName = "Job";
            info.Description = "职业";
            info.Values = new List<string>();
            info.Values.AddRange(new []{"1","2","3"});
            info.ValueDes = new List<string>();
            info.ValueDes.AddRange(new [] { "侠客", "神箭", "机巧" });
            
            d.Data.Add(info);

            PDesDataInfo info2 = new PDesDataInfo();
            info2.PropertyName = "Sex";
            info2.Description = "性别";
            info2.Values = new List<string>();
            info2.Values.AddRange(new[] { "1", "0" });
            info2.ValueDes = new List<string>();
            info2.ValueDes.AddRange(new[] { "男", "女" });
            d.Data.Add(info2);


            string s = JsonMapper.ToJson(d);
            IOUtils.SaveFile(s, "Assets/sc100.json");
        }
    }

    enum NodeType
    {
        A,B
    }
}
