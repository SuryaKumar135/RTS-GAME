using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace RTSGame
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animation), typeof(BoxCollider))]
    public class UnitBehaviour : SelectableObject
    {
        public enum UnitType { Builder, Soldier }

        [Header("Team")]
        public int teamID = 0;

        [Header("Unit Settings")]
        [SerializeField] private UnitType unitType;
        [SerializeField] private UnitData unitData;

        private NavMeshAgent agent;
        private Animation anim;

        private AnimationClip idleClip;
        private AnimationClip moveClip;
        private AnimationClip attackClip;
        private AnimationClip dieClip;

        private Coroutine movementCheckRoutine;
        private Coroutine workRoutine;

        private BuildingBehaviour currentBuildingTarget;

        private void Awake()
        {
            base.Awake();
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animation>();

            if (unitData != null)
                unitData.InitializeUnitData(this);
        }

        private void OnDisable()
        {
            // Remove from building if this unit is removed from game
            if (currentBuildingTarget != null)
            {
                currentBuildingTarget.RemoveBuilder(this);
                currentBuildingTarget = null;
            }
        }

        public void SetAnimationClips(AnimationClip idle, AnimationClip move, AnimationClip attack, AnimationClip die)
        {
            idleClip = idle;
            moveClip = move;
            attackClip = attack;
            dieClip = die;

            if (idleClip) anim.AddClip(idleClip, "Idle");
            if (moveClip) anim.AddClip(moveClip, "Move");
            if (attackClip) anim.AddClip(attackClip, "Attack");
            if (dieClip) anim.AddClip(dieClip, "Die");

            PlayIdle();

            if (movementCheckRoutine == null)
                movementCheckRoutine = StartCoroutine(MovementAnimationLoop());
        }

        public void MoveUnit(Vector3 targetPosition)
        {
            StopWorkAnimation();

            if (currentBuildingTarget != null)
            {
                currentBuildingTarget.RemoveBuilder(this);
                currentBuildingTarget = null;
            }

            agent.SetDestination(targetPosition);
            PlayMove();
        }

        public void ExecuteBuildingAction(BuildingBehaviour building)
        {
            if (currentBuildingTarget != null && currentBuildingTarget != building)
            {
                currentBuildingTarget.RemoveBuilder(this);
                currentBuildingTarget = null;
            }

            if (unitType == UnitType.Builder && building.teamID == teamID)
            {
                if (building.Data.Status == BuildingStatus.Completed)
                    return;

                currentBuildingTarget = building;
                Vector3 targetPos = building.GetNextBuilderPosition();
                agent.SetDestination(targetPos);
                StartCoroutine(BuilderReachBuilding());
            }
            else if (unitType == UnitType.Soldier && building.teamID != teamID)
            {
                currentBuildingTarget = building;
                Vector3 attackPos = building.GetNextBuilderPosition();
                agent.SetDestination(attackPos);
                StartCoroutine(SoldierReachEnemyBuilding());
            }
        }

        private IEnumerator BuilderReachBuilding()
        {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                yield return null;

            if (currentBuildingTarget != null)
            {
                currentBuildingTarget.AssignBuilder(this);

                if (workRoutine != null)
                    StopCoroutine(workRoutine);

                workRoutine = StartCoroutine(WorkAnimationLoop(currentBuildingTarget));
            }
        }

        private IEnumerator WorkAnimationLoop(BuildingBehaviour building)
        {
            while (building.Data.Status == BuildingStatus.UnderConstruction || building.Data.Status == BuildingStatus.UnderRepair)
            {
                PlayAttack(); // Attack is used for work animation
                yield return new WaitForSeconds(1.2f);
            }

            PlayIdle();
        }

        private IEnumerator SoldierReachEnemyBuilding()
        {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                yield return null;

            if (currentBuildingTarget != null && currentBuildingTarget.Data.Status != BuildingStatus.Destroyed)
            {
                if (workRoutine != null)
                    StopCoroutine(workRoutine);

                workRoutine = StartCoroutine(SoldierAttackLoop(currentBuildingTarget));
            }
        }

        private IEnumerator SoldierAttackLoop(BuildingBehaviour target)
        {
            while (target != null && target.Data.Status != BuildingStatus.Destroyed)
            {
                PlayAttack();
                target.TakeDamage(10);
                yield return new WaitForSeconds(1.2f);
            }

            PlayIdle();
        }

        public void StopWorkAnimation()
        {
            if (workRoutine != null)
            {
                StopCoroutine(workRoutine);
                workRoutine = null;
            }
            PlayIdle();
        }

        private IEnumerator MovementAnimationLoop()
        {
            while (true)
            {
                if (agent != null)
                {
                    bool isMoving = agent.hasPath && agent.remainingDistance > agent.stoppingDistance;
                    if (isMoving) PlayMove();
                    else PlayIdle();
                }
                yield return null;
            }
        }

        public void PlayIdle() { if (idleClip && !anim.IsPlaying("Idle")) anim.CrossFade("Idle"); }
        public void PlayMove() { if (moveClip && !anim.IsPlaying("Move")) anim.CrossFade("Move"); }
        public void PlayAttack() { if (attackClip) anim.CrossFade("Attack"); }
        public void PlayDie() { if (dieClip) anim.CrossFade("Die"); }
    }
}
