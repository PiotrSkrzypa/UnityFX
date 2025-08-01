using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSkrzypa.UnityFX.V2
{
    [Serializable]
    [FXComponent("Rendering/Mesh Renderers Extractor")]
    public class FXMeshRenderersExtractor : FXComponent
    {
        [SerializeField] Vector3 volumeSize = new Vector3(1f, 1f, 1f);
        [SerializeField] Vector3 volumeCenterOffset = new Vector3(1f, 1f, 1f);
        [SerializeField] LayerMask layerMask = -1;
        [SerializeField] Transform volumeCenter;
        List<Renderer> foundRenderers;

        public override void Play(PlaybackDirection playbackDirection = PlaybackDirection.Forward, float playbackSpeedModifier = 1)
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
            FXSequencePlayer fXObject = volumeCenter.GetComponent<FXSequencePlayer>();
            if (fXObject == null)
            {
                return;
            }
            FXComponent[] fxComponents = fXObject.FXSequence.Components;
            for (int i = 0; i < fxComponents.Length; i++)
            {
                FXComponent fXComponent = fxComponents[i];
                //if (fXComponent is FXMaterialPropertyBlockAnimation)
                //{
                //    ( (FXMaterialPropertyBlockAnimation)fXComponent ).InjectRenderersList(foundRenderers);
                //}
            }
            callbackActions.OnCompleteAction?.Invoke();
        }

        public List<Renderer> GetFoundRenderers()
        {
            List <Renderer> result = new List<Renderer>(foundRenderers);
            return result;
        }
        public override void Rewind(float playbackSpeedModifier = 1)
        {
            callbackActions.OnRewindCompleteAction?.Invoke();
        }

    }
}