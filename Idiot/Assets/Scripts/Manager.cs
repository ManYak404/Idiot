using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Manager : MonoBehaviour
{
    public GameObject cardPrefab;// card prefab
    public Deck deck;// deck reference
    private float radius = 10f;// Radius of the fan arc
    private float maxArcDegrees = 60f;// Total arc span (e.g. -30° to +30°)
    private float maxAngleSteps = 5f;// Maximum angle steps between cards
    private Vector2 handCenter = new Vector2(0,0);// Center of the hand in world space
    private List<GameObject> cards = new List<GameObject>(); // cards in hand
    private GameObject selectedCard = null;// Currently selected card

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handCenter = new Vector2(0,-(radius-1));
    }

    // Update is called once per frame
    void Update()
    {
        // Press space to draw a card
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }
        CheckForMouseClick();
        MoveSelectedCard();

    }

    private void DrawCard()
    {
        PlayingCard cardData = deck.DrawCard();
        if (cardData != null)
        {
            GameObject cardObj = Instantiate(cardPrefab, handCenter, Quaternion.identity);
            CardVisual visual = cardObj.GetComponent<CardVisual>();
            visual.SetCard(cardData);
            cards.Add(cardObj);
            UpdateFanLayout();
        }
    }

    private void UpdateFanLayout()
    {
        int count = cards.Count;
        if (count == 0) return;

        float angleStep = maxArcDegrees / count;
        float startingAngle = -(maxArcDegrees / 2f);

        if(angleStep > maxAngleSteps)
        {
            angleStep = maxAngleSteps;
            startingAngle = -(angleStep * count)/2f;
        }

        for (int i = 0; i < count; i++)
        {
            // Start from the left (negative angle) and move right
            float angle = startingAngle + angleStep * (i + 0.5f); // center offset with 0.5

            float rad = Mathf.Deg2Rad * angle;
            float x = handCenter.x + radius * Mathf.Sin(rad);
            float y = handCenter.y + radius * Mathf.Cos(rad);

            GameObject card = cards[i];
            card.transform.position = new Vector3(x, y, -1 + (-i * 0.01f));
            card.transform.rotation = Quaternion.Euler(0, 0, -angle); // Flip rotation for downward arc
            card.GetComponent<SpriteRenderer>().sortingOrder = i; // Set sorting order based on left to right
        }
    }

    private void CheckForMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
            // click on card
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Card"))
            {
                GameObject card = hit.collider.gameObject;
                // Check if the clicked card is in cards list (players hand)
                if (cards.Contains(card))
                {
                    ClickedOnHand(card);
                }
                // Card is on the table
                else
                {
                    ClickedOnFloatingCard(card);
                }
            }
            // click on background
            else if (hit.collider != null && hit.collider.gameObject.CompareTag("Background"))
            {
                ClickedOnBackground();
            }
        }
    }

    private void RemoveCardFromHand(GameObject card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            //Destroy(card);
            UpdateFanLayout();
        }
    }

    private void MoveSelectedCard()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (selectedCard != null)
        {
            selectedCard.transform.position = mousePos;
        }
    }

    private void ClickedOnHand(GameObject card)
    {
        // Check if there is a selected card
        if (selectedCard != null)
        {
            selectedCard.layer = LayerMask.NameToLayer("Default");
            // Add card into hand
            cards.Insert(cards.IndexOf(card) + 1, selectedCard);
            // Deselect the previously selected card
            selectedCard = null;
        }
        else
        {
            // Handle card click
            RemoveCardFromHand(card);
            selectedCard = card;
            selectedCard.layer = LayerMask.NameToLayer("Ignore Raycast");
            selectedCard.GetComponent<SpriteRenderer>().sortingOrder = 100; // Bring to front
            selectedCard.transform.rotation = Quaternion.identity; // Reset rotation to default
        }
        UpdateFanLayout();
    }

    private void ClickedOnFloatingCard(GameObject card)
    {
        if(selectedCard != null)
        {
            return;
        }
        selectedCard = card;
        selectedCard.layer = LayerMask.NameToLayer("Ignore Raycast");
        selectedCard.GetComponent<SpriteRenderer>().sortingOrder = 100; // Bring to front
        selectedCard.transform.rotation = Quaternion.identity; // Reset rotation to default
    }

    private void ClickedOnBackground()
    {
        if (selectedCard != null)
        {
            selectedCard.layer = LayerMask.NameToLayer("Default");
            selectedCard = null;
        }
    }
}