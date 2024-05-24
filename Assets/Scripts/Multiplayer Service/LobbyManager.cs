using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using CustomInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;



public class LobbyManager : MonoBehaviour
{
    [SerializeField] string Lobbycode;
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    public string playerName;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };

        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerName = "Atraxx" + UnityEngine.Random.Range(10, 99);
        Debug.Log(playerName);
    }

    private void Update()
    {

        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ListLobbies();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            JoinLobbybyCode(Lobbycode);
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

               await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

              Lobby lobby =  await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }


    public async void CreateLobby()
    {
        try
        {

        string lobbyName = "My Lobby ";
        int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false, // Estado del lobby si es privado o no.
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public,"Capture the flag")}, // Define un Modo de juego
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public,"Risk")}
                }

            };


        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;


        Debug.Log("Created Lobby!: " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            PrintPLayers(hostLobby);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbybyCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
         Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby with code " + lobbyCode);
            PrintPLayers(lobby);


        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT),
                    //new QueryFilter(QueryFilter.FieldOptions.S1,"Capture the flag", QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
        };
    }

    // Muestra a los jugadores en el lobby
    public void PrintPLayers()
    {
        PrintPLayers(joinedLobby);
    }

    public void PrintPLayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby: " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }


    // Esta funcion modifca el modo de juego de tu juego ejemplo: Deathmatch, CaptureFlag, One Bullet etc.
    private async void UpdateLobbyGameMode(string gamemode)
    {
        try
        {
           hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                { "GameMode",new DataObject(DataObject.VisibilityOptions.Public,gamemode)}
            }
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }


    }

    private async void UpdateLobbyMap(string map)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                { "Map",new DataObject(DataObject.VisibilityOptions.Public,map)}
            }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }


    }

    // Esta funcion se usa para cambair el username en tiempo real
    public async void UpdatePlayerName(string newPlayername)
    {
        try
        {
            playerName = newPlayername;
           await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                  {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void LeaveLobby()
    {
        try
        {
           await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    // Esta funcion sirve para echar al jugador de lobby a la fuerza
    public async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id); // el Players[1] el numero 1 indica el jugador que vas a echar empezando el player 1 como el 0 de su ID
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    public async void DeleteLobby()
    {
        try
        {
           await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
