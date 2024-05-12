using PrimeTween;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UpgradeButton : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI upgradeNameText;
    public GlobalUpgrades.Upgrade upgrade;
    private UpgradeTreeManager upgradeTreeManager;
    [SerializeField] UILineRenderer lineRenderer;
    [SerializeField] Button button;
    [SerializeField] TweenButton tweenButton;
    public int columnsIncrement;
    public int rowsIncrement;
    public int rows;
    public int columns;
    private GlobalUpgrades.Upgrade previousUpgrade;
    [SerializeField] private ToolTip toolTip;
    [SerializeField] private GameObject fadingText;
    [SerializeField] private Image buttonImage;
    Tween tweenColor;
    public RectTransform canvas;

    public void SetColors()
    {
        HideToolTip();
        if (upgrade.upgradeLevel < upgrade.upgradesList.Count - 1 && upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost <= GlobalManager.playerMoney)
        {
            tweenButton.canTween = true;
            button.interactable = true;
            if (tweenColor.isAlive)
            {
                tweenColor.Stop();

            }
            tweenColor = Tween.Color(buttonImage, Color.white, 1);
        }
        else
        {
            if (upgrade.upgradeLevel == upgrade.upgradesList.Count - 1)
            {
                tweenButton.canTween = false;
                tweenButton.StopTweens();
                button.interactable = false;
                if (tweenColor.isAlive)
                {
                    tweenColor.Stop();

                }
                tweenColor = Tween.Color(buttonImage, Color.green, 1);
            }
            if (tweenColor.isAlive)
            {
                tweenColor.Stop();

            }
            tweenColor = Tween.Color(buttonImage, Color.gray, 1);
            tweenButton.canTween = false;
            button.interactable = false;
        }

        if (upgrade.previousUpgradeId != -1)
        {
            if (upgradeTreeManager.buttonsList[upgrade.previousUpgradeId].upgrade.thresholdToUnlockNext <= previousUpgrade.upgradeLevel)
            {
                Tween.Color(lineRenderer, Color.green, 1);
            }
            else
            {
                lineRenderer.color = Color.red;
                tweenButton.canTween = false;
                button.interactable = false;
                if (tweenColor.isAlive)
                {
                    tweenColor.Stop();

                }
                tweenColor = Tween.Color(buttonImage, Color.gray, 1);

            }


        }


    }
    private void Awake()
    {
        tweenButton.canTween = false;
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

                    smoothpoints.Add(Bezier(points[0], new Vector2(points[0].x + ((points[1].x - points[0].x)*0.8f), points[0].y), new Vector2(points[1].x - ((points[1].x - points[0].x) * 0.8f), points[1].y), points[1], i / smoothness));
                }
                lineRenderer.points = smoothpoints;
            }
            SetColors();
        }

    }
    public void Initiate()
    {
        toolTip.scale = canvas.localScale.x;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        int totalLength = (int)(4200) ;
        float totalHeight = (int)(3100);

        Debug.Log(totalLength);

        Debug.Log(totalHeight);
        upgradeNameText.text = upgrade.upgradeName;
        transform.position =
            //new Vector2(-1300, +2100)
            new Vector2(-totalLength*0.309f * canvas.localScale.x, totalHeight*0.677f * canvas.localScale.x);
            ;
        //transform.position = new Vector2(2900, -1000);


        columns = GlobalUpgrades.Instance.columnNumber;
        columnsIncrement = 
            //4200 / columns
            totalLength/columns
            ;

        rows = GlobalUpgrades.Instance.rowNumber;
        rowsIncrement =
            //3100 / rows
            (int)totalHeight/rows
            ;


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

    public void ButtonClicked()
    {
        if (GlobalManager.playerMoney >= upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost && upgrade.upgradeLevel < upgrade.upgradesList.Count - 1)
        {
            Debug.Log(GlobalManager.playerMoney);
            GlobalManager.playerMoney -= upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost;
            Debug.Log(GlobalManager.playerMoney);
            var savedText = Instantiate(fadingText, GameObject.FindGameObjectWithTag("UpgradeMenuImage").transform);
            savedText.GetComponent<TextFollowMouse>().value = upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost;
            upgrade.upgradeLevel++;
            GlobalUpgrades.Instance.Upgrades[upgrade.upgradeId].upgradeLevel = upgrade.upgradeLevel;
            upgradeTreeManager.SetAllColors();
            ShowToolTip();
        }
    }

    public void ShowToolTip()
    {
        string text = "";
        if (upgrade.upgradesList.Count != 2)
        {
            text = $"Niveau : <b>{upgrade.upgradeLevel}</b>\n";
            if (upgrade.upgradeLevel >= upgrade.upgradesList.Count - 1)
            {
                text = $"Niveau : <b>Max</b>\n";

            }

        }
        else
        {
            if (upgrade.upgradeLevel == 0) { text = $"<b>Non activé.</b>\n"; }
            else { text = $"<b>Activé.</b>\n"; }
        }
        text += upgrade.upgradeDescription;
        if (upgrade.upgradeLevel < upgrade.upgradesList.Count - 1)
        {

            text = text.Replace("value", upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeValue.ToString());
            text = text.Replace("current", upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue.ToString());
            text = text.Replace("increase", (upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeValue - upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue).ToString());
            text = text.Replace("decrease", (upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue - upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeValue).ToString());
            text = text.Replace("price", upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost.ToString());
            text = text.Replace("level", upgrade.upgradeLevel.ToString());
            if (upgrade.upgradesList.Count != 2)
            {
                text += $"\nActuel : <b>{upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue}</b>";
            }
            if (upgrade.upgradeType == GlobalUpgrades.UpgradeType.JumpHeight)
            {
                text += "x";
            }
            text += $"\nCoût : <b>{upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost}</b>";

        }
        toolTip.ShowTooltip(text);
    }
    public void HideToolTip()
    {
        toolTip.HideTooltip();
    }
    public void GetInfo()
    {
        Debug.Log(upgrade.upgradeId);
        Debug.Log(upgrade.upgradesList[upgrade.upgradeLevel + 1].upgradeCost);
        Debug.Log(buttonImage.color);

        Debug.Log(tweenButton.canTween);

    }
}
