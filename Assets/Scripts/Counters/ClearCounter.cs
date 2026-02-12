using UnityEngine;

/// <summary>
/// Mutfak tezgahını (counter) temsil eder. Oyuncu ile etkileşime girerek
/// mutfak nesneleri oluşturabilir, taşıyabilir veya mevcut nesneyi inceleyebilir.
/// BaseCounter'den miras alır.
/// </summary>
public class ClearCounter : BaseCounter
{
    // BaseCounter'ın Interact davranışını açıkça devral
    public override void Interact(Player player)
    {
        base.Interact(player);
    }
}