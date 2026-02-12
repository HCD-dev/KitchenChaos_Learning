using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameInput : MonoBehaviour
{
    // ========== EVENTLER ==========
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    // ========== INPUT SYSTEM REFERANSI ==========
    private PlayerInputAction PlayerInputActions;

    private void Awake()
    {
        try
        {
            // Input System sýnýfýný oluþtur
            PlayerInputActions = new PlayerInputAction();

            // Player input action map'ini etkinleþtir
            PlayerInputActions.Player.Enable();

            // E tuþu callback'i
            PlayerInputActions.Player.Interact.performed += Interact_performed;

            // F tuþu callback'i (InteractAlternate)
            PlayerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"GameInput Awake hatasý: {ex.Message}");
        }
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        // PlayerInputActions null kontrolü
        if (PlayerInputActions == null)
        {
            Debug.LogError("PlayerInputActions null! Input sistem baþlatýlmamýþ.");
            return Vector2.zero;
        }

        try
        {
            // Input System'den Move action'ýnýn mevcut deðerini oku
            Vector2 inputVector = PlayerInputActions.Player.Move.ReadValue<Vector2>();
            
            // Vektörü normalize et
            inputVector = inputVector.normalized;
            
            return inputVector;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"GetMovementVectorNormalized hatasý: {ex.Message}");
            return Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        if (PlayerInputActions != null)
        {
            try
            {
                // Önce callback'leri kaldýr
                PlayerInputActions.Player.Interact.performed -= Interact_performed;
                PlayerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
                
                // Sonra Disable() çaðýr
                PlayerInputActions.Player.Disable();
                
                // Son olarak Dispose() çaðýr
                PlayerInputActions.Dispose();
                
                // Null yap
                PlayerInputActions = null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"GameInput OnDestroy hatasý: {ex.Message}");
            }
        }
    }
}
    