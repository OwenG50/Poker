using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class BettingManager : MonoBehaviour {
    public PokerGameController pokerGameController;
    private List<int> playerBets = new List<int>();
    private int currentPlayerIndex = 0; // Declare currentPlayerIndex here
    public TMP_InputField betAmountInputField;
    public Button NoButton;
    public Button BetButton;
    
    void Start() {
        InitializePlayerBets();
        BetButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);
        betAmountInputField.gameObject.SetActive(false);
    }

    void InitializePlayerBets() {
        for (int i = 0; i < pokerGameController.numberOfPlayers; i++) {
            playerBets.Add(0); // Initialize bets with 0 for each player
        }
    }

    // Call this method to simulate a player making a bet
    public void PlaceBet(int playerIndex, int amount) {
        if (playerIndex >= 0 && playerIndex < pokerGameController.numberOfPlayers) {
            playerBets[playerIndex] += amount;
            Debug.Log($"Player {playerIndex + 1} bets {amount}. Total bet: {playerBets[playerIndex]}");
            // Move to the next player's turn
            NextPlayerTurn();
        }
    }
    
    public void PlaceBetFromInputField() {
        // Validate input to ensure it's an integer
        int betAmount;
        if (int.TryParse(betAmountInputField.text, out betAmount)) {
            // Call your betting method here
            PlaceBet(currentPlayerIndex, betAmount);
        } else {
            Debug.LogError("Invalid bet amount entered.");
        }
        betAmountInputField.text = ""; // Clear the input field after submitting
    }
    
    public void Check() {
        // Assuming checking is only allowed if no bet has been made in the current round
        if (IsCheckAllowed(currentPlayerIndex)) {
            Debug.Log($"Player {currentPlayerIndex + 1} checks.");
            NextPlayerTurn(); // Move to the next player's turn
        } else {
            Debug.LogError("Check not allowed. There is already a bet on the table.");
        }
    }

    // Determines if a player is allowed to check
    private bool IsCheckAllowed(int playerIndex) {
        // A check is typically allowed if no bets have been placed in the current betting round.
        // This simple version checks if all bets are zero. Adjust logic as needed for your game rules.
        return playerBets.TrueForAll(bet => bet == 0);
    }

    public void HideBetInput() //Hides The Bet Input Field and buttons
    {
        BetButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);
        betAmountInputField.gameObject.SetActive(false);
    }
    
    public void ShowBetInput() //Shows The Bet Input Field and buttons
    {
        BetButton.gameObject.SetActive(true);
        NoButton.gameObject.SetActive(true);
        betAmountInputField.gameObject.SetActive(true);
    }
    
    

    // Advances to the next player's turn
    private void NextPlayerTurn() {
        currentPlayerIndex = (currentPlayerIndex + 1) % pokerGameController.numberOfPlayers;
        Debug.Log($"It's now Player {currentPlayerIndex + 1}'s turn.");
    }

    // Example method to start the next betting round
    public void NextBettingRound() {
        // Reset bets or handle them according to your game rules
        Debug.Log("Starting next betting round.");
        currentPlayerIndex = 0; // Optionally reset the turn to the first player
        // Additional logic to manage the betting round...
    }
    
    
    // Add more methods for checking, folding, and the overall betting logic...
}