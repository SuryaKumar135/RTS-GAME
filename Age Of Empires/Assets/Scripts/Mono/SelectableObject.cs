using UnityEngine;

namespace RTSGame
{
    public class SelectableObject : MonoBehaviour
    {
        [Header("Selection Visuals")]
        public GameObject selectEffectPrefab;

        public ParticleSystem selectEffectInstance;

        protected virtual void Awake()
        {
            if (selectEffectPrefab != null)
            {
                GameObject effectObject = Instantiate(selectEffectPrefab, transform);
                effectObject.transform.localPosition = Vector3.zero;

                selectEffectInstance = effectObject.GetComponent<ParticleSystem>();

                if (selectEffectInstance != null)
                    selectEffectInstance.Stop();
            }
            else
            {
                Debug.LogWarning($"No selectEffectPrefab assigned on {gameObject.name}");
            }
        }

        public virtual void OnSelected()
        {
            if (selectEffectInstance != null)
                selectEffectInstance.Play();
        }

        public virtual void OnDeselected()
        {
            if (selectEffectInstance != null)
            {
                Debug.LogWarning("Deselect");
                selectEffectInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

    }
}
