using UnityEngine;

/// <summary>
/// Counter'larýn ortak özelliklerini içeren temel sýnýf.
/// ClearCounter ve ContainerCounter bu sýnýftan miras alýr.
/// </summary>
public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    // ========== ORTAK ALANLAR ==========
    // Inspector'da atanacak: Bu counter'a eklenecek nesnenin ScriptableObject verisi
    [SerializeField] protected KitchenObjectSO kitchenObjectSO;

    // Inspector'da atanacak: Nesnenin counter üzerinde konumlanacaðý nokta
    [SerializeField] protected Transform CounterTopPoint;

    // Bu counter'da þu anda bulunan mutfak nesnesi (null = boþ counter)
    protected KitchenObject kitchenObject;

    // ========== INTERFACE IMPLEMENTATION ==========
    public virtual Transform GetKitchenObjectFollowTransform() => CounterTopPoint;

    public virtual void SetKitchenObject(KitchenObject kitchenObject) => this.kitchenObject = kitchenObject;

    public virtual KitchenObject GetKitchenObject() => kitchenObject;

    public virtual void ClearKitchenObject() => kitchenObject = null;

    public virtual bool HasKitchenObject() => kitchenObject != null;

    public virtual GameObject GetGameObject() => gameObject;

    // ========== ORTAK INTERACT METODU ==========
    public virtual void Interact()
    {
        // KitchenObjectSO atanmamýþsa hata ver ve çýk
        if (kitchenObjectSO == null)
        {
            Debug.LogError("kitchenObjectSO Inspector'da atanmamýþ! Counter'ý ayarla.");
            return;
        }

        // Counter boþsa yeni nesne oluþtur
        if (kitchenObject == null)
        {
            CreateKitchenObject();
        }
        else
        {
            // Counter doluysa mevcut nesnenin ismini göster
            Debug.Log($"Counter'da mevcut nesne: {kitchenObject.GetKitchenObjectSO().name}");
        }
    }

    public virtual void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            // Counter boþsa
            if (player.HasKitchenObject())
            {
                // Player'daki nesneyi bu counter'a koy
                KitchenObject playerObject = player.GetKitchenObject();
                player.ClearKitchenObject();
                playerObject.SetClearCounter(this);
            }
            else
            {
                // Player'da nesne yok ve counter'da da yok - Yeni nesne oluþtur
                if (kitchenObjectSO == null)
                {
                    Debug.LogError("kitchenObjectSO Inspector'da atanmamýþ!");
                    return;
                }

                CreateKitchenObject();
            }
        }
        else
        {
            // Counter doluysa
            if (player.HasKitchenObject())
            {
                Debug.LogWarning("Player'da zaten nesne var! Býrakmalýsýn baþka counter'a.");
                return;
            }

            // Counter'daki nesneyi player'a ver
            KitchenObject objectToGive = kitchenObject;
            objectToGive.SetClearCounter(player);

            objectToGive.transform.parent = player.GetKitchenObjectFollowTransform();
            objectToGive.transform.localPosition = Vector3.zero;

            player.SetKitchenObject(objectToGive);
        }
    }

    // ========== PROTECTED YARDIMCI METODLAR ==========
    /// <summary>
    /// Mutfak nesnesi oluþturur ve counter'a atar.
    /// </summary>
    protected virtual void CreateKitchenObject()
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, CounterTopPoint);
        kitchenObjectTransform.localPosition = Vector3.zero;

        KitchenObject newKitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        newKitchenObject.SetClearCounter(this);
    }
}