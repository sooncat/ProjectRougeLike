using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnKnowNodeView : BaseView
{

    UINode _rootNode;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        _rootNode = rootNode;
        EventSys.Instance.AddHander(ViewEvent.ShowNodeUnKnow, ShowNodeSafe);
        _rootNode.GetRef("Confirm").GetComponent<Button>().onClick.AddListener(OnConfirmClicked);
        _rootNode.gameObject.SetActive(false);
    }

    void OnConfirmClicked()
    {
        _rootNode.gameObject.SetActive(false);
    }

    void ShowNodeSafe(object p1, object p2)
    {
        _rootNode.gameObject.SetActive(true);
    }
}
