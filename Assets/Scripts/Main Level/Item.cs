using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
namespace Main_Level
{
    public class Item : NetworkBehaviour
    {
        public NetworkVariable<Vector2> ItemPosition = new NetworkVariable<Vector2>();

        private GameObject _playerRef;
        private SpriteRenderer _spriteRenderer;

        public int IdNumber;

        public List<GameObject> m_possessList;

        // Update is called once per frame
        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            if (IsOwner && IsClient)
            {
                UpdateClient();
            }

            UpdateServer();
        }

        void UpdateClient()
        {
            if (_playerRef)
            {
                _spriteRenderer.sortingLayerName = "HeldItem";
                SubmitItemsPositionServerRpc(_playerRef.transform.position);
            }
            else
            {
                _spriteRenderer.sortingLayerName = "Default";
            }
        }

        void UpdateServer()
        {
            transform.position = ItemPosition.Value;
        }
        [ServerRpc]
        void SubmitItemsPositionServerRpc(Vector2 newPos)
        {
            ItemPosition.Value = newPos;
        }

        public void BindPlayer(GameObject playerRef)
        {
            _playerRef = playerRef;
        }

        public void UnbindPlayer()
        {
            _playerRef = null;
        }

        public void AddPlayerToPossessList(GameObject player)
        {
            m_possessList.Add(player);
        }

        public List<GameObject> GetPossessList()
        {
            return m_possessList;
        }
    }
}
