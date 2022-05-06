using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private GameObject _player;

        public GameObject Player
        {
            get
            {
                if (ReferenceEquals(_player, null))
                {
                    _player = GameObject.FindWithTag(ParameterC.PlayerTagName);
                }

                return _player;
            }
        }

        //-----------此处应该放到一个单独脚本中
        public CharacterController CharacterC { get; private set; }
        public Transform PlayerModel { get; private set; }
        public PlayerAnimation PlayerAnim { get; private set; }
        
        //------------------

        public PlayerMotion Motion { get; private set; }
        public PlayerData PlayerData { get; private set; }
        public PlayerInventoryC Inventory { get; private set; }


        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            
            CharacterC = Player.transform.GetComponent<CharacterController>();
            PlayerModel = Player.transform.Find("Model").GetChild(0);
            PlayerAnim = PlayerModel.GetComponent<PlayerAnimation>();

            Motion = new PlayerMotion();
            Inventory = new PlayerInventoryC();
            PlayerData = new PlayerData();
            PlayerData.Init();
            Inventory.SetUsingEquipments();
        }

        public void Update()
        {
            Motion.Update();
        }

        public void FixedUpdate()
        {
            Motion.FixedUpdate();
        }
        

        public float GetPlayerDirection()
        {
            return Player.transform.rotation.eulerAngles.y;
        }

        public void LockPlayer(bool isLock, bool containCamera = true)
        {
            GlobalConditionC.freezePlayerCount += isLock ? 1 : -1;
            if (containCamera)
            {
                GlobalConditionC.freezeCam = isLock;
            }
        }

        public void MovePlayer(Vector3 target)
        {
            LockPlayer(true);
            Player.transform.position = target;
            UIManager.Instance.m_UIRoot.FadeSingle(0.1f, 0.1f, 0.1f);
            MyTimerSys.Instance.AddTimeTask(() => { LockPlayer(false); }, 0.2f, PETimeUnit.Second);
        }

        #region 相机

        public void LockCameraZoom()
        {
            GlobalConditionC.freezeCamZoom = true;
        }

        public void UnlockCameraZoom()
        {
            GlobalConditionC.freezeCamZoom = false;
        }

        #endregion
    }
}