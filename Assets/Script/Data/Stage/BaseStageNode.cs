using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


public class BaseStageNode {

    public int Id;
    /// <summary>
    /// index是显示顺序
    /// </summary>
    public int Index;
    public string Name;
    public string Description;
    public string Icon;
    public string NodeType;
    public List<int> NextNodes;

    public BaseStageNode()
    {}

    public BaseStageNode(int id, int index, string name, string des, string icon)
    {
        Id = id;
        Index = index;
        Name = name;
        Description = des;
        Icon = icon;
        NextNodes = new List<int>();

        NodeType = GetType().Name;
    }

    //是不是需要重写litjson的读写方法...
    
}
