using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if(player.HasKitchenObject())
        {
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                { // Player is holding a plate

                player.GetKitchenObject().DestroySelf();
                }
            
            
        }





    }






}
