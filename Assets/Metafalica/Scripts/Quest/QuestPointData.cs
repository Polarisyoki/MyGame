using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    [Serializable]
    public class QuestPointData
    {
        public int id;
        public string name;
        public Vector3 pos;
        public string prefabPath;
        
        public event MoveToPointListener OnMoveIntoEvent;
        
        public QuestPointData(QuestCfg.QuestPointCfgData data)
        {
            id = data.ID;
            name = data.Name;

            pos = Vector3.zero;
            if (data.Position.Length >= 3)
            {
                float x = 0,y=0,z=0;
                float.TryParse(data.Position[0], out x);
                float.TryParse(data.Position[1], out y);
                float.TryParse(data.Position[2], out z);

                pos.Set(x,y,z);
            }

            prefabPath = data.PrefabPath;
        }

        public void ExecEvent()
        {
            if (OnMoveIntoEvent != null)
            {
                OnMoveIntoEvent(id);
            }
        }
    }
}

