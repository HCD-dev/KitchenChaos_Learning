using UnityEngine;

/// <summary>
/// Mutfak tezgahını (counter) temsil eder. Oyuncu ile etkileşime girerek
/// mutfak nesneleri oluşturabilir, taşıyabilir veya mevcut nesneyi inceleyebilir.
/// BaseCounter'den miras alır.
/// </summary>
public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO KitchenObjectSO;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Eğer tezgah boşsa
            if (player.HasKitchenObject())
            {
                // Oyuncunun elinde bir nesne varsa, onu tezgaha koy
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            // Eğer tezgahda bir nesne varsa
            if (player.HasKitchenObject())
            {
                // Oyuncunun elinde zaten nesne var - işlem yapma
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {//player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    { 
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    // Oyuncunun elinde bir nesne var ama bu bir tabak değil 
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {//counter has a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }

            }
            else
            {
                // Oyuncunun eli boşsa, tezgahtaki nesneyi al
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}