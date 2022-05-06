using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG.FSM
{
    public class BaseFSM
    {
        private StateType _curStateType;
        private Dictionary<StateType, FSMState> _stateDic;

        private FSMState _curFsmState;
        private FSMState _defaultFsmState;

        private StateType tempStateType;

        public BaseFSM()
        {
            _stateDic = new Dictionary<StateType, FSMState>();
        }

        public void Update()
        {
            if (_curFsmState != null)
            {
                _curFsmState.OnUpdate();
                if (_curFsmState.CheckCondition(out tempStateType))
                {
                    if (tempStateType != _curStateType)
                        ChangeState(tempStateType);
                }
            }
        }

        public void SetDefault(StateType stateType)
        {
            if (_stateDic.ContainsKey(stateType))
            {
                _defaultFsmState = _stateDic[stateType];
                _curFsmState = _defaultFsmState;
                _curStateType = stateType;
            }
        }

        public void AddState(StateType stateType, FSMState fsmState)
        {
            if (fsmState == null || stateType == StateType.None)
                return;
            _stateDic.Add(stateType, fsmState);
        }
        
        private void ChangeState(StateType stateType)
        {
            if (_stateDic.TryGetValue(stateType, out FSMState fsmState))
            {
                _curFsmState?.OnExit();
                fsmState.OnEnter();
                _curFsmState = fsmState;
                _curStateType = stateType;
            }
        }
    }
}