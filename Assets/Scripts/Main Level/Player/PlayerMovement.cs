using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector2> InputDirection = new NetworkVariable<Vector2>();



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
    void UpdateServer()
    {
        if (InputDirection.Value.sqrMagnitude != 0.0f)
        {
            if (InputDirection.Value.x > 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (InputDirection.Value.x < 0)
            {
                transform.localScale = new Vector2(1, 1);
            }

            Vector2 newPos = (Vector2)transform.position + InputDirection.Value * m_speed * Time.deltaTime;
            transform.position = newPos;

        }
    }


    private void UpdateClient()
    {
        //Vector2 newPos = (Vector2) transform.position + (m_InputDirection * m_speed * Time.deltaTime);
        SubmitPositionRequestServerRpc(m_InputDirection);
    }


    void Update()
    {

        if (IsOwner && IsClient)
        {
            UpdateClient();
        }
        UpdateServer();
        //updating FOV
        if (IsLocalPlayer)
        {

            //m_shadow.transform.position = transform.position;
            m_fov.setOrigin(this.transform.position);

        }

    }

    public void OnMovement(InputValue a)
    {
        m_InputDirection = a.Get<Vector2>();
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector2 newPosition)
    {
        InputDirection.Value = newPosition;
    }


}
