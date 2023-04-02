using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

public class LobbySystem : MonoBehaviour
{
    bool isClientInLobby = false;
    string playerName;
    [SerializeField]
    UILobbyHandler UIHandler;
    Lobby joinedLobby;
    [SerializeField]
    LobbyHandler lobbyHandler;
    float timeToUpdateLobby = 0.6f;
    float timerUpdateLobby = 0.6f;
    
    async void Start()
    {
        playerName = "Player" + Random.Range(0, 1000);
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    void FixedUpdate()
    {
        if(isClientInLobby)
        {
            if(timerUpdateLobby > 0)
            {
                timerUpdateLobby -= Time.fixedDeltaTime;
            }
            else
            {
                Task.Run(() => CheckForRelayKey());
                if(lobbyHandler.connectKey != "0" && lobbyHandler.connectKey != null && lobbyHandler.connectKey != " ")
                {
                    Debug.Log("вывы: " + lobbyHandler.connectKey);
                    SceneManager.LoadScene(1, LoadSceneMode.Single);
                }
                UIHandler.ChangeLobbyName(joinedLobby.Name);
                UIHandler.ChangePlayerCount(joinedLobby.Players.Count, joinedLobby.MaxPlayers);
                timerUpdateLobby = timeToUpdateLobby;
            }
        }
        if(lobbyHandler.isHost)
        {
            if(timerUpdateLobby > 0)
            {
                timerUpdateLobby -= Time.fixedDeltaTime;
            }
            else
            {
                Task.Run(() => UpdateLobbyInfo());
                UIHandler.ChangeLobbyName(joinedLobby.Name);
                UIHandler.ChangePlayerCount(joinedLobby.Players.Count, joinedLobby.MaxPlayers);
                timerUpdateLobby = timeToUpdateLobby;
            }
        }
    }
    public async Task CheckForRelayKey()
    {
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        string keyData = joinedLobby.Data["key"].Value;
            if(keyData != "0" && keyData != null)
            {
                Debug.Log(keyData);
                lobbyHandler.isHost = false;
                lobbyHandler.connectKey = keyData;
                return;
            }
    }
    public async Task UpdateLobbyInfo()
    {
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
    }
    public async Task CreateLobby(string _name)
    {
        try
        {
            string name = _name;
            int maxPlayers = 3;
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions() {
                    IsPrivate = false,
                    Player = new Player {
                        Data = new Dictionary<string, PlayerDataObject>()
                        {
                            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                        }
                    },
                    Data = new Dictionary<string, DataObject> {
                        { "key", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                    }
            };
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers, lobbyOptions); 
            Debug.Log("Created Lobby with name: " + name);
            lobbyHandler.isHost = true;
            GoToLobby();
        }
        catch (LobbyServiceException e) 
        {
            Debug.Log(e);
        }
    }
    public async Task ConnectToLobby(string name)
    {
        try
        {
            var queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            foreach(var lobby in queryResponse.Results)
            {
                if(lobby.Name == name)
                {
                    joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
                    Debug.Log("Connected to Lobby with name: " + name);
                    lobbyHandler.isHost = false;
                    isClientInLobby = true;
                    GoToLobby();
                    return;
                }
            }
            Debug.Log("Lobby not found!");
        }
        catch (LobbyServiceException e) 
        {
            Debug.Log(e);
        }
    }
    public async Task GoToLobby()
    {
        UIHandler.ShowLobby(true);
        lobbyHandler.currentLobby = joinedLobby;
        UIHandler.ChangeLobbyName(joinedLobby.Name);
        UIHandler.ChangePlayerCount(joinedLobby.Players.Count, joinedLobby.MaxPlayers);
    }
}
