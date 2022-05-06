using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerParam
    {
        public Vector2 moveInput;
        public bool run = true;
        public Vector3 velocity;

        public bool jump;
        public bool isJumpAnimEnd;

        public bool isGrounded;
        public bool ready2Fall;
        public bool roll;
        
        public bool attack;
        public int attackCombo;

    }
}

