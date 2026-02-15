using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        // Null check'ler ekledim
        if (recipeSO == null)
        {
            Debug.LogError("DeliveryManagerSingleUI: recipeSO null!", gameObject);
            return;
        }

        if (recipeNameText == null)
        {
            Debug.LogError("DeliveryManagerSingleUI: recipeNameText Inspector'da atanmamýþ!", gameObject);
            return;
        }

        if (iconContainer == null)
        {
            Debug.LogError("DeliveryManagerSingleUI: iconContainer Inspector'da atanmamýþ!", gameObject);
            return;
        }

        if (iconTemplate == null)
        {
            Debug.LogError("DeliveryManagerSingleUI: iconTemplate Inspector'da atanmamýþ!", gameObject);
            return;
        }

        recipeNameText.text = recipeSO.recipeName;

        // Önce tüm child'larý (template hariç) sil
        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        // Null check ekledim
        if (recipeSO.kitchenObjectSOList == null || recipeSO.kitchenObjectSOList.Count == 0)
        {
            Debug.LogWarning("DeliveryManagerSingleUI: recipeSO.kitchenObjectSOList boþ!", gameObject);
            return;
        }

        // Yeni icon'larý oluþtur
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            // Null check ekledim
            if (kitchenObjectSO == null)
            {
                Debug.LogWarning("DeliveryManagerSingleUI: kitchenObjectSO null!", gameObject);
                continue;
            }

            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage == null)
            {
                Debug.LogError("DeliveryManagerSingleUI: iconTemplate'da Image component'i yok!", gameObject);
                continue;
            }

            iconImage.sprite = kitchenObjectSO.sprite;
        }
    }
}
