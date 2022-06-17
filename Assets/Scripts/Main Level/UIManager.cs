using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject joinGameUI;
    public GameObject startGameButton;
    public GameObject m_inputFieldPassword;

    void Start()
    {
        GameManager.Instance.UpdateState += UpdateText;
        GameManager.Instance.MatchFound += MatchFound;

    }

    public void UpdateText(string text)
    {
        statusText.text = text;
    }

    public void TogglestatusText(bool text)
    {
        statusText.enabled = text;
    }

    public void MatchFound(string message)
    {
        joinGameUI.SetActive(false);
        if (message == "host")
        {
            startGameButton.SetActive(true);
        }
    }

}
