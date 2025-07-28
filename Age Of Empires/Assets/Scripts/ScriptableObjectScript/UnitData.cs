
using UnityEngine;
using UnityEngine.AI;
namespace RTSGame
{
    [CreateAssetMenu(fileName = "SelectableData", menuName = "RTS/Unit Data")]
    public class UnitData : ScriptableObject
    {
        public CommonData CommonData;

        public float speed;

        //Initialize Data
        public void InitializeUnitData(UnitBehaviour unitBehaviour)
        {
            unitBehaviour.GetComponent<NavMeshAgent>().speed = speed;
        }
    }
}

