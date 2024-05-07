using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class UpgradeButton : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI upgradeNameText;
    public GlobalUpgrades.Upgrade upgrade;
    private UpgradeTreeManager upgradeTreeManager;
    [SerializeField] UILineRenderer lineRenderer;
    public int columnsIncrement;
    public int rowsIncrement;
    public int rows;
    public int columns;
    private GlobalUpgrades.Upgrade previousUpgrade;

    public void SetLineColor()
    {
        if (upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.thresholdToUnlockNext <= previousUpgrade.upgradeLevel)
        {
            lineRenderer.color = Color.white;
        }
        else
        {
            lineRenderer.color = Color.gray;
        }
    }
    public void LinkButtons()
    {
        if (upgrade.previousUpgradeId != -1)
        {
            previousUpgrade = upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade;
            Vector2 start = new Vector2(upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].transform.localPosition.x
                , upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].transform.localPosition.y
                );

            Vector2 end = Vector2.zero;

            start.x -= transform.localPosition.x;
            start.y -= transform.localPosition.y;

            List<Vector2> points = new List<Vector2>
            {
            start,end
            };

            lineRenderer.points = points;
            lineRenderer.transform.SetSiblingIndex(0);
            transform.SetSiblingIndex(1);
            float smoothness = 50f;
            List<Vector2> smoothpoints = new();
            if (points[0].y != points[1].y)
            {
                for (float i = 0f; i < smoothness; i++)
                {

                    smoothpoints.Add(Bezier(points[0], new Vector2(points[0].x + 1000, points[0].y), new Vector2(points[1].x - 1000, points[1].y), points[1], i / smoothness));
                }
                lineRenderer.points = smoothpoints;
            }
            SetLineColor();
        }

    }
    public void Initiate()
    {

        upgradeNameText.text = upgrade.upgradeName;
        transform.position = new Vector2(-1300, +2100);
        //transform.position = new Vector2(2900, -1000);


        columns = GlobalUpgrades.Instance.columnNumber;
        columnsIncrement = 4200 / columns;

        rows = GlobalUpgrades.Instance.rowNumber;
        rowsIncrement = 3100 / rows;


        transform.localPosition = new Vector2(transform.localPosition.x + (upgrade.column * columnsIncrement), transform.localPosition.y - (upgrade.row * rowsIncrement));
        upgradeTreeManager = GetComponentInParent<UpgradeTreeManager>();
    }

    Vector2 Bezier(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }

    private void ButtonClicked()
    {

    }
}
