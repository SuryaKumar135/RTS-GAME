using UnityEngine;
using UnityEngine.AI;

namespace RTSGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitBehaviour : SelectableObject
    {
        public enum UnitType
        {
            Builder,
            Soldier
        }

        [SerializeField] private UnitType unitType;
        private NavMeshAgent agent;

        void Awake()
        {
            base.Awake();
            agent = GetComponent<NavMeshAgent>();
        }

        public void ExecuteAction(Vector3 targetPosition)
        {
            // Example: move to clicked position
            if (agent != null)
            {
                agent.SetDestination(targetPosition);
            }

            // You can branch behavior:
            if (unitType == UnitType.Builder)
            {
                Debug.Log($"{name} (Builder) moving to build at {targetPosition}");
            }
            else if (unitType == UnitType.Soldier)
            {
                Debug.Log($"{name} (Soldier) moving to attack at {targetPosition}");
            }
        }
    }
}