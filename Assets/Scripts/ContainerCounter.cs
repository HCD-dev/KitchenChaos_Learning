using System;
using UnityEngine;

/// <summary>
/// Konteyner counter'ý temsil eder. Ýlk etkileþimde nesne doðrudan oyuncunun eline alýnýr.
/// Counter'ýn üstünde durmasý yerine oyuncu tarafýndan hemen taþýnýr.
/// BaseCounter'den miras alýr.
/// </summary>
public class ContainerCounter : BaseCounter
{
    /// <summary>
    /// Oyuncu ContainerCounter ile etkileþime girdiðinde çaðrýlýr.
    /// Ýlk etkileþimde nesne doðrudan oyuncunun eline alýnýr (CounterTopPoint'te beklemez).
    /// Oyuncu'da zaten nesne varsa, yeni nesne verilmez (eline aldýðý nesne korunur).
    /// </summary>
    ///

    public event EventHandler OnPlayerInteract;
    
    public override void Interact(Player player)
    {
        // Eðer player'da zaten nesne varsa hiçbir þey yapma
        if (player.HasKitchenObject())
        {
            Debug.LogWarning("Player'da zaten nesne var! Býrakmalýsýn baþka counter'a.");
            return;
        }

        if (kitchenObject == null)
        {
            // Counter boþsa yeni nesne oluþtur ve doðrudan oyuncuya ver
            if (kitchenObjectSO == null)
            {
                Debug.LogError("kitchenObjectSO Inspector'da atanmamýþ!");
                return;
            }

            // Animasyonu çal
            OnPlayerInteract?.Invoke(this, EventArgs.Empty);

            // Yeni nesne oluþtur (CounterTopPoint'te deðil, doðrudan player'ýn eline alýnacak)
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            
            KitchenObject newKitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            
            // Player'ýn hold point'ine ayarla (CounterTopPoint'te deðil)
            newKitchenObject.transform.parent = player.GetKitchenObjectFollowTransform();
            newKitchenObject.transform.localPosition = Vector3.zero;
            
            // Player'a ver
            newKitchenObject.SetClearCounter(player);
            player.SetKitchenObject(newKitchenObject);
        }
        else
        {
            // Counter'da nesne varsa (bu durumda olmamasý gerekir, ama güvenlik için)
            // Counter'daki nesneyi player'a ver (varsa)
            KitchenObject objectToGive = kitchenObject;
            objectToGive.SetClearCounter(player);

            objectToGive.transform.parent = player.GetKitchenObjectFollowTransform();
            objectToGive.transform.localPosition = Vector3.zero;

            player.SetKitchenObject(objectToGive);
        }
    }
}
