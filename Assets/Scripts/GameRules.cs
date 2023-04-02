using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameRules : NetworkBehaviour
{
    int playerCount = 0;
    List<PlayerController> players;
    public GameObject winnerTabObject;
    public UnityEngine.UI.Image winnerColor;
    public TMPro.TMP_Text winnerGold;
    PlayerController lastAlivePlayer;
    void Start()
    {
        players = new List<PlayerController>();
    }
    void Update()
    {
        if(!IsOwner) return;
        if(players.Count <= 1) return;
        lastAlivePlayer = null;
        int aliveCount = 0;
        foreach(var player in players)
        {
            if(player.health.Value > 0)
            {
                lastAlivePlayer = player;
                aliveCount++;
            }
        }
        if(aliveCount <= 1 && lastAlivePlayer != null)
        {
            winnerTabObject.SetActive(true);
            winnerColor.color = new Color(lastAlivePlayer.color_r.Value, lastAlivePlayer.color_g.Value, lastAlivePlayer.color_b.Value);
            winnerGold.text = "is winner! His gold is: " + lastAlivePlayer.goldCount.Value;
            ShowEndPanelClientRpc(winnerColor.color, lastAlivePlayer.goldCount.Value);
        }
    }
    public void AddPlayer(PlayerController player)
    {
        players.Add(player);
        playerCount++;
    }
    [ClientRpc]
    public void ShowEndPanelClientRpc(Color playerColor, int gold)
    {
        winnerTabObject.SetActive(true);
        winnerColor.color = playerColor;
        winnerGold.text = "is winner! His gold is: " + gold;
    }
}
