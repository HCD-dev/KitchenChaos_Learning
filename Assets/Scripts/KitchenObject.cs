using System;
using UnityEngine;

/// <summary>
/// Mutfak nesnesini (bıçak, tabak, malzeme vb.) temsil eder.
/// Hangi counter'da olduğunu takip eder ve pozisyonunu buna göre günceller.
/// </summary>
public class KitchenObject : MonoBehaviour
{
    // Inspector'da atanacak: Bu nesneye ait veri (isim, sprite, vb.)
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Bu nesnenin şu anda bağlı olduğu parent (counter, player vb.) - null = counter'da değil
    internal IKitchenObjectParent kitchenObjectParent;

    /// <summary>
    /// Nesnenin ScriptableObject verisini döndürür (UI'da göstermek için).
    /// </summary>
    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    /// <summary>
    /// Bu nesneyi yeni bir parent'a taşır (Counter veya Player).
    /// Eski parent'ı temizler, hedef parent'taki mevcut nesneyi imha eder.
    /// </summary>
    /// <param name="newParent">Taşınacak hedef parent (null = parent'dan çıkar)</param>
    public void SetClearCounter(IKitchenObjectParent newParent)
    {
        // 🔑 ADIM 1: Eski parent'dan kendimizi çıkar
        if (kitchenObjectParent != null)
        {
            kitchenObjectParent.ClearKitchenObject();
        }

        // 🔑 ADIM 2: Yeni parent'a geçiş yap
        if (newParent != null)
        {
            // Hedef parent dolu mu?
            if (newParent.HasKitchenObject())
            {
                KitchenObject existingObject = newParent.GetKitchenObject();

                if (existingObject != null && existingObject != this)
                {
                    if (existingObject.kitchenObjectParent != null)
                    {
                        existingObject.kitchenObjectParent.ClearKitchenObject();
                    }
                    Destroy(existingObject.gameObject);
                }
            }

            kitchenObjectParent = newParent;
            newParent.SetKitchenObject(this);

            transform.parent = newParent.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
        }
        else
        {
            // 🔑 ADIM 3: Parent null ise (oyuncu eline aldıysa)
            kitchenObjectParent = null;
            // Transform parent ayarlama burada YAPMAYıN - Player.Interact'te ayarlanacak
        }
    }

    /// <summary>
    /// Bu nesnenin bağlı olduğu parent'ı döndürür.
    /// </summary>
    public IKitchenObjectParent GetClearCounter() => kitchenObjectParent;

    internal void DestorySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }
    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject newKitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        newKitchenObject.SetClearCounter(kitchenObjectParent);
        return newKitchenObject;
    }
}   