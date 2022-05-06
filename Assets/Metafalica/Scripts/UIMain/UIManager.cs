using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class UIManager : Singleton<UIManager>
    {
        public UIRoot m_UIRoot;

        private UIState _uiState;
        private Stack<GameObject> windowStack;
        private Stack<UIState> uiStates;

        private GameObject baseUI; //血条之类的
        private GameUIC gameUiC;
        private bool canDialogue = false;

        private const string _settingStr = "Setting";
        private const string _infantryPackStr = "InfantryPack";
        private const string _playerStateStr = "PlayerState";
        private const string _questWindow = "QuestWindow";

        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);

            m_UIRoot = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_UIRoot).GetComponent<UIRoot>();
            m_UIRoot.Init();

            _uiState = UIState.None;
            windowStack = new Stack<GameObject>();
            uiStates = new Stack<UIState>();

            UITipManager.Instance.Init(mono);
        }


        public void Update()
        {
            if (_uiState == UIState.None)
            {
                if (InputManager.GetPressDown(GameKeyName.BAG))
                {
                    OpenWindow(GamePath.Res_Prefab_File_InfantryPack, UIState.InfantryPack);
                }
                else if (InputManager.GetPressDown(GameKeyName.PLAYERSTATE))
                {
                    OpenWindow(GamePath.Res_Prefab_File_PlayerStatePanel, UIState.PlayerState);
                }
                else if (InputManager.GetPressDown(GameKeyName.QUEST))
                {
                    OpenWindow(GamePath.Res_Prefab_File_QuestPanel, UIState.Quest);
                }
            }
            else if (_uiState == UIState.InfantryPack)
            {
                if (InputManager.GetPressDown(GameKeyName.BAG))
                {
                    CloseWindow();
                }
            }
            else if (_uiState == UIState.PlayerState)
            {
                if (InputManager.GetPressDown(GameKeyName.PLAYERSTATE))
                {
                    CloseWindow();
                }
            }
            else if (_uiState == UIState.Quest)
            {
                if (InputManager.GetPressDown(GameKeyName.QUEST))
                {
                    CloseWindow();
                }
            }

            if (canDialogue)
            {
                if (InputManager.GetPressDown(GameKeyName.DIALOGUE))
                {
                    gameUiC.ExecOption();
                }

                if (InputManager.GetAxisMouseScrollWheel() > 0.01f)
                {
                    gameUiC.ChangeSelect(false);
                }
                else if (InputManager.GetAxisMouseScrollWheel() < -0.01f)
                {
                    gameUiC.ChangeSelect(true);
                }
            }
        }

        public UIState GetUIState()
        {
            return _uiState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">窗口预制体路径</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public GameObject OpenWindow(string path, UIState state)
        {
            GameObject obj = ObjectManager.Instance.InstantiateObject(path, m_UIRoot.m_Canvas);
            return OpenWindow(obj, state);
        }

        public GameObject OpenWindow(GameObject window, UIState state)
        {
            if (GlobalConditionC.freezeUI)
                return null;

            if (_uiState != UIState.None)
                return null;

            if (windowStack.Count > 0)
            {
                windowStack.Peek().SetActive(false);
            }

            var go = window;
            go.SetActive(true);

            windowStack.Push(go);
            uiStates.Push(state);
            _uiState = state;

            baseUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PlayerManager.Instance.LockPlayer(true);

            return go;
        }

        public void CloseWindow(bool isDestroy = true)
        {
            GameObject go = windowStack.Pop();
            uiStates.Pop();

            ObjectManager.Instance.ReleaseObject(go, 0, isDestroy);

            if (windowStack.Count > 0)
            {
                windowStack.Peek().SetActive(true);
                _uiState = uiStates.Peek();
            }
            else
            {
                _uiState = UIState.None;

                baseUI.SetActive(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                PlayerManager.Instance.LockPlayer(false);
            }
        }

        public void SetBaseUI(GameObject ui, GameUIC gUiC)
        {
            baseUI = ui;
            gameUiC = gUiC;
        }

        #region 交互选项

        public void PushInteractiveOption(int id, string name, Action action)
        {
            gameUiC.AddInteractiveOption(id, name, action);
            canDialogue = true;
            PlayerManager.Instance.LockCameraZoom();
        }

        public void PopInteractiveOption(int id)
        {
            gameUiC.RemoveInteractiveOption(id,
                () =>
                {
                    canDialogue = false;
                    PlayerManager.Instance.UnlockCameraZoom();
                });
        }

        //切换场景前执行
        public void PopAllInteractiveOption()
        {
            gameUiC.RemoveAllInteractiveOption(
                () =>
                {
                    canDialogue = false;
                    PlayerManager.Instance.UnlockCameraZoom();
                });
        }

        #endregion
    }

    public enum UIState
    {
        None,
        Setting,
        PlayerState,
        InfantryPack,
        Shopping,
        Dialogue,
        Quest,
    }
}