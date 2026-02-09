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
    private ClearCounter clearCounter;

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
        // Örnek: Counter A'daydım, Counter B'ye gidiyorum → Önce A'yı temizle
        if (clearCounter != null)
        {
            clearCounter.ClearKitchenObject(); // Bu çağrı ClearCounter.kitchenObject = null yapar!
        }

        // 🔑 ADIM 2: Yeni counter'a geçiş yap
        if (newCounter != null)
        {
            // Hedef counter dolu mu? (Counter B'de zaten nesne var mı?)
            if (newCounter.HasKitchenObject())
            {
                KitchenObject existingObject = newCounter.GetKitchenObject();

                // Mevcut nesne biz değilsek imha et (kendini imha etme riskini önle)
                if (existingObject != null && existingObject != this)
                {
                    // Önce mevcut nesnenin counter'ını temizle (referans tutarsızlığını önle)
                    ClearCounter oldCounter = existingObject.clearCounter;
                    if (oldCounter != null)
                    {
                        oldCounter.ClearKitchenObject();
                    }

                    // Sonra nesneyi imha et
                    Destroy(existingObject.gameObject);
                }
            }

            // 🔑 ADIM 3: Yeni counter'ı ata ve transform'u güncelle
            clearCounter = newCounter;                          // Nesne artık bu counter'a ait
            newCounter.SetKitchenObject(this);                  // Counter da bu nesneyi tanısın

            transform.parent = newCounter.GetKitchenObjectFollowTransform(); // Counter'ın child'ı yap
            transform.localPosition = Vector3.zero;             // Counter'ın merkezine hizala
        }
        else
        {
            // 🔑 ADIM 4: Counter null ise (oyuncu eline aldıysa)
            clearCounter = null;
            transform.parent = null; // Dünya hiyerarşisine çıkar
        }
    }

    /// <summary>
    /// Bu nesnenin bağlı olduğu counter'ı döndürür.
    /// </summary>
    public ClearCounter GetClearCounter() => clearCounter;
}