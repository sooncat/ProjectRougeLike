using System.Collections;
using System.Collections.Generic;


public class StageConfig {

    public int Id;
    public string Name;
    public string Description;
    public List<StageLayer> Layers;
    public List<int> ItemIds; 

    public StageConfig()
    {}

    public StageConfig(int id, string name, string des)
    {
        Id = id;
        Name = name;
        Description = des;
        Layers = new List<StageLayer>();
        ItemIds = new List<int>();
    }

    public BaseStageNode GetNode(int id)
    {
        foreach (StageLayer layer in Layers)
        {
            foreach (BaseStageNode node in layer.Nodes)
            {
                if(node.Id == id)
                {
                    return node;
                }
            }
        }
        return null;
    }

    public void Sort()
    {
        Layers.Sort((StageLayer l1, StageLayer l2) => { if (l1.Layer == l2.Layer) return 0; if (l1.Layer > l2.Layer)return 1; return -1; });
        foreach (StageLayer layer in Layers)
        {
            layer.Nodes.Sort((BaseStageNode n1, BaseStageNode n2) => { if (n1.Index == n2.Index) return 0; if (n1.Index > n2.Index)return 1; return -1; });
        }
    }
}
