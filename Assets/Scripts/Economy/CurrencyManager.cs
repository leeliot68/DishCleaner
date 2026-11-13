
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int startingMoney = 0;

    public int Money { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Money = startingMoney;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        Money += amount;
        Debug.Log($"Earned ${amount}. Total: ${Money}");
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount < 0 || Money < amount)
            return false;

        Money -= amount;
        return true;
    }
}
