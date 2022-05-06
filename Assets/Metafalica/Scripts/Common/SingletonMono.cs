using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        _instance = new GameObject().AddComponent<T>();
                        _instance.name = typeof(T).Name;
                        _instance.gameObject.AddComponent<DontDestroyOnLoadC>();
                    }
                }
                return _instance;
            }
        }
    }
}

