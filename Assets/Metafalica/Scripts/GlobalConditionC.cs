using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Metafalica.RPG
{
    public class GlobalConditionC
    {
        public static bool freezeUI = false;
        public static bool freezeCam = false;//锁定相机
        public static bool freezeCamZoom = false; //锁定滚轮拉近拉远
        
        public static int freezePlayerCount = 0;// >0 锁定玩家，不让移动
        public static bool IsFreezePlay => freezePlayerCount > 0;
    }
}

