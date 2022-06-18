using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Text;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager _instance;
    public static GameManager Instance => _instance;

    private string _lobbyId;

    private RelayHostData _hostData;
    private RelayJoinData _joinData;

    public bool IsRelayEnabled => Transport != null &&
                                  Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;
    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();


    //Setup events
    public UnityAction<string> UpdateState;
    public UnityAction<string> MatchFound;
    public UnityAction<string> LobbyCode;


    public GameObject m_inputField;
    public string currentLobbyCode;

    private void Awake()
    {
        // just a basic singleton
        if (_instance is null)
        {
            _instance = this;
            return;
        }

        Destroy(this);
    }

    async void Start()
    {
        //initialize unity services
        await UnityServices.InitializeAsync();

        //Setup events listeners
        SetupEvents();

        //Unity Login
        await SignInAnonymouslyAsync();

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;

    }



    #region UnityLogin

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            //Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            //Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.Log(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };
    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }


    #endregion

    #region Lobby
    public static async Task<RelayJoinData> JoinGame(string joinCode)
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //Ask Unity Services for allocation data based on a join code
        JoinAllocation allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);

        //Populate the joining data
        RelayJoinData data = new RelayJoinData
        {
            // WARNING allocation.RelayServer is deprecated. It's best to read from ServerEndpoints.
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,

            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Key = allocation.Key,
        };
        return data;
    }
    public async void FindLobbyByCode()
    {
        if (InputCodeEmpty())
        {
            UpdateState?.Invoke("Input a Code");
            return;
        }

        RelayJoinData A = await JoinGame(GetInputFieldCode());
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    A.IPv4Address,
                    A.Port,
                    A.AllocationIDBytes,
                    A.Key,
                    A.ConnectionData,
                    A.HostConnectionData);
        currentLobbyCode = GetInputFieldCode();
        NetworkManager.Singleton.StartClient();
    }

    //public async void FindMatch()
    //{



    //    Debug.Log("Looking for a lobby...");

    //    //if (InputPasswordEmpty())
    //    //{
    //    //    UpdateState?.Invoke("Input A Password");
    //    //    return;
    //    //}




    //    UpdateState?.Invoke("Looking for a lobby");
    //    try
    //    {
    //        //looking for a lobby

    //        // Add options to the matchmaking (mode, rank, etc...)
    //        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();



    //        // Quick-join a random lobby
    //        Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(GetInputPassword(), options);



    //        Debug.Log($"Joined lobby: {lobby.Id}");
    //        Debug.Log($"Lobby Players: {lobby.Players.Count}");

    //        //Retrieve the Relay code previously set in the create match
    //        string joinCode = lobby.Data["joinCode"].Value;

    //        Debug.Log($"Received code: {joinCode}");

    //        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

    //        // Create Object
    //        _joinData = new RelayJoinData
    //        {
    //            Key = allocation.Key,
    //            Port = (ushort)allocation.RelayServer.Port,
    //            AllocationID = allocation.AllocationId,
    //            AllocationIDBytes = allocation.AllocationIdBytes,
    //            ConnectionData = allocation.ConnectionData,
    //            HostConnectionData = allocation.HostConnectionData,
    //            IPv4Address = allocation.RelayServer.IpV4
    //        };

    //        //Set transport data
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
    //            _joinData.IPv4Address,
    //            _joinData.Port,
    //            _joinData.AllocationIDBytes,
    //            _joinData.Key,
    //            _joinData.ConnectionData,
    //            _joinData.HostConnectionData);

    //        //NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(GetInputPassword());


    //        // Finally start the client
    //        NetworkManager.Singleton.StartClient();




    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        UpdateState?.Invoke("Couldn't find a lobby - creating a lobby");
    //        Debug.Log($"Cannot find a lobby: {e}");
    //        //CreateMatch();
    //    }
    //}

    public async void HostLobbyByCode()
    {
        Debug.Log("Creating a new lobby...");

        //External connections
        int maxConnections = 5;

        try
        {

            Debug.Log("Creating Relay Object...");
            //Create Relay object

            Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);

            _hostData = new RelayHostData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };
            Debug.Log("Retrieving JoinCode...");
            // Retrieve JoinCode
            _hostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            string lobbyName = "game_lobby";
            int maxPlayers = 5;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;

            // Put the JoinCode in the lobby data, visible by every member
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _hostData.JoinCode)
                },
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            _lobbyId = lobby.Id;
            Debug.Log($"Created lobby: {lobby.Id}");

            //Heart beat the lobby every 15 seconds.
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            //Now that the Relay and Lobby are set...

            //Set Transports data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                _hostData.IPv4Address,
                _hostData.Port,
                _hostData.AllocationIDBytes,
                _hostData.Key,
                _hostData.ConnectionData);
            
            currentLobbyCode = _hostData.JoinCode;
            //NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;



            //Finally start host
            NetworkManager.Singleton.StartHost();


        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            throw;
        }
    }

    //private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callBack)
    //{
    //    string password = Encoding.ASCII.GetString(connectionData);



    //    bool approveConnection = password == currentPassword;

    //    callBack(true, null, approveConnection, null, null);
    //}

    public bool InputCodeEmpty()
    {
        return string.IsNullOrEmpty(m_inputField.GetComponent<TMP_InputField>().text);
    }

    public string GetInputFieldCode()
    {
        return m_inputField.GetComponent<TMP_InputField>().text;
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateState?.Invoke("Lobby");
            LobbyCode?.Invoke(currentLobbyCode);

        }
        if (NetworkManager.Singleton.IsHost)
        {
            MatchFound?.Invoke("host");
        }

    }


    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("Lobby Heartbeat");
            yield return delay;
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void OnDestroy()
    {
        //We need to delete the lobby when we're not using it
        Lobbies.Instance.DeleteLobbyAsync(_lobbyId);

        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;

    }

    #endregion

    #region Relay

    // RelayHostData represents the necessary information
    // for a Host to host a game on Relay
    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }

    public struct RelayJoinData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }

    #endregion
}
