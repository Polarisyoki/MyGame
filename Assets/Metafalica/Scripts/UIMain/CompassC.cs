using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class CompassC : MonoBehaviour
    {
        public Transform point;
        private Vector3 direction;

        private void Start()
        {
            direction = Vector3.zero;
        }

        private void LateUpdate()
        {
            direction.z = -PlayerManager.Instance.GetPlayerDirection();
            point.rotation = Quaternion.Euler(direction);
        }
    }
}

