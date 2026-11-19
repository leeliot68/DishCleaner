
using NUnit.Framework.Interfaces;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [SerializeField] private ItemData data;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public ItemData Data => data;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    public void Initialize(ItemData itemData)
    {
        data = itemData;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sprite = data != null
            ? data.sprite
            : null;
    }
}
