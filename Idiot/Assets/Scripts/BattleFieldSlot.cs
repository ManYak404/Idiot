using UnityEngine;
using System.Collections.Generic;

public class BattleFieldSlot : MonoBehaviour
{
    public float cardZDistance = 5f;
    public List<CardVisual> cardsOnBattlefield = new List<CardVisual>(2);


    public bool PlaceCardOnBattlefield(GameObject card, bool isCardPlayers)
    {
        Debug.Log($"Placing card on battlefield. Card: {card.name}, Is Player's Card: {isCardPlayers}");

        if (card == null)
        {
            Debug.LogError("Cannot place a null card on the battlefield.");
            return false;
        }
        CardVisual cardVisual = card.GetComponent<CardVisual>();        
        if (cardsOnBattlefield.Count >= 2)
        {
            Debug.LogError("Cannot place more than 2 cards on the battlefield.");
            return false;
        }

        // Check that card is from attacker and that battlefield is empty
        if (cardsOnBattlefield.Count == 0 && isCardPlayers == Manager.managerInstance.isPlayerAttacker)
        {
            // Check that this is not the first placed card of the round
            if (Manager.managerInstance.playableRanks.Count != 0)
            {
                bool isPlayable = false;
                // Check if the card is playable based on its rank
                foreach (Rank rank in Manager.managerInstance.playableRanks)
                {
                    if (cardVisual.GetCard().rank == rank)
                    {
                        Debug.Log($"Card {card.name} is playable on the battlefield.");
                        isPlayable = true;
                        break;
                    }
                }
                if (!isPlayable)
                {
                    Debug.Log($"Card {card.name} is not playable on the battlefield.");
                    return false;
                }
            }
            Debug.Log("Placing card on empty battlefield slot.");
            AttachCard(cardVisual, Quaternion.identity, 10);
            return true;
        }
        // check that card is from defender and that battlefield has exactly one card
        else if (cardsOnBattlefield.Count == 1 && isCardPlayers != Manager.managerInstance.isPlayerAttacker)
        {
            if (cardVisual.GetCard().LargerThanPlayingCard(cardsOnBattlefield[0].GetCard()))
            {
                AttachCard(cardVisual, Quaternion.Euler(0, 0, -35f), 20);
                Manager.managerInstance.NextBattlefield(); // Move to the next battlefield
                return true;
            }
        }

        // Invalid move
        Debug.Log("Invalid move: Cannot place card on battlefield.");
        return false;
    }

    private void AttachCard(CardVisual cardVisual, Quaternion rotation, int sortingOrder)
    {
        // Detach from UI parent to keep it in world space
        cardVisual.transform.SetParent(null);

        // Get world center of the UI slot
        Vector3 worldPos = GetWorldCenterOfSlot();
        worldPos.z = cardZDistance; // place in front of camera

        cardVisual.transform.position = worldPos;
        cardVisual.transform.rotation = rotation;
        cardVisual.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        cardVisual.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        cardsOnBattlefield.Add(cardVisual);

        // add card rank to playable ranks
        if (!Manager.managerInstance.playableRanks.Contains(cardVisual.GetCard().rank))
        {
            Manager.managerInstance.playableRanks.Add(cardVisual.GetCard().rank);
        }
    }

    private Vector3 GetWorldCenterOfSlot()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        // Average bottom-left and top-right
        Vector3 center = (corners[0] + corners[2]) / 2f;
        return center;
    }

    public void ClearBattlefield()
    {
        foreach (var card in cardsOnBattlefield)
        {
            if (card != null)
                Destroy(card.gameObject); // Or return it to hand/deck
        }
        cardsOnBattlefield.Clear();
    }
}
