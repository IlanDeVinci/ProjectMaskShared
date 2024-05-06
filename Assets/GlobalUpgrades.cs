using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalUpgrades : MonoBehaviour
{
    private static GlobalUpgrades instance;
    public static GlobalUpgrades Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] public List<Upgrade>? Upgrades;

    [Serializable]
    public class Upgrade
    {
        [SerializeField] public int upgradeId;
        [SerializeField] public bool isUpgradeAcquired;
        [SerializeField] public int previousUpgradeId;
        [SerializeField] public int upgradeCost;
        [SerializeField] public string upgradeName;
        [SerializeField] public string upgradeDescription;
        [SerializeField] public int column;
        [SerializeField] public int row;
    }
}
