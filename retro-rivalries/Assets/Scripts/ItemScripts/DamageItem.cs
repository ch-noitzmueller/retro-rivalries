using UnityEngine;

public class DamageItem : ItemBase
{
    [SerializeField] private int spikeDamage; // Damage durch die Spikes

    protected override void OnCollect(PlayerController player)
    {
        player.SetSpikes(true, spikeDamage); // Setzt Spikes und übergibt Damage
    }
}