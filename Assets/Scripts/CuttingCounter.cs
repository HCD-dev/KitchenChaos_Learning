using UnityEngine;
using System;
    
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
                player.GetKitchenObject().SetClearCounter(this);
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
        if (!HasKitchenObject())
        {
            // Tezgah boþsa hiçbir þey yapma
            return;
        }

        // Kesme iþlemini arttýr
       
        OnCuttingProgressChanged?.Invoke(this, new OnCuttingProgressChangedEventArgs { cuttingProgress = cuttingProgress });

        // Kesme tamamlandý mý?
             KitchenObjectSO cutKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        // Eski nesneyi yok et
        GetKitchenObject().DestorySelf();
            
            // Yeni kesilmiþ nesneyi oluþtur ve counter'a koy
            
            KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
            

            // Kesme iþlemini sýfýrla
           
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
}
