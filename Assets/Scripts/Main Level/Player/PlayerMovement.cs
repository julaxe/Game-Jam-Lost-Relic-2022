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

    void UpdateServer()
    {
        if (InputDirection.Value.sqrMagnitude != 0.0f)
        {
            if (m_InputDirection.x != 0)
            {
                transform.localScale = new Vector2(-m_InputDirection.x, 1);
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
