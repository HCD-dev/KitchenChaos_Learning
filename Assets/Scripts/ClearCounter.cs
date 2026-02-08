using UnityEngine;
using System;
public class ClearCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform CounterTopPoint;
    public void Interact()
    {
                Debug.Log("ClearCounter ile etkileþimde bulunuldu.");
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, CounterTopPoint);
            kitchenObjectTransform.localPosition = Vector3.zero;

            Debug.Log(kitchenObjectTransform.GetComponent<KitchenObject>().GetKitchenObjectSO());
    }
}
