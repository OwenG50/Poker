using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public HandRanks.Hands HandRanks { get; set; } // Allows player to be set a hand rank
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
    
    public List<int> GetKickers()
{
    List<int> values = handCards.Select(card => card.value).ToList();
    switch (HandRanks)
    {
        case global::HandRanks.Hands.Pair:
        case global::HandRanks.Hands.ThreeOfAKind:
        case global::HandRanks.Hands.FourOfAKind:
            // Find the value forming the pair/triple/quadruple, exclude from kickers
            int mainValue = values.GroupBy(v => v)
                                  .Where(g => g.Count() > 1)
                                  .OrderByDescending(g => g.Count())
                                  .ThenByDescending(g => g.Key)
                                  .Select(g => g.Key).FirstOrDefault();
            return values.Where(v => v != mainValue).ToList();
        case global::HandRanks.Hands.FullHouse:
            // Exclude triple and pair from kickers
            int tripleValue = values.GroupBy(v => v)
                                    .Where(g => g.Count() == 3)
                                    .Select(g => g.Key).FirstOrDefault();
            int pairValue = values.GroupBy(v => v)
                                  .Where(g => g.Count() == 2)
                                  .Select(g => g.Key).FirstOrDefault();
            return values.Where(v => v != tripleValue && v != pairValue).ToList();
        default:
            return values;
    }
}

    
    
    // Additional methods as needed, e.g., to show cards, calculate hand strength...
}

