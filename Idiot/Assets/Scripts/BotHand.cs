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
