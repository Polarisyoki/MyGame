using System.Collections;
using System.Collections.Generic;
using Metafalica.RPG.InputSystem;
using UnityEngine;

namespace Metafalica.RPG
{
    public class CfgManager
    {
        /// <summary>
        /// 通过AB包中的json文件路径获取数据
        /// </summary>
        /// <param name="path">ab包中记录的json文件路径</param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        private static void GetJsonData<T>(string path, out T data)
        {
            TextAsset textAsset = ResourcesManager.Instance.GetJsonFile(path);
            MyJsonUtils.ReadFromJson(textAsset,out data);
        }
        
        //物品
        public static void GetItemInfo(ref DefaultItemData defaultItemData)
        {
            if (defaultItemData == null)
                defaultItemData = new DefaultItemData();
            var data = new ItemCfg();
            GetJsonData(ConfigPath.itemCfg_Path,out data);
            for (int i = 0; i < data.m_Item.Count; i++)
            {
                if (data.m_Item[i].ItemType == (int)ItemType.Equip)
                {
                    var t = data.m_Equip.Find(c => c.ID == data.m_Item[i].ID);
                    defaultItemData.Equips.Add(new Equipment(data.m_Item[i],t));
                }
                defaultItemData.AllItems.Add(new ItemBase(data.m_Item[i]));
            }
        }

        //任务
        public static void GetQuestInfo(ref DefaultQuestData defaultQuestData)
        {
            if (defaultQuestData == null)
                defaultQuestData = new DefaultQuestData();
            var data = new QuestCfg();
            GetJsonData(ConfigPath.questCfg_Path,out data);

            for (int i = 0; i < data.m_Quest.Count; i++)
            {
                defaultQuestData.QuestDatas.Add(new QuestData(data.m_Quest[i]));
            }
            for (int i = 0; i < data.m_QuestDialog.Count; i++)
            {
                defaultQuestData.QuestDialogDatas.Add(new QuestDialogData(data.m_QuestDialog[i]));
            }
            for (int i = 0; i < data.m_QuestPoint.Count; i++)
            {
                defaultQuestData.QuestPointDatas.Add(new QuestPointData(data.m_QuestPoint[i]));
            }
        }
        
        #region NPC
        public static void GetNpcInfo(ref DefaultNpcData defaultNpcData)
        {
            if (defaultNpcData == null)
                defaultNpcData = new DefaultNpcData();
            var data = new NpcCfg();
            GetJsonData(ConfigPath.npcCfg_Path,out data);
            
            for (int i = 0; i < data.m_Npc.Count; i++)
            {
                defaultNpcData.NpcDatas.Add(new NPCData(data.m_Npc[i]));
            }
        }
        

        #endregion

        //默认按键信息
        
        public static void GetInputInfo(ref InputData inputData)
        {
            var data = new InputCfg();
            GetJsonData(ConfigPath.inputCfg_Path,out data);
            inputData = new InputData(data);
        }

    }

    public class ConfigPath
    {
        public static string configPath = GamePath.Res_Config_Folder;
        public static string itemCfg_Path = configPath + "ItemCfg.json";
        public static string questCfg_Path = configPath + "QuestCfg.json";
        public static string npcCfg_Path = configPath + "NpcCfg.json";
        public static string inputCfg_Path = configPath + "InputCfg.json";
    }
}

