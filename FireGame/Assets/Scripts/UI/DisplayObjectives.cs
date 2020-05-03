using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayObjectives : MonoBehaviour {

    public LevelData levelData;

    void Start()
    {
        TextMeshProUGUI textBox = GetComponent<TextMeshProUGUI>();
        textBox.text = levelData.objectiveDescription;
    }
}
