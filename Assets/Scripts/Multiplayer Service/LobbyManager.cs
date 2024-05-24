using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using CustomInspector;

public class LobbyManager : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer;

    private void Update()
    {

        HandleLobbyHeartbeat();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ListLobbies();
        }
    }

    private void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;
            }
        }
    }

    public async void CreateLobby()
    {
        try
        {

        string lobbyName = "My Lobby ";
        int maxPlayers = 4;
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            hostLobby = lobby;

        Debug.Log("Created Lobby!: " + lobby.Name + " " + lobby.MaxPlayers);

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
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}
