using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

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
    public void LinkButtons()
    {
        if(upgrade.previousUpgradeId != -1)
        {
            /*
            lineRenderer.positionCount = 2;
            lineRenderer.alignment = LineAlignment.TransformZ;
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            positions[0] = upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].transform.position;
            positions[1] = transform.position;
            lineRenderer.SetPositions(positions);
            */
            Vector2[] Points = new Vector2[2];
            Points[0] = new Vector2(upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].transform.localPosition.x
                //+ (upgrade.column * columnsIncrement)
                , upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].transform.localPosition.y
                //- (upgrade.row*rowsIncrement)
                );

            Points[1] = new Vector2(transform.localPosition.x 
                //+ (upgrade.column * columnsIncrement)
                ,transform.localPosition.y
                //-(upgrade.row *rowsIncrement)
                );

            Points[0].x -= transform.localPosition.x;
            Points[1].x -= transform.localPosition.x;
            Points[0].y -= transform.localPosition.y;
            Points[1].y -= transform.localPosition.y;

            List<Vector2> points = new List<Vector2>
            {
                Points[0],
                Points[1]
            };

            /*
            if(upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.column != 0)
            {
                if (upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.column > (float)columns / 2f)
                {
                    Points[0].x += columnsIncrement * upgrade.column;
                    Points[1].x += columnsIncrement * upgrade.column;

                }
                if (upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.row > (float)rows / 2f)
                {
                    Points[0].y += rowsIncrement * upgrade.row;
                    Points[1].y += rowsIncrement * upgrade.row;
                }
                if (upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.column < (float)columns / 2f)
                {
                    Points[0].x -= columnsIncrement * upgrade.column;
                    Points[1].x -= columnsIncrement * upgrade.column;
                }
                if (upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.row > (float)rows / 2f)
                {
                    Points[0].y -= rowsIncrement * upgrade.row;
                    Points[1].y -= rowsIncrement * upgrade.row;
                }
            }
            */

            /*
            lineRenderer.Points = Points;
            */
            lineRenderer.points = points;
            lineRenderer.transform.SetSiblingIndex(0);
            transform.SetSiblingIndex(1);
            float smoothness = 20f;
            List<Vector2> smoothpoints = new();
            for (float i = 0f; i < smoothness; i++)
            {
                smoothpoints.Add(Bezier(points[0], points[1], i/smoothness));
            }
            lineRenderer.points = smoothpoints;
            Debug.Log(lineRenderer.points);
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
