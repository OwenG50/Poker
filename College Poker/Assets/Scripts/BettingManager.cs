using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class BettingManager : MonoBehaviour
{
    public List<Player> players;
    private int currentPlayerIndex;
    private int currentBet;
    private int totalPot;

    public void SetPlayers(List<Player> players)
    {
        this.players = players;
        Initialize(players);
    }
    
    
    public void Initialize(List<Player> players)
    {
        this.players = players;
        currentPlayerIndex = 0;
        currentBet = 0;
        totalPot = 0;
    }
    
    public void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        if (players[currentPlayerIndex].isFolded || players[currentPlayerIndex].IsAllIn)
        {
            NextPlayer(); // Skip folded or all-in players automatically
        }
    }

    public void PlayerFold()
    {
        players[currentPlayerIndex].Fold();
        NextPlayer();
    }

    public void PlayerCheck()
    {
        if (players[currentPlayerIndex].CurrentBet < currentBet)
        {
            Debug.Log("Check not allowed, player must call or raise.");
            return;
        }

        NextPlayer();
    }


    public void PlayerCall()
    {
        int chipsNeeded = currentBet - players[currentPlayerIndex].CurrentBet;
        if (players[currentPlayerIndex].chips >= chipsNeeded)
        {
            players[currentPlayerIndex].UpdateChips(-chipsNeeded);
            players[currentPlayerIndex].CurrentBet = currentBet;
            totalPot += chipsNeeded;
        }
        else
        {
            Debug.LogError("Nt enough chips to call");
        }
    }
    
    public void PlayerRaise(int raiseAmount)
    {
        if (raiseAmount > players[currentPlayerIndex].chips)
        {
            Debug.LogError("Not enough chips to raise.");
            return;
        }

        int totalNeeded = currentBet + raiseAmount - players[currentPlayerIndex].CurrentBet;
        players[currentPlayerIndex].UpdateChips(-totalNeeded);
        players[currentPlayerIndex].CurrentBet += totalNeeded;
        currentBet += raiseAmount;
        totalPot += totalNeeded;
        NextPlayer();
    }

    public void PlayerAllIn()
    {
        int allInAmount = players[currentPlayerIndex].chips;
        players[currentPlayerIndex].UpdateChips(-allInAmount);
        players[currentPlayerIndex].CurrentBet += allInAmount;
        players[currentPlayerIndex].IsAllIn = true;
        totalPot += allInAmount;
        if (players[currentPlayerIndex].CurrentBet > currentBet)
        {
            currentBet = players[currentPlayerIndex].CurrentBet; // Adjust current bet if it's a raise
        }
        NextPlayer();
    }

    public void DisplayCurrentPot()
    {
        Debug.Log("Current Pot: " + totalPot);
    }
    
    
}