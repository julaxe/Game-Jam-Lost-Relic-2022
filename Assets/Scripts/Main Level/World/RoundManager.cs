using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
public enum RoundType
{
    PossessionRound,
    SeekingRound,
    EndGame
}
public class RoundManager : NetworkBehaviour
{
    public static event Action StartRoundEvent;
    public NetworkVariable<bool> m_changeRound = new NetworkVariable<bool>();


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
        m_changeRound.Value = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(m_changeRound.Value);
        if (m_changeRound.Value)
        {
            StartRoundEvent?.Invoke();
            m_changeRound.Value = false;
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
        yield return new WaitForSeconds(m_possessionRoundAmountOfTime);
        Debug.Log("Start Seek Round");
        m_currentRound = RoundType.SeekingRound;
        StartRoundEvent?.Invoke();
        SubmitEventRequestServerRpc(true);
    }

    [ServerRpc]
    void SubmitEventRequestServerRpc(bool a)
    {
        m_changeRound.Value = a;
    }

}
