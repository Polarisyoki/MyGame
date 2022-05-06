using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class DestroyAtAwake : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(this.gameObject);
        }
    }

}
