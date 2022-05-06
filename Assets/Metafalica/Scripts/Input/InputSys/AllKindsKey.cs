using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG.InputSystem
{
    // public enum VirtualKeyType
    // {
    //     ValueKey,
    //     TapKey,
    //     PressKey,
    //     MultiKey,
    //     ComboKey,
    //     AxisKey,
    // }


    #region VirtualKey,所有按键类型的基类

    public abstract class VirtualKey
    {
        public string name;
        protected bool enable;

        public virtual void Init()
        {
            this.enable = true;
        }

        public virtual void SetEnable(bool enable)
        {
            this.enable = enable;
        }

        public abstract void Update();
        public abstract void SetKeyCode(params KeyCode[] keyCodes);
    }

    #endregion

    #region ValueKey,值类型按键；情景：移动键等

    public class ValueKey : VirtualKey
    {
        public Vector2 range = new Vector2(0, 1); //范围
        public float start = 0;
        public Vector2 speed = 5 * Vector2.one; //(增加速度,减少速度)
        public KeyCode keyCode;
        public float value = 0;

        public override void Init()
        {
            base.Init();
            value = start;
        }

        public override void SetEnable(bool enable)
        {
            base.SetEnable(enable);
            value = start;
        }

        public override void SetKeyCode(params KeyCode[] keyCodes)
        {
            if (keyCodes.Length > 0)
            {
                this.keyCode = keyCodes[0];
            }
        }

        public override void Update()
        {
            //速度>0，起始值在范围内且范围没问题
            if (speed.x <= 0 || speed.y <= 0 || range.x >= range.y || start >= range.y || start <= range.x)
                return;

            if (Input.GetKey(keyCode) && enable)
            {
                if (value < range.y)
                {
                    value += speed.x * Time.deltaTime;
                    value = Mathf.Clamp(value, range.x, range.y);
                }
            }
            else
            {
                if (value > range.x)
                {
                    value -= speed.y * Time.deltaTime;
                    value = Mathf.Clamp(value, range.x, range.y);
                }
            }
        }
    }

    #endregion

    #region TapKey,连按触发；情景：鼠标双击触发等

    public class TapKey : VirtualKey
    {
        public int clickCount = 1; //点几次触发
        public float maxClickInterval = 0.5f; //两次点击之间的最大间隔
        public KeyCode keyCode;
        public bool isTrigger; //是否触发
        public int curCount; //当前点击次数
        private float curClickInterval; //当前点击间隔

        public override void SetEnable(bool enable)
        {
            base.SetEnable(enable);
            isTrigger = false;
            curCount = 0;
            curClickInterval = 0;
        }

        public override void SetKeyCode(params KeyCode[] keyCodes)
        {
            if (keyCodes.Length > 0)
            {
                this.keyCode = keyCodes[0];
            }
        }

        public override void Update()
        {
            if (!enable || clickCount < 1)
                return;

            isTrigger = false;
            //单击
            if (clickCount == 1)
            {
                isTrigger = UnityEngine.Input.GetKeyDown(keyCode);
            }
            else if (maxClickInterval > 0)
            {
                curClickInterval += Time.deltaTime;
                if (curClickInterval <= maxClickInterval)
                {
                    if (UnityEngine.Input.GetKeyDown(keyCode))
                    {
                        curCount++;
                        curClickInterval = 0;
                        if (curCount >= clickCount)
                        {
                            isTrigger = true;
                            curCount = 0;
                        }
                    }
                }
                else
                {
                    //超时重置
                    curCount = 0;
                    curClickInterval = 0;
                }
            }
        }
    }

    #endregion

    #region PressKey,按下、松开、长按触发

    public class PressKey : VirtualKey
    {
        public enum PressKeyType
        {
            DOWN,
            PRESSING,
            UP,
        }

        public PressKeyType type;
        public KeyCode keyCode;

        public bool isDown;
        public bool isPressing;
        public bool isUp;
        public float pressTime;

        public override void SetEnable(bool enable)
        {
            base.SetEnable(enable);
            isDown = false;
            isPressing = false;
            isUp = false;
            pressTime = 0;
        }

        public override void SetKeyCode(params KeyCode[] keyCodes)
        {
            if (keyCodes.Length > 0)
            {
                this.keyCode = keyCodes[0];
            }
        }

        public override void Update()
        {
            if (!enable) return;

            switch (type)
            {
                case PressKeyType.DOWN:
                    isDown = UnityEngine.Input.GetKeyDown(keyCode);
                    break;
                case PressKeyType.UP:
                    isUp = UnityEngine.Input.GetKeyUp(keyCode);
                    break;
                case PressKeyType.PRESSING:
                    isPressing = UnityEngine.Input.GetKey(keyCode);
                    if (isPressing)
                    {
                        pressTime += Time.deltaTime;
                    }
                    else
                    {
                        pressTime = 0;
                    }

                    break;
            }
        }
    }

    #endregion

    #region MultiKey,同时按下多个键

    public class MultiKey : VirtualKey
    {
        public int count = 2; //组合键数量
        public List<KeyCode> keys;
        public float interval = 0.5f; //检测的最大间隔

        public bool isTrigger;
        private float curInterval;
        private bool[] keyState; //标记每个键是否按下

        public override void Init()
        {
            base.Init();
            keyState = new bool[count];
            for (int i = 0; i < count; i++)
            {
                keyState[i] = false;
            }
        }

        public override void SetEnable(bool enable)
        {
            base.SetEnable(enable);
            this.isTrigger = false;
            Reset();
        }

        public override void SetKeyCode(params KeyCode[] keyCodes)
        {
            keys.Clear();
            for (int i = 0; i < keyCodes.Length; i++)
            {
                keys.Add(keyCodes[i]);
            }
        }

        public override void Update()
        {
            if (!enable || keys == null || keys.Count < count || keyState == null)
                return;

            isTrigger = false;
            curInterval += Time.deltaTime;

            if (curInterval <= interval)
            {
                for (int i = 0; i < count; i++)
                {
                    if (UnityEngine.Input.GetKeyDown(keys[i]))
                    {
                        keyState[i] = true;
                    }
                }

                if (AllKeyTrigger())
                {
                    isTrigger = true;
                    Reset();
                }
            }
            else
            {
                Reset();
            }
        }

        private bool AllKeyTrigger()
        {
            for (int i = 0; i < count; i++)
            {
                if (!keyState[i]) return false;
            }

            return true;
        }

        private void Reset()
        {
            curInterval = 0;
            for (int i = 0; i < count; i++)
            {
                keyState[i] = false;
            }
        }
    }

    #endregion

    #region ComboKey,检测任意数量任意键的combo；如：↑↓↑↓←→AB

    public class ComboKey : VirtualKey
    {
        public List<KeyCode> keys;
        public float interval = 0.5f; //两个键之间的最大间隔
        public bool isTrigger;
        public int combo; //连击数
        private float curInterval;

        public override void SetEnable(bool enable)
        {
            base.SetEnable(enable);
            isTrigger = false;
            combo = 0;
            curInterval = 0;
        }

        public override void SetKeyCode(params KeyCode[] keyCodes)
        {
            keys.Clear();
            for (int i = 0; i < keyCodes.Length; i++)
            {
                keys.Add(keyCodes[i]);
            }
        }

        public override void Update()
        {
            if (!enable || keys == null || keys.Count <= 0 || interval <= 0)
                return;
            isTrigger = false;
            curInterval += Time.deltaTime;
            if (curInterval <= interval)
            {
                if (Input.GetKeyDown(keys[combo]))
                {
                    curInterval = 0;
                    combo++;
                    if (combo == keys.Count)
                    {
                        isTrigger = true;
                        combo = 0;
                    }
                }
                else
                {
                    combo = 0;
                    curInterval = 0;
                }
            }
            else
            {
                combo = 0;
                curInterval = 0;
            }
        }
    }

    #endregion

    #region AxisKey,一维键/二维键；滚轮/方向键等

    public class AxisKey : VirtualKey
    {
        //维度
        public enum AxisKeyDimension
        {
            AXIS_1D, //一维
            AXIS_2D, //二维
        }

        //变化类型
        public enum AxisKeyType
        {
            GRADUAL, //渐变
            SUDDEN, //立即变化
        }

        public class Axis1D
        {
            public KeyCode posKey, negKey;
            public Vector2 range = new Vector2(-1, 1);
            public float start = 0;

            //(正半轴朝最大值变化速度，正半轴朝最小值变化速度)
            //(负半轴朝最小值变化速度，负半轴朝最大值变化速度)
            //这里均为正值
            public Vector2 posSpeed = Vector2.one * 5f;
            public Vector2 negSpeed = Vector2.one * 5f;
            public float value;

            public void Reset()
            {
                value = start;
            }

            /// <summary>矫正value值</summary>
            public void ClampValue(float min, float max)
            {
                value = Mathf.Clamp(value, min, max);
            }

            public bool Check()
            {
                return posSpeed.x > 0 && posSpeed.y > 0 && negSpeed.x > 0 && negSpeed.y > 0
                       && range.x < start && range.y > start;
            }
        }

        public AxisKeyDimension dim = AxisKeyDimension.AXIS_1D;
        public AxisKeyType type = AxisKeyType.GRADUAL;

        //一维------------------
        public Axis1D dir = new Axis1D();

        //二维-------------------
        public Axis1D hor = new Axis1D();
        public Axis1D ver = new Axis1D();

        public float GetValue1D => dir.value;
        public Vector2 GetValue2D => new Vector2(hor.value, ver.value);


        public override void Init()
        {
            base.Init();
            dir.Reset();
            hor.Reset();
            ver.Reset();
        }

        public override void SetKeyCode(params KeyCode[] keyCodes)
        {
            if (keyCodes.Length >= 4)
            {
                hor.posKey = keyCodes[0];
                hor.negKey = keyCodes[1];
                ver.posKey = keyCodes[2];
                ver.negKey = keyCodes[3];
            }
            else if (keyCodes.Length >= 2)
            {
                dir.posKey = keyCodes[0];
                dir.negKey = keyCodes[1];
            }
        }

        public override void Update()
        {
            if (!Check())
                return;
            switch (dim)
            {
                case AxisKeyDimension.AXIS_1D:
                    switch (type)
                    {
                        case AxisKeyType.SUDDEN:
                            SetSuddenValue(ref dir);
                            break;
                        case AxisKeyType.GRADUAL:
                            SetGradualValue(ref dir);
                            break;
                    }

                    break;
                case AxisKeyDimension.AXIS_2D:
                    switch (type)
                    {
                        case AxisKeyType.SUDDEN:
                            SetSuddenValue(ref hor);
                            SetSuddenValue(ref ver);
                            break;
                        case AxisKeyType.GRADUAL:
                            SetGradualValue(ref hor);
                            SetGradualValue(ref ver);
                            break;
                    }

                    break;
            }
        }

        private bool Check()
        {
            if (dim == AxisKeyDimension.AXIS_1D)
            {
                return dir.Check();
            }
            else if (dim == AxisKeyDimension.AXIS_2D)
            {
                return hor.Check() && ver.Check();
            }

            return false;
        }

        private void SetSuddenValue(ref Axis1D axis1D)
        {
            if (enable && UnityEngine.Input.GetKey(axis1D.posKey))
            {
                axis1D.value = axis1D.range.y;
            }
            else if (enable && UnityEngine.Input.GetKey(axis1D.negKey))
            {
                axis1D.value = axis1D.range.x;
            }
            else
            {
                axis1D.value = axis1D.start;
            }
        }

        private void SetGradualValue(ref Axis1D axis1D)
        {
            if (enable && UnityEngine.Input.GetKey(axis1D.posKey))
            {
                // if (axis1D.value >= axis1D.start)
                // {
                //     axis1D.value += axis1D.posSpeed.x * Time.deltaTime;
                //     axis1D.ClampValue(axis1D.range.x, axis1D.range.y);
                // }
                // else
                // {
                //     axis1D.value += axis1D.negSpeed.y * Time.deltaTime;
                // }
                if (axis1D.value < axis1D.start)
                {
                    axis1D.value = axis1D.start;
                }

                axis1D.value += axis1D.posSpeed.x * Time.deltaTime;
                axis1D.ClampValue(axis1D.range.x, axis1D.range.y);
            }
            else if (enable && UnityEngine.Input.GetKey(axis1D.negKey))
            {
                // if (axis1D.value > axis1D.start)
                // {
                //     axis1D.value -= axis1D.posSpeed.y * Time.deltaTime;
                // }
                // else
                // {
                //     axis1D.value -= axis1D.negSpeed.x * Time.deltaTime;
                //     axis1D.ClampValue(axis1D.range.x, axis1D.range.y);
                // }
                if (axis1D.value > axis1D.start)
                {
                    axis1D.value = axis1D.start;
                }
                axis1D.value -= axis1D.negSpeed.x * Time.deltaTime;
                axis1D.ClampValue(axis1D.range.x, axis1D.range.y);
            }
            else
            {
                if (axis1D.value > axis1D.start)
                {
                    axis1D.value -= axis1D.posSpeed.y * Time.deltaTime;
                    axis1D.ClampValue(axis1D.start, axis1D.range.y);
                }
                else if (axis1D.value < axis1D.start)
                {
                    axis1D.value += axis1D.negSpeed.y * Time.deltaTime;
                    axis1D.ClampValue(axis1D.range.x, axis1D.start);
                }
            }
        }
    }

    #endregion
}