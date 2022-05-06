using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

namespace Metafalica.RPG
{
    public class StorageManager
    {
        private static string PlayerAttr_Path = Application.streamingAssetsPath + "/PlayerAttrData.json";
        private static string Bag_Path = Application.streamingAssetsPath + "/BagData.json";
        private static string Quest_Path = Application.streamingAssetsPath + "/QuestData.json";
        
        
        #region 玩家信息

        public class Storage_PlayerAttr
        {
            [JsonProperty("Lv")]
            public int CurLv;
            [JsonProperty("Exp")]
            public int CurExp;
            [JsonProperty("StatusPoints")]
            public int StatusPoints;
            [JsonProperty("AddPoint")]
            public int[] AddPoint;//str,agi,wis
        }

        public static Storage_PlayerAttr LoadPlayerAttr()
        {
            if (!File.Exists(PlayerAttr_Path))
                return null;
            MyJsonUtils.ReadFromJson(PlayerAttr_Path, out Storage_PlayerAttr _playerAttrData);
            return _playerAttrData;
        }
        
        public static void SavePlayerAttr(Storage_PlayerAttr data)
        {
            MyJsonUtils.WriteToJson(PlayerAttr_Path, data);
        }

        #endregion
        
        #region 道具
        public class Storage_Bag
        {
            public List<Storage_Bag_Equipment> Equipments;
            public List<Storage_Bag_Item> MatItems;
            public List<Storage_Bag_Item> TaskItems;
            public int Mana;
        }
        
        [Serializable]
        public class Storage_Bag_Equipment
        {
            public int KindsID;
            public int SelfID;
            public EquipAttr Attr;
            public bool IsUse;
        }
        [Serializable]
        public class Storage_Bag_Item
        {
            public int KindsID;
            public int Count;
        }
        

        public static Storage_Bag LoadBagData()
        {
            if (!File.Exists(Bag_Path))
                return null;
            MyJsonUtils.ReadFromJson(Bag_Path, out Storage_Bag _bagData);
            return _bagData;
        }
        
        public static void SaveBagData(PlayerInventoryC.BagData bagData)
        {
            Storage_Bag data = new Storage_Bag();
            data.Equipments = new List<Storage_Bag_Equipment>();
            foreach (var val in bagData.m_Equipment)
            {
                Storage_Bag_Equipment s = new Storage_Bag_Equipment();
                s.KindsID = val.ID;
                s.SelfID = val.m_SelfID;
                s.Attr = val.m_Attr;
                s.IsUse = val.m_IsUse;
                data.Equipments.Add(s);
            }
            data.MatItems = new List<Storage_Bag_Item>();
            foreach (var val in bagData.m_Mat)
            {
                Storage_Bag_Item s = new Storage_Bag_Item();
                s.KindsID = val.ID;
                s.Count = val.CurCount;
                data.MatItems.Add(s);
            }
            data.TaskItems = new List<Storage_Bag_Item>();
            foreach (var val in bagData.m_Task)
            {
                Storage_Bag_Item s = new Storage_Bag_Item();
                s.KindsID = val.ID;
                s.Count = val.CurCount;
                data.TaskItems.Add(s);
            }

            data.Mana = bagData.Money;
            MyJsonUtils.WriteToJson(Bag_Path, data);
        }

        #endregion

        #region 任务

        [Serializable]
        public class Storage_Quest
        {
            //存储完成次数
            public List<Storage_AllQuestCompleteTimes> AllTimes;
            //存储已接取任务
            public List<Storage_QuestInProgress> AllInProgresses;

            public Storage_Quest()
            {
                AllTimes = new List<Storage_AllQuestCompleteTimes>();
                AllInProgresses = new List<Storage_QuestInProgress>();
            }
        }
        [Serializable]
        public class Storage_AllQuestCompleteTimes
        {
            public int ID;
            public int CompleteTimes;

            public Storage_AllQuestCompleteTimes(int id, int times)
            {
                this.ID = id;
                this.CompleteTimes = times;
            }
        }
        [Serializable]
        public class Storage_QuestInProgress
        {
            public int ID;
            public int CompleteTimes;
            public Storage_QuestInProgress(int id, int times)
            {
                this.ID = id;
                this.CompleteTimes = times;
            }
        }

        public static void SaveQuestData(List<QuestData> questData,List<QuestData> curQuest)
        {
            Storage_Quest data = new Storage_Quest();
            for (int i = 0; i < questData.Count; i++)
            {
                if(questData[i].m_CompleteTimes == 0)
                    continue;
                data.AllTimes.Add(new Storage_AllQuestCompleteTimes(questData[i].m_Id,questData[i].m_CompleteTimes));
            }

            for (int i = 0; i < curQuest.Count; i++)
            {
                data.AllInProgresses.Add(new Storage_QuestInProgress(curQuest[i].m_Id,curQuest[i].m_CompleteTimes));
            }
            MyJsonUtils.WriteToJson(Quest_Path, data);
        }
        public static Storage_Quest LoadQuestData()
        {
            if (!File.Exists(Quest_Path))
                return null;
            MyJsonUtils.ReadFromJson(Quest_Path, out Storage_Quest _questData);
            return _questData;
        }

        #endregion

        
    }
}

