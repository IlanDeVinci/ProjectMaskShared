using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
#nullable enable
public static class GlobalUpgrades
{
    public static List<Upgrade> upgradeList;
    public class Upgrade
    {
        public int upgradeId;
        public bool isUpgradeAcquired;
        public Upgrade? previousUpgrade;
        public int upgradeCost;
        public string upgradeName;
        public string upgradeDescription;
    }
}
