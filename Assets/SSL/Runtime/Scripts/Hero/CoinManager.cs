using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        GlobalManager.playerMoney = 0;
    }

    private void Update()
    {
        coinText.text = GlobalManager.playerMoney.ToString();
    }
}