using System.Collections;
using System.Collections.Generic;
using Main_Level;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class PlayerBehavior : NetworkBehaviour
{
    private NetworkObject _networkObject;
    
    //Network Variables

    //Variables
    public GameObject m_itemHeld;
    public GameObject m_CurrentItemPossessed;
    public GameObject m_itemInRange;
    public bool playerLost;
    [SerializeField] private GameObject m_camera;

    private PlayerVFxController vFxController;



    //Events
    public static event Action PlayHasItem;
    public static event Action GameOver;


    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
        vFxController = GetComponent<PlayerVFxController>();
    }

    void Start()
    {
        playerLost = false;
        //Creating camera for player
        if (IsLocalPlayer)
        {
            Instantiate(m_camera, this.transform);
        }
        //Subscribing to events
        RoundManager.Instance.changeRound += ChangeRound;
       
    }
    void Update()
    {
    }
    public void OnDestroyPossess(InputValue a)
    {
        
        if (!IsOwner) return;
        if (m_itemHeld == null || playerLost) { return; }
        if (RoundManager.Instance.currentRound == RoundType.PossessionRound)
        {
            PossesItem();
        }
        else // hide and seek round
        {
            DestroyItem();
        }
    }

    private void PossesItem()
    {
        m_CurrentItemPossessed = m_itemHeld;
        m_CurrentItemPossessed.GetComponent<ItemBehaviour>().AddPlayerToPossessList();

        vFxController.StartLink(m_CurrentItemPossessed);
    }

    private void DestroyItem()
    {
        //SubmitItemIDRequestServerRpc(m_itemHeld.GetComponent<Item>().IdNumber);
        m_itemHeld.GetComponent<ItemBehaviour>().DestroyItem();
        //RemoveItemFromGame(m_itemHeld.GetComponent<Item>().IdNumber);
        m_itemHeld = null;
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

    private void DropItem()
    {
        m_itemHeld.GetComponent<ItemBehaviour>().Unbind();
        m_itemHeld = null;
    }

    private void PickUpItem()
    {
        m_itemHeld = m_itemInRange;
        m_itemHeld.GetComponent<ItemBehaviour>().Bind(_networkObject);
    }

    public void GameOverForPlayer()
    {
        GameOver?.Invoke();
        playerLost = true;
        m_CurrentItemPossessed = null;
        Debug.Log("GameOver for you!");
    }
    private void ChangeRound()
    {

        if (RoundManager.Instance.currentRound  == RoundType.ResetGame)
        {
            playerLost = false;
            return;
        }


        if (RoundManager.Instance.currentRound  == RoundType.SeekingRound)
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
                m_CurrentItemPossessed.GetComponent<ItemBehaviour>().AddPlayerToPossessList();
            }
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
                return;
            }
        }
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

}
