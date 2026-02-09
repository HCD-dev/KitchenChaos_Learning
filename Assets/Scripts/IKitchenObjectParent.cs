using UnityEngine;

/// <summary>
/// Mutfak nesnesinin parent'ý olabilecek herhangi bir objenin (counter, player)
/// implement etmesi gereken sözleþme. GameObject eriþimi için GetGameObject() içerir.
/// </summary>
public interface IKitchenObjectParent
{
    /// <summary>
    /// Nesnenin takip edeceði transform'u döndürür (counter üstü, player eli vb.)
    /// </summary>
    Transform GetKitchenObjectFollowTransform();

    /// <summary>
    /// Bu parent'a mutfak nesnesi atar.
    /// </summary>
    void SetKitchenObject(KitchenObject kitchenObject);

    /// <summary>
    /// Bu parent'taki mutfak nesnesini döndürür.
    /// </summary>
    KitchenObject GetKitchenObject();

    /// <summary>
    /// Parent'taki nesneyi temizler (null yapar).
    /// </summary>
    void ClearKitchenObject();

    /// <summary>
    /// Parent'ta nesne olup olmadýðýný kontrol eder.
    /// </summary>
    bool HasKitchenObject();

    /// <summary>
    /// Parent objesinin GameObject referansýný döndürür.
    /// (Interface'te .gameObject eriþimi için kritik!)
    /// </summary>
    GameObject GetGameObject();
    void Interact(Player player);
}