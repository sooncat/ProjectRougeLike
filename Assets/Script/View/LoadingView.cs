using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : BaseView {

    UINode _rootNode;

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);
        _rootNode = rootNode;

        DontDestroyOnLoad(rootNode.gameObject);
        
        rootNode.gameObject.SetActive(false);
        rootNode.GetRef("Slider").GetComponent<Slider>().value = 0f;

        EventSys.Instance.AddHander(ViewEvent.LoadingShow, (param1, param2) =>
        {
            rootNode.GetRef("Slider").GetComponent<Slider>().value = 0f;
            rootNode.gameObject.SetActive(true);
        });
        EventSys.Instance.AddHander(ViewEvent.LoadingUpdate, (param1, param2) =>
        {
            float val = (float)param1;
            rootNode.GetRef("Slider").GetComponent<Slider>().value = val;
        });
        EventSys.Instance.AddHander(ViewEvent.LoadingHide, (param1, param2) =>
        {
            rootNode.GetRef("Slider").GetComponent<Slider>().value = 1f;
            rootNode.gameObject.SetActive(false);
        });
    }


}
