using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Metafalica.RPG
{
    public class ItemCfg
    {
        [JsonProperty("全部道具")]
        public List<ItemData> m_Item;
        [JsonProperty("装备")]
        public List<EquipData> m_Equip;
        
        [Serializable]
        public class ItemData
        {
            public int ID;
            public string Name;
            public int ItemType;
            public int MaxStack;
            public string IconPath;
            public string Description;
        }
        
        [Serializable]
        public class EquipData
        {
            public int ID;
            public string Name;
            public int EquipType;
            public int[] HP;
            public int[] ATK;
            public int[] MATK;
            public int[] DEF;
            public int[] MDEF;
            public int[] STR;
            public int[] AGI;
            public int[] WIS;
        }
    }
        
    
    
}

