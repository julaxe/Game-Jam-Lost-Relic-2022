using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemBehaviour : NetworkBehaviour
{
    private NetworkObject _playerRef;

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
    }

    public void Unbind()
    {
        _playerRef = null;
    }

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
}
