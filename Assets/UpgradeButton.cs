using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;



[Serializable]
public class UpgradeButton : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI upgradeNameText;
    public GlobalUpgrades.Upgrade upgrade;
    private UpgradeTreeManager upgradeTreeManager;
    [SerializeField] LineRenderer lineRenderer;

    public void LinkButtons()
    {
        Debug.Log("linked");
        if(upgrade.previousUpgradeId != -1)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.alignment = LineAlignment.TransformZ;
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            positions[0] = upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].transform.position;
            positions[1] = transform.position;
            lineRenderer.SetPositions(positions);
        }

    }
    public void Initiate()
    {
        
        upgradeNameText.text = upgrade.upgradeName;
        transform.position = new Vector2(-1300, +2100);
        //transform.position = new Vector2(2900, -1000);


        int columns = GlobalUpgrades.Instance.columnNumber;
        int columnIncrement = 4200 / columns;

        int rows = GlobalUpgrades.Instance.rowNumber;
        int rowIncrement = 3100 / rows;


        transform.position = new Vector2(transform.position.x + (upgrade.column * columnIncrement), transform.position.y - (upgrade.row * rowIncrement));
        upgradeTreeManager = GetComponentInParent<UpgradeTreeManager>();


        /*
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
        */
    }
}
