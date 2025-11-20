using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int currency;

    public void InitializeCurrency(int startingCurrency)
    {
        currency = startingCurrency;
        UpdateCurrencyUI();
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        UpdateCurrencyUI();
    }

    public void RemoveCurrency(int amount)
    {
        if (currency >= amount)
        {
            currency -= amount;
            UpdateCurrencyUI();
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void UpdateCurrencyUI()
    {
        UIManager.instance.UpdateCurrencyUI(currency);
    }
}
