using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
public class LobbyHandler : MonoBehaviour
{
    public bool isHost = true;
    public Lobby currentLobby;
    public string connectKey = "0";
    void Start()
    {
        connectKey = "0";
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentLobby != null)
        {
            Debug.Log(currentLobby.Name);
        }
    }
}
