using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spieler Referenzen")]
    [SerializeField] private PlayerController playerA;
    [SerializeField] private PlayerController playerB;

    [Header("Manager Referenzen")]
    [SerializeField] private UIManager uiManager;

    private bool isGameOver; // GameOver Flag
    public bool IsGameOver // Leseberechtigung erteilen
    {
        get { return isGameOver; }
        private set { isGameOver = value; }
    }

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Wird aufgerufen, wenn ein Player verliert
    public void HandlePlayerLoss(PlayerController loser)
    {
        if (isGameOver) return; // Verhindert doppelte Aufrufe
        isGameOver = true;

        PlayerController winner = (loser == playerA) ? playerB : playerA;  // Bestimmt Winner

        PlayGameOverSound();
        uiManager.ShowGameOverScreen(winner.name); // Übergibt an UI Manager
    }

    private void PlayGameOverSound()
    {
        audioSource.Play();
    }

    // Getter für Außenzugriff auf Player A/B
    internal PlayerController GetPlayerA() => playerA;
    internal PlayerController GetPlayerB() => playerB;
}