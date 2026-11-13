
using UnityEngine;

public class SellingStation : MonoBehaviour, IInteractable
{
    [SerializeField] private int plateSellValue = 5;

    public void Interact(PlayerInteractor player)
    {
        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        if (!inventory.TryRemovePlate(PlateType.Clean))
        {
            Debug.Log("You need a clean plate to sell!");
            return;
        }

        CurrencyManager.Instance.AddMoney(plateSellValue);

        Debug.Log("Sold a clean plate!");
    }
}
