
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;

    private void Update()
    {
        if (CurrencyManager.Instance == null)
            return;

        moneyText.text = $"${CurrencyManager.Instance.Money:N0}";
    }
}
