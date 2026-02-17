using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GamePlayingCloakUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Update()
    {
        if (KitchenGameManager.Instance == null) return;
        
        timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
    }
}

