using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoundType
{
    PossessionRound,
    SeekingRound,
    EndGame
}
public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    [SerializeField]
    private float m_possessionRoundAmountOfTime;
    private RoundType m_currentRound;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartPossessionRound());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartPossessionRound()
    {
        m_currentRound = RoundType.PossessionRound;
        yield return new WaitForSeconds(m_possessionRoundAmountOfTime);
        Debug.Log("Start Seek Round");
        m_currentRound = RoundType.SeekingRound;
    }



}
