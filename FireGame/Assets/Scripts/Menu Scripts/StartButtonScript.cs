using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour {

    public void OnClick()
    {
        string level = GetComponentInChildren<TextMeshProUGUI>().text;
        SceneManager.LoadScene(level);
    }
}
