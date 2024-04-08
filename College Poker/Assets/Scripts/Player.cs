using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public TextMeshProUGUI chipCountText;

    

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
    
    public void UpdateChipCountDisplay() {
        if (chipCountText != null) {
            chipCountText.text = $"Chips: {chips}";
        }
    }
    
    public void AddChips(int amount) {
        chips += amount;
        UpdateChipCountDisplay(); // Update the display after changing the chip count
    }
    
    public void SubtractChips(int amount) {
        // Ensure the player cannot have negative chips
        chips = Mathf.Max(0, chips - amount);
        
        // Update the UI
        UpdateChipCountDisplay();
    }
    
    public string GetHandAsString() {
        string handString = "";
        foreach (Card card in handCards) { // Assuming handCards is a List<Card> of the player's cards
            handString += card.ToString() + ", "; // Implement ToString() in Card to return a string representation of the card
        }
        return handString.TrimEnd(',', ' ');
    }

    

    // Additional methods as needed, e.g., to show cards, calculate hand strength...
}

