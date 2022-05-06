using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Metafalica.RPG
{
    public class DefaultNpcData
    {
        public List<NPCData> NpcDatas = new List<NPCData>();
    }
    
    public class NPCManager : Singleton<NPCManager>
    {
        private DefaultNpcData _defaultNpcData;
        
        private Dictionary<int, NPCData> _npc = new Dictionary<int, NPCData>();

        public Dictionary<int, NPCData> AllNpc => _npc;
        
        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            CfgManager.GetNpcInfo(ref _defaultNpcData);

            for (int i = 0; i < _defaultNpcData.NpcDatas.Count; i++)
            {
                SpawnNpc(_defaultNpcData.NpcDatas[i]);
                _npc.Add(_defaultNpcData.NpcDatas[i].id,_defaultNpcData.NpcDatas[i]);
            }
        }


        public void SpawnNpc(NPCData data)
        {
            ObjectManager.Instance.InstantiateOjectAsync(GamePath.GetNpcPrefabPath(data.prefabPath), OnSpawnEnd,
                LoadResPriority.RES_MIDDLE,false,true,data);
        }

        private void OnSpawnEnd(string path,Object go, params object[] param)
        {
            if (param.Length > 0)
            {
                GameObject obj = go as GameObject;
                obj.GetComponent<NPCBase>().Init(param[0] as NPCData);
            }
            
        }
        

    }

}
