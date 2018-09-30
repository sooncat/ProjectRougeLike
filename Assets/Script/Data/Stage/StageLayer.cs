using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLayer {

    public int Id;
    public string Name;
    public string Description;
    public int Layer;

    public List<BaseStageNode> Nodes;
    
    public StageLayer()
    {
        
    }

    public StageLayer(int id, string name, string des)
    {
        Id = id;
        Layer = id;
        Name = name;
        Description = des;
        Nodes = new List<BaseStageNode>();
    }
}
