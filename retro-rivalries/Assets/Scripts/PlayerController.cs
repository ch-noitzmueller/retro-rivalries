using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Settings für Movement
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 300f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float friction = 0.2f;

    // Settings/Werte für Player
    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool hasSpikes;

    // References zu Objects/Assets
    [Header("References")]
    [SerializeField] private GameObject spikesVisual;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip damageSound;

    // Components und Variables
    private Rigidbody2D rb;
    
    private Vector2 moveInput; // Player Input
    [SerializeField] private InputAction moveAction; // Input Map
    
    private AudioSource audioSource; // Audiosource für SFX
    private Vector3 originalScale; // Skalierung

    private GameManager gameManager; // Referenz zum GameManager
    private int spikeDamage; // Damage-Wert der Spikes (wird von SerializedField von DamageItem Prefab übergeben)

    private void Awake()
    {
        // Initialisierung
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        gameManager = Object.FindFirstObjectByType<GameManager>(); // Finden des GameManagers
    }

    private void Start()
    {
        // Leben zurücksetzen und Spikes standardmäßig deaktivieren
        currentHealth = maxHealth;
        spikesVisual?.SetActive(false);
        originalScale = transform.localScale;
        UpdateSize(); // Größe an Gesundheit anpassen
    }

    private void OnEnable() => moveAction?.Enable(); // Input aktivieren
    private void OnDisable() => moveAction?.Disable(); // Input deaktivieren

    private void Update()
    {
        // Input auslesen
        moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    private void FixedUpdate()
    {
        // Bewegung anwenden
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (moveInput != Vector2.zero)
        {
            // Bewegung basierend auf Input
            rb.AddForce(moveInput * acceleration * Time.fixedDeltaTime, ForceMode2D.Force);
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed); // Geschwindigkeit deckeln (wichtig)
        }
        else
        {
            // Abbremsen/Drag
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlaySound(bounceSound); // Collision SFX abspielen

        if (collision.gameObject.CompareTag("Player") && hasSpikes) // Wenn Player-Collision während Spikes aktiv:
        {
            // Holt sich Player aus GameManager, wenn nicht PlayerA -> PlayerB (ternärer Operator dank MoodleGPT)
            var otherPlayer = gameManager.GetPlayerA() == this ? gameManager.GetPlayerB() : gameManager.GetPlayerA();

            otherPlayer.TakeDamage(spikeDamage); // Damage abziehen
            SetSpikes(false); // Spikes deaktivieren
        }
    }

    // Heal-Methode
    internal void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth); // Lebenspunkte addieren (nur zwischen 0 und 100 bzw. maxHealth)
        UpdateSize(); // Größe anpassen
    }

    // Damage-Methode
    private void TakeDamage(int damage)
    {
        currentHealth -= damage; // Lebenspunkte abziehen
        PlaySound(damageSound);

        if (currentHealth <= 0) // Bei 0 (oder weniger) Lebenspunkten explodieren
        {
            currentHealth = 0;
            Explode();
        }

        UpdateSize(); // Größe anpassen
    }

    // Spikes setzen/entfernen
    internal void SetSpikes(bool value, int damage = 0)
    {
        // Spikes bzw. Damage aktivieren/deaktivieren
        hasSpikes = value;
        spikeDamage = hasSpikes ? damage : 0;
        spikesVisual?.SetActive(value);
    }

    // Methode um Player-Größe anzupassen
    private void UpdateSize()
    {
        // Basierend auf den Lebenspunkten skalieren
        float sizeFactor = Mathf.Clamp((float)currentHealth / maxHealth, 0.3f, 1f);
        transform.localScale = originalScale * sizeFactor;
    }

    // Explodier-Methode (evtl. ausbessern. derzeit 3s hardcoded)
    private void Explode()
    {
        if (explosionPrefab)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity); // Prefab anlegen
            Destroy(explosion, 3f);
        }

        gameManager?.HandlePlayerLoss(this); // GameManager über GameOver informieren
        Destroy(gameObject); // Player zerstören
    }

    private void PlaySound(AudioClip clip)
    {
        // SFX abspielen
        if (audioSource && clip && currentHealth > 0) // Spiele nicht, falls Player defeated
        {
            audioSource.PlayOneShot(clip);
        }
    }
}