using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class DontDestroyOnLoadC : MonoBehaviour
    {
        private int count = 0;
        private void Awake()
        {
            if (count == 0)
            {
                count++;
                DontDestroyOnLoad(transform.gameObject);
            }
            else
            {
                Destroy(this);
            }
        
        }
    }
}

