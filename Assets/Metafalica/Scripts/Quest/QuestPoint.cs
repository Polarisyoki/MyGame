using System;
using UnityEngine;

namespace Metafalica.RPG
{

    public class QuestPoint : MonoBehaviour
    {
        [SerializeField] private int _id;
        public int Id => _id;
        [SerializeField] private string _name;
        public string Name => _name;
        
        
        public QuestPointData m_QuestPointData;
        
        public virtual void Init(QuestPointData data)
        {
            _id = data.id;
            _name = data.name;
            transform.position = data.pos;

            m_QuestPointData = data;
            
            QuestManager.Instance.m_AllQuestPoint.Add(_id,this);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            m_QuestPointData.ExecEvent();
        }

        private void OnTriggerStay(Collider other)
        {
            //todo
        }
    
        private void OnTriggerExit(Collider other)
        {
            //todo
        }
        
    }
}

