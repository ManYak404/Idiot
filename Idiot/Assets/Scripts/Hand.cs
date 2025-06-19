using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public class Hand : MonoBehaviour
{
    public GameObject cardPrefab;// card prefab
    public List<GameObject> cards = new List<GameObject>(); // cards in hand

    private float radius = 100f;// Radius of the fan arc
    private float maxArcDegrees = 9f;// Total arc span (e.g. -1/2 to +1/2)
    private float maxAngleSteps = 1f;// Maximum angle steps between cards
    private Vector2 handCenter = new Vector2(0, 0f);// Center of the hand in world space

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handCenter = new Vector2(0, -(radius + 5f));
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void ResignRound()
    {
        Debug.Log("Defender resigned the round.");
        // Pick up all cards from all battlefields
        foreach (GameObject battlefield in Manager.managerInstance.battlefields)
        {
            BattleFieldSlot slot = battlefield.GetComponent<BattleFieldSlot>();
            foreach (CardVisual card in slot.cardsOnBattlefield)
            {
                if (card != null)
                {
                    card.transform.SetParent(transform); // Move card back to hand
                    CreateCardToHand(card.GetCard()); // Recreate card in hand
                    Destroy(card.gameObject); // Destroy the card visual
                }
            }
            slot.cardsOnBattlefield.Clear(); // Clear the battlefield slot
        }
        Manager.managerInstance.RoundAttackerVictory(); // Switch to attacker turn
    }



    public void SetHandCenter(bool isPlayerHand)
    {
        if (isPlayerHand)
        {
            handCenter = new Vector2(0, -(radius + 5f)); // Player's hand center
        }
        else
        {
            handCenter = new Vector2(0, (radius + 5f)); // Bot's hand center
        }
    }

    public virtual bool CreateCardToHand(PlayingCard card)
    {
        if (card == null)
        {
            Debug.LogError("Cannot add a null card to hand.");
            return false;
        }

        GameObject newCard = Instantiate(cardPrefab, transform);
        CardVisual cardVisual = newCard.GetComponent<CardVisual>();
        cardVisual.SetCard(card);
        cards.Add(newCard);

        // Position the card in a fan shape
        UpdateFanLayout();
        return true;
    }

    public virtual void PlaceCardInHand(GameObject card, int index)
    {
        if (card != null)
        {
            card.layer = LayerMask.NameToLayer("Default");
            cards.Insert(index, card);
        }
        else
        {
            Debug.LogError("Cannot place a null card in hand.");
            return;
        }
        UpdateFanLayout();
    }

    public virtual void RemoveCardFromHand(GameObject card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            UpdateFanLayout();
        }
    }



    public virtual void UpdateFanLayout()
    {
        float flip; // flip factor for card rotation 
        if (handCenter.y < 0) // player hand flip
        {
            flip = 1f;
        }
        else // bot hand flip
        {
            flip = -1f;
        }
        int count = cards.Count;
        if (count == 0) return;

        float angleStep = maxArcDegrees / count;
        float startingAngle = -(maxArcDegrees / 2f);

        if (angleStep > maxAngleSteps)
        {
            angleStep = maxAngleSteps;
            startingAngle = -(angleStep * count) / 2f;
        }

        for (int i = 0; i < count; i++)
        {
            // Start from the left (negative angle) and move right
            float angle = startingAngle + angleStep * (i + 0.5f); // center offset with 0.5

            float rad = Mathf.Deg2Rad * angle;
            float x = handCenter.x + radius * Mathf.Sin(rad);
            float y = handCenter.y + flip * radius * Mathf.Cos(rad);

            GameObject card = cards[i];
            card.transform.position = new Vector3(x, y, -1 + (-i * 0.01f));
            card.transform.rotation = Quaternion.Euler(0, 0, flip * -angle); // Flip rotation for downward arc
            card.GetComponent<SpriteRenderer>().sortingOrder = i; // Set sorting order based on left to right
        }
    }

    public void SortCards()
    { 
        // Sort cards based on suit and rank, with trump cards last
        cards.Sort((a, b) =>
        {
            var cardA = a.GetComponent<CardVisual>().GetCard();
            var cardB = b.GetComponent<CardVisual>().GetCard();
            Suit trump = Manager.managerInstance.trumpSuit;

            bool aIsTrump = cardA.suit == trump;
            bool bIsTrump = cardB.suit == trump;

            // Non-trump cards come before trump cards
            if (aIsTrump && !bIsTrump) return 1;
            if (!aIsTrump && bIsTrump) return -1;

            // If both are trump or both are non-trump, sort by suit then rank
            if (cardA.suit != cardB.suit)
                return cardA.suit.CompareTo(cardB.suit);

            return cardA.rank.CompareTo(cardB.rank);
        });
    }
}
