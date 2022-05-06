using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Metafalica.RPG
{
    //上连StateC.cs
    public class StatePanelC : UIBaseWindow
    {
        public Transform leftArea;
        public Transform rightArea;
        private GameObject showItemInfo;

        private List<Text> playerStateInfo; // Level,EXP,HP,ATK,DEF,RGS,STR,AGI,WIS,propertyPoints
        private List<GameObject> equipmentCell;
        private List<Button> propertyChangeBtn;

        private int curPropertyPointsDefault; //当前可分配总点数
        private int curPropertyPoints; //当前未分配点数
        private int[] recordData; //记录临时加点
        private PlayerData _playerData;

        void Start()
        {
            _playerData = PlayerManager.Instance.PlayerData;
            showItemInfo = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_ShowItemInfo, transform);
            showItemInfo.SetActive(false);
            
            InitList();
            InitEquipmentCell();
            InitPropertyBtn();
            UpdatePanelInfo();
        }

        void InitList()
        {
            playerStateInfo = new List<Text>();
            Transform tf = rightArea.GetChild(0);
            for (int i = 0; i < tf.childCount; ++i)
            {
                playerStateInfo.Add(tf.GetChild(i).GetChild(1).GetComponent<Text>());
            }

            propertyChangeBtn = new List<Button>();
            tf = rightArea.GetChild(1);
            propertyChangeBtn.Add(tf.GetChild(0).GetChild(0).GetComponent<Button>()); //STR -
            propertyChangeBtn.Add(tf.GetChild(0).GetChild(1).GetComponent<Button>()); //STR +
            propertyChangeBtn.Add(tf.GetChild(1).GetChild(0).GetComponent<Button>()); //AGI -
            propertyChangeBtn.Add(tf.GetChild(1).GetChild(1).GetComponent<Button>()); //AGI +
            propertyChangeBtn.Add(tf.GetChild(2).GetChild(0).GetComponent<Button>()); //WIS -
            propertyChangeBtn.Add(tf.GetChild(2).GetChild(1).GetComponent<Button>()); //WIS +
            propertyChangeBtn.Add(tf.GetChild(3).GetChild(0).GetComponent<Button>()); //confirm
        }

        void InitEquipmentCell()
        {
            for (int i = 1; i < leftArea.childCount; ++i)
            {
                GameObject go = leftArea.GetChild(i).gameObject;
                int index = i;

                Equipment wp = _playerData.GetCurEquipmentData(index);
                if(wp == null)
                    continue;
                go.GetComponent<Image>().sprite = wp.Icon;
                EventTriggerListener.Get(go, index).onEnter = EnterEquipmentCell;
                EventTriggerListener.Get(go, index).onExit = ExitEquipmentCell;
            }
        }

        void EnterEquipmentCell(GameObject go)
        {
            showItemInfo.SetActive(true);
            UpdateEquipmentCellPanel(go.GetComponent<EventTriggerListener>().index);
        }

        void ExitEquipmentCell(GameObject go)
        {
            showItemInfo.SetActive(false);
        }

        void UpdateEquipmentCellPanel(int index)
        {
            Equipment wp = _playerData.GetCurEquipmentData(index);
            if (wp == null) showItemInfo.SetActive(false);
            showItemInfo.GetOrAddComponent<ShowItemInfo>().UpdateInfo(wp);
        }

        void InitPropertyBtn()
        {
            //str-+,agi-+,wis-+;
            for (int i = 0; i < 6; i++)
            {
                int temp = i / 2;
                bool flag = i % 2 != 0;
                propertyChangeBtn[i].onClick.AddListener(
                    () => { ChangePartValue(playerStateInfo[temp + 6], flag, temp); });
            }

            propertyChangeBtn[6].onClick.AddListener(
                () =>
                {
                    _playerData.ChangeAddAttr(curPropertyPoints, recordData);
                    UpdatePanelInfo();
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isPlus"></param>
        /// <param name="index">0:STR,1:AGI,2:WIS</param>
        void ChangePartValue(Text t, bool isPlus, int index)
        {
            if (curPropertyPoints == 0 && isPlus)
                return;
            if (curPropertyPoints == curPropertyPointsDefault && !isPlus)
                return;
            int d = isPlus ? 1 : -1;
            int val = int.Parse(t.text);
            recordData[index] += d;
            val += d;
            curPropertyPoints -= d;
            t.text = val + "";
            playerStateInfo[9].text = curPropertyPoints + "";
        }

        void UpdatePanelInfo()
        {
            playerStateInfo[0].text = "" + _playerData.Level;
            playerStateInfo[1].text = _playerData.EXP + "/" + _playerData.GetCurLevelUpgradeExp();
            playerStateInfo[2].text = "" + _playerData.MaxHP;
            playerStateInfo[3].text = _playerData.ATK + "|" + _playerData.MATK;
            playerStateInfo[4].text = _playerData.DEF + "";
            playerStateInfo[5].text = _playerData.MDEF + "";

            recordData = new int[3];
            playerStateInfo[6].text = _playerData.STR + "";
            playerStateInfo[7].text = _playerData.AGI + "";
            playerStateInfo[8].text = _playerData.WIS + "";

            curPropertyPointsDefault = _playerData.StatusPoint;
            playerStateInfo[9].text = curPropertyPointsDefault + "";
            rightArea.GetChild(1).gameObject.SetActive(curPropertyPointsDefault != 0);
            curPropertyPoints = curPropertyPointsDefault;
        }
    }
}