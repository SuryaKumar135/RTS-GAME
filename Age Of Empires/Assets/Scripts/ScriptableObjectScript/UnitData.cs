using UnityEngine;
using UnityEngine.AI;

namespace RTSGame
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "RTS/Unit Data")]
    public class UnitData : ScriptableObject
    {
        public CommonData CommonData;

        [Header("Movement")]
        public float speed = 3.5f;

        [Header("Animation Clips")]
        public AnimationClip idleClip;
        public AnimationClip moveClip;
        public AnimationClip attackClip;
        public AnimationClip dieClip;


        [Header("Attack Power")]
        [SerializeField] private Vector2 attackPower;

        public void InitializeUnitData(UnitBehaviour unit)
        {
            // Movement setup
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.speed = speed;

            // Animation setup
            unit.SetAnimationClips(idleClip, moveClip, attackClip, dieClip);
        }

       // public 
    }
}
