using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class PokerGameController : MonoBehaviour
{

    // Lists
    public List<GameObject> cardPrefabs; // Assign all card prefabs in the inspector
    private List<Card> deck = new List<Card>(); //List of all cards in the deck
    public List<Card> communityCards = new List<Card>(); //List of all dealt community cards
    public List<Player> players = new List<Player>(); // list of all players in the game
    public int startingChips; // Example starting chips, adjust as needed

    // Etc...
    public Transform[] playerHandPositions; //Assign in inspector
    public Transform communityCardsContainer;
    public int numberOfPlayers; // Can be adjusted to change the number of players
    public GameState currentState;

    // UI Components
    public TextMeshProUGUI chipCountTextPrefab; // Assign in Inspector
    public Transform chipCountParent;

    void Start()
    {

        TransitionToState(GameState.Setup);

        // Other game setup..
    }

    public void SubtractTest()
    {
        SubtractChipsFromPlayer(0, 50);
    }

    public void AddTest()
    {
        AddChipsToPlayer(2, 50);
    }


    public void TransitionToState(GameState newState)
    {
        currentState = newState;
        OnEnterState(newState);
    }

    // What happens in each state
    private void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Setup:
                // Initialize game setup here for Deck and Players
                InitializeDeck();
                InitializePlayers();
                NextState();
                break;
            case GameState.PreFlop:
                // Shuffle and Deal Cards to all players
                ShuffleDeck();
                DealCardsToPlayers();
                break;
            case GameState.Flop:
                // Reveal flop cards
                DealFlop();
                DetermineHandRank();
                break;
            case GameState.Turn:
                DealTurn();
                DetermineHandRank();
                break;
            case GameState.River:
                DealRiver();
                DetermineHandRank();
                break;
            case GameState.Showdown:
                // Determine winner
                Debug.Log("Showdown State called");
                DetermineHandWinner();
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
    public void NextState()
    {
        switch (currentState)
        {
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
            case GameState.River:
                TransitionToState(GameState.Showdown);
                break;
            case GameState.Showdown:
                TransitionToState(GameState.EndRound);
                break;
            case GameState.EndRound:
                TransitionToState(GameState.Setup);
                break;
        }
    }

    // All possible game states
    public enum GameState
    {
        Setup,
        PreFlop,
        Flop,
        Turn,
        River,
        Showdown,
        EndRound
    }


    void InitializeDeck()
    {
        //Initialize the Deck
        foreach (GameObject cardPrefab in cardPrefabs)
        {
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

    void InitializePlayers()
    {
        players.Clear(); // Clear existing players list

        for (int i = 0; i < numberOfPlayers; i++)
        {
            Player newPlayer = new Player(startingChips);
            players.Add(newPlayer);

            Debug.Log($"Player {i + 1} Starting Chips: {newPlayer.chips}");

            // Create a new empty GameObject for the player's hand
            GameObject playerHandContainer = new GameObject($"Player {i + 1}'s Hand");

            // Set the position of the player's hand container
            if (i < playerHandPositions.Length)
            {
                playerHandContainer.transform.position = playerHandPositions[i].position;
            }

            // Store the reference to the player's hand container in the Player class
            newPlayer.handContainer = playerHandContainer;

            /// Instantiate chip count text under the UI parent
            TextMeshProUGUI chipCountText = Instantiate(chipCountTextPrefab, chipCountParent);
            chipCountText.transform.localScale = Vector3.one; // Ensure it's not scaled weirdly
            chipCountText.text = $"Chips: {newPlayer.chips}";

            // Adjust position if necessary. This example positions it at the origin of the parent.
            chipCountText.rectTransform.anchoredPosition = Vector3.zero;

            // Keep a reference in Player class if needed for updating later
            // // Assign the TextMeshProUGUI to the player
            newPlayer.chipCountText = chipCountText;

            float yOffset = 45;
            float xOffset = -30;

            // Position it near the player's hand
            PositionChipCountText(chipCountText, newPlayer.handContainer.transform.position, yOffset, xOffset);

        }

        foreach (var player in players)
        {
            player.UpdateChipCountDisplay(); // Ensure initial display is correct
        }

    }

    void PositionChipCountText(TextMeshProUGUI chipCountText, Vector3 worldPosition, float yOffset, float xOffset)
    {
        Canvas
            canvas = chipCountParent
                .GetComponent<Canvas>(); // Assuming chipCountParent is your UI parent within a Canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Convert world position to screen point
        if (Camera.main != null)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);

            // Convert screen point to RectTransform position
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera,
                out uiPosition);

            uiPosition.y -= yOffset;
            uiPosition.x -= xOffset;

            // Set the position of the TextMeshProUGUI object
            chipCountText.rectTransform.anchoredPosition = uiPosition;
        }
    }




    int GetCardValue(string valuePart)
    {
        switch (valuePart)
        {
            case "A": return 14;
            case "K": return 13;
            case "Q": return 12;
            case "J": return 11;
            case "T": return 10;
            default: return int.Parse(valuePart);
        }
    }

    string ConvertSuit(string suitLetter)
    {
        switch (suitLetter)
        {
            case "C": return "Clubs";
            case "D": return "Diamonds";
            case "H": return "Hearts";
            case "S": return "Spades";
            default: return "Unknown";
        }
    }

    void ShuffleDeck()
    {
        // Start from the last element and swap one by one.
        // We don't need to run for the first element that's why i > 0
        for (int i = deck.Count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            // Swap arr[i] with the element at random index
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }

        Debug.Log("Deck Shuffled");
    }


    Card DrawCard()
    {
        if (deck.Count > 0)
        {
            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
        }
        else
        {
            Debug.LogError("Attempted to draw a card from an empty deck.");
            return null; // Return null if the deck is empty (should not happen in a correctly configured game)
        }
    }

    // Method to deal two cards to each player
    void DealCardsToPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i < playerHandPositions.Length)
            {
                // Deal the first card
                Card firstCard = DrawCard();
                players[i].AddCard(firstCard);

                // Instantiate the first card at the position of the player's hand container
                GameObject firstCardObj =
                    InstantiateCard(firstCard, Vector3.zero, 0, i, false, 10f); // Adjusted for rotation
                firstCardObj.transform.SetParent(players[i].handContainer.transform, false);
                firstCardObj.transform.localPosition = new Vector3(0, 0, 0); // Local position is set after parenting

                // Deal the second card
                Card secondCard = DrawCard();
                players[i].AddCard(secondCard);

                // Instantiate the second card next to the first card
                GameObject secondCardObj =
                    InstantiateCard(secondCard, Vector3.zero, 1, i, false, -10f); // Adjusted for rotation
                secondCardObj.transform.SetParent(players[i].handContainer.transform, false);
                secondCardObj.transform.localPosition =
                    new Vector3(1f, 0, 0); // Local position is set after parenting to place it next to the first card

                string playerHand = $"Player {i + 1} Hand: ";
                foreach (Card card in players[i].handCards)
                {
                    playerHand += $"{card.ToString()}, ";
                }

                Debug.Log(playerHand.TrimEnd(',', ' '));
            }
        }

        Debug.Log("Cards dealt to players and placed in their hands.");

    }

    public void DealFlop()
    {
        communityCards.Clear(); // Clear previous cards
        DrawCard(); // Burn card
        Vector3 startPosition = new Vector3(-4, 0, 0); // Example start position

        for (int i = 0; i < 3; i++)
        {
            Card card = DrawCard();
            communityCards.Add(card);
            InstantiateCard(card, startPosition + new Vector3(i * 1.5f, 0, 0), i, -1, true);
        }

        Debug.Log("Flop dealt. Community Cards: " + GetCardNames(communityCards));
    }

    public void DealTurn()
    {
        DrawCard(); // Burn card
        Card turnCard = DrawCard();
        communityCards.Add(turnCard); // Add the turn card to the community cards

        Vector3 startPosition = new Vector3(-4 + 3 * 1.5f, 0, 0); // Adjusting position based on the flop cards
        InstantiateCard(turnCard, startPosition, communityCards.Count - 1, -1, true);
        Debug.Log("Turn dealt. Community Cards: " + GetCardNames(communityCards));
    }

    public void DealRiver()
    {
        DrawCard(); // Burn card
        Card riverCard = DrawCard();
        communityCards.Add(riverCard);

        Vector3 startPosition = new Vector3(-4 + 4 * 1.5f, 0, 0); // Adjusting position based on the flop and turn cards
        InstantiateCard(riverCard, startPosition, communityCards.Count - 1, -1, true);
        Debug.Log("River dealt. Community Cards: " + GetCardNames(communityCards));
    }

    GameObject InstantiateCard(Card card, Vector3 startPosition, int cardIndex, int playerIndex = -1,
        bool isCommunityCard = false, float zRotation = 0f)
    {
        GameObject cardPrefab = cardPrefabs.Find(prefab =>
            prefab.name.Equals(card.GetPrefabName(), StringComparison.OrdinalIgnoreCase));
        if (cardPrefab != null)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, zRotation);
            GameObject cardObj = Instantiate(cardPrefab, startPosition, rotation,
                isCommunityCard ? communityCardsContainer : null);
            cardObj.transform.localScale = new Vector3(0.1f, 0.1f, 1); // Adjust scale as needed
            return cardObj;
        }
        else
        {
            Debug.LogError("Card prefab not found for: " + card.GetPrefabName());
            return null;
        }
    }


    // Utility method to convert a list of cards to string for debug logging
    string GetCardNames(List<Card> cards)
    {
        string cardNames = "";
        foreach (Card card in cards)
        {
            cardNames += card.ToString() + ", ";
        }

        return cardNames.TrimEnd(',', ' ');
    }

    // Subtracts chips to player while taking in input values for who and amount
    void SubtractChipsFromPlayer(int playerIndex, int amount)
    {
        if (playerIndex >= 0 && playerIndex < players.Count)
        {
            players[playerIndex].SubtractChips(amount);
        }
        else
        {
            Debug.LogError("Invalid player index.");
        }
    }

    // Adds chips to player while taking in input values for who and amount
    void AddChipsToPlayer(int playerIndex, int amount)
    {
        if (playerIndex >= 0 && playerIndex < players.Count)
        {
            players[playerIndex].AddChips(amount);
        }
        else
        {
            Debug.LogError("Invalid player index.");
        }
    }

    // Useful to see how to call to a specific player and debug.log
    void LogPlayerHandsAndChips()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            string playerInfo = $"Player {i + 1}: Hand = [{player.GetHandAsString()}], Chips = {player.chips}";
            Debug.Log(playerInfo);
        }
    }

    // Iterates through each player and assigns them their current hand rank
    private void DetermineHandRank()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            HandRanks handRanks = gameObject.AddComponent<HandRanks>();
            
            List<Card> combinedCards = new List<Card>(communityCards);
            combinedCards.AddRange(player.handCards); //Combines player and community cards into one list

            player.HandRanks = handRanks.DetermineHands(combinedCards);
            
            Debug.Log($"Player {i+1} has a {player.HandRanks}"); //prints hands to debug console
        }
    }

    private void DetermineHandWinner()
    {
        List<Player> playersNotFolded = players.Where(p => !p.isFolded).ToList();

        if (playersNotFolded.Count > 0)
        {
            Player handWinner = playersNotFolded[0];
            List<Player> tiedPlayers = new List<Player> { handWinner };

            for (int i = 1; i < playersNotFolded.Count; i++)
            {
                if (handWinner.HandRanks < playersNotFolded[i].HandRanks)
                {
                    handWinner = playersNotFolded[i];
                    tiedPlayers.Clear();
                    tiedPlayers.Add(handWinner);
                }
                else if (handWinner.HandRanks == playersNotFolded[i].HandRanks)
                {
                    tiedPlayers.Add(playersNotFolded[i]);
                }
            }

            if (tiedPlayers.Count > 1)
            {
                var (winners, kicker) = DetermineWinnerByKickers(tiedPlayers);
                if (winners.Count > 1)
                {
                    string winnerNames = string.Join(", ", winners.Select(p => $"Player {players.IndexOf(p) + 1}"));
                    Debug.Log($"It's a tie between {winnerNames}!");
                }
                else
                {
                    Player winner = winners[0];
                    if (kicker.HasValue)
                    {
                        Debug.Log($"The Winner by kicker is Player {players.IndexOf(winner) + 1} with {winner.HandRanks} using a {GetCardNameFromValue(kicker.Value)} as a kicker!");
                    }
                    else
                    {
                        Debug.Log($"The Winner is Player {players.IndexOf(winner) + 1} with a {winner.HandRanks}");
                    }
                }
            }
            else
            {
                Debug.Log($"The Winner is Player {players.IndexOf(handWinner) + 1} with a {handWinner.HandRanks}");
            }
        }
        else
        {
            Debug.Log("No players remain in the hand.");
        }
    }

    public string GetCardNameFromValue(int cardValue)
    {
        switch (cardValue)
        {
            case 11:
                return "Jack";
            case 12:
                return "Queen";
            case 13:
                return "King";
            case 14:
                return "Ace";
            default:
                return $"{cardValue}";
        }
    }

    private (List<Player>, int?) DetermineWinnerByKickers(List<Player> tiedPlayers)
    {
        List<Player> currentBestPlayers = new List<Player> { tiedPlayers[0] };
        List<int> bestKickers = currentBestPlayers[0].GetKickers().OrderByDescending(v => v).ToList();
        int? decisiveKicker = null;

        foreach (Player player in tiedPlayers.Skip(1))
        {
            List<int> challengerKickers = player.GetKickers().OrderByDescending(v => v).ToList();
            bool isTied = true;
            for (int i = 0; i < Math.Min(bestKickers.Count, challengerKickers.Count); i++)
            {
                if (challengerKickers[i] > bestKickers[i])
                {
                    currentBestPlayers = new List<Player> { player };
                    bestKickers = challengerKickers;
                    decisiveKicker = challengerKickers[i];
                    isTied = false;
                    break;
                }
                else if (challengerKickers[i] < bestKickers[i])
                {
                    isTied = false;
                    break;
                }
            }

            if (isTied)
            {
                if (challengerKickers.SequenceEqual(bestKickers))
                {
                    currentBestPlayers.Add(player); // Add player to tied winners if all kickers match
                }
                else if (challengerKickers.Count > bestKickers.Count)
                {
                    currentBestPlayers = new List<Player> { player };
                    bestKickers = challengerKickers;
                    decisiveKicker = challengerKickers[bestKickers.Count];
                }
            }
        }

        return (currentBestPlayers, decisiveKicker);
    }




}
      


