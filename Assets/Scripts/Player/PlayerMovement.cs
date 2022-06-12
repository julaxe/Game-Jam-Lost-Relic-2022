using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();



    [Header("Input")]
    private PlayerInput m_inputs;
    private InputAction m_movement, m_destroy, m_pickup;
    private Vector2 m_InputDirection;
    [SerializeField] private float m_speed;
    

    void ServerPositionRequest()
    {
        Position.Value = (Vector2)transform.position + m_InputDirection * m_speed * Time.deltaTime;
    }
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = (Vector2)transform.position + m_InputDirection * m_speed * Time.deltaTime;
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    void Update()
    {
        if (m_InputDirection.SqrMagnitude() != 0)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                ServerPositionRequest();
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
            transform.position = Position.Value;
        }
    }
    

    public void OnMovement(InputValue a)
    {
        if (!IsOwner) return;
        
        m_InputDirection = a.Get<Vector2>();
        
        
    }

}
