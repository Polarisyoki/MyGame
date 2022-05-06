using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Metafalica.RPG
{
    public class NpcCfg
    {
        [JsonProperty("Npc")]
        public List<NpcCfgData> m_Npc;
    }

    [Serializable]
    public class NpcCfgData
    {
        public int ID;
        public string Name;
        public string[] Position;
        public string[] Rotation;
        public string[] EntryDialog;
        public string[] ExitDialog;
        public string PrefabPath;
    }
}

