using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using TMPro;
public enum RoundType
{
    WaitingRoom,
    PossessionRound,
    SeekingRound,
    EndGame,
    ResetGame
}
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
    
    private RoundType m_currentRound;
    private int m_numberOfPlayers;

    //Events
    public static event Action NextRound;

    void Start()
    {
        SubmitEventRequestServerRpc(RoundType.WaitingRoom);
        m_currentRound = RoundType.WaitingRoom;

        m_numberOfPlayers = 0;

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
                break;

            case RoundType.PossessionRound:

                //Check if next round starts
                if (m_changeRound.Value == m_currentRound) { return; }
                //Setting next round variables 
                NumberOfPlayerLeftGUI.SetActive(true);
                m_currentRound = RoundType.SeekingRound;
                NextRound?.Invoke();
                break;

            case RoundType.SeekingRound:

                //UI for number of players
                NumberOfPlayerLeftGUI.GetComponent<TextMeshProUGUI>().text = "Number Of Players: " + numberOfPlayersLeft.Value.ToString();
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
    }

    //Reset everything here
    public void ReturnToWaitingRoomButton()
    {
        GameOverUI.SetActive(false);
        m_currentRound = RoundType.WaitingRoom;
        NextRound?.Invoke();
        
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
        
        Debug.Log("Start Seek Round");
        m_currentRound = RoundType.SeekingRound;
        NextRound?.Invoke();
        if (IsOwner)
            SubmitEventRequestServerRpc(RoundType.SeekingRound);
        
        NumberOfPlayerLeftGUI.SetActive(true);
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
