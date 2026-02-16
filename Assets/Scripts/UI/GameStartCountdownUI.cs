using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        KitchemGameManager.Instance.OnStateChanged += KitchemGameManager_OnStateChanged;
        //Hide();
    }

    private void KitchemGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if(KitchemGameManager.Instance.IsCountdownToStartActive())
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
        countdownText.text = Mathf.Ceil(KitchemGameManager.Instance.GetCountdownToStartTimer()).ToString();

    }
    private void Show() {       
        gameObject.SetActive(true);
    }
    private void Hide() {  
        gameObject.SetActive(false);
    }
}
