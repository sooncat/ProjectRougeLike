/*
该脚本绑在拖拽结束后，要落入的物体
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Dropable : MonoBehaviour, IDropHandler
{

    public int ActionId;
    public Action<int> OnDroped;
    
    public void OnDrop(PointerEventData eventData)
    {
        //if (eventData.pointerDrag != null)
        //{
            //Image img = eventData.pointerDrag.GetComponent<Image>();
            //if (img != null)
            //{
            //    GetComponent<Image>().sprite = img.sprite;
            //    GetComponent<Image>().color = new Color(1, 1, 1, 1);//我刚开始把图片设为了透明
            //}
        //}

        if(OnDroped!=null)
        {
            OnDroped(ActionId);
        }
    }
}
