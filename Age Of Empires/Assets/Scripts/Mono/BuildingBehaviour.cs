using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSGame
{
    public class BuildingBehaviour : MonoBehaviour
    {
        [Header("Team")]
        public int teamID = 0;

        //[Header("Builder Placement")]
        //public float builderSpacing = 2f;
        private int builderIndex = 0;

        [Header("Data")]
        public BuildingData Data;

        [Header("Builder Info")]
        public List<UnitBehaviour> assignedBuilders = new List<UnitBehaviour>();

        private float buildProgress = 0f;
        private Coroutine constructionRoutine;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        [SerializeField] private Image timerImage;

        [Header("Builder Assign Angle")]
        [SerializeField] float startAngle = -90f, arcAngle = 180f;

        [Header("Particle")]
        [SerializeField] private ParticleSystem damageEffect;
        [SerializeField] private ParticleSystem destructionEffect;

        private void Awake()
        {
            if (Data?.CommonData != null)
                Data.CommonData.Health = Data.CommonData.MaxHealth;

            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            UpdateMesh();
                
        }

        public void AssignBuilder(UnitBehaviour builder)
        {
            if (Data.Status == BuildingStatus.Completed)
            {
                Debug.LogWarning($"[BuildingBehaviour] {name} is already completed.");
                return;
            }

            if (assignedBuilders.Contains(builder))
                return;

            if (assignedBuilders.Count < Data.MaxBuildersAllowed)
            {
                assignedBuilders.Add(builder);
                Debug.Log($"[BuildingBehaviour] Builder assigned: {builder.name}");

                if (Data.Status == BuildingStatus.Initial)
                    StartConstruction();
                else if (Data.Status == BuildingStatus.UnderConstruction || Data.Status == BuildingStatus.UnderRepair)
                    RestartConstruction();
                else if (Data.Status == BuildingStatus.Destroyed)
                    StartRepair();
            }
        }

        public void RemoveBuilder(UnitBehaviour builder)
        {
            if (assignedBuilders.Remove(builder) &&
                (Data.Status == BuildingStatus.UnderConstruction || Data.Status == BuildingStatus.UnderRepair))
            {
                RestartConstruction();
            }
        }

        private void RestartConstruction()
        {
            if (constructionRoutine != null)
                StopCoroutine(constructionRoutine);

            constructionRoutine = StartCoroutine(ConstructionCoroutine(buildProgress));
        }

        public void StartConstruction()
        {
            Data.Status = BuildingStatus.UnderConstruction;
            buildProgress = 0f;
            UpdateMesh();

            if (constructionRoutine != null)
                StopCoroutine(constructionRoutine);

            constructionRoutine = StartCoroutine(ConstructionCoroutine());
        }

        private void StartRepair()
        {
            // Reset visual & logic
            Data.Status = BuildingStatus.UnderRepair;
            buildProgress = 0f;
            UpdateMesh();

            //if (destructionEffect != null && destructionEffect.isPlaying)
            //    destructionEffect.Stop();

            if (constructionRoutine != null)
                StopCoroutine(constructionRoutine);

            constructionRoutine = StartCoroutine(ConstructionCoroutine());
        }

        private IEnumerator ConstructionCoroutine(float initialProgress = 0f)
        {
            buildProgress = initialProgress;

            while (buildProgress < 1f &&
                   (Data.Status == BuildingStatus.UnderConstruction || Data.Status == BuildingStatus.UnderRepair) &&
                   assignedBuilders.Count>0)
            {
                int builderCount = Mathf.Max(assignedBuilders.Count, Data.MinBuildersNeeded);
                float effectiveTime = Data.GetEffectiveBuildTime(builderCount);

                buildProgress += Time.deltaTime / effectiveTime;

                if (timerImage != null)
                    timerImage.fillAmount = buildProgress;

                yield return null;
            }

            buildProgress = 1f;
            Data.CommonData.Health = Data.CommonData.MaxHealth;

            foreach (var builder in assignedBuilders)
                builder.StopWorkAnimation();

            Data.Status = BuildingStatus.Completed;

            if (timerImage != null)
                timerImage.fillAmount = 0f;

            UpdateMesh();
        }

        public void TakeDamage(int damage)
        {
            if (Data?.CommonData == null || Data.Status == BuildingStatus.Destroyed)
                return;

            Data.CommonData.ModifyHealth(-damage);
            Debug.Log($"{name} took {damage} damage. Health: {Data.CommonData.Health}");

            if (damageEffect != null)
                damageEffect.Play();

            if (Data.CommonData.Health <= 0f)
            {
                Data.Status = BuildingStatus.Destroyed;
                PlayDestructionEffect();
                UpdateMesh();
            }
        }

        private void PlayDestructionEffect()
        {
            if (destructionEffect != null)
                destructionEffect.Play();
        }

        private void StopDestructionEffect()
        {
            if (destructionEffect != null && destructionEffect.isPlaying)
                destructionEffect.Stop();
        }



        public Vector3 GetNextBuilderPosition()
        {
            int maxSpots = Mathf.Max(Data.MaxBuildersAllowed, 1);
            float angleStep = arcAngle / (maxSpots - 1);
            float angle = startAngle + (builderIndex % maxSpots) * angleStep;
            builderIndex++;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * Data.builderSpacing;
            return transform.position + offset;
        }

        private void UpdateMesh()
        {
            if (meshFilter == null || Data == null) return;

            switch (Data.Status)
            {
                case BuildingStatus.Initial:
                    meshFilter.sharedMesh = Data.Initial;
                    StopDestructionEffect();
                    break;
                case BuildingStatus.UnderConstruction:
                    meshFilter.sharedMesh = Data.UnderConstructionMesh;
                    StopDestructionEffect();
                    break;
                case BuildingStatus.Completed:
                    meshFilter.sharedMesh = Data.CompletedMesh;
                    StopDestructionEffect();
                    break;
                case BuildingStatus.UnderRepair:
                    PlayDestructionEffect();
                    meshFilter.sharedMesh = Data.UnderRepairMesh;
                    break;
                case BuildingStatus.Destroyed:
                    PlayDestructionEffect();
                    meshFilter.sharedMesh = Data.CompletedMesh; // Optional: Use destroyed mesh if available
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Data == null) return;

            Gizmos.color = Color.green;
            int maxSpots = Mathf.Max(Data.MaxBuildersAllowed, 1);
            float angleStep = arcAngle / (maxSpots - 1);

            for (int i = 0; i < maxSpots; i++)
            {
                float angle = startAngle + i * angleStep;
                float rad = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * Data.builderSpacing;
                Gizmos.DrawSphere(transform.position + offset, 0.3f);
            }
        }
    }
}
