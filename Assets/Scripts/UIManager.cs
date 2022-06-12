using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject playButton;
    void Start()
    {
        GameManager.Instance.UpdateState += UpdateText;
        GameManager.Instance.MatchFound += MatchFound;
    }

    public void UpdateText(string text)
    {
        statusText.text = text;
    }

    public void MatchFound()
    {
        playButton.SetActive(false);
    }
}
