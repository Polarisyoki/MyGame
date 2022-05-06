using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG.FSM
{
    public enum StateType
    {
        None,
        Enter, //入场
        Idle,
        Move,
        Jump,
        Fall,
        Attack,
        Damage,
        Die,
    }
    public class FSMState
    {
        //达成条件切换到的状态
        // private Dictionary<Func<bool>, StateType> _conditionDic;
        
        private Action _enterAction;
        private Action _updateAction;
        private Action _exitAction;
        private Func<StateType> _condition;

        public void SetEnterAction(Action action)
        {
            _enterAction = action;
        }
        public void SetUpdateAction(Action action)
        {
            _updateAction = action;
        }
        public void SetExitAction(Action action)
        {
            _exitAction = action;
        }

        public virtual void OnEnter()
        {
            _enterAction?.Invoke();
        }

        public virtual void OnUpdate()
        {
            _updateAction?.Invoke();
        }

        public virtual void OnExit()
        {
            _exitAction?.Invoke();
        }
        
        public void SetCondition(Func<StateType> condition)
        {
            _condition = condition;
        }

        public bool CheckCondition(out StateType stateType)
        {
            if (ReferenceEquals(_condition, null))
            {
                stateType = StateType.None;
                return false;
            }
            stateType = _condition();
            if (stateType == StateType.None)
                return false;
            else
                return true;
        }
    }
    
}