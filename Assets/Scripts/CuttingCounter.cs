using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class CuttingCounter : BaseCounter
{
    // ========== ÝNCELEYÝCÝ AYARLARI ==========
    [SerializeField] private int cuttingProgressMax = 3;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
  



    // ========== EVENTLER ==========
    public event EventHandler<OnCuttingProgressChangedEventArgs> OnCuttingProgressChanged;

    public class OnCuttingProgressChangedEventArgs : EventArgs
    {
        public int cuttingProgress;
    }

    // ========== PRIVATE FIELDS ==========
    private int cuttingProgress;

    private void Start()
    {
        cuttingProgress = 0;
    }

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            // Tezgah boþ, mutfak objesi yok
            if(player.HasKitchenObject())
            {
                // Oyuncunun elinde mutfak objesi var, onu tezgaha koy
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Oyuncunun elindeki mutfak objesi kesme tariflerinden biriyle eþleþiyor, tezgaha koy  
                    player.GetKitchenObject().SetClearCounter(this);

                }
            }
            else
            {
                // Oyuncunun elinde mutfak objesi yok, hiçbir þey yapma
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

            KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
            
            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO cutKitchenObjectSO = GetOutputForInput(inputKitchenObjectSO);
                GetKitchenObject().DestorySelf();
                KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
                
               
                cuttingProgress = 0;
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
        }else { 
            return null;
        }
       
    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
