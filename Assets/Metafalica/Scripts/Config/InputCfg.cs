using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Metafalica.RPG
{
    [Serializable]
    public class InputCfg
    {
        [JsonProperty("Key")]
        public InputCfgData[] m_Keys;
        
        [Serializable]
        public class InputCfgData
        {
            public string Name;
            public int Enable;
            public int VirtualKeyType;//什么类型的按键
            public string[] KeyCode;
            public int ClickCount;//点几次触发
            public int KeyType;//presskey and axiskey共用
            public int AxisKeyDimension;
        }
    
    }

}
