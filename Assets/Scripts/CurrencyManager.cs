using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int currencyAmount;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        currencyAmount = 0;
    }

    public void addMoney(int val) { currencyAmount += val; }
    public void removeMoney(int val) { currencyAmount -= val; }
}
