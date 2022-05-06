using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Metafalica.RPG
{
    public class LookatPlayer : MonoBehaviour
    {
        private void Start()
        {
            Vector3 v = this.transform.localScale;
            this.transform.localScale = new Vector3(-v.x, v.y, v.z);
        }

        void Update()
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}

