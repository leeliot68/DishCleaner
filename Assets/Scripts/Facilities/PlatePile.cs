using UnityEngine;

public class PlatePile : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteractor player)
    {
        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        if (inventory.TryPickup(PlateType.Dirty))
        {
            Debug.Log("Took a dirty plate from the pile!");
        }
        else
        {
            Debug.Log("Already holding a plate!");
        }
    }
}
