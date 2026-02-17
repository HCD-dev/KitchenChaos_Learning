using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        // Biraz bekleyerek singleton'ýn initialize olmasýný saðla
        if (KitchenGameManager.Instance == null)
        {
            Debug.LogError("GameStartCountdownUI: KitchenGameManager.Instance is null! Make sure KitchenGameManager is in the scene and loaded before this script.");
            return;
        }
        
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance != null && KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        if (KitchenGameManager.Instance != null)
        {
            countdownText.text = Mathf.Ceil(KitchenGameManager.Instance.GetCountdownToStartTimer()).ToString();
        }
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

