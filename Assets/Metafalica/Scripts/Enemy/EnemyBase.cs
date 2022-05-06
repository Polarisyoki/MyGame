using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using DG.Tweening;

namespace Metafalica.RPG
{

    public class EnemyBase : MonoBehaviour
    {
        public int ID;
        public string Name;
        public int maxHP;
        private int curHP;
    
        public Image HPSlider;
    
        public event BattleQuestListener OnBattleQuestEvent;

        public virtual void Init()
        {
            OnSwapn();
        }
        
        protected virtual void OnSwapn()
        {
            curHP = maxHP;
            HpUiChange();
            
            QuestManager.Instance?.NewEnemyAddListener(this);
            
            if (!GameManager.Instance.AllEnemy.ContainsKey(ID))
            {
                GameManager.Instance.AllEnemy.Add(ID,new List<EnemyBase>());
            }
            GameManager.Instance.AllEnemy[ID].Add(this);
        }
    
        protected virtual void OnDamage(int damage)
        {
            if (curHP <= 0)
                return;
            curHP -= damage;
            if (curHP <= 0)
            {
                curHP = 0;
                Death();
            }
    
            HpUiChange();
        }
    
        protected virtual void HpUiChange()
        {
            if (HPSlider != null)
            {
                HPSlider.DOKill();
                float endVal = curHP * 1f / maxHP;
                HPSlider.DOFillAmount(endVal, 0.1f);
            }
        }
        
        protected virtual void Death()
        {
            if (OnBattleQuestEvent != null)
            {
                OnBattleQuestEvent(ID);
            }
        }
        
    
        private void OnDestroy()
        {
            // GameManager.Instance.AllEnemy[ID].Remove(this);
        }
    }
}

