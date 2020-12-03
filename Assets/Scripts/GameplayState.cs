public static class GameplayState
{
    public static PlayerControllability controllability = PlayerControllability.Full;
    public static bool isPaused = false;
    public static int barrels = 0;
    public static int feededBarrels = 0;

    public static void LevelStart()
    {
        //TODO: [!] !!!!! логика работы с этим классом плохая. когда-нибудь мы застрянем в неверном PlayerControllability
        isPaused = false;
        controllability = PlayerControllability.Full;
    }
}

public enum PlayerControllability
{
    Full,
    InDialogue,
}
