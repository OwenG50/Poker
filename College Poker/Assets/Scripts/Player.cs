using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public List<Card> handCards = new List<Card>();
    public bool isFolded = false;
    public int chips;
    public GameObject handContainer;
    public bool IsTurn;
    public int CurrentBet;
    public bool IsCheckAllowed;
    public bool IsAllIn;
    public int ChipCount;

    public Player(int startingChips) {
        chips = startingChips;
    }

    public void AddCard(Card card) {
        handCards.Add(card);
    }

    public void Fold() {
        isFolded = true;
    }

    public void UpdateChips(int amount) {
        chips += amount;
    }

    // Reset player state for new round
    public void ResetForNewRound() {
        handCards.Clear();
        isFolded = false;
    }

    // Additional methods as needed, e.g., to show cards, calculate hand strength...
}

