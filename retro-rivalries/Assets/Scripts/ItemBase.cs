using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab; // Referenz auf Animation
    
    private AudioSource audioSource;
    private bool isCollected;

    protected abstract void OnCollect(PlayerController player); // Von Children zu überschreibende Methode

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected || !collision.CompareTag("Player")) return; // Sicherheits-Check, ob Item eingesammelt wurde oder ob der Collider kein Player ist

        isCollected = true; // Markiert Item als gesammelt
        HandleItemCollection(collision.GetComponent<PlayerController>());
    }

    private void HandleItemCollection(PlayerController player)
    {
        if (player != null)
        {
            OnCollect(player);
        }
        
        Explode();
    }

    // Item deaktivieren
    private void DisableItem()
    {
        var collider = GetComponent<Collider2D>(); // Collider deaktivieren
        if (collider) collider.enabled = false;

        var spriteRenderer = GetComponent<SpriteRenderer>(); // Ausblenden
        if (spriteRenderer) spriteRenderer.enabled = false;
    }
    
    // Einsammel-Effekt
    private void Explode()
    {
        if (explosionPrefab)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f); // hardcoded -> ausbessern!
        }

        if (audioSource && audioSource.clip)
        {
            audioSource.Play();
        }

        DisableItem(); // Deaktiviert Item
        
        Destroy(gameObject, audioSource?.clip?.length ?? 0f); // Entfernt Item (sobald Sound gespielt wurde oder sofort) 
    }
}