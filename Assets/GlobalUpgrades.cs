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


        foreach(Upgrade upgrade in Upgrades)
        {
            if(upgrade.column > columnNumber) columnNumber = upgrade.column;
            if(upgrade.row > rowNumber) rowNumber = upgrade.row;
        }
    }

    [SerializeField] public List<Upgrade>? Upgrades;
    [SerializeField] public int rowNumber = 0;
    [SerializeField] public int columnNumber = 0;


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
