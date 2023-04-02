using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILobbyHandler : MonoBehaviour
{
    public GameObject lobbyCreatePanel;
    public GameObject lobbyPanel;
    [SerializeField]
    TMPro.TMP_Text lobbyPlayersCount;
    [SerializeField]
    TMPro.TMP_Text lobbyName;
    [SerializeField]
    TMPro.TMP_InputField createInput;
    [SerializeField]
    TMPro.TMP_InputField connectInput;
    [SerializeField]
    LobbySystem lobbySystem;
    [SerializeField]
    GameObject startButton;
    public async void OnClickConnect()
    {
        await lobbySystem.ConnectToLobby(connectInput.text);
        startButton.SetActive(false);
    }
    public async void OnClickCreate()
    {
        await lobbySystem.CreateLobby(createInput.text);
        startButton.SetActive(true);
    }
    public void OnClickStart()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void ShowLobby(bool turn)
    {
        if(turn)
        {
            lobbyCreatePanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }
        else
        {
            lobbyCreatePanel.SetActive(true);
            lobbyPanel.SetActive(false);
        }
    }
    public void ChangePlayerCount(int playerCount, int maxPlayers)
    {
        lobbyPlayersCount.text = "Players Count: " + playerCount + "/" + maxPlayers;
    }
    public void ChangeLobbyName(string Name)
    {
        lobbyName.text = "Lobby: " + Name;
    }
}
