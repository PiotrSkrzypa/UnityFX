using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class MaterialPropertyBlockTweenAnimation : BaseFXComponent
    {
        [SerializeField] private List<Renderer> meshRenderers;
        [SerializeField][SerializeReference] private List<MaterialPropertyBlockParameter> parametersToAnimate;
        Dictionary<Renderer, MaterialPropertyBlock> rendererPropertyBlockPairs;
        Dictionary<MaterialPropertyBlock, List<MaterialPropertyBlockParameter>> propertyBlockParametersPairs;

        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void Update(float progress)
        {
            if (progress == 0f)
            {
                InitializeParameters();
            }
            UpdateParameters(progress);
            ApplyPropertyBlocks();
        }

        private void InitializeParameters()
        {
            rendererPropertyBlockPairs = new Dictionary<Renderer, MaterialPropertyBlock>();
            propertyBlockParametersPairs = new Dictionary<MaterialPropertyBlock, List<MaterialPropertyBlockParameter>>();
            foreach (var renderer in meshRenderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                Renderer rendererTmp = renderer;
                renderer.GetPropertyBlock(block);
                rendererPropertyBlockPairs.Add(rendererTmp, block);
                propertyBlockParametersPairs.Add(block, new List<MaterialPropertyBlockParameter>());
                foreach (var param in parametersToAnimate)
                {
                    MaterialPropertyBlockParameter copyParam = param.GetCopy();
                    propertyBlockParametersPairs[block].Add(copyParam);
                    copyParam.GetOriginalValue(rendererTmp, block);
                }
            }
        }
        private void UpdateParameters(float progress)
        {
            foreach(var item in propertyBlockParametersPairs)
            {
                foreach( var param in item.Value)
                {
                    param.Update(item.Key, progress);
                }
            }
        }
        private void ApplyPropertyBlocks()
        {
            foreach (var item in rendererPropertyBlockPairs) 
            {
                item.Key.SetPropertyBlock(item.Value);
            }
        }
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
            protected int parameterNameID;
            public abstract void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock);
            public abstract void Update(MaterialPropertyBlock block, float progress);
            public abstract MaterialPropertyBlockParameter GetCopy();
        }

        [Serializable]
        public class ColorParameter : MaterialPropertyBlockParameter
        {
            [ColorUsage(true, true)] public Color targetColor;
            [ColorUsage(true, true)] Color originalColor;

            public override void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                parameterNameID = Shader.PropertyToID(parameterName);
                originalColor = renderer.sharedMaterial.GetColor(parameterName);
                materialPropertyBlock.SetColor(parameterNameID, originalColor);
            }
            public override void Update(MaterialPropertyBlock block, float progress)
            {
                block.SetColor(parameterNameID, Color.LerpUnclamped(originalColor, targetColor, progress));
            }

            public override MaterialPropertyBlockParameter GetCopy()
            {
                ColorParameter copy = new ColorParameter();
                copy.parameterName = parameterName;
                copy.targetColor = targetColor;
                return copy;
            }

        }

        [Serializable]
        public class FloatParameter : MaterialPropertyBlockParameter
        {
            public float targetValue;
            float originalValue;

            public override void Update(MaterialPropertyBlock block, float progress)
            {
                block.SetFloat(parameterNameID, Mathf.LerpUnclamped(originalValue, targetValue, progress));
            }
            public override void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                parameterNameID = Shader.PropertyToID(parameterName);
                originalValue = renderer.sharedMaterial.GetFloat(parameterName);
                materialPropertyBlock.SetFloat(parameterNameID, originalValue);
            }
            public override MaterialPropertyBlockParameter GetCopy()
            {
                FloatParameter copy = new FloatParameter();
                copy.parameterName = parameterName;
                copy.targetValue = targetValue;
                return copy;
            }
        }

        [Serializable]
        public class IntegerParameter : MaterialPropertyBlockParameter
        {
            public int targetValue;
            int originalValue;

            public override void Update(MaterialPropertyBlock block, float progress)
            {
                block.SetInteger(parameterNameID, Mathf.RoundToInt(Mathf.LerpUnclamped(originalValue, targetValue, progress)));
            }
            public override void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                parameterNameID = Shader.PropertyToID(parameterName);
                originalValue = renderer.sharedMaterial.GetInteger(parameterName);
                materialPropertyBlock.SetInteger(parameterNameID, originalValue);
            }
            public override MaterialPropertyBlockParameter GetCopy()
            {
                IntegerParameter copy = new IntegerParameter();
                copy.parameterName = parameterName;
                copy.targetValue= targetValue;
                return copy;
            }
        }

        [Serializable]
        public class Vector2Parameter : MaterialPropertyBlockParameter
        {
            public Vector2 targetValue;
            Vector2 originalValue;

            public override void Update(MaterialPropertyBlock block, float progress)
            {
                block.SetVector(parameterNameID, Vector2.LerpUnclamped(originalValue, targetValue, progress));
            }
            public override void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                parameterNameID = Shader.PropertyToID(parameterName);
                originalValue = renderer.sharedMaterial.GetVector(parameterName);
                materialPropertyBlock.SetVector(parameterNameID, originalValue);
            }
            public override MaterialPropertyBlockParameter GetCopy()
            {
                Vector2Parameter copy = new Vector2Parameter();
                copy.parameterName = parameterName;
                copy.targetValue = targetValue;
                return copy;
            }
        }

        [Serializable]
        public class Vector3Parameter : MaterialPropertyBlockParameter
        {
            public Vector3 targetValue;
            Vector3 originalValue;

            public override void Update(MaterialPropertyBlock block, float progress)
            {
                block.SetVector(parameterNameID, Vector3.LerpUnclamped(originalValue, targetValue, progress));
            }
            public override void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                parameterNameID = Shader.PropertyToID(parameterName);
                originalValue = renderer.sharedMaterial.GetVector(parameterName);
                materialPropertyBlock.SetVector(parameterNameID, originalValue);
            }
            public override MaterialPropertyBlockParameter GetCopy()
            {
                Vector3Parameter copy = new Vector3Parameter();
                copy.parameterName = parameterName;
                copy.targetValue = targetValue;
                return copy;
            }
        }

        [Serializable]
        public class Vector4Parameter : MaterialPropertyBlockParameter
        {
            public Vector4 targetValue;
            Vector4 originalValue;

            public override void Update(MaterialPropertyBlock block, float progress)
            {
                block.SetVector(parameterNameID, Vector4.LerpUnclamped(originalValue, targetValue, progress));
            }
            public override void GetOriginalValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                parameterNameID = Shader.PropertyToID(parameterName);
                originalValue = renderer.sharedMaterial.GetVector(parameterName);
                materialPropertyBlock.SetVector(parameterNameID, originalValue);
            }
            public override MaterialPropertyBlockParameter GetCopy()
            {
                Vector4Parameter copy = new Vector4Parameter();
                copy.parameterName = parameterName;
                copy.targetValue = targetValue;
                return copy;
            }
        }

        #endregion
    }
}
