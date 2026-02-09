using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class Player : MonoBehaviour
{
    
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    public bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    public void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Birden fazla Player instance'ý var!");
        }
        Instance = this;

    }
    void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;

    }
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
       if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();

    }


    public bool IsWalking()
    {
        return isWalking;
    }
    private void HandleInteraction()
    {
        // Raycast yönü - oyuncunun BAKTAÐI yön (transform.forward)
        Vector3 rayDirection = transform.forward;

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit raycastHit, interactionDistance, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();


        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

        // lastInteractDir'i her hareket güncellemesinde güncelle
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // X hareket denemeleri
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f);
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                // Z hareket denemeleri
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
                else
                {
                    //Her iki yönde de hareket edilemiyor
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        isWalking = moveDir != Vector3.zero;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }




    }
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        { selectedCounter = selectedCounter });

    }

}
