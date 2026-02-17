using System.Collections.Generic;
using System;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{


    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeSuccess;
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingrecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeSOListMax = 4;
    private int successfulRecipesAmount ;

    private void Awake()
    {
        Instance = this;
        waitingrecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
                    
            // Null check'ler ekledim
            if (recipeListSO == null)
            {
                Debug.LogError("DeliveryManager: recipeListSO Inspector'da atanmamýþ!", gameObject);
                return;
            }

            if (recipeListSO.recipeSOList == null || recipeListSO.recipeSOList.Count == 0)
            {
                Debug.LogError("DeliveryManager: recipeListSO.recipeSOList boþ veya null!", gameObject);
                return;
            }

            // Koþul düzeltildi: <=  yerine <  kullanýldý
            if (waitingrecipeSOList.Count < waitingRecipeSOListMax)
            {
                RecipeSO waitingrecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingrecipeSOList.Add(waitingrecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
                
            }
        }
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        // Null check ekledim
        if (plateKitchenObject == null)
        {
            Debug.LogError("DeliveryManager: plateKitchenObject null!", gameObject);
            return;
        }

        List<KitchenObjectSO> plateIngredients = plateKitchenObject.GetKitchenObjectSOList();
        
        // Null check ekledim
        if (plateIngredients == null)
        {
            Debug.LogError("DeliveryManager: plateIngredients null!", gameObject);
            return;
        }

        for (int i = 0; i < waitingrecipeSOList.Count; i++)
        {
            RecipeSO waitingrecipeSO = waitingrecipeSOList[i];
            
            // Null check ekledim
            if (waitingrecipeSO == null || waitingrecipeSO.kitchenObjectSOList == null)
            {
                Debug.LogWarning("DeliveryManager: Bekleyen recipe null!", gameObject);
                continue;
            }

            // 1. Önce sayý kontrolü yap
            if (waitingrecipeSO.kitchenObjectSOList.Count != plateIngredients.Count)
            {
                continue;
            }

            // 2. Recipe'deki tüm malzemelerin plate'de olup olmadýðýný kontrol et
            bool plateMatchesRecipe = true;
            foreach (KitchenObjectSO recipeKitchenObjectSO in waitingrecipeSO.kitchenObjectSOList)
            {
                // Plate listesinde bu malzeme var mý?
                if (!plateIngredients.Contains(recipeKitchenObjectSO))
                {
                    plateMatchesRecipe = false;
                    break;
                }
            }

            // 3. Eðer tüm malzemeler eþleþirse, teslim baþarýlý
            if (plateMatchesRecipe)
            {
                successfulRecipesAmount++;
                OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                waitingrecipeSOList.RemoveAt(i);
                Debug.Log("Recipe Delivered: " + waitingrecipeSO.recipeName);
                return;
            }
        }

        // Hiçbir recipe ile eþleþmedi
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        Debug.Log("Incorrect Delivery");
    }
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingrecipeSOList;
    }
    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }


}









