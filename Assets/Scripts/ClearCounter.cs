using UnityEngine;

/// <summary>
/// Mutfak tezgahını (counter) temsil eder. Oyuncu ile etkileşime girerek
/// mutfak nesneleri oluşturabilir, taşıyabilir veya mevcut nesneyi inceleyebilir.
/// </summary>
public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    // Inspector'da atanacak: Bu counter'a eklenecek nesnenin ScriptableObject verisi
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Inspector'da atanacak: Nesnenin counter üzerinde konumlanacağı nokta (boş child Transform)
    [SerializeField] private Transform CounterTopPoint;

    // Inspector'da atanacak: Test amaçlı ikinci counter (T tuşu ile taşıma için)
    [SerializeField] private ClearCounter secondClearCounter;

    // Inspector'da işaretlenecek: Test modunu aktif/pasif yapar
    [SerializeField] private bool testing;

    // Bu counter'da şu anda bulunan mutfak nesnesi (null = boş counter)
    private KitchenObject kitchenObject;

    /// <summary>
    /// Her frame'de çalışır. Test modu aktifse T tuşu ile nesneyi başka counter'a taşır.
    /// </summary>
    private void Update()
    {
        // Test modu açık ve T tuşuna basıldı mı?
        if (testing && Input.GetKeyDown(KeyCode.T))
        {
            // ✅ GÜVENLİ TAŞIMA: Önce geçici değişkene al!
            // Neden? SetClearCounter() çağrısı sonrası this.kitchenObject NULL olur.
            // Eğer direkt kitchenObject.SetClearCounter() yaparsak,
            // hemen sonrasında kitchenObject.GetClearCounter() çağrısı NullReferenceException fırlatır!
            if (kitchenObject != null && secondClearCounter != null)
            {
                KitchenObject objectToMove = kitchenObject; // 🔑 KRİTİK: Referansı kopyala

                // Nesneyi ikinci counter'a taşı (bu işlem ilk counter'ı temizler!)
                objectToMove.SetClearCounter(secondClearCounter);

                // Güvenli debug: objectToMove hala geçerli referans tutar
                Debug.Log($"Nesne taşındı: {(objectToMove.GetClearCounter() != null ? objectToMove.GetClearCounter().gameObject.name : "null")}");
            }
            else if (kitchenObject == null)
            {
                Debug.LogWarning("Counter'da taşınacak bir nesne yok!");
            }
            else if (secondClearCounter == null)
            {
                Debug.LogWarning("secondClearCounter Inspector'da atanmamış!");
            }
        }
    }

    /// <summary>
    /// Oyuncu counter ile etkileşime girdiğinde çağrılır (E tuşu vb.).
    /// Counter boşsa nesne oluşturur, doluysa mevcut nesnenin bilgisini gösterir.
    /// </summary>
    public void Interact()
    {
        // KitchenObjectSO atanmamışsa hata ver ve çık
        if (kitchenObjectSO == null)
        {
            Debug.LogError("kitchenObjectSO Inspector'da atanmamış! Counter'ı ayarla.");
            return;
        }

        // Counter boşsa yeni nesne oluştur
        if (kitchenObject == null)
        {
            // Prefab'ı CounterTopPoint konumunda yarat
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, CounterTopPoint);

            // Nesneyi tam olarak counter'ın merkezine hizala
            kitchenObjectTransform.localPosition = Vector3.zero;

            // Yaratılan objeden KitchenObject bileşenini al
            kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

            // Nesneye "sen bu counter'dasın" bilgisini ver
            kitchenObject.SetClearCounter(this);
        }
        else
        {
            // Counter doluysa mevcut nesnenin ismini göster
            //kitchenObject.SetClearCounter(player);
            Debug.Log($"Counter'da mevcut nesne: {kitchenObject.GetKitchenObjectSO().name}");
        }
    }

    public void Interact(Player player)
    {
        // Burada oyuncu ile counter etkileşimini tanımlayabilirsiniz.
        // Şimdilik mevcut Interact() metodunu çağırıyoruz.
        Interact();
    }

    /// <summary>
    /// Nesnenin takip edeceği transform'u döndürür (counter'ın üst yüzeyi).
    /// </summary>
    public Transform GetKitchenObjectFollowTransform() => CounterTopPoint;

    /// <summary>
    /// Bu counter'a mutfak nesnesi atar.
    /// </summary>
    public void SetKitchenObject(KitchenObject kitchenObject) => this.kitchenObject = kitchenObject;

    /// <summary>
    /// Bu counter'daki mutfak nesnesini döndürür.
    /// </summary>
    public KitchenObject GetKitchenObject() => kitchenObject;

    /// <summary>
    /// Counter'daki nesneyi temizler (null yapar).
    /// DİKKAT: Bu metod, KitchenObject.SetClearCounter() içinden çağrılır!
    /// </summary>
    public void ClearKitchenObject() => kitchenObject = null;

    /// <summary>
    /// Counter'da nesne olup olmadığını kontrol eder.
    /// </summary>
    public bool HasKitchenObject() => kitchenObject != null;

    /// <summary>
    /// Bu counter'ın GameObject'ini döndürür.
    /// </summary>
    public GameObject GetGameObject() => gameObject;
}