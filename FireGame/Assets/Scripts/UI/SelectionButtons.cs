using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionButtons : MonoBehaviour {
    public GameObject Result;
    public GameObject ItemCost;
    public GlobalLevelData LvlData;
    public int position;
    public int UniqueID;


    private void Start()
    {
        position = transform.parent.GetSiblingIndex() - 1;
    }

    public void Increase()
    {
        int amountnum = int.Parse(GetComponent<TMP_InputField>().text);
        if (amountnum < 30)
        {
            amountnum++;
            GetComponent<TMP_InputField>().text = amountnum.ToString();
            Cost();
        }
    }
    public void Decrease()
    {
        int amountnum = int.Parse(GetComponent<TMP_InputField>().text);
        if (amountnum!= 0) {
            amountnum--;
            GetComponent<TMP_InputField>().text = amountnum.ToString();
            Cost();
        }
    }
    public void Cost()
    {
        //find the price per unit
        int price = int.Parse(ItemCost.GetComponent<TextMeshProUGUI>().text);
        //find the amount from your own field
        int amount = int.Parse(GetComponent<TMP_InputField>().text);
        //write the amount to actionchoices for spawning them later
        LvlData.actionchoices[UniqueID] = amount;

        ///calculate local price and print it to your text object
        price = price * amount;
        Result.GetComponent<TextMeshProUGUI>().text = "$" + price.ToString();
        LvlData.actionmoneys[position] = price;

        //do the total price calculation!!!
        GameObject g = GameObject.Find("SelectionUI");
        FinalCost cScript = g.GetComponentInChildren<FinalCost>();
        cScript.Cost();
    }
}