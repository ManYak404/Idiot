using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<PlayingCard> cards = new List<PlayingCard>();

    void Start()
    {
        CreateDeck();
        ShuffleDeck();
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

    public PlayingCard DrawCard()
    {
        if (cards.Count == 0) return null;
        PlayingCard card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }
}
