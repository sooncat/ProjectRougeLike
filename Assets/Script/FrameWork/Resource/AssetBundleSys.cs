using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.initialworld.framework
{
    public class AssetBundleSys : ISystem
    {

        private AssetBundleSys _instance;
        public AssetBundleSys Instance
        {
            get { return _instance; }
        }

        Dictionary<string, AssetBundle> _cache;



        public override void Init()
        {
            base.Init();

            _instance = this;
            _cache = new Dictionary<string, AssetBundle>();

            EventSys.Instance.AddHander(FrameEvent.StartLoadAssetBundleAsync, OnStartLoadAssetBundle);
            EventSys.Instance.AddHander(FrameEvent.ClearAssetBundleChche, Clear);
        }

        /// <summary>
        /// 异步加载，推荐
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        void OnStartLoadAssetBundle(object p1, object p2)
        {
            string path = (string)p1;
            if (_cache.ContainsKey(path))
            {
                EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, _cache[path]);
            }
            StartCoroutine(LoadAssetBundleAsync(path));
        }

        IEnumerator LoadAssetBundleAsync(string path)
        {
            string realPath = GetRealPath(path);
            WWW www = new WWW(realPath);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                throw new Exception(www.error);
            }
            AssetBundle ab = www.assetBundle;
            //ab.Unload(false);
            _cache.Add(path, ab);
            www.Dispose();

            EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, ab);
        }

        /// <summary>
        /// 同步加载，不推荐
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string path)
        {
            if (_cache.ContainsKey(path))
            {
                EventSys.Instance.AddEvent(FrameEvent.EndLoadAssetBundleAsync, _cache[path]);
            }
            string realPath = GetRealPath(path);

            WWW www = new WWW(realPath);
            while (!www.isDone)
            { }
            if (!string.IsNullOrEmpty(www.error))
            {
                throw new Exception(www.error);
            }
            AssetBundle ab = www.assetBundle;
            //ab.Unload(false);
            _cache.Add(path, ab);
            www.Dispose();

            return ab;
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
            bool isClearAll = (bool)p1;
            foreach (KeyValuePair<string, AssetBundle> pair in _cache)
            {
                pair.Value.Unload(isClearAll);
            }
            _cache.Clear();
        }

        /// <summary>
        /// 得到WWW使用的地址格式，如果是网络地址则无需修改
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