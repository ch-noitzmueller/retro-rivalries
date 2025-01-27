using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs; // Item-Prefab Liste
    [SerializeField] private float spawnInterval; // Spawn Intervall
    
    private GameObject currentItem; // Referenz auf das aktuell aktive Item
    private GameManager gameManager; // Referenz auf GameManager

    // Grenzen für Spawn-Positions (derzeit hardcoded)
    private readonly Vector2 minBounds = new Vector2(-8.0f, -4.0f);
    private readonly Vector2 maxBounds = new Vector2(8.0f, 4.0f);
    
    private const float buffer = 0.5f;  // Buffer um sicherzustellen, dass Items nicht zu nah am Rand gespawnt werden
    
    private void Awake()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>(); // Sucht GameManager
    }
    
    private void Start()
    {
        // Wiederholt den Spawn-Vorgang in regelmäßigen Abständen
        // Quelle: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MonoBehaviour.InvokeRepeating.html
        InvokeRepeating(nameof(SpawnItem), spawnInterval, spawnInterval);
    }

    // Gibt zufällige Position (innerhalb der Bounds)
    private Vector2 GetRandomPosition()
    {
        // Berücksichtigt Buffer
        float x = Random.Range(minBounds.x + buffer, maxBounds.x - buffer);
        float y = Random.Range(minBounds.y + buffer, maxBounds.y - buffer);
        return new Vector2(x, y);
    }
    
    // Gibt zufälliges Item aus der Liste
    private GameObject GetRandomItem()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) // Wenn nicht leer
        {
            Debug.LogError("Keine Items zugewiesen");
            return null;
        }
        
        return itemPrefabs[Random.Range(0, itemPrefabs.Length)]; // Nimmt zufälliges Prefab
    }
    
    // Spawnt Item
    private void SpawnItem()
    {
        if (gameManager != null && gameManager.IsGameOver) return; // Stoppt Spawnen bei GameOver
        if (currentItem != null) return; // Stoppt Spawnen wenn noch eines existiert
        
        var spawnPosition = GetRandomPosition();
        var randomItem = GetRandomItem();
        
        // Instanziiert Item (an ausgerechneter Position)
        if (randomItem)
        {
            currentItem = Instantiate(randomItem, spawnPosition, Quaternion.identity);
        }
    }
}