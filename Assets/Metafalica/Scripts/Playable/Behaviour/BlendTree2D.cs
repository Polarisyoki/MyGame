using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Metafalica.RPG.AnimationSys
{
    public struct BlendClip2D
    {
        public AnimationClip Clip;
        public Vector2 Pos;
    }

    public class BlendTree2D : AnimBehaviour
    {
        private struct DataPair
        {
            public float x;
            public float y;
            public float output;
        }

        private AnimationMixerPlayable _mixer;
        private Vector2 _pointer;
        private float _total;
        private int _clipCount;

        private ComputeShader _computeShader;
        private ComputeBuffer _computeBuffer;
        private DataPair[] _dataPair;
        private int _kernel;
        private int _pointerX;
        private int _pointerY;

        public BlendTree2D(PlayableGraph graph, BlendClip2D[] clips, float enterTime = 0, float eps = 1e-5f) :
            base(graph, enterTime)
        {
            _dataPair = new DataPair[clips.Length];
            _mixer = AnimationMixerPlayable.Create(graph);
            m_AdapterPlayable.AddInput(_mixer, 0, 1);
            for (int i = 0; i < clips.Length; i++)
            {
                _mixer.AddInput(AnimationClipPlayable.Create(graph, clips[i].Clip), 0);
                _dataPair[i].x = clips[i].Pos.x;
                _dataPair[i].y = clips[i].Pos.y;
            }

            _computeShader = AnimHelper.GetComputeShader("Blend2D");
            _computeBuffer = new ComputeBuffer(clips.Length, 12);
            _kernel = _computeShader.FindKernel("Compute");
            _computeShader.SetBuffer(_kernel,"dataBuffer",_computeBuffer);
            _computeShader.SetFloat("eps",eps);
            _pointerX = Shader.PropertyToID("pointerX");
            _pointerY = Shader.PropertyToID("pointerY");
            _clipCount = clips.Length;
            _pointer.Set(1,1);
            SetPointer(0,0);
            
            SetDisable();
        }

        
        public void SetPointer(Vector2 vector)
        {
            SetPointer(vector.x, vector.y);
        }
        public void SetPointer(float x, float y)
        {
            if(_pointer.x == x && _pointer.y == y)
            {
                return;
            }
            _pointer.Set(x, y);

            int i;
            _computeShader.SetFloat(_pointerX, x);
            _computeShader.SetFloat(_pointerY, y);

            _computeBuffer.SetData(_dataPair);
            _computeShader.Dispatch(_kernel, _clipCount, 1, 1);
            _computeBuffer.GetData(_dataPair);
            for (i = 0; i < _clipCount; i++)
            {
                _total += _dataPair[i].output;
            }
            for (i = 0; i < _clipCount; i++)
            {
                _mixer.SetInputWeight(i, _dataPair[i].output / _total);
            }
            _total = 0f;
        }

        public override void SetEnable()
        {
            base.SetEnable();
            
            SetPointer(0,0);
            for (int i = 0; i < _clipCount; i++)
            {
                _mixer.GetInput(i).Play();
                _mixer.GetInput(i).SetTime(0);
            }
            _mixer.SetTime(0);
            _mixer.Play();
            m_AdapterPlayable.SetTime(0);
            m_AdapterPlayable.Play();
            // m_AnimLength = ((AnimationClipPlayable)_mixer.GetInput(0)).GetAnimationClip().length;
        }

        public override void SetDisable()
        {
            base.SetDisable();
            for (int i = 0; i < _clipCount; i++)
            {
                _mixer.GetInput(i).Pause();
            }
            _mixer.Pause();
            m_AdapterPlayable.Pause();
        }

        public override void Stop()
        {
            base.Stop();
            _computeBuffer.Dispose();
        }
    }
}