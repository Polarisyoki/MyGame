using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Metafalica.RPG
{
    public class NPC_Cocoa : NPCBase
    {
        public override void Init(NPCData data)
        {
            base.Init(data);
            
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PushInteractiveOption(Id, Name, ()=>
                {
                    HideOption();
                    Dialogue();
                });
            }

        }

        protected override void Dialogue()
        {
            base.Dialogue();
            DialogueC.Instance.PushSentence(m_NpcData.entryDialog,ShowOption);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PopInteractiveOption(Id);
            }
        }

        private void Task()
        {
            var quests = QuestManager.Instance.GetNpcAllQuest(this.Id);
            if (quests == null || quests.Count == 0)
            {
                return;
            }

            for (int i = 0; i < quests.Count; i++)
            {
                int j = i;
                UIManager.Instance.PushInteractiveOption(quests[i].m_Id, quests[i].m_Name, () =>
                {
                    HideOption();
                    TaskDialog(quests[j]);
                });
                
            }
        }

        private void TaskDialog(QuestData questData)
        {
            if (QuestManager.Instance.HasQuest(questData))
            {
                if (questData.IsComplete())
                {
                    DialogueC.Instance.PushSentence(QuestManager.Instance.GetRandomQuestDialog(questData.m_Id, QuestDialogData.QuestPhase.CompleteQuest),ShowOption);
                    QuestManager.Instance.CompleteQuest(questData);
                }
                else
                {
                    DialogueC.Instance.PushSentence(QuestManager.Instance.GetRandomQuestDialog(questData.m_Id, QuestDialogData.QuestPhase.OngoingQuest),ShowOption);
                }
            }
            else
            {
                if (QuestManager.Instance.AcceptQuest(questData))
                {
                    DialogueC.Instance.PushSentence(QuestManager.Instance.GetRandomQuestDialog(questData.m_Id, QuestDialogData.QuestPhase.AcceptQuest));
                }
                else
                {
                    UITipManager.Instance.PushMessage("可接取任务已达上限");
                    ShowOption();
                }
            }
        }
        
        private void Leave()
        {
            UIManager.Instance.PushInteractiveOption(-1, "再见", () =>
            {
                HideOption();
                DialogueC.Instance.PushSentence(m_NpcData.exitDialog);
            });
        }

        private void ShowOption()
        {
            Task();
            Leave();
        }

        private void HideOption()
        {
            UIManager.Instance.PopAllInteractiveOption();
        }
        
    }
}
