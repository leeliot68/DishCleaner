
using UnityEngine;

public class HeldObject : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite dirtyPlateSprite;
    [SerializeField] private Sprite cleanPlateSprite;

    private void Update()
    {
        switch (inventory.HeldPlate)
        {
            case PlateType.Dirty:
                spriteRenderer.sprite = dirtyPlateSprite;
                break;

            case PlateType.Clean:
                spriteRenderer.sprite = cleanPlateSprite;
                break;

            default:
                spriteRenderer.sprite = null;
                break;
        }
    }
}
