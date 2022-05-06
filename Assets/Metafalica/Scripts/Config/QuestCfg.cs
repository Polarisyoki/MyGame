using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Metafalica.RPG
{
    public class QuestCfg
    {
        [JsonProperty("任务")]
        public List<QuestCfgData> m_Quest;
        [JsonProperty("任务对话")]
        public List<QuestDialogCfgData> m_QuestDialog;
        [JsonProperty("任务调查点")]
        public List<QuestPointCfgData> m_QuestPoint;
    
        [Serializable]
        public class QuestCfgData
        {
            public int ID;
            public string Name;
            public string Description;
            public int RewardCash;
            public int RewardExp;
            public int[][] RewardItem;
            public int Repeatable;
            public int TaskTaker;
            public int[][] QuestConditions;
            public int QuestType;
            public int[][] Quests;
            public int SubmissionNpcID;
        }
    
        [Serializable]
        public class QuestDialogCfgData
        {
            public int ID;
            public int QuestPhase;
            public string[][] Dialog;
        }
    
        [Serializable]
        public class QuestPointCfgData
        {
            public int ID;
            public string Name;
            public string[] Position;
            public string PrefabPath;
        }
    }
}

