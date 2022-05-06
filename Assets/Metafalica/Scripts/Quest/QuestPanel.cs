using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class QuestPanel : UIBaseWindow
    {
        public ScrollRect _srLeftQuestList;
        public Text _txtQuestIntro;
        public Text _txtQuestProgress;
        public ScrollRect _srRewardList;
        public Transform _tExpCell;
        public Text _txtExpCell_Amount;
        public Transform _tManaCell;
        public Text _txtManaCell_Amount;
        public Button _btnClose;

        private void GetFeild()
        {
            _srLeftQuestList = this.transform.Find("LeftArea/QuestNameList").GetComponent<ScrollRect>();
            _txtQuestIntro = this.transform.Find("RightArea/T_QuestIntro/Txt_QuestIntro").GetComponent<Text>();
            _txtQuestProgress = this.transform.Find("RightArea/T_QuestProgress/Txt_QuestProgress").GetComponent<Text>();
            _srRewardList = this.transform.Find("RightArea/T_Reward/RewardList").GetComponent<ScrollRect>();
            _tExpCell = _srRewardList.content.Find("ExpCell");
            _txtExpCell_Amount = _tExpCell.Find("Count").GetComponent<Text>();
            _tManaCell = _srRewardList.content.Find("ManaCell");
            _txtManaCell_Amount = _tManaCell.Find("Count").GetComponent<Text>();
            _btnClose = this.transform.Find("Btn_Close").GetComponent<Button>();
        }
        
        private void SetBtn()
        {
            _btnClose.onClick.AddListener(CloseWindow);
        }

        private void Awake()
        {
            GetFeild();
            SetBtn();
        }

        private void OnEnable()
        {
            InitQuestList();
        }

        private void InitQuestList()
        {
            var quests = QuestManager.Instance.m_OngoingQuest;
            if (quests == null || quests.Count == 0)
            {
                _txtQuestProgress.text = string.Empty;
                _txtQuestIntro.text = string.Empty;
                _srRewardList.gameObject.SetActive(false);
            }
            else
            {
                ReflashPanel(quests[0]);
                for (int i = 0; i < quests.Count; i++)
                {
                    AddOneQuest(quests[i],i);
                }
            }
        }

        private void AddOneQuest(QuestData data,int index = 0)
        {
            // var go = Instantiate(ManagerPrefabsVars.GetVars().QuestNameCell, _srLeftQuestList.content);
            var go = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_QuestNameCell,
                _srLeftQuestList.content);
            go.transform.GetComponentInChildren<Text>().text = data.m_Name;
            go.transform.Find("Mask").gameObject.SetActive(index == 0);
            go.transform.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                ReflashPanel(data);
                go.transform.Find("Mask").gameObject.SetActive(true);
            });
            
        }

        private void ReflashPanel(QuestData data)
        {
            for (int i = 0; i < _srLeftQuestList.content.childCount; i++)
            {
                _srLeftQuestList.content.GetChild(i).Find("Mask").gameObject.SetActive(false);
            }
            _txtQuestIntro.text = data.m_Description;
            _txtQuestProgress.text = data.GetProgress();
            _tExpCell.gameObject.SetActive(data.m_RewardExp > 0);
            _txtExpCell_Amount.text = data.m_RewardExp.ToString();
            _tManaCell.gameObject.SetActive(data.m_RewardCash > 0);
            _txtManaCell_Amount.text = data.m_RewardCash.ToString();
            
            if (data.m_RewardItem == null || data.m_RewardItem.Count == 0)
                return;
            for (int i = 0; i < data.m_RewardItem.Count; i++)
            {
                // var item = Instantiate(ManagerPrefabsVars.GetVars().ItemCell, _srRewardList.content);
                var item = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_ItemCell,
                    _srRewardList.content);
                var temp = ItemManager.Instance.GetItemFromId(data.m_RewardItem[i].Id);
                int t = data.m_RewardItem[i].Amount;
                item.GetComponent<UIItemCellView>().Init(new UIItemCellParam()
                {
                    icon = temp.Icon,
                    itemType = temp.ItemType,
                    amount = t,
                });
                
            }
                
            

        }
        
    }
}

