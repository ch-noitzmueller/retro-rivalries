using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Dieser Code basiert mainly auf folgender Tutorial-Reihe: https://www.youtube.com/watch?v=0mYtI21Fmg4&list=PLgCVPIIZ3xL_FVLhDrC3atsy8CiZzAMh6

public class UIManager : MonoBehaviour
{
    [Header("UI Documents")]
    [SerializeField] private UIDocument gameOverUIDocument;
    [SerializeField] private UIDocument mainMenuUIDocument;

    private void Awake()
    {
        // Deaktiviere Game-Over-Screen und aktiviere Hauptmenü
        gameOverUIDocument?.gameObject.SetActive(false);
        mainMenuUIDocument?.gameObject.SetActive(true);
    }

    private void Start()
    {
        SetupMainMenu();
    }

    // Initialisiert das Hauptmenü
    private void SetupMainMenu()
    {
        var root = mainMenuUIDocument.rootVisualElement; // Zugriff auf Root-UI-Elemente
        var startButton = root?.Q<Button>("StartButton"); // Suche nach #StartButton

        if (startButton == null)
        {
            // Fehler ausgeben, wenn der Button nicht gefunden wurde
            Debug.Log("StartButton konnte nicht gefunden werden");
            return;
        }

        // Registriere Click-Functionality
        startButton.clicked += () => LoadScene("MainScene"); // Lade Hauptszene bei Klicken
    }

    // Zeigt den Game-Over-Screen
    public void ShowGameOverScreen(string winnerName)
    {
        gameOverUIDocument.gameObject.SetActive(true); // Aktiviere Game-Over-Screen

        var root = gameOverUIDocument.rootVisualElement; // Zugriff auf Root-UI-Elemente

        // Suche nach UI-Elementen im Game-Over-Screen
        var winnerLabel = root?.Q<Label>("WinnerLabel");
        var restartButton = root?.Q<Button>("RestartButton");
        var mainMenuButton = root?.Q<Button>("MainMenuButton");

        // Überschreibe #WinnerLabel
        if (winnerLabel != null)
        {
            winnerLabel.text = $"{winnerName} hat gewonnen!";
        }

        // Registriere Click-Functionality
        restartButton.clicked += RestartGame;
        mainMenuButton.clicked += LoadMainMenu;
    }

    // Lädt aktuelle Szene neu
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Lädt Hauptmenü
    private void LoadMainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    // Lädt spezifische Szene
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}