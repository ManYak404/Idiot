using UnityEngine;

public class CardVisual : MonoBehaviour
{
    //public SpriteRenderer renderer;
    private PlayingCard cardData;

    public void SetCard(PlayingCard card)
    {
        Sprite cardSprite = null;
        string path = "";
        //make sure incoming card exists
        if(card == null)
        {
            Debug.LogError("Card is null. Cannot set card visual.");
            return;
        }

        cardData = card;
        
        //set renderer depending on card state
        if(card.isFaceUp)
        {
            path = $"Cards/card-{card.suit}-{(int)card.rank}"; // e.g., Resources/Cards/card-hearts-1.png
            cardSprite = Resources.Load<Sprite>(path);
            gameObject.GetComponent<SpriteRenderer>().sprite = cardSprite;
        }
        else
        {
            path = $"Cards/card-back1.png"; // e.g., Cards/card-back1
            cardSprite = Resources.Load<Sprite>(path);
            gameObject.GetComponent<SpriteRenderer>().sprite = cardSprite;
        }

        gameObject.name = $"{card.suit}-{(int)card.rank}"; // e.g., hearts-1

        if (cardSprite == null)
        {
            Debug.LogError($"Failed to load sprite at path: Resources/{path}");
        }
    }

    public PlayingCard GetCard()
    {
        return cardData;
    }
}
