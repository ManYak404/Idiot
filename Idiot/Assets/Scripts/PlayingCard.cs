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

    public bool LargerThanPlayingCard(PlayingCard otherCard)
    {
        if (otherCard == null)
        {
            Debug.LogError("Cannot compare to a null card.");
            return false;
        }

        if(this.suit != otherCard.suit)
        {
            // If suits are different, compare with the trump suit
            if (Manager.managerInstance.trumpSuit == this.suit)
            {
                return true; // This card is a trump card
            }
            else if (Manager.managerInstance.trumpSuit == otherCard.suit)
            {
                return false; // Other card is a trump card
            }
            else
            {
                // If neither card is a trump, they are incomparable
                return false;
            }
        }

        return this.suit == otherCard.suit && this.rank > otherCard.rank;
    }
}
