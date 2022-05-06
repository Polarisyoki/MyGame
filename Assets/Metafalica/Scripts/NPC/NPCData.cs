using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class NPCData
    {
        public int id;
        public string name;
        public Vector3 pos;
        public Vector3 rot;
        public DialogueData[] entryDialog;
        public DialogueData[] exitDialog;
        public string prefabPath;

        public event DialogueQuestListener OnDialogueQuestEvent;

        public NPCData(NpcCfgData data)
        {
            this.id = data.ID;
            this.name = data.Name;
            this.pos = Vector3.zero;
            this.rot = Vector3.zero;
            this.prefabPath = data.PrefabPath;
            
            if (data.EntryDialog == null)
            {
                this.entryDialog = new DialogueData[0];
            }
            else
            {
                this.entryDialog = new DialogueData[data.EntryDialog.Length];
                for (int i = 0; i < data.EntryDialog.Length; i++)
                {
                    entryDialog[i] = new DialogueData(name, data.EntryDialog[i]);
                }
            }
            
            if (data.ExitDialog == null)
            {
                this.exitDialog = new DialogueData[0];
            }
            else
            {
                this.exitDialog = new DialogueData[data.ExitDialog.Length];
                for (int i = 0; i < data.ExitDialog.Length; i++)
                {
                    exitDialog[i] = new DialogueData(name, data.ExitDialog[i]);
                }
            }

            float x, y, z;
            x = y = z = 0;
            if (data.Position.Length >= 3)
            {
                float.TryParse(data.Position[0],out x);
                float.TryParse(data.Position[1],out y);
                float.TryParse(data.Position[2],out z);
                pos.Set(x,y,z);
            }

            x = y = z = 0;
            if (data.Rotation.Length >= 3)
            {
                float.TryParse(data.Rotation[0],out x);
                float.TryParse(data.Rotation[1],out y);
                float.TryParse(data.Rotation[2],out z);
                rot.Set(x,y,z);
            }
            
        }

        public void ExecEvent()
        {
            if (OnDialogueQuestEvent != null)
                OnDialogueQuestEvent(id);
        }
    }

}
