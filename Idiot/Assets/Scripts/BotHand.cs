using UnityEngine;

public class BotHand : Hand
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public override bool CreateCardToHand(PlayingCard card)
    {
        card.isFaceUp = false; // Ensure the card is face down for the bot
        return base.CreateCardToHand(card);
    }

    public override void PlaceCardInHand(GameObject card, int index)
    {
        // Ensure the card is face down when placed in the bot's hand
        CardVisual cardVisual = card.GetComponent<CardVisual>();
        if (cardVisual != null)
        {
            cardVisual.SetFlip(false);
        }
        base.PlaceCardInHand(card, index);
    }

    public override void RemoveCardFromHand(GameObject card)
    {
        // Ensure the card is face down when removed from the bot's hand
        CardVisual cardVisual = card.GetComponent<CardVisual>();
        if (cardVisual != null)
        {
            cardVisual.SetFlip(true);
        }
        base.RemoveCardFromHand(card);
    }



    public void MakeBotMove()
    {
        GameObject battlefield = Manager.managerInstance.battlefields[Manager.managerInstance.currentBattlefieldIndex];
        //bot is defender
        if (Manager.managerInstance.isPlayerAttacker)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (battlefield.GetComponent<BattleFieldSlot>().PlaceCardOnBattlefield(cards[i], false))
                {
                    // Place the first valid card and exit
                    RemoveCardFromHand(cards[i]);
                    Manager.managerInstance.isPlayerMakingMove = true; // Player can now make a move
                    return;
                }
            }
            ResignRound();
        }
        //bot is attacker
        else if (!Manager.managerInstance.isPlayerAttacker)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (battlefield.GetComponent<BattleFieldSlot>().PlaceCardOnBattlefield(cards[i], false))
                {
                    // Place the first valid card and exit
                    RemoveCardFromHand(cards[i]);
                    Manager.managerInstance.isPlayerMakingMove = true; // Player can now make a move
                    return;
                }
            }
            Manager.managerInstance.RoundDraw(); // If no valid cards, draw the round
        }
    }

    public override void UpdateFanLayout()
    {
        SortCards(); // Sort cards before updating layout

        base.UpdateFanLayout(); // Call base to position cards visually
    }

}
