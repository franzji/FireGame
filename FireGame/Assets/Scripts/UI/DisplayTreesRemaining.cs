using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayTreesRemaining : MonoBehaviour
{
    private float percentTreesRemaining;
    private TextMeshProUGUI _treeText;
    private TextMeshProUGUI treeText
    {
        get
        {
            if (_treeText == null)
                _treeText = GetComponent<TextMeshProUGUI>();
            return _treeText;
        }
        set
        {
            _treeText = value;
        }
    }

    public void UpdateTreesRemaining(float newPercentage)
    {
        percentTreesRemaining = newPercentage;
        int actualPercent = Mathf.CeilToInt(percentTreesRemaining * 100);
        treeText.text = actualPercent.ToString() + "% trees remaining.";
    }
}
