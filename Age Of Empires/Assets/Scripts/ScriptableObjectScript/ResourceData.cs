
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ResourceData")]
public class ResourceData : ScriptableObject
{
    public int WOOD;
    public int WHEAT;
    public int GOLD;

    public void SetWood(int value) => WOOD = value;
    public void SetWheat(int value) => WHEAT = value;
    public void SetGold(int value) => GOLD = value;
}
