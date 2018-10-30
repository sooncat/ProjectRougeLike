using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 框架用事件，为了区别专用负数表示
/// </summary>
public enum FrameEvent : int 
{
    StartNone = -999,

    StartLoadAssetBundleAsyncInStreaming,   //开始加载一个AssetBundle
    StartLoadAssetBundleAsyncInPersistent,  //开始加载一个AssetBundle
    EndLoadAssetBundleAsync,                //加载一个AssetBundle完毕
    ClearAssetBundleChche,                  //清理加载AssetBundle的缓存

    AddPreLoadRes,                          //添加需要预加载的资源路径
    PreLoadStart,                           //开始预加载
    PreLoadUpdatePercent,                   //更新预加载进度
    PreloadEnd,                             //预加载结束

}
