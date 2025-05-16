using UnityEngine;

[System.Serializable]
public class PlayingCard
{
    public Suit suit;
    public Rank rank;
    public bool isFaceUp = true; // Default to face down

    public PlayingCard(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
}
