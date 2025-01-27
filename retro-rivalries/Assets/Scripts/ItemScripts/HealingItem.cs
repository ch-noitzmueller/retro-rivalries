using UnityEngine;

public class HealingItem : ItemBase
{
    [SerializeField] private int healAmount; // Wie viel gehealed wird

    protected override void OnCollect(PlayerController player)
    {
        player.Heal(healAmount); // Heilt Player und übergibt Amount
    }
}