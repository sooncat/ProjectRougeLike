using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace com.initialworld.framework
{

    /// <summary>
    /// AssetBundle基本用法：
    /// 游戏资源放在StreamingAssets文件夹中，用LoadFromFile（同步或异步）加载
    /// 更新资源在persistentAssets文件夹中，用UnityWebRequest异步加载
    /// </summary>
    public class AssetBundleSys : ISystem
    {
        private static AssetBundleSys _instance;
        public static AssetBundleSys Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 普通的资源缓存，会被指令清理
        /// </summary>
        Dictionary<string, AssetBundle> _assetCache;

        /// <summary>
        /// 需要常驻内存的AssetBundle
        /// </summary>
        Dictionary<string, AssetBundle> _residentAssetCache;

        /// <summary>
        /// 可以在此注册需要常驻内存的资源列表
        /// </summary>
        List<string> _residentAssetList;

        public override void Init()
        {
            base.Init();

            _instance = this;
            _assetCache = new Dictionary<string, AssetBundle>();
            _residentAssetCache = new Dictionary<string, AssetBundle>();
            _residentAssetList = new List<string>();

            EventSys.Instance.AddHander(FrameEvent.StartLoadAssetBundleAsyncInPersistent, OnStartLoadAssetBundleInPersistent);
            EventSys.Instance.AddHander(FrameEvent.StartLoadAssetBundleAsyncInStreaming, (p1,p2)=>{OnStartLoadLoadAssetBundleAsyncInStreaming((string)p1);});
            EventSys.Instance.AddHander(FrameEvent.ClearAssetBundleChche, Clear);
            EventSys.Instance.AddHander(FrameEvent.RegistResidentAssetBundle, (p1, p2) => { OnRegistResidentAssetBundle((string)p1); });
        }

        void OnRegistResidentAssetBundle(string assetBundlePath)
        {
            if(!_residentAssetList.Contains(assetBundlePath))
            {
                _residentAssetList.Add(assetBundlePath);
            }
        }

        void AddCache(string path, AssetBundle ab)
        {
            if(_residentAssetList.Contains(path))
            {
                _residentAssetCache.Add(path, ab);
            }
            else
            {
                _assetCache.Add(path, ab);    
            }
        }

        AssetBundle GetCache(string path)
        {
            AssetBundle result;
            _residentAssetCache.TryGetValue(path, out result);
            if(result != null)
            {
                return result;
            }
            _assetCache.TryGetValue(path, out result);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 异步加载，一般用于加载persistentData目录下的文件
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        void OnStartLoadAssetBundleInPersistent(object p1, object p2)
        {
            string path = (string)p1;
            AssetBundle result = GetCache(path);
            if (result != null)
            {
                EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, path, result);
            }
            StartCoroutine(LoadAssetBundleAsync(path));
        }

        IEnumerator LoadAssetBundleAsync(string path)
        {
            string realPath = GetRealPath(path);
            UnityWebRequest request = UnityWebRequest.GetAssetBundle(realPath, 0);
            yield return request.SendWebRequest();

            if (!string.IsNullOrEmpty(request.error))
            {
                throw new Exception(request.error);
            }
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
            //ab.Unload(false);
            AddCache(path, ab);
            request.Dispose();

            EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, path, ab);
        }

        /// <summary>
        /// 同步加载Application.streamingAssetsPath目录下的资源
        /// </summary>
        /// <param name="path">需要Application.streamingAssetsPath前缀</param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundleInStreaming(string path)
        {
            AssetBundle result = GetCache(path);
            if (result != null)
            {
                return result;
                //EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, path, _assetCache[path]);
            }
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            //ab.Unload(false);
            AddCache(path, ab);
            return ab;
        }

        /// <summary>
        /// 异步加载Application.streamingAssetsPath目录下的资源
        /// </summary>
        /// <param name="path"></param>
        public void OnStartLoadLoadAssetBundleAsyncInStreaming(string path)
        {
            AssetBundle result = GetCache(path);
            if (result != null)
            {
                EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, path, result);
            }
            StartCoroutine(LoadAssetBundleAsyncInStreaming(path));
        }

        IEnumerator LoadAssetBundleAsyncInStreaming(string path)
        {
            //string path = Path.Combine(Application.streamingAssetsPath, "myassetBundle");
            var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleLoadRequest;

            var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                yield break;
            }
            AddCache(path, myLoadedAssetBundle);
            EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, path, myLoadedAssetBundle);
        }

        /// <summary>
        /// 清理，p1=true是彻底清理
        /// 可以在资源使用完成后false清理，
        /// 在切换游戏状态时可以true清理。
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        void Clear(object p1, object p2)
        {
            CatDebug.LogFunc("isTrue=" + p1);
            bool isClearAll = (bool)p1;
            foreach (KeyValuePair<string, AssetBundle> pair in _assetCache)
            {
                pair.Value.Unload(isClearAll);
            }
            _assetCache.Clear();
        }

        /// <summary>
        /// 得到WWW使用的地址格式，如果是网络地址则无需修改
        /// 用于异步读取persistentData目录下的资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetRealPath(string path)
        {
            if (path.StartsWith("http"))
            {
                return path;
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "file://" + path;
                case RuntimePlatform.IPhonePlayer:
                    return "file://" + path;
                default:
                    return "file:///" + path;
            }
        }
    }
}