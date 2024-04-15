using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[System.Serializable]
public class Card : IComparable<Card> {
    public string suit;
    public int value;
    public Sprite cardImage;
    public Sprite cardBackImage; 
    public bool isFaceUp = false;

    public Card(string suit, int value, Sprite cardImage, Sprite cardBackImage = null) {
        this.suit = suit;
        this.value = value;
        this.cardImage = cardImage;
        this.cardBackImage = cardBackImage;
    }

    public string DisplayName {
        get {
            string valueName = value > 10 ? new string[] {"Jack", "Queen", "King", "Ace"}[value - 11] : value.ToString();
            return $"{valueName} of {suit}";
        }
    }

    public override string ToString() {
        return DisplayName;
    }

    public int CompareTo(Card other) {
        if (other == null) return 1;

        int valueComparison = value.CompareTo(other.value);
        if (valueComparison != 0) return valueComparison;

        return suit.CompareTo(other.suit); // Example: May need adjustments based on how suits are prioritized
    }

    // Method to get the prefab name
    public string GetPrefabName() {
        char suitInitial = suit[0]; // Assumes suit is "Clubs", "Diamonds", "Hearts", "Spades"
        string valueString = value == 10 ? "T" : // Adjusted for ten-value cards
            value >= 2 && value <= 9 ? value.ToString() :
            value == 11 ? "J" : 
            value == 12 ? "Q" : 
            value == 13 ? "K" : "A";
        return valueString + suitInitial.ToString().ToLower();
    }
    
 

    
    // Additional functionality as needed...
}

