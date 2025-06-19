using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    CardBackColor deckColor = CardBackColor.Blue; // Default deck color
    GameMode gameMode = GameMode.EasyBot; // Default game mode
    public Image deckColorImage; // Reference to the deck color image in the UI

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void OnClickEasyBot()
    {
        GameSetup.mode = gameMode; // Set game mode to EasyBot
        GameSetup.deckColor = deckColor; // Set deck color to Blue
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); // Load the game scene
    }

    public void OnClickHardBot()
    {
        GameSetup.mode = GameMode.HardBot; // Set game mode to HardBot
        GameSetup.deckColor = deckColor; // Set deck color to Blue
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); // Load the game scene
    }

    public void OnClickMultiplayer()
    {
        GameSetup.mode = GameMode.Multiplayer; // Set game mode to Multiplayer
        GameSetup.deckColor = deckColor; // Set deck color to Blue
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); // Load the game scene
    }



    public void OnClickNext()
    {
        // Cycle through deck colors
        switch (deckColor)
        {
            case CardBackColor.Blue:
                deckColor = CardBackColor.Red;
                break;
            case CardBackColor.Red:
                deckColor = CardBackColor.Green;
                break;
            case CardBackColor.Green:
                deckColor = CardBackColor.Yellow;
                break;
            case CardBackColor.Yellow:
                deckColor = CardBackColor.Blue; // Loop back to Blue
                break;
        }

        // Update the UI image to reflect the new deck color
        UpdateDeckColorImage();
    }

    public void OnClickPrev()
    {
        // Cycle through deck colors in reverse
        switch (deckColor)
        {
            case CardBackColor.Blue:
                deckColor = CardBackColor.Yellow;
                break;
            case CardBackColor.Red:
                deckColor = CardBackColor.Blue;
                break;
            case CardBackColor.Green:
                deckColor = CardBackColor.Red;
                break;
            case CardBackColor.Yellow:
                deckColor = CardBackColor.Green; // Loop back to Green
                break;
        }

        // Update the UI image to reflect the new deck color
        UpdateDeckColorImage();
    }

    public void UpdateDeckColorImage()
    {
        string path = $"Cards/card-back" + (int)deckColor; // e.g., Cards/card-back1
        Sprite cardSprite = Resources.Load<Sprite>(path);
        if (cardSprite != null)
        {
            deckColorImage.sprite = cardSprite; // Update the UI image with the new sprite
        }
        else
        {
            Debug.LogError($"Failed to load sprite at path: Resources/{path}");
        }
    }
}
