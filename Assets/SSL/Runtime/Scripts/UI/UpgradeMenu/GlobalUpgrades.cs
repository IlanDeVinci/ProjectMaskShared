using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalUpgrades : MonoBehaviour
{
    private static GlobalUpgrades instance;
    public static GlobalUpgrades Instance => instance;

    public enum UpgradeType
    {
        KnifeDamage,
        Hp,
        InvisibilityTime,
        JumpHeight,
        InvisibilityCooldown,
        Resistance,
        KnifeRange,
        DoubleKnife,
        DoubleDamage,
        KnifePiercing
    }
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

        int i = 0;
        foreach(Upgrade upgrade in Upgrades)
        {
            upgrade.upgradeId = i;
            if(upgrade.column > columnNumber) columnNumber = upgrade.column;
            if(upgrade.row > rowNumber) rowNumber = upgrade.row;
            i++;
        }
    }

    [SerializeField] public List<Upgrade> Upgrades;
    [HideInInspector] public int rowNumber = 0;
    [HideInInspector] public int columnNumber = 0;


    [Serializable]
    public class Upgrade
    {
        [HideInInspector] public int upgradeId;
        [HideInInspector] public bool isUpgradeAcquiredFull;
        [SerializeField] public string upgradeName;
        [SerializeField] public string upgradeDescription;
        [SerializeField] public int column;
        [SerializeField] public int row;
        [SerializeField] public UpgradeType upgradeType;
        [SerializeField] public int thresholdToUnlockNext;
        [SerializeField] public int previousUpgradeId;
        [SerializeField] public List<SingleUpgrade> upgradesList;
        [HideInInspector] public int upgradeLevel = 0;
        [Serializable]
        public class SingleUpgrade
        {
            [HideInInspector] public bool isUpgradeAcquired;
            [SerializeField] public int upgradeCost;
            [SerializeField] public int upgradeValue;
        }
    }

}
