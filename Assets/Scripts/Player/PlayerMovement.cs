using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();



    [Header("Input")]
    private PlayerInput m_inputs;
    private InputAction m_movement, m_destroy, m_pickup;
    private Vector2 m_InputDirection;
    [SerializeField] private float m_speed;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ServerPositionRequest();
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    void ServerPositionRequest()
    {
        var randomPosition = GetRandomPositionOnPlane();
        transform.position = randomPosition;
        Position.Value = randomPosition;
    }
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandomPositionOnPlane();
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    void Update()
    {
        //if (m_inputs != null) { return; }
        //m_InputDirection = m_movement.ReadValue<Vector2>(); 
        
        Movement();
    }

    //public void Setinputs(PlayerInput playerInput)
    //{
    //    m_inputs = playerInput;

    //    m_movement = m_inputs.actions["Movement"];
    //    //m_destroy = m_inputs.actions["Destroy"];
    //    //m_pickup = m_inputs.actions["Pickup"];




    //}
    [ServerRpc]
    private void Movement()
    {
        transform.position += new Vector3(m_InputDirection.x, m_InputDirection.y, 0) * m_speed * Time.deltaTime;
            
    }

    public void OnMovement(InputValue a)
    {
        m_InputDirection = a.Get<Vector2>(); 
    }

}
