using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    private PlayingCard trumpCard;
    public List<PlayingCard> cards = new List<PlayingCard>();

    void Start()
    {
        
    }

    public void StartDeck()
    {
        CreateDeck();
        ShuffleDeck();
        CreateTrumpSuit();
    }

    void CreateDeck()
    {
        cards.Clear();
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                cards.Add(new PlayingCard(suit, rank));
            }
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int rnd = Random.Range(i, cards.Count);
            (cards[i], cards[rnd]) = (cards[rnd], cards[i]);
        }
    }

    private void CreateTrumpSuit()
    {
        trumpCard = cards[0]; // Take the last card as the trump card

        Debug.Log($"Trump suit is: {trumpCard.suit}");
    }

    public PlayingCard GetTrumpCard()
    {
        return trumpCard;
    }

    public PlayingCard DrawCard()
    {
        if (cards.Count == 0) return null;
        PlayingCard card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }

    public bool isEmpty()
    {
        return cards.Count <= 0;
    }
}
