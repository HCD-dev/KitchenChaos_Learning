using UnityEngine;
using System;


public class CuttingCounter : BaseCounter
{

    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
    public event EventHandler Oncut;

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
                    playerKitchenObject.SetClearCounter(this);
                    cuttingProgress = 0;
                    
                    // Taþýndýktan SONRA recipe'yi bul
                    CuttingRecipeSO recipe = GetCuttingRecipeSOWithInput(kitchenObjectSO);
                    if (recipe != null)
                    {
                        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
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
                // Oyuncunun elinde mutfak objesi var, tezgah dolu, hiçbir þey yapma
            }
            else
            {
                // Oyuncunun elinde mutfak objesi yok, tezgah dolu, mutfak objesini oyuncuya ver
                GetKitchenObject().SetClearCounter(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            Oncut?.Invoke(this, EventArgs.Empty);
            KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

            if (cuttingRecipeSO != null)
            {
                OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
                {
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                });

                if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
                {
                    KitchenObjectSO cutKitchenObjectSO = GetOutputForInput(inputKitchenObjectSO);
                    GetKitchenObject().DestorySelf();
                    KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
                    
                    cuttingProgress = 0;
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
