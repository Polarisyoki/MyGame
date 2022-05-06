using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Metafalica.RPG
{
    public class ObjectManager : Singleton<ObjectManager>
    {
        //对象池节点
        public Transform RecyclePoolTf;

        //场景节点
        public Transform SceneTf;

        //对象池
        protected Dictionary<uint, List<ResourceObj>> m_ObjectPoolDic = new Dictionary<uint, List<ResourceObj>>();

        //暂存ResObj的dic
        protected Dictionary<int, ResourceObj> m_ResourceObjDic = new Dictionary<int, ResourceObj>();

        // ResourceObj的类对象池
        protected ClassObjectPool<ResourceObj> m_ResourceObjClassPool;

        //根据异步的guid储存ResourceObj,判断是否正在异步加载
        protected Dictionary<long, ResourceObj> m_AsyncResObjs = new Dictionary<long, ResourceObj>();

        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            GameObject go = new GameObject();
            go.name = "PoolManager";
            go.transform.SetParent(mono.transform);

            RecyclePoolTf = new GameObject().transform;
            RecyclePoolTf.name = "RecycleTfs";
            RecyclePoolTf.SetParent(go.transform);
            RecyclePoolTf.gameObject.SetActive(false);

            SceneTf = new GameObject().transform;
            SceneTf.name = "SceneTfs";
            SceneTf.SetParent(go.transform);

            m_ResourceObjClassPool = GetOrCreateClassPool<ResourceObj>(1000);
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearCache()
        {
            List<uint> tempList = new List<uint>();
            foreach (uint key in m_ObjectPoolDic.Keys)
            {
                List<ResourceObj> list = m_ObjectPoolDic[key];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ResourceObj resObj = list[i];
                    if (ReferenceEquals(resObj.m_CloneObj, null) || resObj.m_bClear)
                    {
                        GameObject.Destroy(resObj.m_CloneObj);
                        m_ResourceObjDic.Remove(resObj.m_CloneObj.GetInstanceID());
                        resObj.Reset();
                        m_ResourceObjClassPool.Recycle(resObj);
                    }
                }

                if (list.Count <= 0)
                {
                    tempList.Add(key);
                }
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                uint temp = tempList[i];
                if (m_ObjectPoolDic.ContainsKey(temp))
                {
                    m_ObjectPoolDic.Remove(temp);
                }
            }

            tempList.Clear();
        }

        /// <summary>
        /// 清除某个资源在对象池中所有的对象
        /// </summary>
        /// <param name="crc"></param>
        public void ClearPoolObject(uint crc)
        {
            List<ResourceObj> list = null;
            if (!m_ObjectPoolDic.TryGetValue(crc, out list) || list == null)
                return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                ResourceObj resObj = list[i];
                if (resObj.m_bClear)
                {
                    list.Remove(resObj);
                    int tempID = resObj.m_CloneObj.GetInstanceID();
                    GameObject.Destroy(resObj.m_CloneObj);
                    resObj.Reset();
                    m_ResourceObjDic.Remove(tempID);
                    m_ResourceObjClassPool.Recycle(resObj);
                }
            }

            if (list.Count <= 0)
            {
                m_ObjectPoolDic.Remove(crc);
            }
        }

        /// <summary>
        /// 从对象池取对象
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        protected ResourceObj GetObjectFromPool(uint crc)
        {
            List<ResourceObj> list = null;
            if (m_ObjectPoolDic.TryGetValue(crc, out list) && list != null && list.Count > 0)
            {
                ResourcesManager.Instance.IncreaseResourceRef(crc);
                ResourceObj resObj = list[0];
                list.RemoveAt(0);
                GameObject obj = resObj.m_CloneObj;
                if (!ReferenceEquals(obj, null))
                {
                    resObj.m_Already = false;
#if UNITY_EDITOR
                    if (obj.name.EndsWith("(Recycle)"))
                    {
                        obj.name = obj.name.Replace("(Recycle)", "");
                    }
#endif
                }

                return resObj;
            }

            return null;
        }

        /// <summary>
        /// 取消异步加载
        /// </summary>
        /// <param name="guid"></param>
        public void CancelLoad(long guid)
        {
            ResourceObj resObj = null;
            if (m_AsyncResObjs.TryGetValue(guid, out resObj) && ResourcesManager.Instance.CancelLoad(resObj))
            {
                m_AsyncResObjs.Remove(guid);
                resObj.Reset();
                m_ResourceObjClassPool.Recycle(resObj);
            }
        }

        /// <summary>
        /// 是否正在异步加载
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool IsingAsyncLoad(long guid)
        {
            return m_AsyncResObjs[guid] != null;
        }

        /// <summary>
        /// 该对象是否是对象池创建的
        /// </summary>
        /// <returns></returns>
        public bool IsObjectManagerCreat(GameObject obj)
        {
            ResourceObj resObj = m_ResourceObjDic[obj.GetInstanceID()];
            return resObj != null;
        }

        /// <summary>
        /// 预加载GameObject
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">预加载个数</param>
        /// <param name="clear">跳场景是否清除</param>
        public void PreLoadGameObject(string path, int count = 1, bool clear = false)
        {
            List<GameObject> tempGameObjList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = InstantiateObject(path, false, clear);
                tempGameObjList.Add(obj);
            }

            for (int i = 0; i < count; i++)
            {
                GameObject obj = tempGameObjList[i];
                ReleaseObject(obj);
                obj = null;
            }

            tempGameObjList.Clear();
        }

        /// <summary>
        /// 同步加载GameObject
        /// </summary>
        public GameObject InstantiateObject(string path, bool setSceneObj = false, bool bClear = true)
        {
            return InstantiateObject(path, setSceneObj ? SceneTf : null, bClear);
        }

        public GameObject InstantiateObject(string path, Transform parent, bool bClear = true)
        {
            uint crc = MyCrc32.GetCRC32(path);
            ResourceObj resObj = GetObjectFromPool(crc);
            if (resObj == null)
            {
                resObj = m_ResourceObjClassPool.Spawn(true);
                resObj.m_Crc = crc;
                resObj.m_bClear = bClear;
                //ResourceManager提供加载方法
                resObj = ResourcesManager.Instance.LoadResource(path, resObj);

                if (resObj.m_ResItem.m_Obj != null)
                {
                    resObj.m_CloneObj = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;
                }
            }

            if (parent != null)
            {
                resObj.m_CloneObj.transform.SetParent(parent, false);
            }


            int tempId = resObj.m_CloneObj.GetInstanceID();
            if (!m_ResourceObjDic.ContainsKey(tempId))
            {
                m_ResourceObjDic.Add(tempId, resObj);
            }

            return resObj.m_CloneObj;
        }

        /// <summary>
        /// 异步对象加载
        /// </summary>
        public long InstantiateOjectAsync(string path, OnAsyncObjFinish dealFinish, LoadResPriority priority,
            bool setSceneObject = false, bool bClear = true, params object[] param)
        {
            if (string.IsNullOrEmpty(path))
                return 0;

            uint crc = MyCrc32.GetCRC32(path);
            ResourceObj resObj = GetObjectFromPool(crc);
            if (resObj != null)
            {
                if (setSceneObject)
                {
                    resObj.m_CloneObj.transform.SetParent(SceneTf, false);
                }

                if (dealFinish != null)
                {
                    dealFinish(path, resObj.m_CloneObj, param);
                }

                return resObj.m_Guid;
            }

            long guid = ResourcesManager.Instance.CreateGuid();
            resObj = m_ResourceObjClassPool.Spawn(true);
            resObj.m_Crc = crc;
            resObj.m_SetSceneParent = setSceneObject;
            resObj.m_bClear = bClear;
            resObj.m_DealFinish = dealFinish;
            resObj.m_Params = param;

            ResourcesManager.Instance.AsyncLoadResource(path, resObj, OnLoadResourceObjFinish, priority);
            return guid;
        }

        /// <summary>
        /// 资源加载完成回调
        /// </summary>
        void OnLoadResourceObjFinish(string path, ResourceObj resObj,params object[] param)
        {
            if (resObj == null)
                return;
            if (resObj.m_ResItem.m_Obj == null)
            {
#if UNITY_EDITOR
                Debug.LogError("异步资源加载的资源为空:" + path);
#endif
            }
            else
            {
                resObj.m_CloneObj = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;
            }

            //加载完成就从正在加载的异步中移除
            if (m_AsyncResObjs.ContainsKey(resObj.m_Guid))
            {
                m_AsyncResObjs.Remove(resObj.m_Guid);
            }

            if (ReferenceEquals(resObj.m_CloneObj, null) && resObj.m_SetSceneParent)
            {
                resObj.m_CloneObj.transform.SetParent(SceneTf, false);
            }

            if (resObj.m_DealFinish != null)
            {
                int tempID = resObj.m_CloneObj.GetInstanceID();
                if (!m_ResourceObjDic.ContainsKey(tempID))
                {
                    m_ResourceObjDic.Add(tempID, resObj);
                }
                resObj.m_DealFinish(path, resObj.m_CloneObj, resObj.m_Params);
            }
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="maxCacheCount">最大缓存个数(如果是0则清除，负数则不限)</param>
        /// <param name="destroyCache">是否销毁缓存</param>
        /// <param name="recycleParent">是否进入回收父对象下</param>
        public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destroyCache = false,
            bool recycleParent = true)
        {
            if (obj == null)
                return;
            ResourceObj resObj = null;
            int tempID = obj.GetInstanceID();
            if (!m_ResourceObjDic.TryGetValue(tempID, out resObj))
            {
                Debug.Log(obj.name + "对象不是ObjectManager创建的!");
                return;
            }

            if (resObj == null)
            {
                Debug.LogError("缓存的ResourceObj为空!");
                return;
            }

            if (resObj.m_Already)
            {
                Debug.LogError("该对象已经被放回对象池，检查自己是否清空引用!");
                return;
            }
#if UNITY_EDITOR
            obj.name += "(Recycle)";
#endif

            List<ResourceObj> list = null;
            if (maxCacheCount == 0)
            {
                m_ResourceObjDic.Remove(tempID);
                ResourcesManager.Instance.ReleaseResource(resObj, destroyCache);
                resObj.Reset();
                m_ResourceObjClassPool.Recycle(resObj);
            }
            else //回收到对象池
            {
                if (!m_ObjectPoolDic.TryGetValue(resObj.m_Crc, out list) || list == null)
                {
                    list = new List<ResourceObj>();
                    m_ObjectPoolDic.Add(resObj.m_Crc, list);
                }

                if (resObj.m_CloneObj)
                {
                    if (recycleParent)
                    {
                        resObj.m_CloneObj.transform.SetParent(RecyclePoolTf, false);
                    }
                    else
                    {
                        resObj.m_CloneObj.SetActive(false);
                    }
                }

                if (maxCacheCount < 0 || list.Count < maxCacheCount)
                {
                    list.Add(resObj);
                    resObj.m_Already = true;
                    ResourcesManager.Instance.DecreaseResourceRef(resObj);
                }
                else
                {
                    m_ResourceObjDic.Remove(tempID);
                    ResourcesManager.Instance.ReleaseResource(resObj, destroyCache);
                    resObj.Reset();
                    m_ResourceObjClassPool.Recycle(resObj);
                }
            }
        }

        #region 类对象池的使用

        protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();

        /// <summary>
        /// 创建类对象池，创建完成后外部可以保存ClassObjectPool<T>，方便生成和回收
        /// </summary>
        /// <param name="maxCount"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ClassObjectPool<T> GetOrCreateClassPool<T>(int maxCount) where T : class, new()
        {
            Type type = typeof(T);
            object outobj = null;
            if (!m_ClassPoolDic.TryGetValue(type, out outobj) || outobj == null)
            {
                ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxCount);
                m_ClassPoolDic.Add(type, newPool);
                return newPool;
            }

            return outobj as ClassObjectPool<T>;
        }

        #endregion
    }
}