using System;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    private void Start()
    {
        KitchemGameManager.Instance.OnStateChanged += KitchemGameManager_OnStateChanged;
        //Hide();
    }

    private void KitchemGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (KitchemGameManager.Instance.IsGameOver())
        {
            Show();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
        }
        else
        {
            Hide();
        }
    }
    private void Update()
    {
        

    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }


}
