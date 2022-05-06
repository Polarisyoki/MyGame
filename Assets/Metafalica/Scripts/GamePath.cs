using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class GamePath
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string Main_Path = Application.dataPath;
#else
    // public static string MainPath = ;
#endif

        public static string MyFolder = Main_Path + "Metafalica/";

        
        public const string Res_Folder = "Assets/Metafalica/res/";
    
        //----------------------------------------------------------------
        #region 资源_预制体
        
        public const string Res_Prefab_Folder = Res_Folder + "prefabs/";
        
        //------------UI
        public const string Res_Prefab_UI_Folder = Res_Prefab_Folder + "ui/";
        
        //UI根节点
        public const string Res_Prefab_File_UIRoot = Res_Prefab_UI_Folder + "UIRoot.prefab";
        //对话框
        public const string Res_Prefab_File_DialogueView = Res_Prefab_UI_Folder + "DialogueView.prefab";
        //背包
        public const string Res_Prefab_File_InfantryPack = Res_Prefab_UI_Folder + "InfantryPack.prefab";
        //选择项
        public const string Res_Prefab_File_InteractiveOptionCell = Res_Prefab_UI_Folder + "InteractiveOptionCell.prefab";
        //物品格子
        public const string Res_Prefab_File_ItemCell = Res_Prefab_UI_Folder + "ItemCell.prefab";
        //信息提示区
        public const string Res_Prefab_File_MessageArea = Res_Prefab_UI_Folder + "MessageArea.prefab";
        //玩家状态面板
        public const string Res_Prefab_File_PlayerStatePanel = Res_Prefab_UI_Folder + "PlayerStatePanel.prefab";
        //道具展示卡
        public const string Res_Prefab_File_ShowItemInfo = Res_Prefab_UI_Folder + "ShowItemInfo.prefab";
        //任务名称项
        public const string Res_Prefab_File_QuestNameCell = Res_Prefab_UI_Folder + "QuestNameCell.prefab";
        //任务面板
        public const string Res_Prefab_File_QuestPanel = Res_Prefab_UI_Folder + "QuestPanel.prefab";
        
        //--------任务(非UI)-----------
        public const string Res_Prefab_Quest_Folder = Res_Prefab_Folder + "quest/";
        public static string GetQuestPointPath(string path)
        {
            return Res_Prefab_Quest_Folder + path + ".prefab";
        }
        
        //-------NPC
        public const string Res_Prefab_NPC_Folder = Res_Prefab_Folder + "npc/";
        public static string GetNpcPrefabPath(string path)
        {
            return Res_Prefab_NPC_Folder + path + ".prefab";
        }
        #endregion

        

        #region 资源_图片
        public const string Res_Sprite_Folder = Res_Folder;
        #endregion

        
        #region 资源_配置文件(json)
        public const string Res_Config_Folder = Res_Folder + "config/";
        #endregion
    }
}

