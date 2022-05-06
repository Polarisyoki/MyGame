using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class DungeonEntranceBase : MonoBehaviour
    {
        [Header("Dungeon ID:9110000~9199999")]
        public int DungeonID;
        public string DungeonName;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PushInteractiveOption(DungeonID,DungeonName,IntoDungeon);
            }
        
        }

        protected virtual void IntoDungeon()
        {
            UIManager.Instance.PopInteractiveOption(DungeonID);
            MySceneManager.Instance.LoadScene(ParameterC.DungeonSceneName,DungeonID.ToString());
        }
    
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PopInteractiveOption(DungeonID);
            }
        
        }
    }

}
