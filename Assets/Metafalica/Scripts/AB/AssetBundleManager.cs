using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Object = UnityEngine.Object;


namespace Metafalica.RPG
{
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        //资源关系依赖配表，可以根据crc来得到对应资源块
        protected Dictionary<uint, ResourceItem> m_ResouceItemDic = new Dictionary<uint, ResourceItem>();
        //储存加载的ab包,key为crc
        protected Dictionary<uint, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
        //AssetBundleItem类对象池
        protected ClassObjectPool<AssetBundleItem> m_AssetBundleItemPool =
            ObjectManager.Instance.GetOrCreateClassPool<AssetBundleItem>(500);

        private static string ABTargetFolder = Application.streamingAssetsPath + "/myAB/";
        
        public bool LoadAssetBundleConfig()
        {
#if UNITY_EDITOR
            if (!ResourcesManager.Instance.m_LoadFromAssetBundle)
                return false;
#endif
            m_ResouceItemDic.Clear();
            string configPath = ABTargetFolder + "abconfig";
            AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
            TextAsset textAsset = configAB.LoadAsset<TextAsset>("AssetBundleConfig");
            if (textAsset == null)
            {
                Debug.LogError("AssetBundleConfig is no exist!");
                return false;
            }

            MemoryStream stream = new MemoryStream(textAsset.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            AssetBundleConfig config = (AssetBundleConfig) bf.Deserialize(stream);
            stream.Close();

            for (int i = 0; i < config.ABList.Count; i++)
            {
                ABBase abBase = config.ABList[i];
                ResourceItem item = new ResourceItem();
                item.m_Crc = abBase.Crc;
                item.m_AssetName = abBase.AssetName;
                item.m_ABName = abBase.ABName;
                item.m_DependAssetBundle = abBase.ABDependce;
                if (m_ResouceItemDic.ContainsKey(item.m_Crc))
                {
                    Debug.LogError("重复的Crc 资源名:" + item.m_AssetName + " ab包名:" + item.m_ABName);
                }
                else
                {
                    m_ResouceItemDic.Add(item.m_Crc, item);
                }
            }

            return true;
        }

        /// <summary>
        /// 根据路径的crc加载中间类ResourceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResourceItem LoadResourceAssetBundle(uint crc)
        {
            ResourceItem item = null;
            if (!m_ResouceItemDic.TryGetValue(crc, out item) || item == null)
            {
                Debug.LogError(string.Format("LoadResourceAssetBundle error:can not find crc {0} in AssetBundleConfig",
                    crc.ToString()));
                return item;
            }

            if (item.m_AssetBundle != null)
            {
                return item;
            }

            item.m_AssetBundle = LoadAssetBundle(item.m_ABName);
            if (item.m_DependAssetBundle != null)
            {
                for (int i = 0; i < item.m_DependAssetBundle.Count; i++)
                {
                    LoadAssetBundle(item.m_DependAssetBundle[i]);
                }
            }

            return item;
        }

        /// <summary>
        /// 根据名字加载单个AssetBundle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string name)
        {
            AssetBundleItem item = null;
            uint crc = MyCrc32.GetCRC32(name);

            if (!m_AssetBundleItemDic.TryGetValue(crc, out item))
            {
                AssetBundle ab = null;
                string fullPath = ABTargetFolder + name;
                if (File.Exists(fullPath))
                {
                    ab = AssetBundle.LoadFromFile(fullPath);
                }

                if (ab == null)
                {
                    Debug.LogError("Load AssetBundle Error:" + fullPath);
                }

                item = m_AssetBundleItemPool.Spawn(true);
                item.assetBundle = ab;
                item.RefCount++;
                m_AssetBundleItemDic.Add(crc,item);
            }
            else
            {
                item.RefCount++;
            }
            
            return item.assetBundle;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="item"></param>
        public void ReleaseAsset(ResourceItem item)
        {
            if (item == null)
                return;
            if (item.m_DependAssetBundle != null && item.m_DependAssetBundle.Count > 0)
            {
                for (int i = 0; i < item.m_DependAssetBundle.Count; i++)
                {
                    UnloadAssetBundle(item.m_DependAssetBundle[i]);
                }
            }
            UnloadAssetBundle(item.m_ABName);
        }

        private void UnloadAssetBundle(string name)
        {
            AssetBundleItem item = null;
            uint crc = MyCrc32.GetCRC32(name);
            if (m_AssetBundleItemDic.TryGetValue(crc, out item) && item != null)
            {
                item.RefCount--;
                if (item.RefCount <= 0 && ReferenceEquals(item.assetBundle,null) )
                {
                    item.assetBundle.Unload(true);
                    item.Reset();
                    m_AssetBundleItemPool.Recycle(item);
                    m_AssetBundleItemDic.Remove(crc);
                }
            }
        }

        /// <summary>
        /// 根据crc差找ResourceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResourceItem FindResourceItem(uint crc)
        {
            m_ResouceItemDic.TryGetValue(crc, out var item);
            return item;
        }
    }

    public class AssetBundleItem
    {
        public AssetBundle assetBundle = null;
        public int RefCount;

        public void Reset()
        {
            assetBundle = null;
            RefCount = 0;
        }
    }

    public class ResourceItem
    {
        //资源路径的CRC
        public uint m_Crc = 0;

        //该资源的文件名
        public string m_AssetName = String.Empty;

        //该资源所在的AssetBundle名
        public string m_ABName = string.Empty;

        //该资源所依赖的AssetBundle
        public List<string> m_DependAssetBundle = null;
        
        //该资源所在的AB包
        public AssetBundle m_AssetBundle;
        
        //-----------------------------------
        //资源对象
        public Object m_Obj = null;
        //资源唯一标识
        public int m_Guid = 0;
        //资源最后所使用的时间
        public float m_LastUseTime = 0;
        //引用计数
        protected int m_RefCount = 0;
        //是否跳场景清除
        public bool m_Clear = true;
        public int RefCount
        {
            get => m_RefCount;
            set
            {
                m_RefCount = value;
                if (m_RefCount < 0)
                {
                    Debug.LogError("RefCount < 0,"+m_RefCount+","+(m_Obj != null ? m_Obj.name : "name is null"));
                }
            }
        }
    }
}