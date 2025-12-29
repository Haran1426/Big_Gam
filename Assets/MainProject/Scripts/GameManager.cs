using UnityEngine;

public enum Language
{
    English,
    Korean
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Language language;

    public Language GetLanguageSetting()
    {
        return language;
    }
}
