
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "DishCleaner/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public int sellValue;
}
