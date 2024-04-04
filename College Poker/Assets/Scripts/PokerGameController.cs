using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class PokerGameController : MonoBehaviour {
    public List<GameObject> cardPrefabs; // Assign all card prefabs in the inspector
    private List<Card> deck = new List<Card>();
    private List<Card> communityCards = new List<Card>();
    private List<List<Card>> playerHands = new List<List<Card>>();
    public int numberOfPlayers; // Can be adjusted to change the number of players
    public Transform[] playerHandPositions; //Assign in inspector
    public Button dealFlopButton;
    public Button dealTurnButton;
    public Button dealRiverButton;
    private List<GameObject> instantiatedCommunityCards = new List<GameObject>();
    public List<List<GameObject>> instantiatedPlayerCards = new List<List<GameObject>>();
    public Sprite foldedCardSprite;

    void Start() {
        InitializeDeck();
        Debug.Log("Deck initialized with " + deck.Count + " cards.");

        ShuffleDeck();
        Debug.Log("Deck shuffled.");

        DealHands();
        Debug.Log("Hands dealt to " + numberOfPlayers + " players.");
        

        // Other game setup..
    }

    public void DisplayPlayerHands() {
        for (int playerIndex = 0; playerIndex < playerHandPositions.Length; playerIndex++) {
            Vector3 startPosition = playerHandPositions[playerIndex].position;
            List<Card> hand = playerHands[playerIndex];
            for (int cardIndex = 0; cardIndex < hand.Count; cardIndex++) {
                // This method now uses the Transform position directly
                InstantiateCard(hand[cardIndex], startPosition + new Vector3(cardIndex * 2f, 0, 0), 0); // Adjust spacing as needed
            }
        }
    }

    
    void InitializeDeck() {
        foreach (GameObject cardPrefab in cardPrefabs) {
            string cardName = cardPrefab.name.ToUpper();
            string valuePart = cardName.Substring(0, cardName.Length - 1);
            string suit = cardName.Substring(cardName.Length - 1);
            int value = GetCardValue(valuePart);
            Sprite cardImage = cardPrefab.GetComponent<SpriteRenderer>().sprite;
            Card card = new Card(ConvertSuit(suit), value, cardImage);
            deck.Add(card);
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
    }

    void DealHands() {
        for (int i = 0; i < numberOfPlayers; i++) {
            List<Card> hand = new List<Card> { DrawCard(), DrawCard() };
            playerHands.Add(hand);
        }
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

    public void DealFlop() {
        communityCards.Clear(); // Clear previous cards
        DrawCard(); // Burn card
        Vector3 startPosition = new Vector3(-4, 0, 0); // Example start position
        for (int i = 0; i < 3; i++) {
            Card card = DrawCard();
            communityCards.Add(card);
            InstantiateCard(card, startPosition, i);
        }
        dealFlopButton.gameObject.SetActive(false);
    }


    public void DealTurn() {
        DrawCard(); // Burn card
        Card turnCard = DrawCard();
        communityCards.Add(turnCard); // Add the turn card to the community cards
    
        // Updated call to match the expected method signature
        Vector3 startPosition = new Vector3(-4f, 0, 0); // Adjust as needed
        InstantiateCard(turnCard, startPosition, communityCards.Count - 1);
        Debug.Log("Turn dealt. Community Cards: " + GetCardNames(communityCards));
        
        dealTurnButton.gameObject.SetActive(false);
    }

    public void DealRiver() {
        DrawCard(); // Burn card
        Card riverCard = DrawCard();
        communityCards.Add(riverCard); // Add the river card to the community cards
    
        // Updated call to match the expected method signature
        Vector3 startPosition = new Vector3(-4f, 0, 0); // Adjust as needed
        InstantiateCard(riverCard, startPosition, communityCards.Count - 1);
        Debug.Log("River dealt. Community Cards: " + GetCardNames(communityCards));
        dealRiverButton.gameObject.SetActive(false);
    }
    
    void InstantiateCard(Card card, Vector3 startPosition, int cardIndex, int playerIndex = -1) {
        GameObject cardPrefab = cardPrefabs.Find(prefab => prefab.name.Equals(card.GetPrefabName(), StringComparison.OrdinalIgnoreCase));
        if (cardPrefab != null) {
            GameObject cardObj = Instantiate(cardPrefab, startPosition + new Vector3(cardIndex * 2f, 0, 0), Quaternion.identity);
            cardObj.transform.localScale = new Vector3(0.15f, 0.15f, 1); // Adjust as needed

            // Add to the appropriate list
            if (playerIndex == -1) {
                instantiatedCommunityCards.Add(cardObj);
            } else {
                while (instantiatedPlayerCards.Count <= playerIndex) {
                    instantiatedPlayerCards.Add(new List<GameObject>());
                }
                instantiatedPlayerCards[playerIndex].Add(cardObj);
            }
        } else {
            Debug.LogError("Card prefab not found for: " + card.GetPrefabName());
        }
    }

    
    /// Utility method to convert a list of cards to string for debug logging
    string GetCardNames(List<Card> cards) {
        string cardNames = "";
        foreach (Card card in cards) {
            cardNames += card.ToString() + ", ";
        }
        return cardNames.TrimEnd(',', ' ');
    }

    public void StartNewHand()
    {
        // Destroy and clear community cards
        foreach (GameObject card in instantiatedCommunityCards) {
            Destroy(card);
        }
        instantiatedCommunityCards.Clear();

        // Destroy and clear player hands
        foreach (List<GameObject> hand in instantiatedPlayerCards) {
            foreach (GameObject card in hand) {
                Destroy(card);
            }
            hand.Clear();
        }
        instantiatedPlayerCards.Clear();

        // Reinitialize the game for a new hand
        deck.Clear();
        InitializeDeck();
        ShuffleDeck();
        playerHands.Clear(); // Clear player hands data
        DealHands(); // Optionally deal hands again immediately

        // Reset and show UI buttons
        dealFlopButton.gameObject.SetActive(true);
        dealTurnButton.gameObject.SetActive(true);
        dealRiverButton.gameObject.SetActive(true);
    }
    
    public void MarkHandAsFolded(int playerIndex) {
        // Assuming you have a reference to the player's hand GameObjects or card images
        foreach (var cardObj in instantiatedPlayerCards[playerIndex]) {
            cardObj.GetComponent<SpriteRenderer>().sprite = foldedCardSprite; // Changing the card to a folded graphic
            //cardObj.SetActive(false);
        }
    }

    
}
