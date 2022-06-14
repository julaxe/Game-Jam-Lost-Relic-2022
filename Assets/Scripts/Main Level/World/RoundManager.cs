using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
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


    [Header("Round Settings")]
    [SerializeField]
    private float m_possessionRoundAmountOfTime;
    private RoundType m_currentRound;
    



    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        m_changeRound.Value = RoundType.WaitingRoom;
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

                m_currentRound = RoundType.SeekingRound;
                NextRound?.Invoke();

                break;
            case RoundType.SeekingRound:

                if (m_changeRound.Value == m_currentRound) { return; }

                m_currentRound = RoundType.EndGame;
                NextRound?.Invoke();

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
    }


    IEnumerator StartPossessionRound()
    {
        m_currentRound = RoundType.PossessionRound;
        SubmitEventRequestServerRpc(RoundType.PossessionRound);

        yield return new WaitForSeconds(m_possessionRoundAmountOfTime);
        
        Debug.Log("Start Seek Round");
        m_currentRound = RoundType.SeekingRound;
        NextRound?.Invoke();
        SubmitEventRequestServerRpc(RoundType.SeekingRound);
    }

    [ServerRpc]
    void SubmitEventRequestServerRpc(RoundType a)
    {
        m_changeRound.Value = a;
    }

}
