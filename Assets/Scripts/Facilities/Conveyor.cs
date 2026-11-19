
using UnityEngine;

[RequireComponent(typeof(PlaceableFacility))]
public class Conveyor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ConveyorDirection direction =
        ConveyorDirection.Right;

    [SerializeField] private Transform visual;
    [Header("Items")]
    [SerializeField] private Item itemPrefab;
    [SerializeField] private ItemData dirtyPlateData;
    [SerializeField] private ItemData cleanPlateData;

    public ConveyorDirection Direction => direction;

    public ConveyorLine Line { get; private set; }

    public Vector3Int GridPosition =>
        GetComponent<PlaceableFacility>().GridPosition;

    public void SetLine(ConveyorLine line)
    {
        Line = line;
    }

    public Vector3Int GetOutputCell()
    {
        Vector3Int offset = direction switch
        {
            ConveyorDirection.Up =>
                Vector3Int.up,

            ConveyorDirection.Right =>
                Vector3Int.right,

            ConveyorDirection.Down =>
                Vector3Int.down,

            ConveyorDirection.Left =>
                Vector3Int.left,

            _ => Vector3Int.zero
        };

        return GridPosition + offset;
    }

    public void SetDirection(ConveyorDirection newDirection)
    {
        direction = newDirection;
        UpdateVisual();
    }
    public static ConveyorDirection GetNextDirection(
    ConveyorDirection current)
    {
        return current switch
        {
            ConveyorDirection.Up => ConveyorDirection.Right,
            ConveyorDirection.Right => ConveyorDirection.Down,
            ConveyorDirection.Down => ConveyorDirection.Left,
            ConveyorDirection.Left => ConveyorDirection.Up,
            _ => ConveyorDirection.Right
        };
    }

    public void Interact(PlayerInteractor player)
    {
        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null || Line == null)
            return;

        if (inventory.HeldPlate != PlateType.None)
        {
            PlacePlate(inventory);
        }
        else
        {
            TakePlate(inventory);
        }
    }

    private void PlacePlate(PlayerInventory inventory)
    {
        PlateType plateType = inventory.HeldPlate;

        ItemData data = plateType switch
        {
            PlateType.Dirty => dirtyPlateData,
            PlateType.Clean => cleanPlateData,
            _ => null
        };

        if (data == null || itemPrefab == null)
            return;

        Item item = Instantiate(
            itemPrefab,
            transform.position,
            Quaternion.identity
        );

        item.Initialize(data);

        if (Line.TryAddItem(item, this))
        {
            inventory.TryRemovePlate(plateType);
        }
        else
        {
            Destroy(item.gameObject);
        }
    }

    private void TakePlate(PlayerInventory inventory)
    {
        if (!Line.TryTakeItem(this, out Item item))
            return;

        PlateType plateType =
            item.Data == dirtyPlateData
                ? PlateType.Dirty
                : PlateType.Clean;

        if (inventory.TryPickup(plateType))
        {
            Destroy(item.gameObject);
        }
        else
        {
            // Return the plate if inventory pickup fails.
            Line.TryAddItem(item, this);
        }
    }


    private void Start()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (visual == null)
            return;

        // Sprite is drawn pointing right by default.
        float angle = direction switch
        {
            ConveyorDirection.Right => 0f,
            ConveyorDirection.Up => 90f,
            ConveyorDirection.Left => 180f,
            ConveyorDirection.Down => 270f,
            _ => 0f
        };

        visual.localRotation =
            Quaternion.Euler(0f, 0f, angle);
    }
}
