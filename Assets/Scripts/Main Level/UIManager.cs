using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject joinGameUI;
    public GameObject startGameButton;
    [SerializeField] private GameObject numberOfPlayerLeftGUI;
    [SerializeField] private GameObject gameOverUI;
    public GameObject m_inputFieldPassword;
    public GameObject m_popMenu;
    public TextMeshProUGUI m_CodeText;
    
    
    public bool m_isPopupMenuOpen = false;
    public string m_LobbyCode;


    void Start()
    {
        GameManager.Instance.UpdateState += UpdateText;
        GameManager.Instance.MatchFound += MatchFound;
        GameManager.Instance.LobbyCode += UpdateLobbyCode;
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

    public void UpdateLobbyCode(string code)
    {
        m_LobbyCode = code;
    }

    public void ChangeRoundUI(RoundType round)
    {
        switch (round)
        {
            case RoundType.WaitingRoom:
                break;
            case RoundType.PossessionRound:
                startGameButton.SetActive(false);
                UpdateText("Possession Round");
                break;
            case RoundType.SeekingRound:
                StartCoroutine(DisableStatusText());
                numberOfPlayerLeftGUI.GetComponent<TextMeshProUGUI>().text = ": " + RoundManager.Instance.m_numberOfPlayers;
                numberOfPlayerLeftGUI.SetActive(true);
                break;
            case RoundType.EndGame:
                numberOfPlayerLeftGUI.SetActive(false);
                gameOverUI.SetActive(true);
                break;
        }
    }

    public void OnPopMenu(InputValue a)
    {
        if (m_isPopupMenuOpen)
        {
            m_popMenu.SetActive(false);
            m_isPopupMenuOpen = false;
        }
        else
        {
            m_popMenu.SetActive(true);
            m_isPopupMenuOpen = true;
            m_CodeText.text = m_LobbyCode;
        }
    }
    
    //coroutine that shows "seeking round" for a few seconds and then disappears.
    IEnumerator DisableStatusText()
    {
        UpdateText("Seeking Round");
        yield return new WaitForSeconds(10);
        TogglestatusText(false);
    }

}
