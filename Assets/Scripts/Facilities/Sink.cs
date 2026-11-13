
using System.Collections;
using UnityEngine;

public class Sink : MonoBehaviour, IInteractable
{
    [SerializeField] private float washDuration = 2f;

    private bool isWashing = false;
    private SinkState state = SinkState.Empty;

    public void Interact(PlayerInteractor player)
    {
        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        // Collect a completed plate
        if (state == SinkState.Ready &&
            inventory.HeldPlate == PlateType.None)
        {
            if (inventory.TryPickup(PlateType.Clean))
                state = SinkState.Empty;

            return;
        }

        // Deposit a dirty plate
        if (inventory.HeldPlate == PlateType.Dirty)
        {
            if (state != SinkState.Empty)
            {
                Debug.Log("Sink full!");
                return;
            }

            if (inventory.TryRemovePlate(PlateType.Dirty))
            {
                state = SinkState.Washing;
                StartCoroutine(WashPlate());
            }

            return;
        }

        if (inventory.HeldPlate == PlateType.Clean)
        {
            Debug.Log("Need a dirty plate!");
            return;
        }

        if (state == SinkState.Washing)
            Debug.Log("Washing in progress!");
        else
            Debug.Log("Need a dirty plate!");
    }


    private IEnumerator WashPlate()
    {
        Debug.Log("Washing started...");

        yield return new WaitForSeconds(washDuration);

        state = SinkState.Ready;

        Debug.Log("Washing complete! Plate ready for collection.");
    }


    public SinkState getState()
    {
        return state;
    }
}
