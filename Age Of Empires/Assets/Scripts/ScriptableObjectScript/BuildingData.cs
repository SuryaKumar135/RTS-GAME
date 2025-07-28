using UnityEngine;

namespace RTSGame
{
    public class BuildingData : ScriptableObject
    {
        public CommonData CommonData;
        public BuildingStatus Status;

        [Header("Builder Settings")]
        public int MinBuildersNeeded = 1;
        public int MaxBuildersAllowed = 3;

        [Header("Base Times (seconds)")]
        public float BaseBuildTime = 15f;
        public float BaseRepairTime = 10f;

        [Header("Meshes")]
        public Mesh Initial;
        public Mesh UnderConstructionMesh;
        public Mesh CompletedMesh;
        public Mesh UnderRepairMesh;
        public Mesh DestroyedMesh;

        public float GetEffectiveBuildTime(int builderCount)
        {
            builderCount = Mathf.Clamp(builderCount, MinBuildersNeeded, MaxBuildersAllowed);
            return BaseBuildTime / builderCount;
        }

        public float GetEffectiveRepairTime(int builderCount)
        {
            builderCount = Mathf.Clamp(builderCount, MinBuildersNeeded, MaxBuildersAllowed);
            return BaseRepairTime / builderCount;
        }
    }

    public enum BuildingStatus
    {
        Initial,
        UnderConstruction,
        Completed,
        UnderRepair,
        Destroyed
    }
}
