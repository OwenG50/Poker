using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandRanks : MonoBehaviour
{
    public enum Hands
    {
        HighCard = 1,
        Pair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        Straight = 5,
        Flush = 6,
        FullHouse = 7,
        FourOfAKind = 8,
        StraightFlush = 9
    }

    public Hands DetermineHands(List<Card> cards)
    {
        // First, sort the cards. Sorting by value 
        cards.Sort();
        
        //When you run DetermineHands(); it returns one of these possible hands as an enum
        if (IsStraightFlush(cards))
            return Hands.StraightFlush;
        else if (IsFourOfAKind(cards))
            return Hands.FourOfAKind;
        else if (IsFullHouse(cards))
            return Hands.FullHouse;
        else if (IsFlush(cards))
            return Hands.Flush;
        else if (IsStraight(cards))
            return Hands.Straight;
        else if (IsThreeOfAKind(cards))
            return Hands.ThreeOfAKind;
        else if (IsTwoPair(cards))
            return Hands.TwoPair;
        else if (IsPair(cards))
            return Hands.Pair;
        else
            return Hands.HighCard;

    }

    private bool IsStraightFlush(List<Card> cards)
    {
        return IsStraight(cards) && IsFlush(cards);
    }

    private bool IsFourOfAKind(List<Card> cards)
    {
        List<int> cardValues = new List<int>(); // New list to store cardValues for each card in hand
        foreach (Card card in cards)
        {
            cardValues.Add(card.value);
        }
        Dictionary<int, int> valueCount = new Dictionary<int, int>();

        // Count how many times each value appears
        foreach (int value in cardValues)
        {
            if (!valueCount.ContainsKey(value))
            {
                valueCount[value] = 1;
            }
            else
            {
                valueCount[value]++;
            }
        }

        // Check if any value appears four or more times
        foreach (int count in valueCount.Values)
        {
            if (count >= 4)
            {
                return true;
            }
        }

        // No four of a kind found
        return false;
        
    }

    private bool IsFullHouse(List<Card> cards)
    {
        return IsThreeOfAKind(cards) && IsPair(cards);
    }

    private bool IsFlush(List<Card> cards)
    {
        List<string> cardSuits = new List<string>(); // New list to store cardSuits for each card in hand
        
        foreach (Card card in cards)
        {
            cardSuits.Add(card.suit);
        }

        Dictionary<string, int> suitCounts = new Dictionary<string, int>();

        // Count the occurrences of each suit
        foreach (string suit in cardSuits)
        {
            if (!suitCounts.ContainsKey(suit))
            {
                suitCounts[suit] = 1;
            }
            else
            {
                suitCounts[suit]++;
            }
        }

        // Check if any suit occurs five or more times
        foreach (int count in suitCounts.Values)
        {
            if (count >= 5)
            {
                return true;
            }
        }

        // No flush found
        return false;
    }

    private bool IsStraight(List<Card> cards)
    {
        List<int> cardValues = new List<int>(); // New list to store cardValues for each card in hand
        foreach (Card card in cards)
        {
            cardValues.Add(card.value);
        }
        cardValues.Sort(); // Sorts cardValues in ascending order
        
        // Edge case for an ace acting as low card
        if (cardValues.Contains(2) && cardValues.Contains(3) && cardValues.Contains(4) && cardValues.Contains(5) && cardValues.Contains(14))
        {
            return true;
        }
        
        for (int i = 0; i <= cardValues.Count - 5; i++) // Testing regular straights
        {
            if (cardValues[i] == cardValues[i + 1] - 1 &&
                cardValues[i + 1] == cardValues[i + 2] - 1 &&
                cardValues[i + 2] == cardValues[i + 3] - 1 &&
                cardValues[i + 3] == cardValues[i + 4] - 1)
            {
                return true;
            }
        }

        return false;
    }
    private bool IsThreeOfAKind(List<Card> cards)
    {
        List<int> cardValues = new List<int>(); // New list to store cardValues for each card in hand
        foreach (Card card in cards)
        {
            cardValues.Add(card.value);
        }

        Dictionary<int, int> valueCount = new Dictionary<int, int>();

        // Count how many times each value appears
        foreach (int value in cardValues)
        {
            if (!valueCount.ContainsKey(value))
            {
                valueCount[value] = 1;
            }
            else
            {
                valueCount[value]++;
            }
        }

        // Check if any value appears two times
        foreach (int count in valueCount.Values)
        {
            if (count == 3)
            {
                return true;
            }
        }

        // No pair of a kind found
        return false;
    }
    private bool IsTwoPair(List<Card> cards)
    {
        List<int> cardValues = new List<int>(); // New list to store cardValues for each card in hand
        foreach (Card card in cards)
        {
            cardValues.Add(card.value);
        }
        Dictionary<int, int> valueCounts = new Dictionary<int, int>();

        // Count the occurrences of each value
        foreach (int value in cardValues)
        {
            if (!valueCounts.ContainsKey(value))
            {
                valueCounts[value] = 1;
            }
            else
            {
                valueCounts[value]++;
            }
        }

        // Count the number of pairs
        int pairCount = 0;
        foreach (int count in valueCounts.Values)
        {
            if (count == 2)
            {
                pairCount++;
            }
        }

        // Check if two pairs exist
        return pairCount >= 2;
    }
    private bool IsPair(List<Card> cards)
    {
        List<int> cardValues = new List<int>(); // New list to store cardValues for each card in hand
        foreach (Card card in cards)
        {
            cardValues.Add(card.value);
        }

        Dictionary<int, int> valueCount = new Dictionary<int, int>();

        // Count how many times each value appears
        foreach (int value in cardValues)
        {
            if (!valueCount.ContainsKey(value))
            {
                valueCount[value] = 1;
            }
            else
            {
                valueCount[value]++;
            }
        }

        // Check if any value appears two times
        foreach (int count in valueCount.Values)
        {
            if (count == 2)
            {
                return true;
            }
        }

        // No pair of a kind found
        return false;
    }
    
}
    



