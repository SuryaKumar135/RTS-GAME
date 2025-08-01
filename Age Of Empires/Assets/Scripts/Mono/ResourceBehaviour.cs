//using Mono.Cecil;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace RTSGame
//{
//    public class ResourceBehaviour : MonoBehaviour
//    {
//        [Header("Resource Type")]
//        public ResourceType resourceType;

//        [Header("Common Data")]
//        public CommonData data;

//        [Header("Visuals")]
//        [SerializeField] private Mesh resourceMesh;
//        [SerializeField] private ParticleSystem gatherEffect;

//        private MeshFilter meshFilter;
//        private int gatherIndex = 0;
//        private List<UnitBehaviour> assignedGatherers = new List<UnitBehaviour>();

//        [Header("Gathering Setup")]
//        [SerializeField] float startAngle = -90f, arcAngle = 180f;
//        public float gatherSpacing = 2f;

//        private void Awake()
//        {
//            meshFilter = GetComponent<MeshFilter>();
//            if (meshFilter && resourceMesh)
//                meshFilter.sharedMesh = resourceMesh;

//            data.Health = data.MaxHealth;
//        }

//        public void AssignGatherer(UnitBehaviour builder)
//        {
//            if (data.Health <= 0 || assignedGatherers.Contains(builder))
//                return;

//            assignedGatherers.Add(builder);
//            builder.GatherFromResource(this);
//        }

//        public void RemoveGatherer(UnitBehaviour builder)
//        {
//            assignedGatherers.Remove(builder);
//        }

//        public Vector3 GetNextGatherPosition()
//        {
//            int maxSpots = Mathf.Max(3, 1);
//            float angleStep = arcAngle / (maxSpots - 1);
//            float angle = startAngle + (gatherIndex % maxSpots) * angleStep;
//            gatherIndex++;

//            float rad = angle * Mathf.Deg2Rad;
//            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * gatherSpacing;
//            return transform.position + offset;
//        }

//        public void Gather(int amount, ResourceManager manager)
//        {
//            if (data.Health <= 0) return;

//            data.Health -= amount;
//            if (gatherEffect) gatherEffect.Play();

//            switch (resourceType)
//            {
//                case ResourceType.Wood: manager.AddWood(amount); break;
//                case ResourceType.Wheat: manager.AddWheat(amount); break;
//                case ResourceType.Gold: manager.AddGold(amount); break;
//            }

//            if (data.Health <= 0)
//                DestroyResource();
//        }

//        private void DestroyResource()
//        {
//            if (gatherEffect) gatherEffect.Play();
//            gameObject.SetActive(false);
//        }

//        private void OnDrawGizmosSelected()
//        {
//            Gizmos.color = Color.yellow;
//            int maxSpots = Mathf.Max(3, 1);
//            float angleStep = arcAngle / (maxSpots - 1);

//            for (int i = 0; i < maxSpots; i++)
//            {
//                float angle = startAngle + i * angleStep;
//                float rad = angle * Mathf.Deg2Rad;
//                Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * gatherSpacing;
//                Gizmos.DrawSphere(transform.position + offset, 0.3f);
//            }
//        }
//    }
//}
