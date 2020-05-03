using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalCost : MonoBehaviour
{
    public List<GameObject> unitDisplays;
    public Transform unitDisplayHolderTransform;
    public GameObject TotalPriceText;
    public GlobalLevelData LvlData;
    public LevelData ThisLvlData;
    private float total;

    public void Start()
    {
        for (int i = 0; i < LvlData.actionmoneys.Length; i++)
        {
            LvlData.actionmoneys[i] = 0;
        }
        for (int i = 0; i < LvlData.actionchoices.Length; i++)
        {
            LvlData.actionchoices[i] = 0;
        }
        TotalPriceText = GameObject.Find("TotalPriceText");

        TotalPriceText.GetComponent<TextMeshProUGUI>().text = "Price: $" + total.ToString() + "/$" + ThisLvlData.maxmoney;
    }

    public void Cost()
    {
        total = 0;
        for (int i = 0; i < LvlData.actionmoneys.Length; i++)
        {
            total = total + LvlData.actionmoneys[i];
        }

        TotalPriceText.GetComponent<TextMeshProUGUI>().text = "Price: $" + total.ToString() + "/$" + ThisLvlData.maxmoney;

        if (ThisLvlData.maxmoney >= total)
        {
            TotalPriceText.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (ThisLvlData.maxmoney < total)
        {
            TotalPriceText.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }

    public void CloseSelection()
    {
        total = 0;

        for (int i = 0; i < LvlData.actionmoneys.Length; i++)
        {
            total = total + LvlData.actionmoneys[i];
        }

        if (ThisLvlData.maxmoney >= total)
        {
            GameObject.Find("Trees").GetComponent<BurnManager>().startGame();

            //spawn in the shit now.
            for (int i = 0; i < LvlData.actionmoneys.Length; i++)
            {
                if (LvlData.actionchoices[i] > 0)
                {
                    Instantiate(unitDisplays[i], unitDisplayHolderTransform);
                }
            }
            this.gameObject.SetActive(false);
        }
    }
}