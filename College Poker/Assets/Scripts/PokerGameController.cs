using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class PokerGameController : MonoBehaviour {
    
    // Lists
    public List<GameObject> cardPrefabs; // Assign all card prefabs in the inspector
    private List<Card> deck = new List<Card>(); //List of all cards in the deck
    private List<Card> communityCards = new List<Card>(); //List of all dealt community cards
    public List<Player> players = new List<Player>(); // list of all players in the game
    private List<GameObject> playerChipCounts = new List<GameObject>(); // list of all the players chip counts
    public int startingChips = 1000; // Example starting chips, adjust as needed
    
    // Etc...
    public Transform[] playerHandPositions; //Assign in inspector
    public Transform communityCardsContainer;
    public int numberOfPlayers; // Can be adjusted to change the number of players
    public GameState currentState;
    
    // UI Components
    [SerializeField]
    private GameObject chipCountPrefab; // Assign in Inspector
    [SerializeField]
    private Canvas canvas; // Reference to your UI Canvas

    void Start() {
        
        TransitionToState(GameState.Setup);
        
        // Other game setup..
    }
    
    
    public void TransitionToState(GameState newState) {
        currentState = newState;
        OnEnterState(newState);
    }
    
    // What happens in each state
    private void OnEnterState(GameState state) {
        switch (state) {
            case GameState.Setup:
                // Initialize game setup here for Deck and Players
                InitializeDeck();
                InitializePlayers();
                InitializePlayerUI();
                break;
            case GameState.PreFlop:
                // Shuffle and Deal Cards to all players
                ShuffleDeck();
                DealCardsToPlayers();
                break;
            case GameState.Flop:
                // Reveal flop cards
                DealFlop();
                break;
            case GameState.Turn:
                DealTurn();
                break;
            case GameState.River:
                DealRiver();
                break;
            case GameState.Showdown:
                // Determine winner
                break;
            case GameState.EndRound:
                // Cleanup and prepare for next round
                break;
            default:
                Debug.LogError("Unhandled state " + currentState.ToString());
                break;
        }
    }
    
    // Example method to move to the next state
    public void NextState() {
        switch (currentState) {
            case GameState.Setup:
                TransitionToState(GameState.PreFlop);
                break;
            case GameState.PreFlop:
                TransitionToState(GameState.Flop);
                break;
            case GameState.Flop:
                TransitionToState(GameState.Turn);
                break;
            case GameState.Turn:
                TransitionToState(GameState.River);
                break;
            case GameState.Showdown:
                TransitionToState(GameState.Showdown);
                break;
            case GameState.EndRound:
                TransitionToState(GameState.Setup);
                break;
        }
    }
    
    // All possible game states
    public enum GameState {
        Setup,
        PreFlop,
        Flop,
        Turn,
        River,
        Showdown,
        EndRound
    }
    

    void InitializeDeck() {     //Initialize the Deck
        foreach (GameObject cardPrefab in cardPrefabs) {
            string cardName = cardPrefab.name.ToUpper();
            string valuePart = cardName.Substring(0, cardName.Length - 1);
            string suit = cardName.Substring(cardName.Length - 1);
            int value = GetCardValue(valuePart);
            Sprite cardImage = cardPrefab.GetComponent<SpriteRenderer>().sprite;
            Card card = new Card(ConvertSuit(suit), value, cardImage);
            deck.Add(card);
        }
        Debug.Log("Deck Initialized");
    }

    void InitializePlayers() {
        players.Clear(); // Clear existing players list
    
        for (int i = 0; i < numberOfPlayers; i++) {
            Player newPlayer = new Player(startingChips);
            players.Add(newPlayer);
        
            // Create a new empty GameObject for the player's hand
            GameObject playerHandContainer = new GameObject($"Player {i + 1}'s Hand");

            // Set the position of the player's hand container
            if (i < playerHandPositions.Length) {
                playerHandContainer.transform.position = playerHandPositions[i].position;
            }

            // Store the reference to the player's hand container in the Player class
            newPlayer.handContainer = playerHandContainer;
        }
    }
    
    int GetCardValue(string valuePart) {
        switch (valuePart) {
            case "A": return 14;
            case "K": return 13;
            case "Q": return 12;
            case "J": return 11;
            case "T": return 10;
            default: return int.Parse(valuePart);
        }
    }

    string ConvertSuit(string suitLetter) {
        switch (suitLetter) {
            case "C": return "Clubs";
            case "D": return "Diamonds";
            case "H": return "Hearts";
            case "S": return "Spades";
            default: return "Unknown";
        }
    }

    void ShuffleDeck() {
        for (int i = 0; i < deck.Count; i++) {
            Card temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("Deck Shuffled");
    }
    
    Card DrawCard() {
        if(deck.Count > 0) {
            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
        } else {
            Debug.LogError("Attempted to draw a card from an empty deck.");
            return null; // Return null if the deck is empty (should not happen in a correctly configured game)
        }
    }

    // Method to deal two cards to each player
    void DealCardsToPlayers() {
        for (int i = 0; i < players.Count; i++) {
            if (i < playerHandPositions.Length) {
                // Deal the first card
                Card firstCard = DrawCard();
                players[i].AddCard(firstCard);
                // Instantiate the first card at the position of the player's hand container
                GameObject firstCardObj = InstantiateCard(firstCard, Vector3.zero, 0); // Position is now Vector3.zero because local position will be used
                firstCardObj.transform.SetParent(players[i].handContainer.transform, false);
                firstCardObj.transform.localPosition = new Vector3(0, 0, 0); // Local position is set after parenting

                // Deal the second card
                Card secondCard = DrawCard();
                players[i].AddCard(secondCard);
                // Instantiate the second card next to the first card
                GameObject secondCardObj = InstantiateCard(secondCard, Vector3.zero, 1); // Position is now Vector3.zero because local position will be used
                secondCardObj.transform.SetParent(players[i].handContainer.transform, false);
                secondCardObj.transform.localPosition = new Vector3(1.5f, 0, 0); // Local position is set after parenting to place it next to the first card
            }
        }
        Debug.Log("Cards dealt to players and placed in their hands.");
    }
    
    public void DealFlop() {
        communityCards.Clear(); // Clear previous cards
        DrawCard(); // Burn card
        Vector3 startPosition = new Vector3(-4, 0, 0); // Example start position
        
        for (int i = 0; i < 3; i++) {
            Card card = DrawCard();
            communityCards.Add(card);
            InstantiateCard(card, startPosition + new Vector3(i * 1.5f, 0, 0), i, -1, true);
        }
        Debug.Log("Flop dealt. Community Cards: " + GetCardNames(communityCards));
    }
    
    public void DealTurn() {
        DrawCard(); // Burn card
        Card turnCard = DrawCard();
        communityCards.Add(turnCard); // Add the turn card to the community cards
    
        Vector3 startPosition = new Vector3(-4 + 3 * 1.5f, 0, 0); // Adjusting position based on the flop cards
        InstantiateCard(turnCard, startPosition, communityCards.Count - 1, -1, true);
        Debug.Log("Turn dealt. Community Cards: " + GetCardNames(communityCards));
        
    }

    public void DealRiver() {
        DrawCard(); // Burn card
        Card riverCard = DrawCard();
        communityCards.Add(riverCard);

        Vector3 startPosition = new Vector3(-4 + 4 * 1.5f, 0, 0); // Adjusting position based on the flop and turn cards
        InstantiateCard(riverCard, startPosition, communityCards.Count - 1, -1, true);
        Debug.Log("River dealt. Community Cards: " + GetCardNames(communityCards));
    }
    
    GameObject InstantiateCard(Card card, Vector3 startPosition, int cardIndex, int playerIndex = -1, bool isCommunityCard = false) {
        GameObject cardPrefab = cardPrefabs.Find(prefab => prefab.name.Equals(card.GetPrefabName(), StringComparison.OrdinalIgnoreCase));
        if (cardPrefab != null) {
            GameObject cardObj = Instantiate(cardPrefab, startPosition, Quaternion.identity, isCommunityCard ? communityCardsContainer : null);
            cardObj.transform.localScale = new Vector3(0.1f, 0.1f, 1); // Adjust scale as needed

            return cardObj;
        } else {
            Debug.LogError("Card prefab not found for: " + card.GetPrefabName());
            return null;
        }
    }
    
    // Utility method to convert a list of cards to string for debug logging
    string GetCardNames(List<Card> cards) {
        string cardNames = "";
        foreach (Card card in cards) {
            cardNames += card.ToString() + ", ";
        }
        return cardNames.TrimEnd(',', ' ');
    }
    
    void InitializePlayerUI() {
        for (int i = 0; i < numberOfPlayers; i++) {
            if (i < playerHandPositions.Length) {
                // Instantiate Chip Count UI to the right of the player's cards
                Vector3 chipPosition = players[i].handContainer.transform.position + new Vector3(2.5f, 0, 0); // Adjust the 2.5f offset as needed
                GameObject chipCountUI = Instantiate(chipCountPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
                RectTransform rt = chipCountUI.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(chipPosition.x, chipPosition.y); // Assuming chipPosition is calculated with screen space in mind
                chipCountUI.GetComponent<TextMeshProUGUI>().text = $"Chips: {players[i].chips}";

                playerChipCounts.Add(chipCountUI);
            }
        }
    }

    // Call whenever a player's chip count changes (e.g., after bets, wins, or losses) to update the display.
    void UpdatePlayerChipsUI() { 
        for (int i = 0; i < players.Count; i++) {
            if (i < playerChipCounts.Count) {
                playerChipCounts[i].GetComponent<Text>().text = $"Chips: {players[i].chips}";
            }
        }
    }
    
    
    
}