using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Metafalica.RPG
{
    public enum ItemType
    {
        Equip = 1,
        Mat = 3,
        Task = 5,
        Other = 999,
    }
    
    public class ItemBase
    {
        public int ID { get; private set; }
        public string Name  { get; private set; }
        public ItemType ItemType { get; private set; }

        public int CurCount { get; private set; }
        
        public int MaxStack  { get; private set; }
        
        public Sprite Icon  { get; private set; }
        public string Description  { get; private set; }

        public ItemBase(ItemBase item, int count = 0)
        {
            this.ID = item.ID;
            this.Name = item.Name;
            this.Icon = item.Icon;
            this.ItemType = item.ItemType;
            this.MaxStack = item.MaxStack;
            this.Description = item.Description;
            this.CurCount = count;
        }

        public ItemBase(ItemCfg.ItemData itemCfg)
        {
            this.ID = itemCfg.ID;
            this.Name = itemCfg.Name;
            this.ItemType = (ItemType) itemCfg.ItemType;
            this.MaxStack = itemCfg.MaxStack;
            this.Description = itemCfg.Description;
            this.CurCount = 0;
            this.Icon = ResourcesManager.Instance.GetSprite(itemCfg.IconPath);
        }

        /// <summary>
        /// 是否爆仓
        /// </summary>
        public bool GainCount(int count)
        {
            this.CurCount += count;
            if (this.CurCount > MaxStack)
            {
                this.CurCount = MaxStack;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 返回是否需要销毁物品
        /// </summary>
        public bool LostCount(int count)
        {
            this.CurCount -= count;
            if (this.CurCount <= 0)
            {
                this.CurCount = 0;
                return true;
            }

            return false;
        }
    }

    public class Item_Mat : ItemBase
    {
        public Item_Mat(ItemBase data, int count = 0) : base(data, count)
        {
        }

        public Item_Mat(ItemCfg.ItemData itemCfg) : base(itemCfg)
        {
        }
    }

    public class Item_Task : ItemBase
    {
        public Item_Task(ItemBase data, int count = 0) : base(data, count)
        {
        }

        public Item_Task(ItemCfg.ItemData itemCfg) : base(itemCfg)
        {
        }
    }
}