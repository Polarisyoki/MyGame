using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Random = UnityEngine.Random;

namespace Metafalica.RPG
{
    [Serializable]
    public class Equipment : ItemBase
    {
        public int m_SelfID { get; private set; }

        public EquipType m_EquipType { get; private set; }
        
        public EquipAttr m_Attr { get; private set; }

        private EquipAttr _minAttr;
        private EquipAttr _maxAttr;
        public bool m_IsUse { get; private set; }

        public Equipment(Equipment item) : base(item)
        {
            this.m_EquipType = item.m_EquipType;
            this._minAttr = item._minAttr;
            this._maxAttr = item._maxAttr;
            this.m_SelfID = item.m_SelfID;
            this.m_IsUse = item.m_IsUse;
            this.m_Attr = new EquipAttr(item.m_Attr);
        }
                
        public Equipment(Equipment item, int selfId) : base(item)
        {
            this.m_EquipType = item.m_EquipType;
            this._minAttr = item._minAttr;
            this._maxAttr = item._maxAttr;
            this.m_SelfID = selfId;
            this.m_IsUse = false;
            SetRandomAttr();
        }
        
        public Equipment(Equipment item, int selfId, EquipAttr attr, bool isUse = false) : base(item)
        {
            this.m_EquipType = item.m_EquipType;
            this._minAttr = item._minAttr;
            this._maxAttr = item._maxAttr;
            this.m_SelfID = selfId;
            this.m_IsUse = isUse;
            this.m_Attr = new EquipAttr(attr);
        }


        public Equipment(ItemCfg.ItemData itemData, ItemCfg.EquipData equipData)
            : base(itemData)
        {
            this.m_EquipType = (EquipType) equipData.EquipType;
            _minAttr.HP = equipData.HP.Length == 0 ? 0 : equipData.HP[0];
            _maxAttr.HP = equipData.HP.Length == 0 ? 0 : equipData.HP[1];
            _minAttr.ATK = equipData.ATK.Length == 0 ? 0 : equipData.ATK[0];
            _maxAttr.ATK = equipData.ATK.Length == 0 ? 0 : equipData.ATK[1];
            _minAttr.MATK = equipData.MATK.Length == 0 ? 0 : equipData.MATK[0];
            _maxAttr.MATK = equipData.MATK.Length == 0 ? 0 : equipData.MATK[1];
            _minAttr.DEF = equipData.DEF.Length == 0 ? 0 : equipData.DEF[0];
            _maxAttr.DEF = equipData.DEF.Length == 0 ? 0 : equipData.DEF[1];
            _minAttr.MDEF = equipData.MDEF.Length == 0 ? 0 : equipData.MDEF[0];
            _maxAttr.MDEF = equipData.MDEF.Length == 0 ? 0 : equipData.MDEF[1];
            _minAttr.STR = equipData.STR.Length == 0 ? 0 : equipData.STR[0];
            _maxAttr.STR = equipData.STR.Length == 0 ? 0 : equipData.STR[1];
            _minAttr.AGI = equipData.AGI.Length == 0 ? 0 : equipData.AGI[0];
            _maxAttr.AGI = equipData.AGI.Length == 0 ? 0 : equipData.AGI[1];
            _minAttr.WIS = equipData.WIS.Length == 0 ? 0 : equipData.WIS[0];
            _maxAttr.WIS = equipData.WIS.Length == 0 ? 0 : equipData.WIS[1];
        }

        public void SetUsingStatus(bool isUse)
        {
            this.m_IsUse = isUse;
        }

        public void SetEquipmentProperty(EquipAttr attr)
        {
            this.m_Attr = attr;
        }

        /// <summary>
        /// 获取武器时，随机一个属性
        /// </summary>
        private void SetRandomAttr()
        {
            EquipAttr ea = new EquipAttr();
            ea.HP = Random.Range(_minAttr.HP, _maxAttr.HP);
            ea.ATK = Random.Range(_minAttr.ATK, _maxAttr.ATK);
            ea.MATK = Random.Range(_minAttr.MATK, _maxAttr.MATK);
            ea.DEF = Random.Range(_minAttr.DEF, _maxAttr.DEF);
            ea.MDEF = Random.Range(_minAttr.MDEF, _maxAttr.MDEF);
            ea.STR = Random.Range(_minAttr.STR, _maxAttr.STR);
            ea.AGI = Random.Range(_minAttr.AGI, _maxAttr.AGI);
            ea.WIS = Random.Range(_minAttr.WIS, _maxAttr.WIS);
            m_Attr = new EquipAttr(ea);
        }
    }

    public enum EquipType
    {
        Weapon = 1,
        Armor = 2,
        Shoes = 3,
        Unknown = 999,
    }

    [System.Serializable]
    public struct EquipAttr
    {
        public int HP;
        public int ATK;
        public int MATK;
        public int DEF;
        public int MDEF;
        public int STR;
        public int AGI;
        public int WIS;
        
        public EquipAttr(EquipAttr data)
        {
            this.HP = data.HP;
            this.ATK = data.ATK;
            this.MATK = data.MATK;
            this.DEF = data.DEF;
            this.MDEF = data.MDEF;
            this.STR = data.STR;
            this.AGI = data.AGI;
            this.WIS = data.WIS;
        }
        
        public string ShowInfo()
        {
            string str = "";
            if (ATK != 0) str += "ATK:" + ATK + "\n";
            if (MATK != 0) str += "MATK:" + MATK + "\n";
            if (DEF != 0) str += "DEF:" + DEF + "\n";
            if (MDEF != 0) str += "MDEF:" + MDEF + "\n";
            if (HP != 0) str += "HP:" + HP + "\n";
            if (STR != 0) str += "STR:" + STR + "\n";
            if (AGI != 0) str += "AGI:" + AGI + "\n";
            if (WIS != 0) str += "WIS:" + WIS + "\n";
            return str;
        }
    }
}