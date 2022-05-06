using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Metafalica.RPG
{
    public class QuestData
    {
        public QuestData(QuestCfg.QuestCfgData data)
        {
            this.m_Id = data.ID;
            this.m_Name = data.Name;
            this.m_Description = data.Description;
            this.m_RewardCash = data.RewardCash;
            this.m_RewardExp = data.RewardExp;
            this.m_RewardItem = new List<RewardItemData>();
            if (data.RewardItem != null)
            {
                for (int i = 0; i < data.RewardItem.Length; i++)
                {
                    this.m_RewardItem.Add(new RewardItemData(data.RewardItem[i]));
                }
            }

            this.m_Repeatable = data.Repeatable == 1;
            this.m_TaskTaker = data.TaskTaker;
            this.m_QuestConditions = new List<QuestCondition>();
            if (data.QuestConditions != null)
            {
                for (int i = 0; i < data.QuestConditions.Length; i++)
                {
                    this.m_QuestConditions.Add(new QuestCondition(data.QuestConditions[i]));
                }
            }

            this.m_QuestType = (QuestType) data.QuestType;
            m_DialogueQuests = new List<DialogueQuest>();
            m_ReachPointQuests = new List<ReachPointQuest>();
            m_BattleQuests = new List<BattleQuest>();
            m_CollectQuests = new List<CollectQuest>();
            for (int i = 0; i < data.Quests.Length; i++)
            {
                switch (m_QuestType)
                {
                    case QuestType.Dialogue:
                        m_DialogueQuests.Add(new DialogueQuest(data.Quests[i]));
                        break;
                    case QuestType.ReachPoint:
                        m_ReachPointQuests.Add(new ReachPointQuest(data.Quests[i]));
                        break;
                    case QuestType.Battle:
                        m_BattleQuests.Add(new BattleQuest(data.Quests[i]));
                        break;
                    case QuestType.Collect:
                        m_CollectQuests.Add(new CollectQuest(data.Quests[i]));
                        break;
                }
            }

            this.m_SubmissionNpcID = data.SubmissionNpcID;
        }

        public int m_Id = 0;
        public string m_Name = "";
        public string m_Description = "";

        public int m_RewardCash = 100;
        public int m_RewardExp = 100;
        public List<RewardItemData> m_RewardItem;

        public bool m_Repeatable = false;
        public int m_CompleteTimes = 0;

        /// <summary>任务接取者</summary>
        public int m_TaskTaker;

        public List<QuestCondition> m_QuestConditions;

        public QuestType m_QuestType;
        public List<DialogueQuest> m_DialogueQuests;
        public List<ReachPointQuest> m_ReachPointQuests;
        public List<BattleQuest> m_BattleQuests;
        public List<CollectQuest> m_CollectQuests;

        /// <summary>
        /// 0代表可以达成条件即可点击按钮提交
        /// </summary>
        public int m_SubmissionNpcID;

        public bool IsComplete()
        {
            if (m_QuestType == QuestType.Dialogue)
            {
                foreach (var val in m_DialogueQuests)
                {
                    if (!val.IsComplete) return false;
                }

                return true;
            }
            else if (m_QuestType == QuestType.ReachPoint)
            {
                foreach (var val in m_ReachPointQuests)
                {
                    if (!val.IsComplete) return false;
                }

                return true;
            }
            else if (m_QuestType == QuestType.Battle)
            {
                foreach (var val in m_BattleQuests)
                {
                    if (!val.IsComplete) return false;
                }

                return true;
            }
            else if (m_QuestType == QuestType.Collect)
            {
                foreach (var val in m_CollectQuests)
                {
                    if (!val.IsComplete) return false;
                }

                return true;
            }

            return false;
        }

        public bool IsEligible
        {
            get
            {
                for (int i = 0; i < m_QuestConditions.Count; i++)
                {
                    if (!m_QuestConditions[i].IsEligible)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 获取任务进度
        /// </summary>
        /// <returns></returns>
        public string GetProgress()
        {
            string str = string.Empty;
            string color = "ff0000";
            string name = "";
            switch (m_QuestType)
            {
                case QuestType.Dialogue:
                    for (int i = 0; i < m_DialogueQuests.Count; i++)
                    {
                        if (m_DialogueQuests[i].IsComplete)
                            color = "00ff00";
                        else color = "ff0000";
                        name = NPCManager.Instance.AllNpc[m_DialogueQuests[i].TargetId].name;
                        str += String.Format("<color=#{0}>和{1}对话</color>\n",color,name);
                    }
                    name = NPCManager.Instance.AllNpc[m_SubmissionNpcID].name;
                    str += String.Format("<color=#ff0000>回复{0}</color>",name);
                    break;
                case QuestType.ReachPoint:
                    for (int i = 0; i < m_ReachPointQuests.Count; i++)
                    {
                        if (m_ReachPointQuests[i].IsComplete)
                            color = "00ff00";
                        else color = "ff0000";
                        name = QuestManager.Instance.m_AllQuestPoint[m_ReachPointQuests[i].TargetId].Name;
                        str += String.Format("<color=#{0}>到{1}看看</color>\n",color,name);
                    }
                    name = NPCManager.Instance.AllNpc[m_SubmissionNpcID].name;
                    str += String.Format("<color=#ff0000>回复{0}</color>",name);
                    break;
                case QuestType.Battle:
                    for (int i = 0; i < m_BattleQuests.Count; i++)
                    {
                        if (m_BattleQuests[i].IsComplete)
                            color = "00ff00";
                        else color = "ff0000";
                        name = GameManager.Instance.AllEnemy[m_BattleQuests[i].TargetId][0].Name;
                        str += String.Format("<color=#{0}>{1}:{2}/{3}</color>\n",
                            color,name,m_BattleQuests[i].CurAmount,m_BattleQuests[i].TargetAmount);
                    }
                    name = NPCManager.Instance.AllNpc[m_SubmissionNpcID].name;
                    str += String.Format("<color=#ff0000>回复{0}</color>",name);
                    break;
                case QuestType.Collect:
                    for (int i = 0; i < m_CollectQuests.Count; i++)
                    {
                        if (m_CollectQuests[i].IsComplete)
                            color = "00ff00";
                        else color = "ff0000";
                        name = ItemManager.Instance.GetItemFromId(m_CollectQuests[i].TargetId).Name;
                        str += String.Format("<color=#{0}>{1}:{2}/{3}</color>\n",
                            color,name,m_CollectQuests[i].CurAmount,m_CollectQuests[i].TargetAmount);
                    }
                    name = NPCManager.Instance.AllNpc[m_SubmissionNpcID].name;
                    str += String.Format("<color=#ff0000>回复{0}</color>",name);
                    break;
            }

            return str;
        }

        public enum QuestType
        {
            Dialogue = 0,
            ReachPoint = 1,
            Battle = 2,
            Collect = 3,
        }
    }

    public abstract class MyQuestObj
    {
        private int _targetId;
        public int TargetId => _targetId;

        private int _targetAmount;
        public int TargetAmount => _targetAmount;

        private int _curAmount;

        public int CurAmount
        {
            get => _curAmount;
            set
            {
                if (value < _targetAmount && value >= 0)
                    _curAmount = value;
                else if (value < 0)
                    _curAmount = 0;
                else
                    _curAmount = _targetAmount;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (_curAmount >= _targetAmount)
                    return true;
                return false;
            }
        }

        public MyQuestObj(int[] data)
        {
            this._targetId = data[0];
            this._targetAmount = data[1];
        }

        protected virtual void UpdateStatus()
        {
            if (IsComplete) return;
            CurAmount++;
        }
    }

    public class DialogueQuest : MyQuestObj
    {
        public DialogueQuest(int[] data) : base(data)
        {
        }

        public void UpdateQuestStatus(int curNpcID)
        {
            if (TargetId == curNpcID)
            {
                UpdateStatus();
            }
        }
    }

    public class ReachPointQuest : MyQuestObj
    {
        public ReachPointQuest(int[] data) : base(data)
        {
        }

        public void UpdateQuestStatus(int curPointID)
        {
            if (TargetId == curPointID)
            {
                UpdateStatus();
            }
        }
    }

    public class BattleQuest : MyQuestObj
    {
        public BattleQuest(int[] data) : base(data)
        {
        }

        public void UpdateQuestStatus(int curEnemyID)
        {
            if (TargetId == curEnemyID)
            {
                UpdateStatus();
            }
        }
    }

    public class CollectQuest : MyQuestObj
    {
        public CollectQuest(int[] data) : base(data)
        {
        }

        public void UpdateQuestStatus(int curItemID, int gainOrLostAmount)
        {
            if (TargetId == curItemID)
            {
                if (gainOrLostAmount > 0)
                {
                    if (IsComplete) return;
                }

                CurAmount += gainOrLostAmount;
            }
        }
    }

    public class RewardItemData
    {
        public int Id;
        public int Amount;

        public RewardItemData(int[] data)
        {
            this.Id = data[0];
            this.Amount = data[1];
        }
    }

    public class QuestCondition
    {
        public AcceptCondition Condition = AcceptCondition.None;
        public int Level;
        public int QuestId;
        public int ItemId;

        public QuestCondition(int[] data)
        {
            Condition = (AcceptCondition) data[0];
            switch (Condition)
            {
                case AcceptCondition.LvGreaterThen:
                case AcceptCondition.lvLessThen:
                case AcceptCondition.LvGreaterOrEuqalsThen:
                case AcceptCondition.LvLessOrEqualsThen:
                    this.Level = data[1];
                    break;
                case AcceptCondition.ComplexQuest:
                case AcceptCondition.NoComplexQuest:
                    this.QuestId = data[1];
                    break;
                case AcceptCondition.HasItem:
                    this.ItemId = data[1];
                    break;
                default:
                    break;
            }
        }

        public bool IsEligible
        {
            get
            {
                switch (Condition)
                {
                    case AcceptCondition.LvGreaterThen:
                        if (PlayerManager.Instance.PlayerData.Level > Level)
                            return true;
                        break;
                    case AcceptCondition.lvLessThen:
                        if (PlayerManager.Instance.PlayerData.Level < Level)
                            return true;
                        break;
                    case AcceptCondition.LvGreaterOrEuqalsThen:
                        if (PlayerManager.Instance.PlayerData.Level >= Level)
                            return true;
                        break;
                    case AcceptCondition.LvLessOrEqualsThen:
                        if (PlayerManager.Instance.PlayerData.Level <= Level)
                            return true;
                        break;
                    case AcceptCondition.ComplexQuest:
                        if (QuestManager.Instance.m_AllQuests.Find(c =>
                                c.m_Id == this.QuestId && c.m_CompleteTimes > 0) !=
                            null)
                            return true;
                        break;
                    case AcceptCondition.NoComplexQuest:
                        if (QuestManager.Instance.m_AllQuests.Find(c =>
                                c.m_Id == this.QuestId && c.m_CompleteTimes == 0) !=
                            null)
                            return true;
                        break;
                    case AcceptCondition.HasItem:
                        if (PlayerManager.Instance.Inventory.IsContainItem(ItemId))
                            return true;
                        break;
                    default:
                        break;
                }

                return false;
            }
        }
    }

    public enum AcceptCondition
    {
        None = 1,
        LvGreaterThen = 2,
        lvLessThen = 3,
        LvGreaterOrEuqalsThen = 4,
        LvLessOrEqualsThen = 5,
        ComplexQuest = 6,
        NoComplexQuest = 7,
        HasItem = 8,
    }
}