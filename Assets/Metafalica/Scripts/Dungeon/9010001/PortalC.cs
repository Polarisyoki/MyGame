using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PortalC : MonoBehaviour
    {
        public WorldDirection worldDirection;
        public int id;
        public string description;
        private bool isOption;
        
        void Start()
        {
            isOption = false;

            HideOrShowSelf();
            DungeonMain9010001.Instance.HideOrShowPortal(HideOrShowSelf);
        }
        

        private void HideOrShowSelf()
        {
            Vector3 pos = this.transform.position;
            if (DungeonMain9010001.Instance.HidePortal(worldDirection))
            {
                this.transform.position = new Vector3(pos.x, -100, pos.z);
            }
            else
            {
                this.transform.position = new Vector3(pos.x, 0, pos.z);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isOption)
                return;
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PushInteractiveOption(id,description,ToNextPos);
                isOption = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                isOption = false;
                UIManager.Instance.PopInteractiveOption(id);
            }
        }

        private void ToNextPos()
        {
            DungeonMain9010001.Instance.MovePlayer(worldDirection);
            isOption = false;
            UIManager.Instance.PopInteractiveOption(id);
        }
        
    }
}
    


