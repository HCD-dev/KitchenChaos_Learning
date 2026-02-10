using UnityEngine;
using System;
    
public class CuttingCounter : BaseCounter
{
    // ========== ÝNCELEYÝCÝ AYARLARI ==========
    [SerializeField] private int cuttingProgressMax = 3;
    [SerializeField] private KitchenObjectSO cutKitchenObjectSO;



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
        if (HasKitchenObject())
        {
            // Tezgah dolu, mutfak objesi var, onu kes
            GetKitchenObject().DestorySelf();
            Transform kitchenObjectTransform = Instantiate(cutKitchenObjectSO.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetClearCounter(this);

        }
    }
}
