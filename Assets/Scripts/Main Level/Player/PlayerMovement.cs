using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{

    [Header("Input")]
    private PlayerInput m_inputs;
    private InputAction m_movement, m_destroy, m_pickup;
    [SerializeField] private Vector2 m_InputDirection;
    [SerializeField] private float m_speed;
    [SerializeField] private PlayerFOV m_fov;
    [SerializeField] private GameObject m_shadow;
    
    private void Start()
    {
        if (IsLocalPlayer)
        {
            GameObject temp = Instantiate(m_shadow);
            m_fov = temp.GetComponentInChildren<PlayerFOV>();
            temp.transform.position = this.transform.position;
        }
    }

    void UpdateScale()
    {
        if (m_InputDirection.sqrMagnitude != 0.0f)
        {
            if (m_InputDirection.x > 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (m_InputDirection.x < 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
        }
    }

    void UpdateMovement()
    {
        Vector2 newPos = (Vector2)transform.position + m_InputDirection * m_speed * Time.deltaTime;
        transform.position = newPos;
        //transform.Translate(m_InputDirection * m_speed * Time.deltaTime);
    }

    void Update()
    {
        if (IsLocalPlayer)
        {

            //m_shadow.transform.position = transform.position;
            m_fov.setOrigin(this.transform.position);

        }
        if (!IsClient) return;
        
        UpdateScale();
        UpdateMovement();
    }

    public void OnMovement(InputValue a)
    {
        m_InputDirection = a.Get<Vector2>();
    }
    
}
