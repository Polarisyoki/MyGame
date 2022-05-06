using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerInput
    {
        private PlayerParam _param;

        public PlayerInput(PlayerMotion motion)
        {
            _param = motion.Param;
        }

        public void Update()
        {
            ChangeMoveMode();
            _param.moveInput = InputManager.GetAxis2D(GameKeyName.MOVE);
            _param.attack = InputManager.GetMouseBtn0();
            _param.jump = Input.GetKeyDown(KeyCode.Space);
        }

        private void ChangeMoveMode()
        {
            if (InputManager.GetTap(GameKeyName.SWITCHMOVESTATE))
            {
                _param.run = !_param.run;

                if (!_param.run)
                {
                    UITipManager.Instance.PushMessage("已切换至行走模式", 0.5f);
                }
                else
                {
                    UITipManager.Instance.PushMessage("已切换至急行模式", 0.5f);
                }
            }
        }
    }
}