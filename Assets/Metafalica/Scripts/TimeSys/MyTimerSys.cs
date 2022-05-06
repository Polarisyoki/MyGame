using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PETimeTask
    {
        public int m_Tid;
        public Action m_CallBack;
        public float m_Delay; //毫秒

        public float m_DesTime; //毫秒

        //执行次数,0代表无限循环
        public int m_Count;

        public PETimeTask(int tid, Action callBack, float delay, float desTime, int count)
        {
            this.m_Tid = tid;
            this.m_CallBack = callBack;
            this.m_Delay = delay;
            this.m_DesTime = desTime;
            this.m_Count = count;
        }
    }

    public class PEFrameTask
    {
        public int m_Tid;
        public Action m_CallBack;
        public int m_Delay; //帧

        public int m_DesFrame; //帧

        //执行次数,0代表无限循环
        public int m_Count;

        public PEFrameTask(int tid, Action callBack, int delay, int desFrame, int count)
        {
            this.m_Tid = tid;
            this.m_CallBack = callBack;
            this.m_Delay = delay;
            this.m_DesFrame = desFrame;
            this.m_Count = count;
        }
    }

    public enum PETimeUnit
    {
        Millsecond,
        Second,
        Minute,
        Hour,
        Day,
    }

    public class MyTimerSys : Singleton<MyTimerSys>
    {
        private static readonly string obj = "lock";
        private int tid;

        private List<int> _tidList = new List<int>();
        private List<int> _recycleTidList = new List<int>();
        private List<PETimeTask> _tempTaskTimeList = new List<PETimeTask>();
        private List<PETimeTask> _taskTimeList = new List<PETimeTask>();

        private int frameCounter;
        private List<PEFrameTask> _tempTaskFrameList = new List<PEFrameTask>();
        private List<PEFrameTask> _taskFrameList = new List<PEFrameTask>();


        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            m_Mono.StartCoroutine(OnUpdate());

            tid = 0;
        }

        private IEnumerator OnUpdate()
        {
            while (true)
            {
                CheckTimeTask();
                CheckFrameTask();
                if (_recycleTidList.Count > 0)
                    RecycleTid();
                yield return null;
                
                frameCounter++;
                if (frameCounter > 100000000)
                {
                    ResetFrameCounter();
                }
            }
        }


        #region TimeTask

        /// <summary>
        /// 添加定时任务
        /// </summary>
        /// <param name="callback">回调</param>
        /// <param name="delay">定时时间</param>
        /// <param name="timeUnit">时间单位</param>
        /// <param name="count">任务次数，0为循环</param>
        public int AddTimeTask(Action callback, float delay, PETimeUnit timeUnit = PETimeUnit.Millsecond,
            int count = 1)
        {
            if (timeUnit != PETimeUnit.Millsecond)
            {
                switch (timeUnit)
                {
                    case PETimeUnit.Second:
                        delay = delay * 1000;
                        break;
                    case PETimeUnit.Minute:
                        delay = delay * 1000 * 60;
                        break;
                    case PETimeUnit.Hour:
                        delay = delay * 1000 * 60 * 60;
                        break;
                    case PETimeUnit.Day:
                        delay = delay * 1000 * 60 * 60 * 24;
                        break;
                    default:
                        break;
                }
            }

            float desTime = Time.realtimeSinceStartup * 1000 + delay;
            int tid = GetTid();
            _tempTaskTimeList.Add(new PETimeTask(tid, callback, delay, desTime, count));
            _tidList.Add(tid);
            return tid;
        }

        public bool DeleteTimeTask(int tid)
        {
            var temp1 = _taskTimeList.Find(c => c.m_Tid == tid);
            if (temp1 != null)
            {
                _tidList.Remove(temp1.m_Tid);
                _taskTimeList.Remove(temp1);
                return true;
            }

            var temp2 = _tempTaskTimeList.Find(c => c.m_Tid == tid);
            if (temp2 != null)
            {
                _tidList.Remove(temp2.m_Tid);
                _tempTaskTimeList.Remove(temp2);
                return true;
            }

            return false;
        }

        public void ReplaceOrAddTimeTask(int tid, Action callback, float delay, PETimeUnit timeUnit = PETimeUnit.Millsecond,
            int count = 1)
        {
            if (timeUnit != PETimeUnit.Millsecond)
            {
                switch (timeUnit)
                {
                    case PETimeUnit.Second:
                        delay = delay * 1000;
                        break;
                    case PETimeUnit.Minute:
                        delay = delay * 1000 * 60;
                        break;
                    case PETimeUnit.Hour:
                        delay = delay * 1000 * 60 * 60;
                        break;
                    case PETimeUnit.Day:
                        delay = delay * 1000 * 60 * 60 * 24;
                        break;
                    default:
                        break;
                }
            }

            float desTime = Time.realtimeSinceStartup * 1000 + delay;
            var task = new PETimeTask(tid, callback, delay, desTime, count);

            if (_taskTimeList.Find(c => c.m_Tid == tid) != null)
            {
                int index = _taskTimeList.FindIndex(c => c.m_Tid == tid);
                _taskTimeList[index] = task;
            }
            else
            {
                if (_tempTaskTimeList.Find(c => c.m_Tid == tid) != null)
                {
                    int index = _tempTaskTimeList.FindIndex(c => c.m_Tid == tid);
                    _tempTaskTimeList[index] = task;
                }
                else
                {
                    //两个列表中都没有该tid的任务则添加该任务
                    _tempTaskTimeList.Add(task);
                    _tidList.Add(tid);
                }
            }
        }

        void CheckTimeTask()
        {
            for (int i = 0; i < _tempTaskTimeList.Count; i++)
            {
                _taskTimeList.Add(_tempTaskTimeList[i]);
            }

            _tempTaskTimeList.Clear();
            for (int i = 0; i < _taskTimeList.Count; i++)
            {
                if (Time.realtimeSinceStartup * 1000 < _taskTimeList[i].m_DesTime)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        _taskTimeList[i].m_CallBack?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    if (_taskTimeList[i].m_Count == 1)
                    {
                        _recycleTidList.Add(_taskTimeList[i].m_Tid);
                        _taskTimeList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        _taskTimeList[i].m_DesTime += _taskTimeList[i].m_Delay;
                        if (_taskTimeList[i].m_Count > 1)
                        {
                            _taskTimeList[i].m_Count -= 1;
                        }
                    }
                }
            }
        }

        #endregion

        #region FrameTask

        /// <summary>
        /// 添加帧定时任务
        /// </summary>
        /// <param name="callback">回调</param>
        /// <param name="delay">延迟帧数</param>
        /// <param name="count">任务次数，0为循环</param>
        public int AddFrameTask(Action callback, int delay, int count = 1)
        {
            int desFrame = frameCounter + delay;
            int tid = GetTid();
            _tempTaskFrameList.Add(new PEFrameTask(tid, callback, delay, desFrame, count));
            _tidList.Add(tid);
            return tid;
        }

        public bool DeleteFrameTask(int tid)
        {
            var temp1 = _taskFrameList.Find(c => c.m_Tid == tid);
            if (temp1 != null)
            {
                _tidList.Remove(temp1.m_Tid);
                _taskFrameList.Remove(temp1);
                return true;
            }

            var temp2 = _tempTaskFrameList.Find(c => c.m_Tid == tid);
            if (temp2 != null)
            {
                _tidList.Remove(temp2.m_Tid);
                _tempTaskFrameList.Remove(temp2);
                return true;
            }

            return false;
        }

        public void ReplaceOrAddFrameTask(int tid, Action callback, int delay, int count = 1)
        {
            int desFrame = frameCounter + delay;
            var task = new PEFrameTask(tid, callback, delay, desFrame, count);

            if (_taskFrameList.Find(c => c.m_Tid == tid) != null)
            {
                int index = _taskFrameList.FindIndex(c => c.m_Tid == tid);
                _taskFrameList[index] = task;
            }
            else
            {
                if (_tempTaskFrameList.Find(c => c.m_Tid == tid) != null)
                {
                    int index = _tempTaskFrameList.FindIndex(c => c.m_Tid == tid);
                    _tempTaskFrameList[index] = task;
                }
                else
                {
                    //两个列表中都没有该tid的任务则添加该任务
                    _tempTaskFrameList.Add(task);
                    _tidList.Add(tid);
                }
            }
        }

        void CheckFrameTask()
        {
            for (int i = 0; i < _tempTaskFrameList.Count; i++)
            {
                _taskFrameList.Add(_tempTaskFrameList[i]);
            }

            _tempTaskFrameList.Clear();
            for (int i = 0; i < _taskFrameList.Count; i++)
            {
                if (frameCounter < _taskFrameList[i].m_DesFrame)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        _taskFrameList[i].m_CallBack?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    if (_taskFrameList[i].m_Count == 1)
                    {
                        _recycleTidList.Add(_taskFrameList[i].m_Tid);
                        _taskFrameList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        _taskFrameList[i].m_DesFrame += _taskFrameList[i].m_Delay;
                        if (_taskFrameList[i].m_Count > 1)
                        {
                            _taskFrameList[i].m_Count -= 1;
                        }
                    }
                }
            }
        }

        #endregion

        private int GetTid()
        {
            lock (obj)
            {
                tid += 1;

                while (true)
                {
                    if (tid == int.MaxValue)
                        tid = 0;
                    if (!_tidList.Contains(tid))
                    {
                        break;
                    }
                    else
                    {
                        tid += 1;
                    }
                }
            }

            return tid;
        }

        private void RecycleTid()
        {
            for (int i = 0; i < _recycleTidList.Count; i++)
            {
                _tidList.Remove(_recycleTidList[i]);
            }

            _recycleTidList.Clear();
        }

        private void ResetFrameCounter()
        {
            for (int i = 0; i < _taskFrameList.Count; i++)
            {
                _taskFrameList[i].m_DesFrame -= frameCounter;
            }
            for (int i = 0; i < _tempTaskFrameList.Count; i++)
            {
                _tempTaskFrameList[i].m_DesFrame -= frameCounter;
            }
            frameCounter = 0;
        }
    }
}