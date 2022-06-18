using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

    public static RoundManager Instance { get; private set; }
    //Variables
    [Header("Round Settings")]
    [SerializeField] private float m_possessionRoundAmountOfTime;

    private UIManager m_uIManager;
    
    public int m_numberOfPlayers = 0;
    public RoundType currentRound = RoundType.WaitingRoom;
    public int gameTime = 0;
    public event UnityAction changeRound;

    private void Awake()
    {
        //singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        m_uIManager = GetComponent<UIManager>();
    }

    void Start()
    {
        //PlayerBehavior.GameOver += PlayerLostRound;
    }

    void Update()
    {
        // switch (m_currentRound)
        // {
        //     case RoundType.WaitingRoom:
        //         
        //         //Setting next round variables 
        //         m_currentRound = RoundType.PossessionRound;
        //         NextRound?.Invoke();
        //         
        //         break;
        //
        //     case RoundType.PossessionRound:
        //         
        //         //Setting next round variables 
        //         NumberOfPlayerLeftGUI.SetActive(true);
        //         m_currentRound = RoundType.SeekingRound;
        //         NextRound?.Invoke();
        //         StartCoroutine(DisableStatusText());
        //         break;
        //
        //     case RoundType.SeekingRound:
        //
        //         //UI for number of players
        //         NumberOfPlayerLeftGUI.GetComponent<TextMeshProUGUI>().text = ": " + numberOfPlayersLeft.Value.ToString();
        //         
        //         //Setting next round variables 
        //         m_currentRound = RoundType.EndGame;
        //         NextRound?.Invoke();
        //         NumberOfPlayerLeftGUI.SetActive(false);
        //         GameOverUI.SetActive(true);
        //         break;
        //
        //     case RoundType.EndGame:
        //         break;
        //
        //     case RoundType.ResetGame:
        //         
        //         //Setting next round variables 
        //         GameOverUI.SetActive(false);
        //         m_currentRound = RoundType.WaitingRoom;
        //         
        //         break;
        //
        //     default:
        //         break;
        // }
    }

    
    //function used to change the round.
    public void ChangeRound(RoundType round)
    {
        currentRound = round;
        //initialize rounds
        switch (currentRound)
        {
            case RoundType.WaitingRoom:
                //we might want to reset everything here?
                break;
            case RoundType.PossessionRound:
                Debug.Log("Game Started");
                m_uIManager.ChangeRoundUI(RoundType.PossessionRound);
                StartCoroutine(CountDown(10, StartSeekingRound));
                break;
            case RoundType.SeekingRound:
                m_numberOfPlayers = NetworkManager.ConnectedClients.Count;
                m_uIManager.ChangeRoundUI(RoundType.SeekingRound);
                break;
            case RoundType.EndGame:
                break;
            case RoundType.ResetGame:
                break;
        }
        changeRound?.Invoke();
    }
    
    //button used in the UI to start the game
    public void StartPossessionRoundButton()
    {
        ChangeRound(RoundType.PossessionRound);
    }

    //function used in a coroutine
    private void StartSeekingRound()
    {
        ChangeRound(RoundType.SeekingRound);
    }
    
    IEnumerator CountDown(int seconds, Action func = null)
    {
        int counter = seconds;
        while (counter > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            counter--;
            Debug.Log(counter);
        }
        //do after countdown is done
        func?.Invoke();
    }
    
}
