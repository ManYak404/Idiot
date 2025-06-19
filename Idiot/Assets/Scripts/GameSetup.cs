using UnityEngine;

public static class GameSetup
{
    public static GameMode mode = GameMode.EasyBot; // Default to EasyBot, will be set when the game starts
    public static CardBackColor deckColor = CardBackColor.Blue; // Default to Blue, will be set when the game starts
}

public enum GameMode
{
    EasyBot, // Easy bot mode
    HardBot, // Hard bot mode
    Multiplayer // Multiplayer mode
}

public enum CardBackColor
{
    Blue = 1, // Blue card back
    Red, // Red card back
    Green, // Green card back
    Yellow // Yellow card back
}