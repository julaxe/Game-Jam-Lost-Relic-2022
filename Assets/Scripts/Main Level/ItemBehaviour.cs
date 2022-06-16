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
    
    public void AddPlayerToPossessList(NetworkObject player)
    {
        possessList.Add(player);
    }

    public List<NetworkObject> GetPossessList()
    {
        return possessList;
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
    

    #endregion
    
}
