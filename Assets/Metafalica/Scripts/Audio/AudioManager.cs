using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class AudioManager : Singleton<AudioManager>
    {
        
        public const string MainVolumeKey = "Volume_Main";
        public const string BGMVolumeKey = "Volume_BGM";
        public const string OtherVolumeKey = "Volume_Other";
        private const int DefaultVolume = 10;
    
        private Dictionary<string, int> volumeDic = new Dictionary<string, int>();
        private readonly Dictionary<string, int> volumeDicDefault = new Dictionary<string, int>()
        {
            {MainVolumeKey, DefaultVolume},
            {BGMVolumeKey, DefaultVolume},
            {OtherVolumeKey, DefaultVolume},
        };
        
        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
        }
    
        public int GetVolume(string key)
        {
            if (volumeDic.ContainsKey(key))
            {
                return volumeDic[key];
            }
            else
            {
                Debug.LogError("不存在该音量键值");
                return DefaultVolume;
            }
        }
    
        public void SetVolume(string key, int value)
        {
            if (volumeDic.ContainsKey(key))
            {
                volumeDic[key] = value;
                PlayerPrefs.SetInt(key, value);
            }
            else
            {
                Debug.LogError("不存在该音量键值");
            }
                
        }
    }
}
