using UnityEngine.Playables;

namespace Metafalica.RPG.AnimationSys
{
    public class Root: AnimBehaviour
    {
        public Root(PlayableGraph graph) : base(graph)
        {
            
        }

        public override void SetEnable()
        {
            base.SetEnable();
            for (int i = 0; i < m_AdapterPlayable.GetInputCount(); i++)
            {
                AnimHelper.SetEnable(m_AdapterPlayable.GetInput(i));
            }
            m_AdapterPlayable.SetTime(0);
            m_AdapterPlayable.Play();
        }

        public override void SetDisable()
        {
            base.SetDisable();
            for (int i = 0; i < m_AdapterPlayable.GetInputCount(); i++)
            {
                AnimHelper.SetDisable(m_AdapterPlayable.GetInput(i));
            }
            m_AdapterPlayable.Pause();
        }

        public override void AddInput(Playable playable)
        {
            m_AdapterPlayable.AddInput(playable, 0, 1);
        }
    }
}