using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour {
    [SerializeField]
    private string winMessage;
    [SerializeField]
    private Color32 winColor;

    [SerializeField]
    private string loseMessage;
    [SerializeField]
    private Color32 loseColor;

    [SerializeField]
    private float mainMenuDelay = 3f;

    [HideInInspector]
    public bool isWin;
    private TextMeshProUGUI textMeshPro;

	// Use this for initialization
	IEnumerator Start () {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

        Debug.Log("isWin in LevelEnd.cs: " + isWin);
        if (isWin)
        {
            textMeshPro.text = winMessage;
            textMeshPro.color = winColor;
        }
        else
        {
            textMeshPro.text = loseMessage;
            textMeshPro.color = loseColor;
        }

        //Wait a period of time
        yield return new WaitForSeconds(mainMenuDelay);
        //Load the main menu scene
        SceneManager.LoadScene(0);
    }
}
