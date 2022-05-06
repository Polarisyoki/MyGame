using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerMotion
    {
        public PlayerParam Param { get; private set; }
        public PlayerFSM FSM { get; private set; }
        public PlayerInput Input { get; private set; }
        
        public PlayerMotor Motor { get; private set; }

        public PlayerMotion()
        {
            Param = new PlayerParam();
            FSM = new PlayerFSM(this);
            Input = new PlayerInput(this);
            
            
            Motor = new PlayerMotor(this);
        }

        public void Update()
        {
            Input.Update();
            FSM.Update();
            Motor.Update();
        }

        public void FixedUpdate()
        {
            Motor.FixedUpdate();
        }
        
    }
}