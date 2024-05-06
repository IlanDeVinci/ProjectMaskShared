using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class ShopManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Image fade;
    private Tween tween;
    [SerializeField] private GameObject fadingText;

    [SerializeField] private ShopObject[] shopObjects;
    [SerializeField] private ToolTip toolTip;

    [Serializable]
    public class ShopObject
    {
        [SerializeField] public string name;
        [SerializeField] public string description;
        [SerializeField] public Image icon;
        [SerializeField] public int currentLevel = 0;
        [SerializeField] public int maxLevel = 3;
        [SerializeField] public TextMeshProUGUI text;

        [SerializeField] public ShopObjectValue[] item;

        [Serializable]
        public class ShopObjectValue
        {
            [SerializeField] public int price;
            [SerializeField] public float value;
        }
    }

    public void ShowToolTip(int id)
    {
        toolTip.ShowTooltip(shopObjects[id].description);
    }

    public void ToNextLevel()
    {
        StartCoroutine(NextLevel());
    }

    private IEnumerator NextLevel()
    {
        yield return StartCoroutine(Fade(1, Ease.OutSine));
        SceneManager.LoadSceneAsync("Stage2");
    }

    private void SetPrices()
    {
        foreach (var obj in shopObjects)
        {
            if (obj.currentLevel < obj.maxLevel)
            {
                obj.text.text = $"{obj.item[obj.currentLevel].price}x";
            }
            else
            {
                obj.text.text = "MAX";
            }
        }

        currencyText.text = $"{GlobalManager.playerMoney}x";
    }

    public void ButtonClick(int id)
    {
        ShopObject obj = shopObjects[id];
        if (obj.currentLevel < obj.maxLevel)
        {
            if (obj.item[obj.currentLevel].price <= GlobalManager.playerMoney)
            {
                GlobalManager.playerMoney -= obj.item[obj.currentLevel].price;
                //Debug.Log($"spent {obj.item[obj.currentLevel].price}");
                switch (obj.name)
                {
                    case "Lumière":


                        break;
                    case "Vitesse":


                        break;
                    case "Radar":

                        break;
                }

                var savedText = Instantiate(fadingText, GameObject.FindGameObjectWithTag("Canvas").transform);
                savedText.GetComponent<TextFollowMouse>().value = obj.item[obj.currentLevel].price;
                obj.currentLevel++;
                SetPrices();
            }
            else
            {
                var savedText = Instantiate(fadingText, GameObject.FindGameObjectWithTag("Canvas").transform);
                savedText.GetComponent<TextFollowMouse>().value = 0;
            }
        }
    }

    private IEnumerator Fade(int end, Ease ease)
    {
        Tween tween = Tween.Custom(fade.color.a, endValue: end, duration: 2, ease: ease,
            onValueChange: newVal => fade.color = new Color(0, 0, 0, newVal));
        yield return tween.ToYieldInstruction();
    }

    void Start()
    {
        currencyText.text = $"{GlobalManager.playerMoney} x";


        fade.color = new Color(0, 0, 0, 1);
        StartCoroutine(Fade(0, Ease.InSine));
        SetPrices();
    }

    // Update is called once per frame
    void Update()
    {
    }
}