using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSGame
{
    public class BuildingBehaviour : MonoBehaviour
    {
        [Header("Data")]
        public BuildingData Data;

        [Header("Builder Info")]
        public List<UnitBehaviour> assignedBuilders = new List<UnitBehaviour>();

        private float buildProgress = 0f;
        private Coroutine constructionRoutine;

        // Mesh handling
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        [SerializeField] private Image timerImage;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            UpdateMesh();
        }

        public void AssignBuilder(UnitBehaviour builder)
        {
            if (assignedBuilders.Count < Data.MaxBuildersAllowed && !assignedBuilders.Contains(builder))
            {
                assignedBuilders.Add(builder);
                Debug.Log($"[BuildingBehaviour] Builder assigned. Total: {assignedBuilders.Count}");

                if (Data.Status == BuildingStatus.Initial)
                {
                    StartConstruction();
                }
                else if (Data.Status == BuildingStatus.UnderConstruction)
                {
                    RestartConstruction();
                }
            }
        }

        public void RemoveBuilder(UnitBehaviour builder)
        {
            if (assignedBuilders.Remove(builder))
            {
                Debug.Log($"[BuildingBehaviour] Builder removed. Remaining: {assignedBuilders.Count}");
                if (Data.Status == BuildingStatus.UnderConstruction)
                {
                    RestartConstruction();
                }
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

        private IEnumerator ConstructionCoroutine(float initialProgress = 0f)
        {
            buildProgress = initialProgress;

            while (buildProgress < 1f && Data.Status == BuildingStatus.UnderConstruction)
            {
                int builderCount = Mathf.Max(assignedBuilders.Count, Data.MinBuildersNeeded);
                float effectiveTime = Data.GetEffectiveBuildTime(builderCount);

                buildProgress += Time.deltaTime / effectiveTime;

                if (timerImage != null)
                    timerImage.fillAmount = buildProgress;

                yield return null;
            }

            buildProgress = 1f;
            Data.Status = BuildingStatus.Completed;

            Debug.Log($"[BuildingBehaviour] {Data.CommonData.Name} construction completed!");
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            if (meshFilter == null || Data == null) return;

            switch (Data.Status)
            {
                case BuildingStatus.Initial:
                    meshFilter.sharedMesh = Data.Initial;
                    break;
                case BuildingStatus.UnderConstruction:
                    meshFilter.sharedMesh = Data.UnderConstructionMesh;
                    break;
                case BuildingStatus.Completed:
                    meshFilter.sharedMesh = Data.CompletedMesh;
                    break;
                case BuildingStatus.UnderRepair:
                    meshFilter.sharedMesh = Data.UnderRepairMesh;
                    break;
                case BuildingStatus.Destroyed:
                    meshFilter.sharedMesh = Data.DestroyedMesh;
                    break;
            }
        }
    }
}
