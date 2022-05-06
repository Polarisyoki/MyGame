using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class GameManager : SingletonMono<GameManager>
    {
        
        private Dictionary<int, List<EnemyBase>> _enemy = new Dictionary<int, List<EnemyBase>>();
        public Dictionary<int, List<EnemyBase>> AllEnemy => _enemy;

        public void Awake()
        {
            MyTimerSys.Instance.Init(this);
            ResourcesManager.Instance.Init(this);
            ObjectManager.Instance.Init(this);

            InputManager.Instance.Init(this);
            ItemManager.Instance.Init(this);
            NPCManager.Instance.Init(this);
            AudioManager.Instance.Init(this);
            
            PlayerManager.Instance.Init(this);
            MySceneManager.Instance.Init(this);
            UIManager.Instance.Init(this);

            BattleManager.Instance.Init(this);
            QuestManager.Instance.Init(this);

        }

        private void Update()
        {
            InputManager.Instance.Update();
            UIManager.Instance.Update();
            PlayerManager.Instance.Update();
        }

        private void FixedUpdate()
        {
            PlayerManager.Instance.FixedUpdate();
        }

        public void OnApplicationQuit()
        {
#if UNITY_EDITOR
            ResourcesManager.Instance.ClearCache();
            Resources.UnloadUnusedAssets();
#endif
        }
    }
}