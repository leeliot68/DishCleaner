
using System.Collections;
using UnityEngine;

public class Sink : MonoBehaviour, IInteractable
{
    [SerializeField] private float washDuration = 2f;

    private bool isWashing = false;

    public void Interact(PlayerInteractor player)
    {
        if (isWashing)
            return;

        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null ||
            inventory.HeldPlate != PlateType.Dirty)
        {
            Debug.Log("You need a dirty plate!");
            return;
        }

        StartCoroutine(WashPlate(inventory));
    }

    private IEnumerator WashPlate(PlayerInventory inventory)
    {
        isWashing = true;

        Debug.Log("Washing started...");

        yield return new WaitForSeconds(washDuration);

        if (inventory != null && inventory.TryWashPlate())
        {
            Debug.Log("Washing complete!");
        }

        isWashing = false;
    }
}
