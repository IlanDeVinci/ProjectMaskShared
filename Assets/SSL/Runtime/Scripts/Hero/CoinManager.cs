using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Tuto")
        GlobalManager.playerMoney = 0;
    }

    private void Update()
    {
        coinText.text = GlobalManager.playerMoney.ToString();
    }
}