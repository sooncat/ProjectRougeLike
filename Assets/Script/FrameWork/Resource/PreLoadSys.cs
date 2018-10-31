using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace com.initialworld.framework
{

    /// <summary>
    /// 注意资源的依赖关系，
    /// 如果资源A依赖于资源B，需要先加载B再加载A
    /// 因此，加载流程为
    /// 1.递归加载manifest文件，向列表中添加所有dependences，直到所有dependences为空
    /// 2.开始加载_resPath内的文件。
    /// </summary>
    public class PreLoadSys : ISystem
    {

        /// <summary>
        /// 资源的引用关系
        /// </summary>
        AssetBundleManifest _abManifest;

        /// <summary>
        /// 需要预加载的资源
        /// </summary>
        HashSet<string> _resPath;

        /// <summary>
        /// 分级加载，key值越小，代表依赖层级越低。
        /// </summary>
        Dictionary<int, HashSet<string>> _hierarchicAsset;

        /// <summary>
        /// 当前正在加载的资源的级别
        /// </summary>
        int _nowLevel;

        /// <summary>
        /// 需要加载的所有资源的数量
        /// </summary>
        int _fullCount;

        /// <summary>
        /// 加载计数器
        /// </summary>
        int _loadedCount;

        public override void Init()
        {
            base.Init();

            _resPath = new HashSet<string>();
            _hierarchicAsset = new Dictionary<int, HashSet<string>>();

            EventSys.Instance.AddHander(FrameEvent.AddPreLoadRes, (p1, p2) => { OnAddPreLoadRes((string)p1); });
            EventSys.Instance.AddHander(FrameEvent.PreLoadStart, OnStartPreLoad);
            EventSys.Instance.AddHander(FrameEvent.EndLoadAssetBundleAsync, (p1, p2) =>
            {
                OnAssetBundleLoaded((string)p1, (AssetBundle)p2);
            });

            PrepareAssetBundleManifest();
        }

        /// <summary>
        /// 准备manifest文件
        /// </summary>
        void PrepareAssetBundleManifest()
        {
            string path = Path.Combine(GameConstants.AssetBundlePath, GameConstants.AssetBundleMainfest);
            EventSys.Instance.AddEventNow(FrameEvent.RegistResidentAssetBundle, path);
            AssetBundle ab = AssetBundleSys.Instance.LoadAssetBundleInStreaming(path);
            _abManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 手动指定预加载资源
        /// </summary>
        /// <param name="assetBundlePath">assetbundleLable,是相对路径</param>
        void OnAddPreLoadRes(string assetBundlePath)
        {
            _resPath.Add(assetBundlePath);
        }

        /// <summary>
        /// 检查所有资源引用并分级
        /// </summary>
        int PrepareResDependencies(string[] res)
        {
            //Dictionary<string, int> resLayerCache = new Dictionary<string, int>();
            int layer = 0;
            foreach (string resPath in res)
            {
                string[] dependencies = _abManifest.GetAllDependencies(resPath);
                if(dependencies.Length > 0){
                    layer = PrepareResDependencies(dependencies);   
                }
                AddHierarchicAsset(layer, resPath);   
            }
            return layer + 1;
        }

        void AddHierarchicAsset(int index, string resPath)
        {
            if (!_hierarchicAsset.ContainsKey(index))
            {
                _hierarchicAsset.Add(index,new HashSet<string>());
            }
            _hierarchicAsset[index].Add(resPath);
        }

        /// <summary>
        /// 检查依赖关系，将未指明的依赖资源加入加载列表
        /// 注意在mainfest文件中查询依赖是使用AssetLabel做Key的
        /// （就是在Inspector最下方的AssetLabels的设定）
        /// （具体表现就是资源的相对路径，不带最前面的"/"，例如“view/loadingui”）
        /// </summary>
        //void PrepareResDependencies()
        //{
        //    HashSet<string> dependList = new HashSet<string>();
        //    foreach (string s in _resPath)
        //    {
        //        string[] dependencies = _abManifest.GetAllDependencies(s);
        //        foreach (string dependency in dependencies)
        //        {
        //            dependList.Add(dependency);    
        //        }
        //    }
        //    AddDependenciesRes(dependList.ToArray());
            
        //    while (dependList.Count > 0)
        //    {
        //        HashSet<string> temp = new HashSet<string>();
        //        foreach (string s in dependList)
        //        {
        //            //Debug.Log("res = " + s);
        //            string[] ds = _abManifest.GetAllDependencies(s);
        //            foreach (string s1 in ds)
        //            {
        //                temp.Add(s1);    
        //            }
        //            AddDependenciesRes(ds);
        //        }
        //        dependList.Clear();
        //        if(temp.Count > 0)
        //        {
        //            dependList = new HashSet<string>(temp);
        //        }
        //    }
        //}

        /// <summary>
        /// 向资源列表中加入依赖资源
        /// </summary>
        /// <param name="dependencies"></param>
        void AddDependenciesRes(string[] dependencies)
        {
            foreach (string d in dependencies)
            {
                _resPath.Add(d);
            }
        }

        /// <summary>
        /// 开始加载所有文件
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        void OnStartPreLoad(object p1, object p2)
        {
            CatDebug.LogFunc();
            if(_resPath.Count == 0)
            {
                EventSys.Instance.AddEvent(FrameEvent.PreloadEnd);
            }
            PrepareResDependencies(_resPath.ToArray());

            //start load first layer res
            _nowLevel = 0;
            StartLoadResInLayer(_nowLevel);
        }

        void OnAssetBundleLoaded(string path, AssetBundle ab)
        {
            //CatDebug.LogFunc("Path = " + path + ", ab.type = " + ab.GetType());
            _loadedCount++;
            if(_loadedCount == _fullCount)
            {
                _nowLevel++;
                StartLoadResInLayer(_nowLevel);
            }
        }

        void StartLoadResInLayer(int index)
        {
            HashSet<string> resPath;
            _hierarchicAsset.TryGetValue(index, out resPath);
            if(resPath == null)
            {
                EventSys.Instance.AddEvent(FrameEvent.PreloadEnd);
                _resPath.Clear();
                _hierarchicAsset.Clear();
                return;
            }
            _fullCount = resPath.Count;
            _loadedCount = 0;
            foreach (string s in resPath)
            {
                string realPath = GameConstants.AssetBundlePath + s;
                //Debug.Log("index = " + index + "realPath = " + realPath);
                EventSys.Instance.AddEvent(FrameEvent.StartLoadAssetBundleAsyncInStreaming, realPath);
            }
        }
         

    }
}