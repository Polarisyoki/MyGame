using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    
    public class PlayerInventoryC
    {
        public class BagData
        {
            public List<Equipment> m_Equipment;
            public List<Item_Mat> m_Mat;
            public List<Item_Task> m_Task;
            private int money = 0;

            public int Money
            {
                get => money;
                set
                {
                    if (value > 99999999) money = 99999999;
                    else if (value < 0) money = 0;
                    else money = value;
                }
            }

            public BagData()
            {
                m_Equipment = new List<Equipment>();
                m_Mat = new List<Item_Mat>();
                m_Task = new List<Item_Task>();
            }
        }

        private BagData _bagData;


        public event ItemInfoListener OnItemChangeEvent;

        public PlayerInventoryC()
        {
            _bagData = new BagData();
            GetStoreData();
        }

        private void GetStoreData()
        {
            StorageManager.Storage_Bag data = StorageManager.LoadBagData();
            if (data == null)
                return;
            foreach (var val in data.Equipments)
            {
                _bagData.m_Equipment.Add(
                    new Equipment(ItemManager.Instance.GetEquipFromId(val.KindsID), val.SelfID, val.Attr, val.IsUse));
            }

            foreach (var val in data.MatItems)
            {
                _bagData.m_Mat.Add(new Item_Mat(ItemManager.Instance.GetItemFromId(val.KindsID), val.Count));
            }

            foreach (var val in data.TaskItems)
            {
                _bagData.m_Task.Add(new Item_Task(ItemManager.Instance.GetItemFromId(val.KindsID), val.Count));
            }

            _bagData.Money = data.Mana;
            SortBagList();
        }

        public int GetBagCount(ItemType bagType)
        {
            if (bagType == ItemType.Equip)
                return _bagData.m_Equipment.Count;
            else if (bagType == ItemType.Mat)
                return _bagData.m_Mat.Count;
            else if (bagType == ItemType.Task)
                return _bagData.m_Task.Count;
            else
                return 0;
        }

        /// <summary>
        /// ????????????????????????????????????
        /// </summary>
        /// <returns></returns>
        public T GetBagItem<T>(int index, ItemType bagType) where T : ItemBase
        {
            if (bagType == ItemType.Equip)
                return _bagData.m_Equipment[index] as T;
            else if (bagType == ItemType.Mat)
                return _bagData.m_Mat[index] as T;
            else if (bagType == ItemType.Task)
                return _bagData.m_Task[index] as T;
            else
                return null;
        }

        public T GetItem<T>(int kindsId, ItemType itemType) where T : ItemBase
        {
            switch (itemType)
            {
                case ItemType.Equip:
                    return _bagData.m_Equipment.Find(c => c.ID == kindsId) as T;
                case ItemType.Mat:
                    return _bagData.m_Mat.Find(c => c.ID == kindsId) as T;
                case ItemType.Task:
                    return _bagData.m_Task.Find(c => c.ID == kindsId) as T;
                default:
                    return null;
            }
        }

        /// <summary>
        /// ??????ID???????????????????????????
        /// ???????????????,???????????????????????????????????????1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetItemCount(int id)
        {
            foreach (var val in _bagData.m_Equipment)
            {
                if (val.ID == id)
                    return 1;
            }

            foreach (var val in _bagData.m_Mat)
            {
                if (val.ID == id)
                    return val.CurCount;
            }

            foreach (var val in _bagData.m_Task)
            {
                if (val.ID == id)
                    return 1;
            }

            return 0;
        }

        public bool IsContainItem(int kindsId)
        {
            if (_bagData.m_Equipment.Find(c => c.ID == kindsId) != null)
                return true;
            if (_bagData.m_Mat.Find(c => c.ID == kindsId) != null)
                return true;
            if (_bagData.m_Task.Find(c => c.ID == kindsId) != null)
                return true;
            return false;
        }


        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <returns></returns>
        public List<Equipment> GetUsingEquipments()
        {
            List<Equipment> list = new List<Equipment>();
            int flag = 0;
            foreach (var val in _bagData.m_Equipment)
            {
                if (val.m_IsUse)
                {
                    if ((flag & (1 << (int) val.m_EquipType)) == 0)
                    {
                        list.Add(val);
                        flag += (1 << (int) val.m_EquipType);
                    }
                    else
                    {
                        val.SetUsingStatus(false);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <returns></returns>
        public void SetUsingEquipments(bool needSave = false)
        {
            PlayerManager.Instance.PlayerData.SetCurEquipmentData(GetUsingEquipments());
            if (needSave)
            {
                UpdateDataBase();
            }
        }

        public void SetUsingEquipments(Equipment eq)
        {
            PlayerManager.Instance.PlayerData.SetCurEquipmentData(eq);
            UpdateDataBase();
        }

        /// <summary>
        /// ????????????ID?????????ID??????
        /// </summary>
        public void SortBagList()
        {
            SortBagList(ItemType.Equip);
            SortBagList(ItemType.Mat);
            SortBagList(ItemType.Task);
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="bagType"></param>
        public void SortBagList(ItemType bagType)
        {
            if (bagType == ItemType.Equip)
            {
                _bagData.m_Equipment.Sort((a, b) =>
                {
                    if (a.ID != b.ID)
                        return a.ID.CompareTo(b.ID);
                    else
                        return a.m_SelfID.CompareTo(b.m_SelfID);
                });
            }
            else if (bagType == ItemType.Mat)
            {
                _bagData.m_Mat.Sort((a, b) => { return a.ID.CompareTo(b.ID); });
            }
            else if (bagType == ItemType.Task)
            {
                _bagData.m_Task.Sort((a, b) => { return a.ID.CompareTo(b.ID); });
            }
        }

        public void DeleteItem(int index, ItemType bagType)
        {
            int id = 0;
            int amount = 0;
            if (bagType == ItemType.Equip)
            {
                id = _bagData.m_Equipment[index].ID;
                amount = 1;
                _bagData.m_Equipment.RemoveAt(index);
            }
            else if (bagType == ItemType.Mat)
            {
                id = _bagData.m_Mat[index].ID;
                amount = _bagData.m_Mat[index].CurCount;
                _bagData.m_Mat.RemoveAt(index);
            }
            else if (bagType == ItemType.Task)
            {
                id = _bagData.m_Task[index].ID;
                amount = 1;
                _bagData.m_Task.RemoveAt(index);
            }

            if (OnItemChangeEvent != null) OnItemChangeEvent(id, -amount);
            UpdateDataBase();
        }

        /// <summary>
        /// ????????????????????????,????????????????????????????????????
        /// </summary>
        public void UpdateDataBase()
        {
            StorageManager.SaveBagData(_bagData);
        }

        /// <summary>
        /// ???????????????????????????????????????????????????????????????????????????
        /// </summary>
        /// <param name="kindsID"></param>
        public void GainItem(int kindsID, int count = 1)
        {
            if (count == 0)
                return;

            ItemBase item = ItemManager.Instance.GetItemFromId(kindsID);
            if (item.ItemType == ItemType.Equip)
            {
                int selfID = 1;
                //????????????ID
                //???????????????????????????????????????????????????????????????????????????
                for (int i = 0; i < _bagData.m_Equipment.Count; ++i)
                {
                    if (_bagData.m_Equipment[i].ID < kindsID)
                        continue;
                    if (_bagData.m_Equipment[i].ID == kindsID)
                    {
                        if (selfID == _bagData.m_Equipment[i].m_SelfID)
                        {
                            selfID++;
                            continue;
                        }
                    }

                    break;
                }

                Equipment t = new Equipment(ItemManager.Instance.GetEquipFromId(kindsID), selfID);
                _bagData.m_Equipment.Add(t);
                UITipManager.Instance.PushMessage("?????????" + item.Name);
            }
            else if (item.ItemType == ItemType.Mat)
            {
                Item_Mat bagItem = GetItem<Item_Mat>(kindsID, item.ItemType);
                if (bagItem != null)
                {
                    if (bagItem.GainCount(count))
                    {
                        UITipManager.Instance.PushMessage(item.Name + "??????????????????");
                    }
                }
                else
                {
                    _bagData.m_Mat.Add(new Item_Mat(item, count));
                }

                UITipManager.Instance.PushMessage("?????????" + item.Name + " x" + count);
            }
            else if (item.ItemType == ItemType.Task)
            {
                Item_Task bagItem = GetItem<Item_Task>(kindsID, item.ItemType);
                if (bagItem != null)
                {
                    if (bagItem.GainCount(count))
                    {
                        UITipManager.Instance.PushMessage(item.Name + "??????????????????");
                    }
                }
                else
                {
                    _bagData.m_Task.Add(new Item_Task(item, count));
                }

                UITipManager.Instance.PushMessage("?????????" + item.Name + " x" + count);
            }

            if (OnItemChangeEvent != null) OnItemChangeEvent(kindsID, count);

            SortBagList(item.ItemType);
            UpdateDataBase();
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="data"></param>
        public void GainItem(List<RewardItemData> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                GainItem(data[i].Id, data[i].Amount);
            }
        }

        public int GetMoney()
        {
            return _bagData.Money;
        }

        public void GainOrLossMoney(int amount, bool isGain = true)
        {
            _bagData.Money += (isGain ? 1 : -1) * amount;
        }
    }
}