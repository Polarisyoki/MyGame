using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Metafalica.RPG.AnimationSys
{
    public static class AnimHelper
    {
        public static void SetOutput(PlayableGraph graph, Animator animator, AnimBehaviour behaviour)
        {
            
        }

        public static void Start(PlayableGraph graph)
        {
            GetAdapter(graph.GetOutputByType<AnimationPlayableOutput>(0).GetSourcePlayable()).SetEnable();
            graph.Play();
        }

        public static void Start(PlayableGraph graph, AnimBehaviour root)
        {
            root.SetEnable();
            graph.Play();
        }

        public static void SetEnable(Playable playable)
        {
            GetAdapter(playable)?.SetEnable();
        }

        public static void SetDisable(Playable playable)
        {
            GetAdapter(playable)?.SetDisable();
        }

        public static AnimAdpter GetAdapter(Playable playable)
        {
            if (typeof(AnimAdpter).IsAssignableFrom(playable.GetPlayableType()))
            {
                return ((ScriptPlayable<AnimAdpter>) playable).GetBehaviour();
            }

            return null;
        }

        public static ComputeShader GetComputeShader(string name)
        {
            var shader = ResourcesManager.Instance.LoadResource<ComputeShader>
                (GamePath.Res_Folder + "shader/" + name + ".compute");
            return Object.Instantiate(shader);
        }
    }
}