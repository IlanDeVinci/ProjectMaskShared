using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI upgradeNameText;
    public GlobalUpgrades.Upgrade upgrade;

    public void Initiate()
    {
        upgradeNameText.text = upgrade.upgradeName;
        switch(upgrade.column)
        {
            case 0:
                transform.position = new Vector2(-1000, 0);
                break;
            case 1:
                transform.position = new Vector2(-500, 0);
                break;
            case 2:
                transform.position = new Vector2(0, 0);
                break;
            case 3:
                transform.position = new Vector2(500, 0);

                break;
        }
        switch (upgrade.row)
        {
            case 0:
                transform.position = new Vector2(transform.position.x, -1000);
                break;
            case 1:
                transform.position = new Vector2(transform.position.x, -500);
                break;
            case 2:
                transform.position = new Vector2(transform.position.x, 0);
                break;
            case 3:
                transform.position = new Vector2(transform.position.x, 500);
                break;
            case 4:
                transform.position = new Vector2(transform.position.x, 1000);
                break;
        }

        transform.position = new Vector2(transform.position.x + 960, transform.position.y + 540);
    }
}
