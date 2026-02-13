using UnityEngine;
using System;


public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnStageChanged;

    // ========== ÝNCELEYÝCÝ AYARLARI ==========
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    // ========== PRIVATE FIELDS ==========
    private int cuttingProgress;

    private void Start()
    {
        cuttingProgress = 0;
        
        // Null check
        if (cuttingRecipeSOArray == null || cuttingRecipeSOArray.Length == 0)
        {
            Debug.LogError($"CuttingCounter: cuttingRecipeSOArray Inspector'da atanmamýþ veya boþ!", gameObject);
        }
    }

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            if(player.HasKitchenObject())
            {
                KitchenObject playerKitchenObject = player.GetKitchenObject();
                
                // Null check
                if (playerKitchenObject == null)
                {
                    Debug.LogWarning("CuttingCounter: Player'ýn KitchenObject'i null!", gameObject);
                    return;
                }

                KitchenObjectSO kitchenObjectSO = playerKitchenObject.GetKitchenObjectSO();
                
                if (HasRecipeWithInput(kitchenObjectSO))
                {
                    // Player'ýn nesnesini counter'a taþý
                    playerKitchenObject.SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    
                    // Taþýndýktan SONRA recipe'yi bul
                    CuttingRecipeSO recipe = GetCuttingRecipeSOWithInput(kitchenObjectSO);
                    if (recipe != null)
                    {
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = (float)cuttingProgress / recipe.cuttingProgressMax
                        });
                    }
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                // Oyuncunun elinde mutfak objesi var, tezgah dolu
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player plate tutuyor, tezgahtaki kesilen malzemeyi plate'ye eklemeye çalýþ
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    { 
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Oyuncunun eli boþsa, tezgahtaki nesneyi al
                GetKitchenObject().SetKitchenObjectParent(player);
                // Nesne alýndýðýnda progress'i sýfýrla
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            OnStageChanged?.Invoke(this, EventArgs.Empty);
            KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

            if (cuttingRecipeSO != null)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                });

                if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
                {
                    KitchenObjectSO cutKitchenObjectSO = GetOutputForInput(inputKitchenObjectSO);
                    GetKitchenObject().DestroySelf();
                        KitchenObject cuttedObject = KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
                    
                    // Oyuncunun plate tutup tutmadýðýný kontrol et
                    if (player.HasKitchenObject() && player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                    {
                        // Kesilmiþ nesneyi plate'e eklemeye çalýþ
                        if (plateKitchenObject.TryAddIngredient(cutKitchenObjectSO))
                        {
                            cuttedObject.DestroySelf();
                        }
                    }
                    
                    cuttingProgress = 0;
                    // Kesme tamamlandýktan sonra progress'i sýfýrla
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        { 
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        // Null check ekledim
        if (cuttingRecipeSOArray == null || cuttingRecipeSOArray.Length == 0)
        {
            Debug.LogWarning($"CuttingCounter: cuttingRecipeSOArray null veya boþ!", gameObject);
            return null;
        }

        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            // Null check ekledim
            if (cuttingRecipeSO != null && cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
