using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class QuestDialogData
    {
        public enum QuestPhase
        {
            None = 0,
            HasQuest = 1,
            AcceptQuest = 2,
            RejectQuest = 3,
            OngoingQuest = 4,
            CompleteQuest = 5,
        }

        public int _questId;
        public QuestPhase _questPhase;
        private List<DialogueData> _dialogueData;
        public List<DialogueData> m_DialogueData => _dialogueData;

        public QuestDialogData(QuestCfg.QuestDialogCfgData data)
        {
            this._questId = data.ID;
            this._questPhase = (QuestPhase) data.QuestPhase;
            
            _dialogueData = new List<DialogueData>();
            if (data.Dialog == null || data.Dialog.Length == 0)
                return;
            string name = string.Empty;
            for (int i = 0; i < data.Dialog.Length; i++)
            {
                _dialogueData.Add(new DialogueData(NPCManager.Instance.AllNpc[int.Parse(data.Dialog[i][0])].name,
                    data.Dialog[i][1]));
            }
        }
    }
}