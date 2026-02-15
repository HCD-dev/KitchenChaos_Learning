using System;
using UnityEngine;

/// <summary>
/// Counter'larýn ortak özelliklerini içeren temel sýnýf.
/// ClearCounter ve ContainerCounter bu sýnýftan miras alýr.
/// </summary>
public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;
    // ========== ORTAK ALANLAR ==========
    // Inspector'da atanacak: Bu counter'a eklenecek nesnenin ScriptableObject verisi
    [SerializeField] protected KitchenObjectSO kitchenObjectSO;

    // Inspector'da atanacak: Nesnenin counter üzerinde konumlanacaðý nokta
    [SerializeField] protected Transform CounterTopPoint;

    // Bu counter'da þu anda bulunan mutfak nesnesi (null = boþ counter)
    protected KitchenObject kitchenObject;

    // ========== INTERFACE IMPLEMENTATION ==========
    public virtual Transform GetKitchenObjectFollowTransform() => CounterTopPoint;

    public virtual void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if(kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public virtual KitchenObject GetKitchenObject() => kitchenObject;

    public virtual void ClearKitchenObject() => kitchenObject = null;

    public virtual bool HasKitchenObject() => kitchenObject != null;

    public virtual GameObject GetGameObject() => gameObject;

    // ========== ORTAK INTERACT METODU ==========
    public virtual void Interact()
    {
        // Bu metod artýk kullanýlmýyor, alt sýnýflar override edecek
        Debug.LogWarning("Interact() metodunu override etmelisiniz!");
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
                playerObject.SetKitchenObjectParent(this);
            }
            // Player'da nesne yok ve counter'da da yok - Yeni nesne oluþturma!
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
            objectToGive.SetKitchenObjectParent(player);

            objectToGive.transform.parent = player.GetKitchenObjectFollowTransform();
            objectToGive.transform.localPosition = Vector3.zero;

            player.SetKitchenObject(objectToGive);
        }
    }

    public virtual void InteractAlternate(Player player)
    {
        // Alternatif etkileþim - alt sýnýflar (CuttingCounter, vb.) override edecek
        Debug.LogWarning("InteractAlternate() metodunu override etmelisiniz!");
    }
    }