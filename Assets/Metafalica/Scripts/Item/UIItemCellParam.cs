using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class UIItemCellParam
    {
        public Sprite icon;
        public ItemType itemType;
        public bool isUse = false;
        public int amount = 0;
        public Action clickAction;

    }
}

