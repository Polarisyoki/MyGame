using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Metafalica.RPG
{
    //上连PlayerManager
    public class PlayerData
    {
        private int _curHP = 100;
        private int _curLevel = 1;
        private int _curExp = 0;
        private int _baseSTR = 4;
        private int _baseAGI = 3;
        private int _baseWIS = 3;
        private int _addSTR = 0;
        private int _addAGI = 0;
        private int _addWIS = 0;

        private EquipAttr _buff = new EquipAttr();

        private int _statusPoint = 0; //当前属性点

        private const int MaxLevel = 100;
        private const int UpgradeGetStatusPoints = 5; //升级可获得的属性点数量

        public UsingEquipment MUsingEquipment = new UsingEquipment(); //武器，衣服，鞋子

        public int Level => _curLevel;
        public int EXP => _curExp;
        public int StatusPoint => _statusPoint;

        public int STR => _baseSTR + _addSTR + MUsingEquipment.GetTotal_STR() + _buff.STR;

        public int AGI => _baseAGI + _addAGI + MUsingEquipment.GetTotal_AGI() + _buff.AGI;

        public int WIS => _baseWIS + _addWIS + MUsingEquipment.GetTotal_WIS() + _buff.WIS;

        public int ATK => STR + MUsingEquipment.GetTotal_ATK() + _buff.ATK;

        public int MATK => WIS + MUsingEquipment.GetTotal_MATK() + _buff.MATK;

        public int DEF => AGI + MUsingEquipment.GetTotal_DEF() + _buff.DEF;

        public int MDEF => AGI / 2 + WIS / 2 + MUsingEquipment.GetTotal_MDEF() + _buff.MDEF;

        public int MaxHP => _curLevel * (1 + _curLevel / 10) + STR + 20 + MUsingEquipment.GetTotal_HP() + _buff.HP;

        public void Init()
        {
            LoadAttr();
            CurrentHPChange(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">伤害值</param>
        /// <param name="type">伤害类型,0:真伤,1:物伤,2:法伤</param>
        public void OnDamage(int amount, int type)
        {
            if (amount <= 0) return;
            amount = -amount;
            if (type == 0)
            {
            }
            else if (type == 1)
            {
                amount = amount * (1 - (DEF + _curLevel) / (DEF + 450));
            }
            else if (type == 2)
            {
                amount = amount * (1 - (MDEF + _curLevel) / (MDEF + 450));
            }

            CurrentHPChange(amount);
        }

        void CurrentHPChange(int value)
        {
            _curHP += value;
            if (_curHP <= 0)
            {
                _curHP = 0;
                UITipManager.Instance.PushMessage("You Die!");
            }
            else if (_curHP > MaxHP)
            {
                _curHP = MaxHP;
            }
            
            // UIManager.Instance.
        }

        public void EXPChange(int value)
        {
            if (_curLevel == MaxLevel)
                return;
            _curExp += value;
            while (_curExp >= GetCurLevelUpgradeExp())
            {
                //玩家升级
                _curExp -= GetCurLevelUpgradeExp();
                _curLevel++;
                _statusPoint += UpgradeGetStatusPoints;
                if (_curLevel > MaxLevel)
                {
                    _curLevel = MaxLevel;
                    _curExp = 0;
                    break;
                }
            }

            SaveAttr();
        }

        /// <summary>
        /// 当前升级所需经验
        /// </summary>
        /// <returns></returns>
        public int GetCurLevelUpgradeExp()
        {
            int t = _curLevel * 10;
            return t;
        }

        public void ChangeAddAttr(int attrPoints, int str, int agi, int wis)
        {
            this._statusPoint = attrPoints;
            _addSTR += str;
            _addAGI += agi;
            _addWIS += wis;

            SaveAttr();
        }

        public void ChangeAddAttr(int attrPoints, int[] data)
        {
            ChangeAddAttr(attrPoints, data[0], data[1], data[2]);
        }

        public void ClearAddAttr()
        {
            this._statusPoint = _addSTR + _addAGI + _addWIS;
            _addSTR = 0;
            _addAGI = 0;
            _addWIS = 0;

            SaveAttr();
        }

        private void LoadAttr()
        {
            var data = StorageManager.LoadPlayerAttr();
            if (data == null)
                return;
            this._curLevel = data.CurLv;
            this._curExp = data.CurExp;
            this._statusPoint = data.StatusPoints;
            this._addSTR = data.AddPoint[0];
            this._addAGI = data.AddPoint[1];
            this._addWIS = data.AddPoint[2];
        }

        private void SaveAttr()
        {
            StorageManager.Storage_PlayerAttr data = new StorageManager.Storage_PlayerAttr();
            data.CurLv = _curLevel;
            data.CurExp = _curExp;
            data.StatusPoints = _statusPoint;
            data.AddPoint = new[] {_addSTR, _addAGI, _addWIS};
            StorageManager.SavePlayerAttr(data);
        }

        public void SetCurEquipmentData(Equipment data)
        {
            MUsingEquipment[(int)data.m_EquipType] = data.m_IsUse ? data : null;
        }
        
        public void SetCurEquipmentData(List<Equipment> equip)
        {
            MUsingEquipment[1] = equip.Find(c => c.m_EquipType == EquipType.Weapon);
            MUsingEquipment[2] = equip.Find(c => c.m_EquipType == EquipType.Armor);
            MUsingEquipment[3] = equip.Find(c => c.m_EquipType == EquipType.Shoes);
        }

        public Equipment GetCurEquipmentData(int index)
        {
            return MUsingEquipment[index];
        }
    }

    public class UsingEquipment
    {
        public Equipment Weapon;
        public Equipment Armor;
        public Equipment Boots;

        public Equipment this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1:
                        return Weapon;
                    case 2:
                        return Armor;
                    case 3:
                        return Boots;
                    default:
                        return null;
                }
            }
            set
            {
                switch (index)
                {
                    case 1:
                        Weapon = value;
                        break;
                    case 2:
                        Armor = value;
                        break;
                    case 3:
                        Boots = value;
                        break;
                    default:
                        return;
                }
            }
        }


        public int GetTotal_STR()
        {
            return Weapon?.m_Attr.STR ?? 0 + Armor?.m_Attr.STR ?? 0 + Boots?.m_Attr.STR ?? 0;
        }

        public int GetTotal_AGI()
        {
            return Weapon?.m_Attr.AGI ?? 0 + Armor?.m_Attr.AGI ?? 0 + Boots?.m_Attr.AGI ?? 0;
        }

        public int GetTotal_WIS()
        {
            return Weapon?.m_Attr.WIS ?? 0 + Armor?.m_Attr.WIS ?? 0 + Boots?.m_Attr.WIS ?? 0;
        }

        public int GetTotal_ATK()
        {
            return Weapon?.m_Attr.ATK ?? 0 + Armor?.m_Attr.ATK ?? 0 + Boots?.m_Attr.ATK ?? 0;
        }

        public int GetTotal_MATK()
        {
            return Weapon?.m_Attr.MATK ?? 0 + Armor?.m_Attr.MATK ?? 0 + Boots?.m_Attr.MATK ?? 0;
        }

        public int GetTotal_DEF()
        {
            return Weapon?.m_Attr.DEF ?? 0 + Armor?.m_Attr.DEF ?? 0 + Boots?.m_Attr.DEF ?? 0;
        }

        public int GetTotal_MDEF()
        {
            return Weapon?.m_Attr.MDEF ?? 0 + Armor?.m_Attr.MDEF ?? 0 + Boots?.m_Attr.MDEF ?? 0;
        }

        public int GetTotal_HP()
        {
            return Weapon?.m_Attr.HP ?? 0 + Armor?.m_Attr.HP ?? 0 + Boots?.m_Attr.HP ?? 0;
        }
    }
}