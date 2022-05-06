using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG.InputSystem
{
    public class InputData
    {
        public KeyContainer<ValueKey> valueKeys = new KeyContainer<ValueKey>();
        public KeyContainer<TapKey> tapKeys = new KeyContainer<TapKey>();
        public KeyContainer<PressKey> pressKeys = new KeyContainer<PressKey>();
        public KeyContainer<MultiKey> multiKeys = new KeyContainer<MultiKey>();
        public KeyContainer<ComboKey> comboKeys = new KeyContainer<ComboKey>();
        public KeyContainer<AxisKey> axisKeys = new KeyContainer<AxisKey>();

        private Dictionary<string, IKeyContainer> _keyContainerDic;

        public Dictionary<string, IKeyContainer> KeyDic
        {
            get
            {
                if (_keyContainerDic == null)
                {
                    InitKeyDic();
                }

                return _keyContainerDic;
            }
        }

        public InputData(InputCfg config)
        {
            for (int i = 0; i < config.m_Keys.Length; i++)
            {
                switch (config.m_Keys[i].VirtualKeyType)
                {
                    case 0:
                        ValueKey valueKey = new ValueKey();
                        valueKey.name = config.m_Keys[i].Name;
                        valueKey.SetEnable(config.m_Keys[i].Enable == 0);
                        valueKey.SetKeyCode(InputManager.TransString2Keycode(config.m_Keys[i].KeyCode));
                        
                        valueKeys.keys.Add(valueKey);
                        break;
                    case 1:
                        TapKey tapKey = new TapKey();
                        tapKey.name = config.m_Keys[i].Name;
                        tapKey.SetEnable(config.m_Keys[i].Enable == 0);
                        tapKey.SetKeyCode(InputManager.TransString2Keycode(config.m_Keys[i].KeyCode));
                        tapKey.clickCount = config.m_Keys[i].ClickCount;
                        
                        tapKeys.keys.Add(tapKey);
                        break;
                    case 2:
                        PressKey pressKey = new PressKey();
                        pressKey.name = config.m_Keys[i].Name;
                        pressKey.SetEnable(config.m_Keys[i].Enable == 0);
                        pressKey.SetKeyCode(InputManager.TransString2Keycode(config.m_Keys[i].KeyCode));
                        pressKey.type = (PressKey.PressKeyType) config.m_Keys[i].KeyType;
                        
                        pressKeys.keys.Add(pressKey);
                        break;
                    case 3:
                        MultiKey multiKey = new MultiKey();
                        multiKey.name = config.m_Keys[i].Name;
                        multiKey.SetEnable(config.m_Keys[i].Enable == 0);
                        multiKey.SetKeyCode(InputManager.TransString2Keycode(config.m_Keys[i].KeyCode));
                        
                        multiKeys.keys.Add(multiKey);
                        break;
                    case 4:
                        ComboKey comboKey = new ComboKey();
                        comboKey.name = config.m_Keys[i].Name;
                        comboKey.SetEnable(config.m_Keys[i].Enable == 0);
                        comboKey.SetKeyCode(InputManager.TransString2Keycode(config.m_Keys[i].KeyCode));
                        
                        comboKeys.keys.Add(comboKey);
                        break;
                    case 5:
                        AxisKey axisKey = new AxisKey();
                        axisKey.name = config.m_Keys[i].Name;
                        axisKey.SetEnable(config.m_Keys[i].Enable == 0);
                        axisKey.SetKeyCode(InputManager.TransString2Keycode(config.m_Keys[i].KeyCode));
                        axisKey.type = (AxisKey.AxisKeyType) config.m_Keys[i].KeyType;
                        axisKey.dim = (AxisKey.AxisKeyDimension) config.m_Keys[i].AxisKeyDimension;
                        
                        axisKeys.keys.Add(axisKey);
                        break;
                }
            }
        }

        private void InitKeyDic()
        {
            _keyContainerDic = new Dictionary<string, IKeyContainer>();
            _keyContainerDic.Add("ValueKey", valueKeys);
            _keyContainerDic.Add("TapKey", tapKeys);
            _keyContainerDic.Add("PressKey", pressKeys);
            _keyContainerDic.Add("MultiKey", multiKeys);
            _keyContainerDic.Add("ComboKey", comboKeys);
            _keyContainerDic.Add("AxisKey", axisKeys);
        }

        public void Init()
        {
            foreach (var value in KeyDic.Values)
            {
                value?.Init();
            }
        }

        public void Update()
        {
            foreach (var value in KeyDic.Values)
            {
                value?.Update();
            }
        }
        
        public void SetKeyCode<T>(string name, params KeyCode[] keyCodes) where T : VirtualKey
        {
            if (keyCodes == null)
                return;
            KeyDic[typeof(T).Name]?.SetKeyCode(name, keyCodes);
        }

        public void EnableKey<T>(string name, bool enable) where T : VirtualKey
        {
            KeyDic[typeof(T).Name]?.EnableKey(name, enable);
        }

        private TValue GetKeyProperty<TKey, TValue>(string name, IKeyContainer container,
            Func<TKey,TValue> function) where TKey : VirtualKey
        {
            TKey key = container.GetKeyObject(name) as TKey;
            if (key == null)
                return default;
            return function(key);
        }

        // 读取各类键的属性

        #region ValueKey

        public float GetValueKeyValue(string name)
        {
            return GetKeyProperty<ValueKey, float>(name, valueKeys, k => k.value);
        }

        #endregion

        #region TapKey

        public bool GetTapKeyState(string name)
        {
            return GetKeyProperty<TapKey, bool>(name, tapKeys, k => k.isTrigger);
        }

        public int GetTapKeyCount(string name)
        {
            return GetKeyProperty<TapKey, int>(name, tapKeys, k => k.curCount);
        }

        #endregion

        #region PressKey

        public bool GetPressKeyDown(string name)
        {
            return GetKeyProperty<PressKey, bool>(name, pressKeys, k => k.isDown);
        }

        public bool GetPressKeyUp(string name)
        {
            return GetKeyProperty<PressKey, bool>(name, pressKeys, k => k.isUp);
        }

        public bool GetPressKeyPressing(string name)
        {
            return GetKeyProperty<PressKey, bool>(name, pressKeys, k => k.isPressing);
        }

        public float GetPressKeyPressTime(string name)
        {
            return GetKeyProperty<PressKey, float>(name, pressKeys, k => k.pressTime);
        }

        #endregion

        #region MultiKey

        public bool GetMultiKeyTriggered(string name)
        {
            return GetKeyProperty<MultiKey, bool>(name, multiKeys, k => k.isTrigger);
        }

        #endregion

        #region ComboKey

        public bool GetComboKeyTriggered(string name)
        {
            return GetKeyProperty<ComboKey, bool>(name, comboKeys, k => k.isTrigger);
        }

        public int GetComboKeyCombo(string name)
        {
            return GetKeyProperty<ComboKey, int>(name, comboKeys, k => k.combo);
        }

        #endregion

        #region AxisKey

        public float GetAxisKeyValue1D(string name)
        {
            return GetKeyProperty<AxisKey, float>(name, axisKeys, k => k.GetValue1D);
        }

        public Vector2 GetAxisKeyValue2D(string name)
        {
            return GetKeyProperty<AxisKey, Vector2>(name, axisKeys, k => k.GetValue2D);
        }

        #endregion
    }
}