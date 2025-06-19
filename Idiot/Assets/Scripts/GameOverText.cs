using UnityEngine;
using UnityEngine.UI;
using TMPro; // Ensure you have TextMeshPro package installed

public class GameOverText : MonoBehaviour
{
    public TextMeshProUGUI resultText; // Assign via inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameResults.playerWon)
            resultText.text = "You Won!";
        else if (GameResults.botWon)
            resultText.text = "You Lost!";
        else
            resultText.text = "It's a Draw!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
