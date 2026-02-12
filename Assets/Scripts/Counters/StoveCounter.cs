using System;
using UnityEngine;

public class StoveCounter : BaseCounter , IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurnedRecipeSO[] burnedRecipeSOArray;

    public State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurnedRecipeSO burnedRecipeSO;

    private void Update()
    {
        if (!HasKitchenObject())
        {
            state = State.Idle;
            fryingTimer = 0f;
            fryingRecipeSO = null;
            burnedRecipeSO = null;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            return;
        }

        switch (state)
        {
            case State.Idle:
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                break;

            case State.Frying:
                if (fryingRecipeSO == null)
                {
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    if (fryingRecipeSO == null)
                    {
                        state = State.Idle;
                        break;
                    }
                }

                fryingTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = fryingTimer / fryingRecipeSO.FryingTimerMax
                });

                if (fryingTimer > fryingRecipeSO.FryingTimerMax)
                {
                    GetKitchenObject().DestorySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                    fryingTimer = 0f;
                    fryingRecipeSO = null;
                    state = State.Fried;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    Debug.Log("Object fried!");
                }
                break;

            case State.Fried:
                if (burnedRecipeSO == null)
                {
                    burnedRecipeSO = GetBurnedRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    if (burnedRecipeSO == null)
                    {
                        state = State.Idle;
                        break;
                    }
                }

                fryingTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = fryingTimer / burnedRecipeSO.BurnedTimerMax
                });

                if (fryingTimer > burnedRecipeSO.BurnedTimerMax)
                {
                    GetKitchenObject().DestorySelf();
                    KitchenObject.SpawnKitchenObject(burnedRecipeSO.output, this);
                    fryingTimer = 0f;
                    burnedRecipeSO = null;
                    state = State.Burned;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    Debug.Log("Object burned!");
                }
                break;

            case State.Burned:
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                break;
        }
    }

    private void Start()
    {
        state = State.Idle;

        if (fryingRecipeSOArray == null || fryingRecipeSOArray.Length == 0)
        {
            Debug.LogError($"StoveCounter: fryingRecipeSOArray Inspector'da atanmamýþ veya boþ!", gameObject);
        }

        if (burnedRecipeSOArray == null || burnedRecipeSOArray.Length == 0)
        {
            Debug.LogWarning($"StoveCounter: burnedRecipeSOArray Inspector'da atanmamýþ veya boþ!", gameObject);
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                KitchenObjectSO kitchenObjectSO = player.GetKitchenObject().GetKitchenObjectSO();
                if (HasRecipeWithInput(kitchenObjectSO))
                {
                    player.GetKitchenObject().SetClearCounter(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
                    fryingTimer = 0f;
                    state = State.Frying;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                }

            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetClearCounter(player);
                state = State.Idle;
                fryingTimer = 0f;
                fryingRecipeSO = null;
                burnedRecipeSO = null;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(inputKitchenObjectSO) != null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        if (fryingRecipeSOArray == null || fryingRecipeSOArray.Length == 0)
        {
            return null;
        }

        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO != null && fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurnedRecipeSO GetBurnedRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        if (burnedRecipeSOArray == null || burnedRecipeSOArray.Length == 0)
        {
            return null;
        }

        foreach (BurnedRecipeSO burnedRecipeSO in burnedRecipeSOArray)
        {
            if (burnedRecipeSO != null && burnedRecipeSO.input == inputKitchenObjectSO)
            {
                return burnedRecipeSO;
            }
        }
        return null;
    }
}
