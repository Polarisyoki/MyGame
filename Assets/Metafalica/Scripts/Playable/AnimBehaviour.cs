using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Metafalica.RPG.AnimationSys
{
    public class AnimBehaviour
    {
        public bool Enable { get; private set; }
        public float RemainTime { get; private set; }

        protected Playable m_AdapterPlayable;
        protected float m_EnterTime;
        protected float m_AnimLength;

        public AnimBehaviour(float enterTime = 0)
        {
            this.m_EnterTime = enterTime;
        }

        public AnimBehaviour(PlayableGraph graph, float enterTime = 0)
        {
            m_AdapterPlayable = ScriptPlayable<AnimAdpter>.Create(graph);
            AnimHelper.GetAdapter(m_AdapterPlayable).Init(this);
            this.m_EnterTime = enterTime;
            this.m_AnimLength = float.NaN;
        }

        public virtual void SetEnable()
        {
            if (Enable) return;
            Enable = true;
            RemainTime = GetAnimLength();
        }

        public virtual void SetDisable()
        {
            if(!Enable) return;
            Enable = false;
        }

        public virtual void Stop()
        {
            
        }

        public virtual void Execute(Playable playable, FrameData info)
        {
            if (!Enable) return;
            RemainTime = RemainTime > 0 ? RemainTime - info.deltaTime : 0;
        }

        public virtual void AddInput(Playable playable)
        {
            
        }

        public void AddInput(AnimBehaviour behaviour)
        {
            AddInput(behaviour.GetAdapterPlayable());
        }

        public virtual Playable GetAdapterPlayable()
        {
            return m_AdapterPlayable;
        }
        public virtual float GetEnterTime()
        {
            return m_EnterTime;
        }
        public virtual float GetAnimLength()
        {
            return m_AnimLength;
        }
    }

}
