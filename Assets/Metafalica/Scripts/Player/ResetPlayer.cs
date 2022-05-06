using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class ResetPlayer : MonoBehaviour
    {
        private void Start()
        {
            if (!GameObject.FindWithTag(ParameterC.PlayerTagName))
            {
                Invoke("Start",0.5f);
                // return;
            }
            else
            {
                Reset();
            }
        }

        public void Reset()
        {
            GameObject player = GameObject.FindWithTag(ParameterC.PlayerTagName);
        
            player.transform.root.position = transform.position;
            player.transform.root.rotation = transform.rotation;
        }
    
    }
}

