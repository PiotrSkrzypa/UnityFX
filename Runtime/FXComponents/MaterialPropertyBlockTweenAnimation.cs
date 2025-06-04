using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class MaterialPropertyBlockTweenAnimation : BaseFXComponent
    {
        [SerializeField] private List<Renderer> meshRenderers;
        [SerializeField][SerializeReference] private List<MaterialPropertyBlockParameter> parametersToAnimate;

        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void Update(float progress)
        {
            
        }

        //protected override async UniTask PlayInternal(CancellationToken cancellationToken, PlaybackSpeed playbackSpeed)
        //{
        //    if (meshRenderers == null || parametersToAnimate == null) return;

        //    List<UniTask> allTasks = new();

        //    float calculatedDuration = GetCalculatedDuration(playbackSpeed);
        //    float from = playbackSpeed.speed > 0 ? 0f : 1f;
        //    float to = playbackSpeed.speed > 0 ? 1f : 0f;
        //    foreach (var renderer in meshRenderers)
        //    {
        //        MaterialPropertyBlock block = new MaterialPropertyBlock();
        //        Renderer rendererTmp = renderer;
        //        renderer.GetPropertyBlock(block);

        //        var sequence = LSequence.Create();
        //        foreach (var param in parametersToAnimate)
        //        {
        //            param.SetDefaultValue(rendererTmp, block);
        //            sequence.Join(param.CreateMotion(block, calculatedDuration, Timing.TimeScaleIndependent));
        //        }
        //        sequence.Append(LMotion.Create(from, to, calculatedDuration)
        //            .WithScheduler(Timing.GetScheduler())
        //            .WithEase(Ease.OutQuad)
        //            .Bind(block, (t, block) =>
        //            {
        //                progress = t;
        //                rendererTmp.SetPropertyBlock(block);
        //            }));
        //        allTasks.Add(sequence.Run().ToUniTask(cancellationToken));
        //    }
        //    await UniTask.WhenAll(allTasks);
        //}
        //protected override async UniTask Rewind(PlaybackSpeed playbackSpeed)
        //{
        //    await UniTask.CompletedTask;
        //}
        protected override void StopInternal()
        {
            foreach (var renderer in meshRenderers)
            {
                MaterialPropertyBlock block = new();
                renderer.SetPropertyBlock(block);
            }
        }

        protected override void ResetInternal()
        {
            foreach (var renderer in meshRenderers)
            {
                MaterialPropertyBlock block = new();
                renderer.SetPropertyBlock(block);
            }
        }

        public void InjectRenderersList(List<Renderer> renderers)
        {
            this.meshRenderers = renderers;
        }

        #region Parameter Definitions

        [Serializable]
        public abstract class MaterialPropertyBlockParameter
        {
            public string parameterName;
            public abstract MotionHandle CreateMotion(MaterialPropertyBlock block, float duration, bool timeScaleIndependent);
            public abstract void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock);
        }

        [Serializable]
        public class ColorParameter : MaterialPropertyBlockParameter
        {
            [ColorUsage(true, true)] public Color targetColor;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
                bool timeScaleIndependent)
            {
                Color startColor = block.GetColor(parameterName);
                Color endColor = targetColor;

                var handle = LMotion.Create(startColor,endColor,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetColor(parameterName, x));
                return handle;
            }

            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetColor(parameterName, renderer.sharedMaterial.GetColor(parameterName));
            }
        }

        [Serializable]
        public class FloatParameter : MaterialPropertyBlockParameter
        {
            public float targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
                bool timeScaleIndependent)
            {
                float startValue = block.GetFloat(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetFloat(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetFloat(parameterName, renderer.sharedMaterial.GetFloat(parameterName));
            }
        }

        [Serializable]
        public class IntegerParameter : MaterialPropertyBlockParameter
        {
            public int targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
                bool timeScaleIndependent)
            {
                int startValue = block.GetInteger(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetInteger(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetInteger(parameterName, renderer.sharedMaterial.GetInteger(parameterName));
            }
        }

        [Serializable]
        public class Vector2Parameter : MaterialPropertyBlockParameter
        {
            public Vector2 targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
               bool timeScaleIndependent)
            {
                Vector2 startValue = block.GetVector(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetVector(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetVector(parameterName, renderer.sharedMaterial.GetVector(parameterName));
            }
        }

        [Serializable]
        public class Vector3Parameter : MaterialPropertyBlockParameter
        {
            public Vector3 targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
               bool timeScaleIndependent)
            {
                Vector3 startValue = block.GetVector(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetVector(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetVector(parameterName, renderer.sharedMaterial.GetVector(parameterName));
            }
        }

        [Serializable]
        public class Vector4Parameter : MaterialPropertyBlockParameter
        {
            public Vector4 targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
               bool timeScaleIndependent)
            {
                Vector4 startValue = block.GetVector(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetVector(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetVector(parameterName, renderer.sharedMaterial.GetVector(parameterName));
            }
        }

        #endregion
    }
}
