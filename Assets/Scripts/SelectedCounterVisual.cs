using UnityEngine;
using System;

/// <summary>
/// Counter seçimini görsel olarak göstermek için kullanýlýr.
/// Her counter'ýn üzerine bu script yerleþtirilir (ClearCounter, ContainerCounter, vb.).
/// Seçili counter'ýn visual'ý açýk, seçili olmayan counter'larýn visual'ý kapalý.
/// </summary>
public class SelectedCounterVisual : MonoBehaviour
{
    // ========== ÝNCELEYÝCÝ AYARLARI ==========
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        Hide();
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if ((UnityEngine.Object)e.selectedCounter == (UnityEngine.Object)baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    { foreach (var visual in visualGameObjectArray)
        {
            visual.SetActive(true);
        }
        
    }
    private void Hide()
    {
        foreach (var visual in visualGameObjectArray)
        {
            visual.SetActive(false);
        }

    }

}