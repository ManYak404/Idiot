using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Manager : MonoBehaviour
{
    public static Manager managerInstance;
    public Deck deck;// deck reference
    public Suit trumpSuit; // The suit of the trump card

    public bool playerWon = false; // Has the player won the game?
    public bool botWon = false; // Has the bot won the game?
    private Hand playerHand;
    private BotHand botHand;
    public bool isPlayerAttacker = true; // Is player the attacker
    public bool isPlayerMakingMove = true; // Is player waiting for second player's turn
    private GameObject selectedCard = null;// Currently selected card
    [SerializeField] public List<GameObject> battlefields = new List<GameObject>();// List of battlefields
    public List<Rank> playableRanks;
    public int currentBattlefieldIndex = 0; // Index of the current battlefield

    public GraphicRaycaster uiRaycaster; // Attach the Canvas's GraphicRaycaster here
    public EventSystem eventSystem;      // Reference to EventSystem

    void Awake()
    {
        // Ensure only one instance of Manager exists
        if (managerInstance == null)
        {
            managerInstance = this;
            //DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck.StartDeck(); // Initialize the deck
        trumpSuit = deck.GetTrumpCard().suit;
        GetComponent<DeckUIManager>().SetTrumpCard(deck.GetTrumpCard()); // Set the trump card in UI
        switch (GameSetup.mode)
        {
            case GameMode.EasyBot:
                StartEasyBotGame(); // Start Easy Bot game
                break;
            case GameMode.HardBot:
                // StartHardBotGame(); // Implement Hard Bot game logic here
                break;
            case GameMode.Multiplayer:
                // StartMultiplayerGame(); // Implement Multiplayer game logic here
                break;
            default:
                Debug.LogError("Unknown game mode: " + GameSetup.mode);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForVictory(); // Check for victory conditions
        // Press space to draw a card
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCardForHand(playerHand);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            DrawCardForHand(botHand);
        }
        if (isPlayerMakingMove)
        {
            CheckForMouseClick();
            MoveSelectedCard();
        }
        else
        {
            // Bot's turn logic
            botHand.MakeBotMove();
        }
    }



    private void StartEasyBotGame()
    {
        playerHand = transform.Find("Player1Hand")?.gameObject.GetComponent<Hand>(); // Initialize bot's hand
        playerHand.SetHandCenter(true); // Set player's hand center
        botHand = transform.Find("Player2Hand")?.gameObject.GetComponent<BotHand>(); // Initialize bot's hand
        botHand.SetHandCenter(false); // Set bot's hand center
        
        DrawCards(); // Draw initial cards for both players

        ResetBattlefield(); // Reset battlefield at the start
    }



    private void CheckForVictory()
    {
        if (playerWon || botWon)
        {
            return; // If either player has already won, skip further checks
        }
        if (deck.isEmpty())
        {
            if (playerHand.cards.Count == 0 && selectedCard == null)
            {
                Debug.Log("Player wins the game!");
                // Handle player victory (e.g., show victory UI, reset game, etc.)
                GameResults.botWon = false;
                GameResults.playerWon = true;
                SceneManager.LoadScene("GameOverScene"); // Load victory scene
                playerWon = true; // Set player victory flag
            }
            else if (botHand.cards.Count == 0)
            {
                Debug.Log("Bot wins the game!");
                // Handle bot victory (e.g., show victory UI, reset game, etc.)
                GameResults.playerWon = false;
                GameResults.botWon = true;
                SceneManager.LoadScene("GameOverScene"); // Load victory scene
                botWon = true; // Set bot victory flag
            }
        }
    }



    private void DrawCards()
    {
        // Determine draw order based on attacker
        Hand first = isPlayerAttacker ? playerHand : botHand;
        Hand second = isPlayerAttacker ? botHand : playerHand;

        DrawUpToSeven(first, DrawCardForHand);
        DrawUpToSeven(second, DrawCardForHand);
    }

    private void DrawUpToSeven(Hand hand, System.Action<Hand> drawCardAction)
    {
        while (hand.cards.Count < 7 && !deck.isEmpty())
        {
            drawCardAction(hand);
        }
    }

    private void DrawCardForHand(Hand hand)
    {
        PlayingCard cardData = deck.DrawCard();
        if (cardData != null)
        {
            hand.CreateCardToHand(cardData);
            hand.UpdateFanLayout();
        }
    }



    public void OnClickEndRoundButton()
    {
        if (isPlayerAttacker)
        {
            RoundDraw(); // Player is the attacker, round is a draw
        }
        else
        {
            // Bot is the attacker, player is the defender
            playerHand.ResignRound(); // Player resigns the round
        }
    }

    public void RoundAttackerVictory()
    {
        DrawCards();
        if (isPlayerAttacker)
        {
            Debug.Log("Player wins the round!");
            isPlayerMakingMove = true; // Player can now initiate round with first attack
        }
        else
        {
            Debug.Log("Bot wins the round!");
            isPlayerMakingMove = false;
        }
        ResetBattlefield(); // Move to the next battlefield
    }

    public void RoundDraw()
    {
        DrawCards(); // Draw cards for both players
        Debug.Log("Round is a draw!");
        if (isPlayerAttacker)
        {
            isPlayerAttacker = false; // Switch to bot's attack
            isPlayerMakingMove = false; // Bot can now make a move
        }
        else
        {
            isPlayerAttacker = true; // Switch to player's attack
            isPlayerMakingMove = true; // Player can now make a move
        }
        ResetBattlefield(); // Move to the next battlefield
    }



    public void NextBattlefield()
    {
        currentBattlefieldIndex = (currentBattlefieldIndex + 1) % battlefields.Count;
        UpdateBattlefieldVisibility();
    }

    public void UpdateBattlefieldVisibility()
    {
        for (int i = 0; i < battlefields.Count; i++)
        {
            battlefields[i].SetActive(i == currentBattlefieldIndex);
        }
    }

    public void ResetBattlefield()
    {
        for (int i = 0; i < battlefields.Count; i++)
        {
            battlefields[i].GetComponent<BattleFieldSlot>().ClearBattlefield();
        }
        playableRanks = new List<Rank>(); // Clear playable ranks
        currentBattlefieldIndex = 0; // Reset to first battlefield
        UpdateBattlefieldVisibility(); // Update visibility after clearing
    } 



    private void MoveSelectedCard()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (selectedCard != null)
        {
            selectedCard.transform.position = mousePos;
        }
    }

    private void CheckForMouseClick()
    {
        if(!isPlayerMakingMove)
        {
            // If player is waiting, ignore clicks
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            // First: try physics raycast for cards
            Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldMousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Card"))
            {
                GameObject card = hit.collider.gameObject;
                if (playerHand.cards.Contains(card))
                {
                    ClickedOnPlayerHand(card);
                }
                else if (botHand.cards.Contains(card))
                {
                    // Error sound
                }
                else
                {
                    // ClickedOnFloatingCard(card);
                }
                return; // Don't check UI if we clicked a card
            }

            // Then: try UI raycast for battlefield buttons
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                GameObject clickedUI = result.gameObject;

                if (clickedUI.CompareTag("Battlefield"))
                {
                    if (selectedCard != null)
                    {
                        if (clickedUI.GetComponent<BattleFieldSlot>().PlaceCardOnBattlefield(selectedCard, true))
                        {
                            //selectedCard.layer = LayerMask.NameToLayer("Default");
                            isPlayerMakingMove = false; // Player has placed a card, waiting for second player's turn
                            selectedCard = null;
                        }
                        else
                        {
                            // Error sound
                        }
                    }
                    return;
                }

                if (clickedUI.CompareTag("Background"))
                {
                    ClickedOnBackground();
                    return;
                }
            }
        }
    }

    private void ClickedOnPlayerHand(GameObject card)
    {
        // Check if there is a selected card
        if (selectedCard != null)
        {
            playerHand.PlaceCardInHand(selectedCard, playerHand.cards.IndexOf(card));
            selectedCard = null; // Deselect the card
        }
        else
        {
            // Pick up card from player hand
            playerHand.RemoveCardFromHand(card);
            selectedCard = card;
            selectedCard.layer = LayerMask.NameToLayer("Ignore Raycast");
            selectedCard.GetComponent<SpriteRenderer>().sortingOrder = 100; // Bring to front
            selectedCard.transform.rotation = Quaternion.identity; // Reset rotation to default
        }
    }

    private void ClickedOnBackground()
    {
        //error sound
    }
}