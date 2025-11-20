using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public TMP_Text currencyText;

    public void SetCurrency(int currency) => currencyText.text = currency.ToString("N0");
}
