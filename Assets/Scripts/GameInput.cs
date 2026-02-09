using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameInput : MonoBehaviour
{
    // ========== EVENTLER ==========
    // E tuþuna basýldýðýnda tetiklenen event
    // Player sýnýfý bu event'e abone olur ve Counter ile etkileþime girer
            public event EventHandler OnInteractAction;

    // ========== INPUT SYSTEM REFERANSI ==========
    // Unity's new Input System'den otomatik üretilen input action sýnýfý
    // WASD, Ok tuþlarý, E tuþu tanýmlanmýþtýr
    private PlayerInputAction PlayerInputActions;

    /// <summary>
    /// Game baþladýðýnda input sistemi baþlatýlýr.
    /// Input action'larý enable edilir ve event callback'leri kaydedilir.
    /// </summary>
    private void Awake()
    {
        // Input System sýnýfýný oluþtur
        // (PlayerInputAction.inputactions dosyasýndan auto-generate edilmiþtir)
        PlayerInputActions = new PlayerInputAction();
        
        // Player input action map'ini etkinleþtir
        // Böylece WASD, Ok tuþlarý, E tuþu takip edilmeye baþlar
        PlayerInputActions.Player.Enable();
        
        // E tuþuna basýldýðýnda ("Interact" action'ý performed olduðunda)
        // Interact_performed metodunu çaðýrýlmasýný saðla
        PlayerInputActions.Player.Interact.performed += Interact_performed;
    }

    /// <summary>
    /// E tuþuna basýldýðýnda tetiklenen callback.
    /// OnInteractAction event'ini tetikleyerek Player sýnýfýný bilgilendirir.
    /// </summary>
    /// <param name="obj">Input System tarafýndan saðlanan callback context</param>
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // OnInteractAction event'ini tetikle
        // (Abone olan tüm methodlar çalýþacak, örneðin Player.GameInput_OnInteractAction)
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// WASD veya Ok tuþlarýndan hareket vektörü okur.
    /// Sonucu normalize eder (magnitude = 1 veya 0).
    /// </summary>
    /// <returns>
    /// Vector2: X=A/D tuþlarý, Y=W/S tuþlarý
    /// Deðerler -1 ile 1 arasýnda, normalize edilmiþ
    /// </returns>
    public Vector2 GetMovementVectorNormalized()
    {
        // Input System'den Move action'ýnýn mevcut deðerini oku
        // Vector2: (x: -1 to 1, y: -1 to 1) þeklinde döner
        Vector2 inputVector = PlayerInputActions.Player.Move.ReadValue<Vector2>();
        
        // Vektörü normalize et
        // Örneðin (1, 1) = diagonal hareket = (0.707, 0.707) (eþit hýz)
        // Bu sayede tüm yönlerde ayný hýzda hareket ederiz
        inputVector = inputVector.normalized;
        
        // Normalize edilmiþ vektörü döndür
        return inputVector;
    }
}
