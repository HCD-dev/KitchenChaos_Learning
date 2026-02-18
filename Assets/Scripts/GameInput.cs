using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using UnityEngine.InputSystem; 


public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public static GameInput Instance { get; private set; }

    // ========== EVENTLER ==========
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    // ========== INPUT SYSTEM REFERANSI ==========
    private PlayerInputAction PlayerInputActions;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }
    private void Awake()
    {
        Instance = this;
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

            // Escape tuþu callback'i (Pause)
            PlayerInputActions.Player.Pause.performed += Pause_performed;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"GameInput Awake hatasý: {ex.Message}");
        }
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            string bindingsJson = PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS);
            PlayerInputActions.LoadBindingOverridesFromJson(bindingsJson);
        }
    }
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
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

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return PlayerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return PlayerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return PlayerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return PlayerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return PlayerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return PlayerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return PlayerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }



    }
    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        PlayerInputActions.Player.Disable();
        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = PlayerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = PlayerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = PlayerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = PlayerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = PlayerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = PlayerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                    inputAction = PlayerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose();
                PlayerInputActions.Player.Enable();
                onActionRebound();
                
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, PlayerInputActions.SaveBindingOverridesAsJson());


                PlayerPrefs.Save();

                
            })
            .Start();
    }
}