using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public delegate void DialogueQuestListener(int npcID);

    public delegate void MoveToPointListener(int pointID);

    public delegate void BattleQuestListener(int enemyID);

    public delegate void ItemInfoListener(int id, int amount);

    public class DefaultQuestData
    {
        public List<QuestData> QuestDatas = new List<QuestData>();
        public List<QuestDialogData> QuestDialogDatas = new List<QuestDialogData>();
        public List<QuestPointData> QuestPointDatas = new List<QuestPointData>();
    }

    public class QuestManager : Singleton<QuestManager>
    {
        private DefaultQuestData _defaultQuestData;
        public List<QuestData> m_AllQuests => _defaultQuestData.QuestDatas;


        public Dictionary<int, QuestPoint> m_AllQuestPoint;


        private List<QuestData> _myquest;
        public List<QuestData> m_OngoingQuest => _myquest;


        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            _myquest = new List<QuestData>();
            m_AllQuestPoint = new Dictionary<int, QuestPoint>();

            CfgManager.GetQuestInfo(ref _defaultQuestData);
            LoadQuestsData();
        }

        /// <summary>
        /// 加载任务完成次数和已接取的任务
        /// </summary>
        private void LoadQuestsData()
        {
            var data = StorageManager.LoadQuestData();
            if (data == null)
                return;
            if (data.AllTimes != null)
            {
                for (int i = 0; i < data.AllTimes.Count; i++)
                {
                    var t = m_AllQuests.Find(c => c.m_Id == data.AllTimes[i].ID);
                    if (t == null)
                        continue;
                    t.m_CompleteTimes = data.AllTimes[i].CompleteTimes;
                }
            }

            if (data.AllInProgresses != null)
            {
                for (int i = 0; i < data.AllInProgresses.Count; i++)
                {
                    var t = m_AllQuests.Find(c => c.m_Id == data.AllInProgresses[i].ID);
                    if (t == null)
                        continue;
                    AcceptQuest(t, true);
                }
            }
        }

        /// <summary>
        /// 接受任务，如果返回false则表示任务列表已经到上限
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AcceptQuest(QuestData data, bool isLoad = false)
        {
            if (_myquest.Count >= 10) return false;
            if (HasQuest(data)) return false;

            if (data.m_QuestType == QuestData.QuestType.Dialogue)
            {
                foreach (var val in data.m_DialogueQuests)
                {
                    try
                    {
                        int id = val.TargetId;
                        NPCManager.Instance.AllNpc[id].OnDialogueQuestEvent += val.UpdateQuestStatus;
                    }
                    catch
                    {
                        Debug.LogErrorFormat("[NPC未加入列表] ID:{0}", val.TargetId);
                        continue;
                    }
                }
            }
            else if (data.m_QuestType == QuestData.QuestType.ReachPoint)
            {
                foreach (var val in data.m_ReachPointQuests)
                {
                    try
                    {
                        var questPointData = GetQuestPointDataFromId(val.TargetId);
                        questPointData.OnMoveIntoEvent += val.UpdateQuestStatus;

                        ObjectManager.Instance.InstantiateObject(GamePath.GetQuestPointPath(questPointData.prefabPath))
                            .GetComponent<QuestPoint>().Init(questPointData);
                    }
                    catch
                    {
                        Debug.LogErrorFormat("[任务点未加入列表] ID:{0}", val.TargetId);
                        continue;
                    }
                }
            }
            else if (data.m_QuestType == QuestData.QuestType.Battle)
            {
                foreach (var val in data.m_BattleQuests)
                {
                    try
                    {
                        foreach (var enemy in GameManager.Instance.AllEnemy[val.TargetId])
                        {
                            enemy.OnBattleQuestEvent += val.UpdateQuestStatus;
                        }
                    }
                    catch
                    {
                        Debug.LogErrorFormat("[敌人未加入列表] ID:{0}", val.TargetId);
                        continue;
                    }
                }
            }
            else if (data.m_QuestType == QuestData.QuestType.Collect)
            {
                foreach (var val in data.m_CollectQuests)
                {
                    PlayerManager.Instance.Inventory.OnItemChangeEvent += val.UpdateQuestStatus;
                    val.UpdateQuestStatus(val.TargetId, PlayerManager.Instance.Inventory.GetItemCount(val.TargetId));
                }
            }

            _myquest.Add(data);
            if (!isLoad)
            {
                StorageManager.SaveQuestData(m_AllQuests, _myquest);
            }

            return true;
        }

        public bool HasQuest(int id)
        {
            return _myquest.Find(c => c.m_Id == id) != null;
        }

        public bool HasQuest(QuestData data)
        {
            return _myquest.Contains(data);
        }

        public void CancelQuest(QuestData data)
        {
            if (data.m_QuestType == QuestData.QuestType.Dialogue)
            {
                foreach (var val in data.m_DialogueQuests)
                {
                    val.CurAmount = 0;
                    NPCManager.Instance.AllNpc[val.TargetId].OnDialogueQuestEvent -= val.UpdateQuestStatus;
                }
            }
            else if (data.m_QuestType == QuestData.QuestType.ReachPoint)
            {
                foreach (var val in data.m_ReachPointQuests)
                {
                    val.CurAmount = 0;
                    m_AllQuestPoint[val.TargetId].m_QuestPointData.OnMoveIntoEvent -= val.UpdateQuestStatus;

                    ObjectManager.Instance.ReleaseObject(m_AllQuestPoint[val.TargetId].gameObject, 0, true);
                    m_AllQuestPoint.Remove(val.TargetId);
                }
            }
            else if (data.m_QuestType == QuestData.QuestType.Battle)
            {
                foreach (var val in data.m_BattleQuests)
                {
                    val.CurAmount = 0;
                    foreach (var enemy in GameManager.Instance.AllEnemy[val.TargetId])
                    {
                        enemy.OnBattleQuestEvent -= val.UpdateQuestStatus;
                    }
                }
            }
            else if (data.m_QuestType == QuestData.QuestType.Collect)
            {
                foreach (var val in data.m_CollectQuests)
                {
                    val.CurAmount = 0;
                    PlayerManager.Instance.Inventory.OnItemChangeEvent -= val.UpdateQuestStatus;
                }
            }

            _myquest.Remove(data);
        }

        public void CancelQuest(int id)
        {
            QuestData data = _myquest.Find(c => c.m_Id == id);
            if (data == null)
                return;
            CancelQuest(data);
        }

        public void CompleteQuest(int id)
        {
            var data = _myquest.Find(c => c.m_Id == id);
            if (data == null)
                return;
            CompleteQuest(data);
        }

        public void CompleteQuest(QuestData data)
        {
            //获得奖励
            PlayerManager.Instance.Inventory.GainOrLossMoney(data.m_RewardCash);
            PlayerManager.Instance.PlayerData.EXPChange(data.m_RewardExp);
            PlayerManager.Instance.Inventory.GainItem(data.m_RewardItem);
            data.m_CompleteTimes++;

            CancelQuest(data);
            StorageManager.SaveQuestData(m_AllQuests, _myquest);
        }

        /// <summary>
        /// 获取NPC所有可接取任务（包括进行中的）
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public List<QuestData> GetNpcAllQuest(int npcId)
        {
            return m_AllQuests.FindAll(c =>
                c.m_TaskTaker == npcId && (c.m_Repeatable || c.m_CompleteTimes == 0) && c.IsEligible);
        }

        /// <summary>
        /// 获取NPC所有进行中的任务
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public List<QuestData> GetNpcOngoingQuest(int npcId)
        {
            return _myquest.FindAll(c => c.m_SubmissionNpcID == npcId);
        }

        /// <summary>
        /// 获取NPC第一个响应的任务 已完成>未完成>null
        /// </summary>
        public QuestData GetNpcCurResponseQuest(int npcId)
        {
            var qdList = _myquest.FindAll(c => c.m_SubmissionNpcID == npcId);
            if (qdList == null || qdList.Count == 0)
                return null;
            var qd = qdList.Find(c => c.IsComplete());
            if (qd == null)
                return qdList[0];
            return qd;
        }

        public void NewEnemyAddListener(EnemyBase enemy)
        {
            for (int i = 0; i < _myquest.Count; i++)
            {
                if (_myquest[i].m_QuestType != QuestData.QuestType.Battle)
                    continue;

                foreach (var value in _myquest[i].m_BattleQuests)
                {
                    if (enemy.ID != value.TargetId)
                        continue;

                    enemy.OnBattleQuestEvent += value.UpdateQuestStatus;
                }
            }
        }

        /// <summary>获取当前任务阶段所有可能的任务对话</summary>
        public List<QuestDialogData> GetQuestDialogs(int questId, QuestDialogData.QuestPhase phase)
        {
            return _defaultQuestData.QuestDialogDatas.FindAll(c => c._questId == questId && c._questPhase == phase);
        }

        /// <summary>获取当前任务阶段随机一条任务对话</summary>
        public List<DialogueData> GetRandomQuestDialog(int questId, QuestDialogData.QuestPhase phase)
        {
            var t = GetQuestDialogs(questId, phase);
            if (t == null || t.Count == 0)
                return null;
            return t[UnityEngine.Random.Range(0, t.Count)].m_DialogueData;
        }

        private QuestPointData GetQuestPointDataFromId(int id)
        {
            return _defaultQuestData.QuestPointDatas.Find(c => c.id == id);
        }
    }
}