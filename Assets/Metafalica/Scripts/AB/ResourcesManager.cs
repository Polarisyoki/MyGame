using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Metafalica.RPG
{
    public enum LoadResPriority
    {
        RES_HIGHT = 0, //最高优先级
        RES_MIDDLE, //一般优先级
        RES_LOW, //低优先级
        RES_NUM,
    }

    public class ResourceObj
    {
        //路径对应crc
        public uint m_Crc = 0;

        //存ResourceItem
        public ResourceItem m_ResItem = null;

        //实例化出来的GameObject
        public GameObject m_CloneObj = null;

        //跳场景是否清除
        public bool m_bClear = true;

        //储存GUID
        public long m_Guid = 0;

        //是否已经放回对象池
        public bool m_Already = false;

        //------------------------
        //是否放到场景节点下
        public bool m_SetSceneParent = false;

        //实例化资源加载完成回调
        public OnAsyncObjFinish m_DealFinish = null;

        //异步参数
        public object[] m_Params = null;


        public void Reset()
        {
            m_Crc = 0;
            m_ResItem = null;
            m_CloneObj = null;
            m_bClear = true;
            m_Guid = 0;
            m_Already = false;
            m_SetSceneParent = false;
            m_DealFinish = null;
            m_Params = null;
        }
    }

    public class AsyncLoadResParam
    {
        public List<AsyncCallBack> m_CallBackList = new List<AsyncCallBack>();
        public uint m_Crc;
        public string m_Path;
        public bool m_IsSprite = false;
        public LoadResPriority m_Priority = LoadResPriority.RES_LOW;

        public void Reset()
        {
            m_CallBackList.Clear();
            m_Crc = 0;
            m_Path = "";
            m_IsSprite = false;
            m_Priority = LoadResPriority.RES_LOW;
        }
    }

    public class AsyncCallBack
    {
        //加载完成回调(针对ObjectManager)
        public OnAsyncFinish m_DealFinish = null;

        //ObjectManager对应的中间类
        public ResourceObj m_ResObj = null;

        //---------------------------
        //加载完成的回调
        public OnAsyncObjFinish m_DealObjFinish = null;

        //回调参数
        public object[] m_Params = null;

        public void Reset()
        {
            m_DealFinish = null;
            m_ResObj = null;
            m_DealObjFinish = null;
            m_Params = null;
        }
    }

    //资源加载完成回调
    public delegate void OnAsyncObjFinish(string path, Object obj,params object[] param);

    //实例化对象加载完成回调
    public delegate void OnAsyncFinish(string path, ResourceObj resObj, params object[] param);

    public class ResourcesManager : Singleton<ResourcesManager>
    {
        protected long m_Guid = 0;

        //是否使用ab包加载（否则使用编辑器加载方式）
        public bool m_LoadFromAssetBundle = false;

        //最长连续卡着加载资源的时间，单位微秒
        private const long MAXLOADRESTIME = 200000;

        //最大缓存个数
        private const int MAXCACHECOUNT = 500;

        //缓存使用的资源列表
        public Dictionary<uint, ResourceItem> AssetDic { get; set; } = new Dictionary<uint, ResourceItem>();

        //缓存引用计数为0的资源列表,达到缓存最大的时候释放列表里最早未用的资源
        protected CMapList<ResourceItem> m_NoRefrenceAssetMapList = new CMapList<ResourceItem>();

        //中间类，回调类的类对象池
        protected ClassObjectPool<AsyncLoadResParam> m_AsyncLoadResParamPool =
            ObjectManager.Instance.GetOrCreateClassPool<AsyncLoadResParam>(50);

        protected ClassObjectPool<AsyncCallBack> m_AsyncCallBackPool =
            ObjectManager.Instance.GetOrCreateClassPool<AsyncCallBack>(100);

        //mono脚本
        protected MonoBehaviour s_StartMono;

        //正在异步加载的资源列表
        protected List<AsyncLoadResParam>[] m_LoadingAssetList =
            new List<AsyncLoadResParam>[(int) LoadResPriority.RES_NUM];

        //正在异步加载的Dic
        protected Dictionary<uint, AsyncLoadResParam> m_LoadingAssetDic = new Dictionary<uint, AsyncLoadResParam>();

        public override void Init(MonoBehaviour mono)
        {
            AssetBundleManager.Instance.LoadAssetBundleConfig();

            for (int i = 0; i < (int) LoadResPriority.RES_NUM; i++)
            {
                m_LoadingAssetList[i] = new List<AsyncLoadResParam>();
            }

            s_StartMono = mono;
            s_StartMono.StartCoroutine(AsyncLoadCor());
        }

        /// <summary>
        /// 创建唯一的GUID
        /// </summary>
        /// <returns></returns>
        public long CreateGuid()
        {
            return m_Guid++;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            List<ResourceItem> tempList = new List<ResourceItem>();
            foreach (ResourceItem item in AssetDic.Values)
            {
                if (item.m_Clear)
                    tempList.Add(item);
            }

            //因为DestroyResourceItem有对AssetDic的移除操作，因此不能直接在上方调用
            foreach (ResourceItem item in tempList)
            {
                DestroyResourceItem(item, true);
            }

            tempList.Clear();
        }

        /// <summary>
        /// 取消异步加载资源
        /// </summary>
        /// <returns></returns>
        public bool CancelLoad(ResourceObj res)
        {
            AsyncLoadResParam para = null;
            if (m_LoadingAssetDic.TryGetValue(res.m_Crc, out para) &&
                m_LoadingAssetList[(int) para.m_Priority].Contains(para))
            {
                for (int i = para.m_CallBackList.Count - 1; i >= 0; i--)
                {
                    AsyncCallBack tempCallBack = para.m_CallBackList[i];
                    if (tempCallBack != null && res == tempCallBack.m_ResObj)
                    {
                        tempCallBack.Reset();
                        m_AsyncCallBackPool.Recycle(tempCallBack);
                        para.m_CallBackList.Remove(tempCallBack);
                    }
                }

                if (para.m_CallBackList.Count <= 0)
                {
                    para.Reset();
                    m_LoadingAssetList[(int) para.m_Priority].Remove(para);
                    m_AsyncLoadResParamPool.Recycle(para);
                    m_LoadingAssetDic.Remove(res.m_Crc);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据ResObj增加引用计数
        /// </summary>
        /// <returns></returns>
        public int IncreaseResourceRef(ResourceObj resObj, int count = 1)
        {
            return resObj != null ? IncreaseResourceRef(resObj.m_Crc, count) : 0;
        }

        /// <summary>
        /// 根据path增加引用计数
        /// </summary>
        /// <returns></returns>
        public int IncreaseResourceRef(uint crc, int count = 1)
        {
            ResourceItem item = null;
            if (!AssetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount += count;
            item.m_LastUseTime = Time.realtimeSinceStartup;

            return item.RefCount;
        }

        /// <summary>
        /// 根据ResObj减少引用计数
        /// </summary>
        /// <param name="resObj"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResourceRef(ResourceObj resObj, int count = 1)
        {
            return resObj != null ? DecreaseResourceRef(resObj.m_Crc, count) : 0;
        }

        /// <summary>
        /// 根据路径减少引用计数
        /// </summary>
        /// <param name="resObj"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResourceRef(uint crc, int count = 1)
        {
            ResourceItem item = null;
            if (!AssetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount -= count;

            return item.RefCount;
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="path"></param>
        public void PreloadRes(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            uint crc = MyCrc32.GetCRC32(path);
            ResourceItem item = GetCasheResourceItem(crc, 0);
            if (item != null)
            {
                return;
            }

            Object obj = null;
#if UNITY_EDITOR
            if (!m_LoadFromAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResourceItem(crc);
                if (item != null && item.m_Obj != null)
                {
                    obj = item.m_Obj;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }

                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResourceAssetBundle(crc);
                if (item != null && item.m_AssetBundle != null)
                {
                    if (item.m_Obj != null)
                    {
                        obj = item.m_Obj;
                    }
                    else
                    {
                        obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
                    }
                }
            }

            CacheResource(path, ref item, crc, obj);
            //跳场景不清空缓存
            item.m_Clear = false;
            ReleaseResource(obj, false);
        }

        /// <summary>
        /// 同步加载资源，针对给ObjectManager的方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <returns></returns>
        public ResourceObj LoadResource(string path, ResourceObj resObj)
        {
            if (resObj == null)
                return null;
            uint crc = resObj.m_Crc == 0 ? MyCrc32.GetCRC32(path) : resObj.m_Crc;
            ResourceItem item = GetCasheResourceItem(crc);
            if (item != null)
            {
                resObj.m_ResItem = item;
                return resObj;
            }

            Object obj = null;
#if UNITY_EDITOR
            if (!m_LoadFromAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResourceItem(crc);
                if (item != null && item.m_Obj != null)
                {
                    obj = item.m_Obj as Object;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }

                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResourceAssetBundle(crc);
                if (item != null && ReferenceEquals(item.m_AssetBundle, null))
                {
                    if (item.m_Obj != null)
                    {
                        obj = item.m_Obj as Object;
                    }
                    else
                    {
                        obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
                    }
                }
            }

            CacheResource(path, ref item, crc, obj);
            resObj.m_ResItem = item;
            item.m_Clear = resObj.m_bClear;
            return resObj;
        }

        /// <summary>
        /// 同步资源加载，外部直接调用，仅加载不需要实例化的资源，如图片、音频等
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            uint crc = MyCrc32.GetCRC32(path);
            ResourceItem item = GetCasheResourceItem(crc);
            if (item != null)
            {
                return item.m_Obj as T;
            }

            T obj = null;
#if UNITY_EDITOR
            if (!m_LoadFromAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResourceItem(crc);
                if (item != null && item.m_Obj != null)
                {
                    obj = item.m_Obj as T;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }

                    obj = LoadAssetByEditor<T>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResourceAssetBundle(crc);
                if (item != null && item.m_AssetBundle != null)
                {
                    if (item.m_Obj != null)
                    {
                        obj = item.m_Obj as T;
                    }
                    else
                    {
                        obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
                    }
                }
            }

            CacheResource(path, ref item, crc, obj);
            return obj;
        }

        /// <summary>
        /// 根据ResourceObj卸载资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroyObj"></param>
        /// <returns></returns>
        public bool ReleaseResource(ResourceObj resObj, bool destroyObj = false)
        {
            if (resObj == null)
                return false;
            ResourceItem item = null;
            if (!AssetDic.TryGetValue(resObj.m_Crc, out item) || item == null)
            {
                Debug.LogError("AssetDic里不存在该资源:" + resObj.m_CloneObj.name + " 可能释放了多次");
                return false;
            }

            GameObject.Destroy(resObj.m_CloneObj);

            item.RefCount--;
            DestroyResourceItem(item, destroyObj);
            return true;
        }

        /// <summary>
        /// 不需要实例化的资源的卸载，根据对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroyObj"></param>
        /// <returns></returns>
        public bool ReleaseResource(Object obj, bool destroyObj = false)
        {
            if (obj == null)
            {
                return false;
            }

            ResourceItem item = null;
            foreach (ResourceItem res in AssetDic.Values)
            {
                if (res.m_Guid == obj.GetInstanceID())
                {
                    item = res;
                }
            }

            if (item == null)
            {
                Debug.LogError("AssetDic里不存在该资源:" + obj.name + " 可能释放了多次");
                return false;
            }

            item.RefCount--;
            DestroyResourceItem(item, destroyObj);
            return true;
        }

        /// <summary>
        /// 不需要实例化的资源的卸载，根据路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destroyObj"></param>
        /// <returns></returns>
        public bool ReleaseResource(string path, bool destroyObj = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            uint crc = MyCrc32.GetCRC32(path);
            ResourceItem item = null;
            if (!AssetDic.TryGetValue(crc, out item) || item == null)
            {
                Debug.LogError("AssetDic里不存在该资源:" + path + " 可能释放了多次");
                return false;
            }

            item.RefCount--;
            DestroyResourceItem(item, destroyObj);
            return true;
        }

        /// <summary>
        /// 缓存加载的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="crc"></param>
        /// <param name="obj"></param>
        /// <param name="addRefCount"></param>
        void CacheResource(string path, ref ResourceItem item, uint crc, Object obj, int addRefCount = 1)
        {
            //缓存太多，清除最久未使用资源
            WashOut();

            if (item == null)
            {
                Debug.LogError("ResourceItem is null,path:" + path);
            }

            if (obj == null)
            {
                Debug.LogError("ResourceLoad Fail:" + path);
            }

            item.m_Obj = obj;
            item.m_Guid = obj.GetInstanceID();
            item.m_LastUseTime = Time.realtimeSinceStartup;
            item.RefCount += addRefCount;
            if (AssetDic.TryGetValue(item.m_Crc, out ResourceItem oldItem))
            {
                AssetDic[item.m_Crc] = item;
            }
            else
            {
                AssetDic.Add(item.m_Crc, item);
            }
        }

        /// <summary>
        /// 缓存太多，清除最久未使用资源
        /// </summary>
        protected void WashOut()
        {
            while (m_NoRefrenceAssetMapList.Size() >= MAXCACHECOUNT)
            {
                for (int i = 0; i < MAXCACHECOUNT / 2; i++)
                {
                    ResourceItem item = m_NoRefrenceAssetMapList.Back();
                    DestroyResourceItem(item, true);
                }
            }
        }

        /// <summary>
        /// 回收一个资源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="destroy"></param>
        protected void DestroyResourceItem(ResourceItem item, bool destroyCashe = false)
        {
            if (item == null || item.RefCount == 0)
                return;

            if (!destroyCashe)
            {
                m_NoRefrenceAssetMapList.InsertToHead(item);
                return;
            }

            if (!AssetDic.Remove(item.m_Crc))
                return;

            m_NoRefrenceAssetMapList.Remove(item);
            //释放AssetBundle引用
            AssetBundleManager.Instance.ReleaseAsset(item);
            //清空资源对应的对象池
            ObjectManager.Instance.ClearPoolObject(item.m_Crc);

            if (item.m_Obj != null)
            {
#if UNITY_EDITOR
                Resources.UnloadUnusedAssets();
#endif
                item.m_Obj = null;
            }
        }

#if UNITY_EDITOR
        protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif

        ResourceItem GetCasheResourceItem(uint crc, int addRefCount = 1)
        {
            ResourceItem item = null;
            if (AssetDic.TryGetValue(crc, out item))
            {
                if (item != null)
                {
                    item.RefCount += addRefCount;
                    item.m_LastUseTime = Time.realtimeSinceStartup;
                }
            }

            return item;
        }

        /// <summary>
        /// 异步加载资源（仅不需要实例化的资源，如音频、图片）
        /// </summary>
        public void AsyncLoadResource(string path, OnAsyncObjFinish dealFinish, LoadResPriority priority,
            bool isSprite = false,uint crc = 0, params object[] param)
        {
            if (crc == 0)
            {
                crc = MyCrc32.GetCRC32(path);
            }

            ResourceItem item = GetCasheResourceItem(crc);
            if (item != null)
            {
                if (dealFinish != null)
                {
                    dealFinish(path, item.m_Obj, param);
                }

                return;
            }

            //判读是否在加载中
            AsyncLoadResParam para = null;
            if (!m_LoadingAssetDic.TryGetValue(crc, out para) || para == null)
            {
                para = m_AsyncLoadResParamPool.Spawn(true);
                para.m_Crc = crc;
                para.m_Path = path;
                para.m_Priority = priority;
                para.m_IsSprite = isSprite;
                m_LoadingAssetDic.Add(crc, para);
                m_LoadingAssetList[(int) priority].Add(para);
            }

            //往回调列表里面加回调
            AsyncCallBack callBack = m_AsyncCallBackPool.Spawn(true);
            callBack.m_DealObjFinish = dealFinish;
            callBack.m_Params = param;
            para.m_CallBackList.Add(callBack);
        }

        /// <summary>
        /// 针对ObjectManager的异步资源加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <param name="dealFinish"></param>
        /// <param name="priority"></param>
        public void AsyncLoadResource(string path, ResourceObj resObj, OnAsyncFinish dealFinish,
            LoadResPriority priority)
        {
            ResourceItem item = GetCasheResourceItem(resObj.m_Crc);
            if (item != null)
            {
                resObj.m_ResItem = item;
                if (dealFinish != null)
                {
                    dealFinish(path, resObj);
                }

                return;
            }

            //判读是否在加载中
            AsyncLoadResParam para = null;
            if (!m_LoadingAssetDic.TryGetValue(resObj.m_Crc, out para) || para == null)
            {
                para = m_AsyncLoadResParamPool.Spawn(true);
                para.m_Crc = resObj.m_Crc;
                para.m_Path = path;
                para.m_Priority = priority;
                m_LoadingAssetDic.Add(resObj.m_Crc, para);
                m_LoadingAssetList[(int) priority].Add(para);
            }

            //往回调列表里面加回调
            AsyncCallBack callBack = m_AsyncCallBackPool.Spawn(true);
            callBack.m_DealFinish = dealFinish;
            callBack.m_ResObj = resObj;
            para.m_CallBackList.Add(callBack);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncLoadCor()
        {
            List<AsyncCallBack> callBackList = null;
            //上一次yield的时间
            long lastYildeTime = System.DateTime.Now.Ticks;
            while (true)
            {
                bool haveYield = false;

                for (int i = 0; i < (int) LoadResPriority.RES_NUM; i++)
                {
                    //↑这里应该不需要循环了
                    if (m_LoadingAssetList[(int) LoadResPriority.RES_HIGHT].Count > 0)
                    {
                        i = (int) LoadResPriority.RES_HIGHT;
                    }
                    else if (m_LoadingAssetList[(int) LoadResPriority.RES_MIDDLE].Count > 0)
                    {
                        i = (int) LoadResPriority.RES_MIDDLE;
                    }

                    List<AsyncLoadResParam> loadingList = m_LoadingAssetList[i];
                    if (loadingList.Count <= 0)
                        continue;
                    AsyncLoadResParam loadingItem = loadingList[0];
                    loadingList.RemoveAt(0);
                    callBackList = loadingItem.m_CallBackList;

                    Object obj = null;
                    ResourceItem item = null;
#if UNITY_EDITOR
                    if (!m_LoadFromAssetBundle)
                    {
                        if (loadingItem.m_IsSprite)
                        {
                            obj = LoadAssetByEditor<Sprite>(loadingItem.m_Path);
                        }
                        else
                        {
                            obj = LoadAssetByEditor<Object>(loadingItem.m_Path);
                        }

                        //模拟异步加载
                        yield return new WaitForSeconds(0.5f);
                        item = AssetBundleManager.Instance.FindResourceItem(loadingItem.m_Crc);
                        if (item == null)
                        {
                            item = new ResourceItem();
                            item.m_Crc = loadingItem.m_Crc;
                        }
                    }
#endif
                    if (obj == null)
                    {
                        item = AssetBundleManager.Instance.LoadResourceAssetBundle(loadingItem.m_Crc);
                        if (item != null && !System.Object.ReferenceEquals(item.m_AssetBundle, null))
                        {
                            AssetBundleRequest abRequest = null;
                            if (loadingItem.m_IsSprite)
                            {
                                abRequest = item.m_AssetBundle.LoadAssetAsync<Sprite>(item.m_AssetName);
                            }
                            else
                            {
                                abRequest = item.m_AssetBundle.LoadAssetAsync(item.m_AssetName);
                            }

                            yield return abRequest;
                            if (abRequest.isDone)
                            {
                                obj = abRequest.asset;
                            }

                            lastYildeTime = System.DateTime.Now.Ticks;
                        }
                    }

                    CacheResource(loadingItem.m_Path, ref item, loadingItem.m_Crc, obj, callBackList.Count);

                    for (int j = 0; j < callBackList.Count; j++)
                    {
                        AsyncCallBack callBack = callBackList[j];
                        if (callBack != null && callBack.m_DealFinish != null && callBack.m_ResObj != null)
                        {
                            ResourceObj tempResObj = callBack.m_ResObj;
                            tempResObj.m_ResItem = item;
                            callBack.m_DealFinish(loadingItem.m_Path, tempResObj, callBack.m_Params);
                            callBack.m_DealFinish = null;
                            tempResObj = null;
                        }

                        if (callBack != null && callBack.m_DealObjFinish != null)
                        {
                            callBack.m_DealObjFinish(loadingItem.m_Path, obj, callBack.m_Params);
                            callBack.m_DealObjFinish = null;
                        }

                        callBack.Reset();
                        m_AsyncCallBackPool.Recycle(callBack);
                    }

                    obj = null;
                    callBackList.Clear();
                    m_LoadingAssetDic.Remove(loadingItem.m_Crc);
                    loadingItem.Reset();
                    m_AsyncLoadResParamPool.Recycle(loadingItem);

                    if (System.DateTime.Now.Ticks - lastYildeTime > MAXLOADRESTIME)
                    {
                        yield return null;
                        lastYildeTime = System.DateTime.Now.Ticks;
                        haveYield = true;
                    }
                }

                if (!haveYield || System.DateTime.Now.Ticks - lastYildeTime > MAXLOADRESTIME)
                {
                    lastYildeTime = System.DateTime.Now.Ticks;
                    yield return null;
                }
            }
        }


        #region MainUseFunc

        public Sprite GetSprite(string path, bool isFullPath = false)
        {
            if (isFullPath)
            {
                return LoadResource<Sprite>(path);
            }

            return LoadResource<Sprite>(GamePath.Res_Sprite_Folder + path + ".png");
        }

        public TextAsset GetJsonFile(string path)
        {
            return LoadResource<TextAsset>(path);
        }

        #endregion
    }
}