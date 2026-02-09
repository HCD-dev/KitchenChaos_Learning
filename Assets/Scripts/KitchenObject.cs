using UnityEngine;

/// <summary>
/// Mutfak nesnesini (bıçak, tabak, malzeme vb.) temsil eder.
/// Hangi counter'da olduğunu takip eder ve pozisyonunu buna göre günceller.
/// </summary>
public class KitchenObject : MonoBehaviour
{
    // Inspector'da atanacak: Bu nesneye ait veri (isim, sprite, vb.)
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Bu nesnenin şu anda bağlı olduğu counter (null = elde taşınıyor)
    internal ClearCounter clearCounter;

    /// <summary>
    /// Nesnenin ScriptableObject verisini döndürür (UI'da göstermek için).
    /// </summary>
    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    /// <summary>
    /// Bu nesneyi yeni bir counter'a taşır.
    /// Eski counter'ı temizler, hedef counter'daki mevcut nesneyi imha eder.
    /// </summary>
    /// <param name="newCounter">Taşınacak hedef counter (null = counter'dan çıkar)</param>
    public void SetClearCounter(ClearCounter newCounter)
    {
        // 🔑 ADIM 1: Eski counter'dan kendimizi çıkar
        if (clearCounter != null)
        {
            clearCounter.ClearKitchenObject();
        }

        // 🔑 ADIM 2: Yeni counter'a geçiş yap
        if (newCounter != null)
        {
            // Hedef counter dolu mu?
            if (newCounter.HasKitchenObject())
            {
                KitchenObject existingObject = newCounter.GetKitchenObject();

                if (existingObject != null && existingObject != this)
                {
                    if (existingObject.clearCounter != null)
                    {
                        existingObject.clearCounter.ClearKitchenObject();
                    }
                    Destroy(existingObject.gameObject);
                }
            }

            clearCounter = newCounter;
            newCounter.SetKitchenObject(this);

            transform.parent = newCounter.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
        }
        else
        {
            // 🔑 ADIM 3: Counter null ise (oyuncu eline aldıysa)
            clearCounter = null;
            // Transform parent ayarlama burada YAPMAYıN - Player.Interact'te ayarlanacak
        }
    }

    /// <summary>
    /// Bu nesnenin bağlı olduğu counter'ı döndürür.
    /// </summary>
    public ClearCounter GetClearCounter() => clearCounter;
}   