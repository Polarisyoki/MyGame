using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static readonly string obj = "lock";
        
        public static T Instance
        {
            get
            {
                lock (obj)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }

        protected MonoBehaviour m_Mono;

        public virtual void Init(MonoBehaviour mono)
        {
            this.m_Mono = mono;
        }
    }
}

