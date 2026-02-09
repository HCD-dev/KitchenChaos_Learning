using UnityEngine;
using System;

/// <summary>
/// Counter seçimini görsel olarak göstermek için kullanýlýr.
/// Her counter'ýn üzerine bu script yerleþtirilir.
/// Seçili counter'ýn visual'ý açýk, seçili olmayan counter'larýn visual'ý kapalý.
/// </summary>
public class SelectedCounterVisual : MonoBehaviour
{
    // ========== ÝNCELEYÝCÝ AYARLARI ==========
    // Bu visual'ýn ait olduðu counter
    // (Bu script her counter'ýn visual objesine atanmalý)
    [SerializeField] private ClearCounter clearCounter;
    
    // Seçili olduðunda gösterilecek visual GameObject
    // (Yani açýk ýþýk, parlama efekti, vb.)
    [SerializeField] private GameObject visualGameObject;

    /// <summary>
    /// Game baþladýðýnda visual baþlatýlýr.
    /// Event listener kaydedilir ve visual kapalý baþlatýlýr.
    /// </summary>
    private void Start()
    {
        // Baþlangýçta visual'ý kapatýn
        // (Henüz bu counter seçili deðil)
        Hide();
        
        // Player'ýn OnSelectedCounterChanged event'ine abone ol
        // Counter seçimi deðiþirse, Player_OnSelectedCounterChanged metodunu çaðýrmasýný istiyoruz
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    /// <summary>
    /// Game biterken event listener'ýný kaldýrarak memory leak'ý önle.
    /// </summary>
    private void OnDestroy()
    {
        // Eðer Player instance'ý hala varsa, event'ten abone oluþunu kaldýr
        // (Memory leak ve null reference exception'ý önler)
        if (Player.Instance != null)
        {
            Player.Instance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
        }
    }

    /// <summary>
    /// Player sýnýfýndan tetiklenen event handler.
    /// Counter seçimi deðiþtiðinde çalýþýr.
    /// </summary>
    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        // Eðer seçilen counter bu counter'sa (bu script'in ait olduðu counter)
        if (e.selectedCounter == clearCounter)
        {
            // Visual'ý aç (ýþýk, particle, efekt, vb. göster)
            Show();
        }
        else
        {
            // Seçilen counter bu deðilse visual'ý kapat
            Hide();
        }
    }

    /// <summary>
    /// Visual GameObject'ini etkinleþtir (ýþýk/efekt açýlýr).
    /// </summary>
    private void Show()
    {
        visualGameObject.SetActive(true);
    }

    /// <summary>
    /// Visual GameObject'ini devre dýþý býrak (ýþýk/efekt kapanýr).
    /// </summary>
    private void Hide()
    {
        visualGameObject.SetActive(false);
    }
}
                