using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerBehavior : MonoBehaviour
{
    public RoundType m_currentRound;
    public GameObject m_itemHeld;

    public GameObject m_CurrentItemPossessed;

    public GameObject m_itemInRange;

    // Start is called before the first frame update
    void Start()
    {
        m_currentRound = RoundType.PossessionRound;
        RoundManager.StartRoundEvent += ChangeRound;
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
        if (m_itemHeld != null)
        {
            m_itemHeld.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
            m_itemHeld.transform.SetParent(null);
            m_itemHeld = null;
            return;
        }

        if (m_itemInRange == null) { return; }

        

        m_itemHeld = m_itemInRange;
        m_itemInRange.transform.SetParent(this.transform);
        m_itemInRange.transform.localPosition = new Vector3(0, -0.5f);
        m_itemInRange.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "HeldItem";

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            m_itemInRange = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            m_itemInRange = null;
        }
    }

    private void ChangeRound()
    {
        m_currentRound = RoundType.SeekingRound;
    }

}
