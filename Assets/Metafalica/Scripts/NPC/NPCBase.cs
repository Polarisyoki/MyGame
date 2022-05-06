using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class NPCBase : MonoBehaviour
    {
        [Header("NPC ID:9010000~9099999")]
        [SerializeField]private int _id;
        public int Id => _id;
    
        [SerializeField]private string _name;
        public string Name => _name;

        private Text _showName;
        
        public NPCData m_NpcData;
        
        private void Awake()
        {
            _showName = transform.Find("Canvas/Text").GetComponent<Text>();
        }

        public virtual void Init(NPCData data)
        {
            
            this._id = data.id;
            this._name = data.name;
            _showName.text = _name;
            
            this.transform.position = data.pos;
            this.transform.rotation = Quaternion.Euler(data.rot);

            this.m_NpcData = data;
        }
        
        protected virtual void Dialogue()
        {
            m_NpcData.ExecEvent();
        }
    }

}
