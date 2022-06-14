using System.Collections;
using System.Collections.Generic;
using Main_Level;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerBehavior : NetworkBehaviour
{
    public RoundType m_currentRound;
    public GameObject m_itemHeld;

    public GameObject m_CurrentItemPossessed;

    public GameObject m_itemInRange;

    // Start is called before the first frame update
    void Start()
    {
        m_currentRound = RoundType.WaitingRoom;
        RoundManager.NextRound += ChangeRound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDestroypPossess(InputValue a)
    {
        if(m_itemHeld == null) { return; }
        if(m_currentRound == RoundType.PossessionRound)
        {
            m_itemHeld = m_CurrentItemPossessed;
        }
        else
        {
            Destroy(m_itemHeld.gameObject);
            m_itemHeld = null;
        }
    }

    public void OnPickup(InputValue a)
    {
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
        if(m_currentRound == RoundType.EndGame)
        {
            m_currentRound = RoundType.WaitingRoom;
            return;
        }

        m_currentRound++; 
    }

}
