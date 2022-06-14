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
    EndGame
}
public class RoundManager : NetworkBehaviour
{
    public static event Action NextRound;
    public NetworkVariable<RoundType> m_changeRound = new NetworkVariable<RoundType>();
    public NetworkVariable<int> numberOfPlayersLeft = new NetworkVariable<int>();



    [Header("Round Settings")]
    [SerializeField]
    private float m_possessionRoundAmountOfTime;
    private RoundType m_currentRound;
    [SerializeField]
    private GameObject startGameButton;
    [SerializeField]
    private GameObject NumberOfPlayerLeftGUI;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        m_changeRound.Value = RoundType.WaitingRoom;
        
        PlayerBehavior.PlayHasItem += addNumberPlayers;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_changeRound.Value);
        
        switch (m_currentRound)
        {
            case RoundType.WaitingRoom:
                
                if (m_changeRound.Value == m_currentRound) { return; }

                m_currentRound = RoundType.PossessionRound;
                NextRound?.Invoke();
                
                break;
            case RoundType.PossessionRound:

                if (m_changeRound.Value == m_currentRound) { return; }
                
                NumberOfPlayerLeftGUI.SetActive(true);
                m_currentRound = RoundType.SeekingRound;
                NextRound?.Invoke();
                

                break;
            case RoundType.SeekingRound:

                NumberOfPlayerLeftGUI.GetComponent<TextMeshProUGUI>().text = "Number Of Players: " + numberOfPlayersLeft.Value.ToString();

                if (m_changeRound.Value == m_currentRound) { return; }

                m_currentRound = RoundType.EndGame;
                NextRound?.Invoke();
                NumberOfPlayerLeftGUI.SetActive(false);


                break;
            case RoundType.EndGame:
                break;
            default:
                break;
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartPossessionRound());
        Debug.Log("Game Started");
        startGameButton.SetActive(false);
    }


    IEnumerator StartPossessionRound()
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

    public void addNumberPlayers()
    {
        numberOfPlayersLeft.Value++;
    }

//     if(numberOfPlayersLeft == 1)
//        {
//            NextRound?.Invoke();
//    SubmitEventRequestServerRpc(RoundType.EndGame);
//}


    

}
