using UnityEngine.Playables;

namespace Metafalica.RPG.AnimationSys
{
    public class AnimAdpter : PlayableBehaviour
    {
        private AnimBehaviour _behaviour;

        public void Init(AnimBehaviour behaviour)
        {
            this._behaviour = behaviour;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            _behaviour?.Execute(playable,info);
        }

        public void SetEnable()
        {
            _behaviour?.SetEnable();
        }

        public void SetDisable()
        {
            _behaviour?.SetDisable();
        }

        public T GetAnimBehaviour<T>() where T : AnimBehaviour
        {
            return _behaviour as T;
        }

        public float GetAnimEnterTime()
        {
            return _behaviour.GetEnterTime();
        }

        public override void OnGraphStop(Playable playable)
        {
            _behaviour?.Stop();
        }
    }
}