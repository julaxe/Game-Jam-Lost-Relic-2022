using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemBehaviour : NetworkBehaviour
{
    private NetworkObject _playerRef;
    private SpriteRenderer _spriteRenderer;
    
    public List<NetworkObject> possessList;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        possessList = new List<NetworkObject>();
    }

    private void Update()
    {
        if (_playerRef != null)
        {
            UpdatePositionWithPlayer_ServerRpc(_playerRef.transform.position);
        }
    }

    public void Bind(NetworkObject playerRef)
    {
        _playerRef = playerRef;
        UpdateLayer_ServerRpc(true);
    }

    public void Unbind()
    {
        _playerRef = null;
        UpdateLayer_ServerRpc(false);
    }
    
    public void AddPlayerToPossessList()
    {
        AddPlayerToPossessList_ServerRpc();
    }

    public NetworkObject GetPossessPlayer()
    {
        return _playerRef;
    }

    public List<NetworkObject> GetPossessList()
    {
        return possessList;
    }

    public void DestroyItem()
    {
        DestroyItem_ServerRpc();
    }
    #region Server-Client
    [ServerRpc(RequireOwnership = false)]
    void UpdatePositionWithPlayer_ServerRpc(Vector3 position)
    {
        UpdatePositionWithPlayer_ClientRpc(position);
    }

    [ClientRpc]
    void UpdatePositionWithPlayer_ClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateLayer_ServerRpc(bool top)
    {
        UpdateLayer_ClientRpc(top);
    }

    [ClientRpc]
    void UpdateLayer_ClientRpc(bool top)
    {
        _spriteRenderer.sortingLayerName = top ? "HeldItem" : "Default";
    }

    [ServerRpc(RequireOwnership = false)]
    void AddPlayerToPossessList_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderId = serverRpcParams.Receive.SenderClientId;

        NetworkObject playerRef = GetNetworkObject(senderId).GetComponent<ItemBehaviour>().GetPossessPlayer();
        possessList.Add(playerRef);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyItem_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        DestroyItem_ClientRpc();
    }

    [ClientRpc]
    void DestroyItem_ClientRpc()
    {
        Destroy(this.gameObject);
    }

    #endregion
    
}
