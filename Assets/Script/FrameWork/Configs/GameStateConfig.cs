using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateConfig {

    public class PreLoadResConfig
    {
        public string AssetBundle;
        public string Prefab;
        public string ClassName;
    }

    public class GameStateDetail
    {
        public string SceneName;
        public List<PreLoadResConfig> PreLoadUi;
        public List<PreLoadResConfig> PreLoadEffect;
        public List<PreLoadResConfig> PreLoadAudio;
    }

    public Dictionary<string, GameStateDetail> GameStateDetails; 
}
