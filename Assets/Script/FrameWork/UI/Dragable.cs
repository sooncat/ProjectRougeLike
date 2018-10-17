/*
该脚本绑在要拖拽的物体上
*/

using System;
using UnityEngine;
using UnityEngine.EventSystems;//要想用拖拽事件必须导入EventSystems
using UnityEngine.UI;

/// <summary>
/// 实现开始拖拽，拖拽中和结束拖拽3个事件接口
/// </summary>
public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    /// <summary>
    /// 场景中的canvas
    /// </summary>
    public Canvas Canv;

    /// <summary>
    /// 拖拽生成的icon
    /// </summary>
    public Sprite DragIcon;

    /// <summary>
    /// //拖拽生成的物品
    /// </summary>
    private GameObject _dragObj;
    /// <summary>
    /// //生成物品的方向等信息
    /// </summary>
    private RectTransform _dragObjRect;

    public int ActionId;
    public Action<int> OnDragStart;

    void Start()
    {
        _dragObjRect = Canv.transform as RectTransform;
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)//
    {
        _dragObj = new GameObject("icon");
        _dragObj.transform.SetParent(Canv.transform, false);//让这个物体在canvas上，此时物品在屏幕中心
        _dragObj.transform.SetAsLastSibling();//将生成的物体设为canvas的最后一个子物体，一般来说最后一个子物体是可操作的
        Image img = _dragObj.AddComponent<Image>();//给生成的拖拽物体添加一个Image组件并获得Image组件的控制权
        img.raycastTarget = false;//让该物体不接受鼠标的照射，目的是底下的物品也能操作
        img.sprite = DragIcon ?? GetComponent<Image>().sprite;
        //_dragObj.GetComponent<RectTransform>().sizeDelta =
        //    new Vector2(Screen.height * 0.1f, Screen.height * 0.1f);//设置新生成物品的尺寸
        img.SetNativeSize();
        ObjFollowMouse(eventData);//让生成的物体跟随鼠标

        if(OnDragStart!=null)
        {
            OnDragStart(ActionId);
        }
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        ObjFollowMouse(eventData);//让生成的物体跟随鼠标
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragObj != null)
        {
            Destroy(_dragObj);//拖拽结束后销毁生成的物体
        }
    }

    private void ObjFollowMouse(PointerEventData eventData)
    {
        if (_dragObj != null)//检测生成的物体是否为空，保险起见
        {
            RectTransform rc = _dragObj.GetComponent<RectTransform>();//得到生成物品的控制权
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle
                (_dragObjRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                rc.position = globalMousePos;
                rc.rotation = _dragObjRect.rotation;
            }
        }
    }
}