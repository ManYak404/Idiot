using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckUIManager : MonoBehaviour
{
    //public GameObject cardPrefab; // Prefab for the card UI
    public Image deckImage; // Image to display the deck
    public Image trumpCardImage; // Image to display the trump card
    public TextMeshProUGUI deckCountText; // Text to display the number of cards in the deck

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        deckCountText.text = "Cards Remaining: " + Manager.managerInstance.deck.cards.Count.ToString();
        if (Manager.managerInstance.deck.cards.Count < 2)
        {
            deckImage.enabled = false;
        }
        if (Manager.managerInstance.deck.cards.Count == 0)
        {
            trumpCardImage.enabled = false;
        }
    }

    public void SetTrumpCard(PlayingCard trumpCard)
    {
        if (trumpCard != null)
        {
            string path = $"Cards/card-{trumpCard.suit}-{(int)trumpCard.rank}"; // e.g., Resources/Cards/card-hearts-1.png
            trumpCardImage.sprite = Resources.Load<Sprite>(path);
            trumpCardImage.color = Color.white; // Ensure the image is visible
        }
        else
        {
            trumpCardImage.color = new Color(0, 0, 0, 0); // Hide the image if no trump card
        }
    }
}
