using System.Collections;
using System.Collections.Generic;
using Main_Level;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class PlayerBehavior : NetworkBehaviour
{
    public static event Action PlayHasItem;
    public static event Action GameOver;

    public NetworkVariable<int> itemId = new NetworkVariable<int>();


    public RoundType m_currentRound;
    public GameObject m_itemHeld;

    public GameObject m_CurrentItemPossessed;

    public GameObject m_itemInRange;
    public bool playerLost;


    // Start is called before the first frame update
    void Start()
    {
        playerLost = false;
        m_currentRound = RoundType.WaitingRoom;
        RoundManager.NextRound += ChangeRound;
        if (IsOwner)
            SubmitItemIDRequestServerRpc(0);
    }

    // Update is called once per frame
    void Update()
    {

        if (itemId.Value != 0)
        {
            RemoveItemFromGame(itemId.Value);
        }
    }
    public void OnDestroyPossess(InputValue a)
    {

        if (m_itemHeld == null || playerLost) { return; }
        if (m_currentRound == RoundType.PossessionRound)
        {
            m_CurrentItemPossessed = m_itemHeld;
            m_CurrentItemPossessed.GetComponent<Item>().AddPlayerToPossessList(this.gameObject);
        }
        else
        {
            if (IsOwner)
                SubmitItemIDRequestServerRpc(m_itemHeld.GetComponent<Item>().IdNumber);

            RemoveItemFromGame(m_itemHeld.GetComponent<Item>().IdNumber);
            m_itemHeld = null;
        }
    }

    public void OnPickup(InputValue a)
    {
        if (playerLost) { return; }
        if (!IsOwner) return;
        if (m_itemHeld != null)
        {
            DropItem();
            return;
        }

        if (m_itemInRange == null) { return; }

        PickUpItem();
    }

    void DropItem()
    {
        m_itemHeld.GetComponent<Item>().UnbindPlayer();
        m_itemHeld = null;
    }

    void PickUpItem()
    {
        m_itemHeld = m_itemInRange;
        m_itemHeld.GetComponent<Item>().BindPlayer(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            m_itemInRange = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            m_itemInRange = null;
        }
    }

    private void ChangeRound()
    {
        m_currentRound++;
        if (m_currentRound == RoundType.ResetGame)
        {
            m_currentRound = RoundType.WaitingRoom;
            playerLost = false;
            return;
        }


        if (m_currentRound == RoundType.SeekingRound)
        {
            if (m_CurrentItemPossessed == null)
            {
                Debug.Log("No Item");
                //Looking for nearest item
                float tempDistance = 1000;
                GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Item");
                for (int i = 0; i < tempArray.Length; i++)
                {
                    if (tempDistance > Vector3.Distance(transform.position, tempArray[i].transform.position))
                    {
                        tempDistance = Vector3.Distance(transform.position, tempArray[i].transform.position);
                        m_CurrentItemPossessed = tempArray[i];

                    }
                }


            }
            m_CurrentItemPossessed.GetComponent<Item>().AddPlayerToPossessList(this.gameObject);
            PlayHasItem?.Invoke();

        }

    }

    private void RemoveItemFromGame(int idNum)
    {
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < tempArray.Length; i++)
        {
            if (tempArray[i].GetComponent<Item>().IdNumber == idNum)
            {
                List<GameObject> tempPlayerList = tempArray[i].GetComponent<Item>().GetPossessList();
                for (int j = 0; j < tempPlayerList.Count; j++)
                {
                    tempPlayerList[j].GetComponent<PlayerBehavior>().playerLost = true;
                    tempPlayerList[j].GetComponent<PlayerBehavior>().m_CurrentItemPossessed = null;

                    GameOver?.Invoke();
                }
                Destroy(tempArray[i].gameObject);
                itemId.Value = 0;
                return;
            }
        }
    }

    [ServerRpc]
    void SubmitItemIDRequestServerRpc(int a)
    {
        itemId.Value = a;
    }

}
