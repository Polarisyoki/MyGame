using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG.InputSystem
{
    public interface IKeyContainer
    {
        VirtualKey GetKeyObject(string name); //获取虚拟键对象
        void SetKeyCode(string name, params KeyCode[] keyCodes); //设置键值
        void EnableKey(string name, bool enable); //启用虚拟键
        void Init(); //初始化
        void Update(); //更新
    }

    public class KeyContainer<T> : IKeyContainer where T : VirtualKey, new()
    {
        public List<T> keys = new List<T>();

        public VirtualKey GetKeyObject(string name)
        {
            if (keys == null)
                return null;
            return keys.Find(key => key.name == name);
        }
        
        public void SetKeyCode(string name, params KeyCode[] keyCodes)
        {
            if(keyCodes == null)
                return;
            VirtualKey key = GetKeyObject(name);
            key?.SetKeyCode(keyCodes);
        }

        public void EnableKey(string name, bool enable)
        {
            VirtualKey key = GetKeyObject(name);
            key?.SetEnable(enable);
        }

        public void Init()
        {
            if (keys == null)
                return;
            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].Init();
            }
        }

        public void Update()
        {
            if (keys == null)
                return;
            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].Update();
            }
        }
    }
}