
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlateType heldPlate = PlateType.None;

    public PlateType HeldPlate => heldPlate;

    public bool TryPickup(PlateType plate)
    {
        if (heldPlate != PlateType.None || plate == PlateType.None)
            return false;

        heldPlate = plate;
        Debug.Log($"Picked up {plate} plate");

        return true;
    }

    public bool TryWashPlate()
    {
        if (heldPlate != PlateType.Dirty)
            return false;

        heldPlate = PlateType.Clean;
        Debug.Log("Plate washed!");

        return true;
    }

    public bool TryRemovePlate(PlateType plate)
    {
        if (heldPlate != plate || plate == PlateType.None)
            return false;

        heldPlate = PlateType.None;
        return true;
    }
}