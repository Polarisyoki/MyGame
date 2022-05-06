using System;
using System.Collections;
using System.Collections.Generic;
using Metafalica.RPG.FSM;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerFSM
    {
        private BaseFSM _fsm;
        private PlayerParam _param;

        public PlayerFSM(PlayerMotion motion)
        {
            _fsm = new BaseFSM();
            _param = motion.Param;

            var idle = new FSMState();
            idle.SetEnterAction(() => { motion.Motor.IdleEnter(); });
            _fsm.AddState(StateType.Idle, idle);
            _fsm.SetDefault(StateType.Idle);

            var move = new FSMState();
            move.SetEnterAction(() => { motion.Motor.MoveEnter(); });
            move.SetUpdateAction(() => { motion.Motor.MoveUpdate(_param.moveInput); });
            move.SetExitAction(() => { motion.Motor.MoveExit(); });
            _fsm.AddState(StateType.Move, move);

            var attack = new FSMState();
            attack.SetEnterAction(() => { motion.Motor.AttackEnter(); });
            attack.SetUpdateAction(() => { motion.Motor.AttackUpdate(); });
            attack.SetExitAction(() => { motion.Motor.AttackExit(); });
            _fsm.AddState(StateType.Attack, attack);
            
            var jump = new  FSMState();
            jump.SetEnterAction(()=>motion.Motor.JumpEnter());
            jump.SetUpdateAction(()=>motion.Motor.JumpUpdate());
            jump.SetExitAction(()=>motion.Motor.JumpExit());
            _fsm.AddState(StateType.Jump, jump);
            
            var fall = new  FSMState();
            fall.SetEnterAction(()=>motion.Motor.FallEnter());
            fall.SetExitAction(()=>motion.Motor.FallExit());
            _fsm.AddState(StateType.Fall, fall);

            idle.SetCondition(IdleTo);
            
            move.SetCondition(MoveTo);
            
            attack.SetCondition(AttackTo);
            
            jump.SetCondition(JumpTo);
            
            fall.SetCondition(FallTo);
        }


        private StateType IdleTo()
        {
            if (GlobalConditionC.IsFreezePlay)
                return StateType.None;

            if (_param.ready2Fall)
                return StateType.Fall;
            
            if (_param.isGrounded && _param.jump)
                return StateType.Jump;
            
            if (_param.attack)
                return StateType.Attack;
            
            if (_param.moveInput.magnitude > 0.1f)
                return StateType.Move;

            return StateType.None;
        }

        private StateType MoveTo()
        {
            if (GlobalConditionC.IsFreezePlay)
                return StateType.Idle;
            
            if (_param.ready2Fall)
                return StateType.Fall;
            
            if (_param.isGrounded && _param.jump)
                return StateType.Jump;
            
            if (_param.attack)
                return StateType.Attack;

            if (_param.moveInput.magnitude <= 0.01f)
                return StateType.Idle;
            
            return StateType.None;
        }

        private StateType AttackTo()
        {
            if (_param.attackCombo == -1)
            {
                if (_param.moveInput.magnitude <= 0.1f)
                    return StateType.Idle;
                else
                    return StateType.Move;
            }
            
            return StateType.None;
        }

        private StateType JumpTo()
        {
            if (_param.isGrounded && _param.velocity.y <= 0)
            {
                if (_param.moveInput.magnitude <= 0.1f)
                    return StateType.Idle;
                else
                    return StateType.Move;
            }

            if (_param.isJumpAnimEnd)
                return StateType.Fall;

            return StateType.None;
        }

        private StateType FallTo()
        {
            if (_param.isGrounded && _param.velocity.y <= 0)
            {
                if (_param.moveInput.magnitude <= 0.1f)
                    return StateType.Idle;
                else
                    return StateType.Move;
            }

            return StateType.None;
        }
        
        public void Update()
        {
            _fsm.Update();
        }
    }
}