using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class DefaultItemData
    {
        //所有物品的基本信息，详细请去下面几个中查找
        public List<ItemBase> AllItems = new List<ItemBase>();
        public List<Equipment> Equips = new List<Equipment>();
    }
    
    public class ItemManager : Singleton<ItemManager>
    {

        private DefaultItemData _defaultItemData;

        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            
            CfgManager.GetItemInfo(ref _defaultItemData);
        }


        /// <summary>通过物品ID获取物品的基本信息</summary>
        public ItemBase GetItemFromId(int id)
        {
            return _defaultItemData.AllItems.Find(c => c.ID == id);
        }
        
        /// <summary>通过武器ID获取武器的信息</summary>
        public Equipment GetEquipFromId(int id)
        {
            return _defaultItemData.Equips.Find(c => c.ID == id);
        }
    }

}
