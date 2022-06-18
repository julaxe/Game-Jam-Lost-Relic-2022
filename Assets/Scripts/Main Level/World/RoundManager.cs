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
    [SerializeField] private int m_possessionRoundAmountOfTime;

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
                StartCoroutine(CountDown(m_possessionRoundAmountOfTime, StartSeekingRound));
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
        gameTime = seconds;
        while (gameTime > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            gameTime--;
            Debug.Log(gameTime);
        }
        //do after countdown is done
        func?.Invoke();
    }
    
}
