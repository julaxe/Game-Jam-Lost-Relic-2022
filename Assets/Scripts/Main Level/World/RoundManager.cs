using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using TMPro;
using UnityEngine.Events;

public enum RoundType
{
    WaitingRoom,
    PossessionRound,
    SeekingRound,
    EndGame,
    ResetGame
}

//Note: Move UI stuff in UIManager if there is time

public class RoundManager : NetworkBehaviour
{
    //Network Variables
    public NetworkVariable<RoundType> m_changeRound = new NetworkVariable<RoundType>();
    public NetworkVariable<int> numberOfPlayersLeft = new NetworkVariable<int>();

    //Variables
    [Header("Round Settings")]
    [SerializeField] private float m_possessionRoundAmountOfTime;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject NumberOfPlayerLeftGUI;


    [SerializeField] private GameObject GameOverUI;
    private UIManager m_uIManager;
    
    private RoundType m_currentRound = RoundType.WaitingRoom;
    private int m_numberOfPlayers = 0;

    //Events
    public static event Action NextRound;

    private void Awake()
    {
        m_uIManager = GetComponent<UIManager>();
    }

    void Start()
    {
        SubmitEventRequestServerRpc(RoundType.WaitingRoom);

        //Subscribing to events
        PlayerBehavior.PlayHasItem += addNumberPlayers;
        PlayerBehavior.GameOver += PlayerLostRound;
    }

    void Update()
    {
        switch (m_currentRound)
        {
            case RoundType.WaitingRoom:
                
                //Check if next round starts
                if (m_changeRound.Value == m_currentRound) { return; }
                //Setting next round variables 
                m_currentRound = RoundType.PossessionRound;
                NextRound?.Invoke();
                m_uIManager.UpdateText("Possession Round");
                break;

            case RoundType.PossessionRound:

                //Check if next round starts
                if (m_changeRound.Value == m_currentRound) { return; }
                //Setting next round variables 
                NumberOfPlayerLeftGUI.SetActive(true);
                m_currentRound = RoundType.SeekingRound;
                NextRound?.Invoke();
                StartCoroutine(DisableStatusText());
                break;

            case RoundType.SeekingRound:

                //UI for number of players
                NumberOfPlayerLeftGUI.GetComponent<TextMeshProUGUI>().text = ": " + numberOfPlayersLeft.Value.ToString();
                //Check if next round starts
                if (m_changeRound.Value == m_currentRound) { return; }
                //Setting next round variables 
                m_currentRound = RoundType.EndGame;
                NextRound?.Invoke();
                NumberOfPlayerLeftGUI.SetActive(false);
                GameOverUI.SetActive(true);
                break;

            case RoundType.EndGame:
                break;

            case RoundType.ResetGame:
                
                //Check if next round starts
                if (m_changeRound.Value == m_currentRound) { return; }
                //Setting next round variables 
                GameOverUI.SetActive(false);
                m_currentRound = RoundType.WaitingRoom;
                
                break;

            default:
                break;
        }
    }
    
    public void addNumberPlayers()
    {
        if(!IsHost) { return; }
        m_numberOfPlayers++;

        SubmitNumberOfPlayersRequestServerRpc(m_numberOfPlayers);
    }
    public void PlayerLostRound()
    {
        if (!IsHost) { return; }

        m_numberOfPlayers--;
        SubmitNumberOfPlayersRequestServerRpc(m_numberOfPlayers);

        if (numberOfPlayersLeft.Value == 0 || numberOfPlayersLeft.Value == 1)
        {
            SubmitEventRequestServerRpc(RoundType.EndGame);
        }
    }
    public void StartPossessionRoundButton()
    {
        StartCoroutine(PossessionRoundTimer());
        Debug.Log("Game Started");
        startGameButton.SetActive(false);
        m_uIManager.UpdateText("Possession Round");
    }

    //Reset everything here
    public void ReturnToWaitingRoomButton()
    {
        GameOverUI.SetActive(false);
        m_currentRound = RoundType.WaitingRoom;
        NextRound?.Invoke();
        
        m_uIManager.UpdateText("Lobby");
        m_uIManager.TogglestatusText(true);

        if (!IsHost) { return; }
        startGameButton.SetActive(true);
        SubmitEventRequestServerRpc(RoundType.WaitingRoom);
    }
    IEnumerator PossessionRoundTimer()
    {
        m_currentRound = RoundType.PossessionRound;
        NextRound?.Invoke();
        if (IsOwner)
            SubmitEventRequestServerRpc(RoundType.PossessionRound);

        yield return new WaitForSeconds(m_possessionRoundAmountOfTime);
                
        StartCoroutine(DisableStatusText());
        Debug.Log("Start Seek Round");
        m_currentRound = RoundType.SeekingRound;
        NextRound?.Invoke();
        if (IsOwner)
            SubmitEventRequestServerRpc(RoundType.SeekingRound);
        
        NumberOfPlayerLeftGUI.SetActive(true);
    }

    IEnumerator DisableStatusText()
    {
        m_uIManager.UpdateText("Seeking Round");
        yield return new WaitForSeconds(10);
        m_uIManager.TogglestatusText(false);
    }

    [ServerRpc]
    void SubmitEventRequestServerRpc(RoundType a)
    {
        m_changeRound.Value = a;
    }

    [ServerRpc]
    void SubmitNumberOfPlayersRequestServerRpc(int a)
    {
        numberOfPlayersLeft.Value = a;
    }
}
