using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Metafalica.RPG.AnimationSys
{
    public class CustomAnimationcontroller
    {   
        private PlayableGraph _graph;
        private AnimationPlayableOutput _output;
        private AnimationLayerMixerPlayable _layerMixer;
        private AnimationMixerPlayable _mixer;

        public CustomAnimationcontroller(string animOutputName, Animator animator)
        {
            _graph = PlayableGraph.Create("CustomAnimationController");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _output = AnimationPlayableOutput.Create(_graph,animOutputName,animator);
            _layerMixer = AnimationLayerMixerPlayable.Create(_graph,0);
            _output.SetSourcePlayable(_layerMixer);
            Play();
        }
        
        public void AddInput(params AnimationClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return;
            _mixer = AnimationMixerPlayable.Create(_graph,clips.Length);
            for (int i = 0; i < clips.Length; i++)
            {
                _mixer.AddInput(AnimationClipPlayable.Create(_graph,clips[i]), 0,0);
            }
            
            _layerMixer.AddInput(_mixer, 0, 1);
        }

        public void SetRun(int index)
        {
            _mixer.SetInputWeight(0,1);
        }
        
        public void Play()
        {
            _graph.Play();

#if UNITY_EDITOR
            GraphVisualizerClient.Show(_graph);
#endif
        }
        
        public void OnDestroy()
        {
            _graph.Destroy();
        }
        
    }
}

