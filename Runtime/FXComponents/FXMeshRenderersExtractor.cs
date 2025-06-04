using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXMeshRenderersExtractor : BaseFXComponent
    {
        [SerializeField] Vector3 volumeSize = new Vector3(1f, 1f, 1f);
        [SerializeField] Vector3 volumeCenterOffset = new Vector3(1f, 1f, 1f);
        [SerializeField] LayerMask layerMask = -1;
        [SerializeField] Transform volumeCenter;
        List<Renderer> foundRenderers;

        protected override void Update(float progress)
        {
            if (progress == 0)
            {
                foundRenderers = new List<Renderer>();
                Collider[] colliders = Physics.OverlapBox(volumeCenter.position + volumeCenterOffset, volumeSize / 2f, Quaternion.identity, layerMask);
                foreach (Collider collider in colliders)
                {
                    List<Renderer> renderers = collider.GetComponentsInChildren<Renderer>().Where(x=>x is MeshRenderer || x is SkinnedMeshRenderer).ToList();
                    if (renderers != null)
                    {
                        if (renderers.Count == 0)
                        {
                            renderers = collider.GetComponentsInChildren<Renderer>()?.Where(x => x is MeshRenderer || x is SkinnedMeshRenderer).ToList();
                        }
                        foundRenderers.AddRange(renderers);
                    }
                }
                FXPlayer fXObject = volumeCenter.GetComponent<FXPlayer>();
                if (fXObject == null)
                {
                    return;
                }
                IFXComponent[] fxComponents = fXObject.Sequence.Components;
                for (int i = 0; i < fxComponents.Length; i++)
                {
                    IFXComponent fXComponent = fxComponents[i];
                    if (fXComponent is MaterialPropertyBlockTweenAnimation)
                    {
                        ( (MaterialPropertyBlockTweenAnimation)fXComponent ).InjectRenderersList(foundRenderers);
                    }
                }
            }
        }

        public List<Renderer> GetFoundRenderers()
        {
            List <Renderer> result = new List<Renderer>(foundRenderers);
            return result;
        }
    }
}