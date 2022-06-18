using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject joinGameUI;
    public GameObject startGameButton;
    public GameObject m_inputFieldPassword;
    public GameObject m_popMenu;
    public TextMeshProUGUI m_CodeText;
    public bool m_isPopupMenuOpen;
    public string m_LobbyCode;


    void Start()
    {
        GameManager.Instance.UpdateState += UpdateText;
        GameManager.Instance.MatchFound += MatchFound;
        GameManager.Instance.LobbyCode += UpdateLobbyCode;


        m_isPopupMenuOpen = false;

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

}
