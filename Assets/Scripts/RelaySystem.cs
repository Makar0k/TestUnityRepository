using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class RelaySystem : MonoBehaviour
{
    public string relayKey;
    LobbyHandler lobbyHandler;
    public void Start()
    {
        lobbyHandler = GameObject.Find("LobbyHandler").GetComponent<LobbyHandler>();
        if(lobbyHandler.isHost)
        {
            CreateRelay();
        }
        else
        {
            JoinRelay(lobbyHandler.connectKey);
        }
    }
    async void CreateRelay()
    {
        try
        {
            
            Allocation aloc = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(aloc.AllocationId);
            relayKey = joinCode;
            Debug.Log("Join code: " + joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                aloc.RelayServer.IpV4,
                (ushort)aloc.RelayServer.Port,
                aloc.AllocationIdBytes,
                aloc.Key,
                aloc.ConnectionData
            );
            NetworkManager.Singleton.StartHost();
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(lobbyHandler.currentLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { "key", new DataObject(DataObject.VisibilityOptions.Member, relayKey)}
                }
            });
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }
    async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Connecting to Relay " + joinCode);
            JoinAllocation jAloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                jAloc.RelayServer.IpV4,
                (ushort)jAloc.RelayServer.Port,
                jAloc.AllocationIdBytes,
                jAloc.Key,
                jAloc.ConnectionData,
                jAloc.HostConnectionData
            );
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }
}
