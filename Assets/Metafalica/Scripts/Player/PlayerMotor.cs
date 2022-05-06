using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerMotor
    {
        private PlayerAnimation playerAnimation;

        private CharacterController characterController;
        private PlayerParam _param;

        private float grivaty = 9.8f;
        private Vector3 velocity;
        private bool isGrounded;

        public PlayerMotor(PlayerMotion motion)
        {
            playerAnimation = PlayerManager.Instance.PlayerAnim;
            characterController = PlayerManager.Instance.CharacterC;
            _param = motion.Param;

            Init();
        }

        private void Init()
        {
            velocity = Vector3.zero;
            _param.velocity = velocity;
        }


        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            DetectGround();
            if (!isGrounded)
            {
                //忽略阻力
                velocity.y -= grivaty * Time.deltaTime;
            }
            else
            {
                if (velocity.y <= 0)
                    velocity.y = 0;
            }

            characterController.Move(velocity * Time.deltaTime);
            _param.velocity = velocity;
        }

        private Vector3 offset = new Vector3(0, 0.1f, 0);

        public void DetectGround()
        {
            isGrounded = Physics.Raycast(characterController.transform.position + offset, Vector3.down, 0.11f, 1 << 10);
            _param.isGrounded = isGrounded;
            _param.ready2Fall = !Physics.Raycast(characterController.transform.position + offset, Vector3.down, 0.3f,
                1 << 10);
        }

        public void IdleEnter()
        {
            ClearSpeed();
            playerAnimation.ChangeAnim("Idle");
        }

        private float playerSpeed = 0;
        private float playerWalkSpeed = 1.5f;
        private float playerRunSpeed = 4f;
        private float moveRate = 0; //起步速率

        public void MoveEnter()
        {
            playerAnimation.ChangeAnim("Move");
            moveRate = 0;
        }

        public void MoveUpdate(Vector2 input)
        {
            if (moveRate <= 1)
            {
                moveRate += Time.deltaTime * 2;
            }

            float speedRate = _param.run ? input.magnitude : 0;
            playerAnimation.ChangeAnim("Speed", speedRate * moveRate);

            playerSpeed = 0;
            if (input.magnitude > 0.1f)
            {
                if (!_param.run)
                {
                    playerSpeed = playerWalkSpeed * input.magnitude;
                }
                else
                {
                    playerSpeed = playerRunSpeed * input.magnitude;
                }
            }

            Vector3 moveDirection = characterController.transform.forward;

            //y轴速度需要继承
            float y = velocity.y;
            velocity = playerSpeed * moveRate * moveDirection;
            velocity.y = y;
        }

        public void MoveExit()
        {
            
        }

        public void ClearSpeed()
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        private float rotateLerpTime = 0.2f;
        private Quaternion targetRotate;
        private bool rotateInit = false;

        public void Rotate(Transform player, float rotX)
        {
            if (!rotateInit)
            {
                rotateInit = true;
                targetRotate = player.rotation;
            }

            rotateLerpTime = 0.25f;
            if (_param.moveInput.magnitude > 0.1f && !GlobalConditionC.IsFreezePlay)
            {
                rotateLerpTime = 0.15f;
                float temp = GetOffsetY(_param.moveInput);
                targetRotate = Quaternion.Euler(0, rotX + temp, 0);
            }


            if (player.rotation != targetRotate)
            {
                player.rotation = Quaternion.Lerp(player.rotation, targetRotate, rotateLerpTime);
            }
        }

        private float GetOffsetY(Vector2 dir)
        {
            if (dir.y == 0)
            {
                if (dir.x < 0)
                    return -90;
                else if (dir.x > 0)
                    return 90;
                else
                    return 0;
            }
            else if (dir.y > 0)
            {
                if (dir.x < 0)
                    return -45;
                else if (dir.x > 0)
                    return 45;
                else
                    return 0;
            }
            else
            {
                if (dir.x < 0)
                    return -135;
                else if (dir.x > 0)
                    return 135;
                else
                    return 180;
            }
        }


        public void JumpEnter()
        {
            PlayerManager.Instance.LockPlayer(true, false);
            playerAnimation.ChangeAnim("Jump");
            velocity += 5 * Vector3.up;
        }

        public void JumpUpdate()
        {
            if (playerAnimation.IsAnimState("Jump", out float time))
            {
                if (time > 1.0f)
                    _param.isJumpAnimEnd = true;
            }
        }

        public void JumpExit()
        {
            PlayerManager.Instance.LockPlayer(false, false);
            _param.isJumpAnimEnd = false;
        }


        public void FallEnter()
        {
            PlayerManager.Instance.LockPlayer(true, false);
            playerAnimation.ChangeAnim("Fall");
        }

        public void FallExit()
        {
            PlayerManager.Instance.LockPlayer(false, false);
            //播放声音
            //落地动作
        }

        public void Roll()
        {
        }


        #region Attack

        private bool canToNextAttack;
        private int maxCombo = 3;

        public void AttackEnter()
        {
            PlayerManager.Instance.LockPlayer(true, false);
            _param.attackCombo = 1;
            playerAnimation.WeaponAppear();
            playerAnimation.ChangeAnim("Attack");
        }

        public void AttackUpdate()
        {
            if (playerAnimation.IsAnimState("Attack" + _param.attackCombo, out float time))
            {
                if (time >= 1f)
                {
                    _param.attackCombo = -1;
                }
                else if (time > 0.8f)
                {
                    if (canToNextAttack)
                    {
                        _param.attackCombo++;
                        playerAnimation.ChangeAnim("AttackCombo");
                        canToNextAttack = false;
                    }
                }
                else if (time > 0.3f)
                {
                    if (_param.attack && _param.attackCombo < maxCombo)
                    {
                        canToNextAttack = true;
                    }
                }
            }
        }

        public void AttackExit()
        {
            _param.attackCombo = 0;
            playerAnimation.WeaponHide();
            PlayerManager.Instance.LockPlayer(false, false);
        }

        #endregion
    }
}