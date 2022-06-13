using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject playButton;
    public GameObject startGameButton;
    void Start()
    {
        GameManager.Instance.UpdateState += UpdateText;
        GameManager.Instance.MatchFound += MatchFound;
    }

    public void UpdateText(string text)
    {
        statusText.text = text;
    }

    public void MatchFound(string message)
    {
        playButton.SetActive(false);
        if (message == "host")
        {
            startGameButton.SetActive(true);
        }
    }
}
