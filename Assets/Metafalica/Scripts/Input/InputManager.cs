using System;
using System.Collections;
using System.Collections.Generic;
using Metafalica.RPG.InputSystem;
using UnityEngine;

namespace Metafalica.RPG
{
    public class DefaultInputData
    {
        public InputData InputData;
    }
    
    public class InputManager : Singleton<InputManager>
    {
        
        public InputData current;
        public bool enable { get; set; }

        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            
            // todo
            // 读表，由于可配置，因此记录在存储管理处
            // 未读到时从存储中读取默认配置
            if (current == null)
            {
                CfgManager.GetInputInfo(ref current);
            }
            
            current?.Init();
            enable = true;
        }

        public void Update()
        {
            if (!enable) return;
            
            current?.Update();
        }

        public static float GetValue(string name)
        {
            return Instance.current.GetValueKeyValue(name);
        }
        public static bool GetTap(string name)
        {
            return Instance.current.GetTapKeyState(name);
        }
        public static int GetTapCount(string name)
        {
            return Instance.current.GetTapKeyCount(name);
        }
        public static bool GetPressDown(string name)
        {
            return Instance.current.GetPressKeyDown(name);
        }
        public static bool GetPressUp(string name)
        {
            return Instance.current.GetPressKeyUp(name);
        }
        public static bool GetPressing(string name)
        {
            return Instance.current.GetPressKeyPressing(name);
        }
        public static float GetPressTime(string name)
        {
            return Instance.current.GetPressKeyPressTime(name);
        }

        public static bool GetMultiDown(string name)
        {
            return Instance.current.GetMultiKeyTriggered(name);
        }

        public static bool GetCombo(string name)
        {
            return Instance.current.GetComboKeyTriggered(name);
        }
        public static int GetComboCount(string name)
        {
            return Instance.current.GetComboKeyCombo(name);
        }
        
        public static float GetAxis1D(string name)
        {
            return Instance.current.GetAxisKeyValue1D(name);
        }
        public static Vector2 GetAxis2D(string name)
        {
            return Instance.current.GetAxisKeyValue2D(name);
        }

        public static float GetAxisMouseX()
        {
            return Input.GetAxis("Mouse X");
        }
        
        public static float GetAxisMouseY()
        {
            return Input.GetAxis("Mouse Y");
        }

        public static float GetAxisMouseScrollWheel()
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
        
        public static KeyCode[] TransString2Keycode(params string[] strs)
        {
            if (strs == null || strs.Length < 1)
                return null;
            KeyCode[] keys = new KeyCode[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                try
                {
                    keys[i] = (KeyCode)Enum.Parse(typeof(KeyCode), strs[i], true);
                }
                catch
                {
                    keys[i] = KeyCode.None;
                }
            }

            return keys;
        }

        //--------------临时使用
        public static bool GetMouseBtn0()
        {
            return Input.GetKeyDown(KeyCode.Mouse0);
        }
    }

}
