using Platformer;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Resource Data")]
    public ResourceData resourceData;

    [Space(3)]
    [Header("Resource Events Channels")]
    public IntEventChannel woodChannel;
    public IntEventChannel wheatChannel;
    public IntEventChannel goldChannel;


    private void OnEnable()
    {
        woodChannel.Invoke(resourceData.WOOD);
        wheatChannel.Invoke(resourceData.WHEAT);
        goldChannel.Invoke(resourceData.GOLD);
    }

    public void AddWood(int amount)
    {
        resourceData.WOOD += amount;
        woodChannel.Invoke(resourceData.WOOD);
    }

    public void AddWheat(int amount)
    {
        resourceData.WHEAT += amount;
        wheatChannel.Invoke(resourceData.WHEAT);
    }

    public void AddGold(int amount)
    {
        resourceData.GOLD += amount;
        goldChannel.Invoke(resourceData.GOLD);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddWood(1);
            AddWheat(20);
            AddGold(1);
        }
    }
}
