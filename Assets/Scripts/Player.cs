using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Oyuncunun hareket, rotasyon ve etkileşim kontrollerini yönetir.
/// Singleton pattern kullanarak global erişim sağlar.
/// Counter seçimini trackler ve event sistemi ile diğer sistemlere bildirir.
/// </summary>
public class Player : MonoBehaviour, IKitchenObjectParent
{


    // ========== CONSTANTS ==========
    private const float PLAYER_RADIUS = 0.7f;
    private const float PLAYER_HEIGHT = 2f;
    private const float FORWARD_CHECK_THRESHOLD = 0.7f; // ~45 derece
    private const float MOVEMENT_THRESHOLD = 0.5f;

    // ========== SİNGLETON PATTERN ==========
    public static Player Instance { get; private set; }

    // ========== EVENTLER ==========
    public event EventHandler OnPickedSomething;
    
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public IKitchenObjectParent selectedCounter;
    }

    // ========== İNCELEYİCİ AYARLARI ==========
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;     
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;

    // ========== PRIVATE FIELDS ==========
    public bool isWalking;
    private Vector3 lastInteractDir;
    private IKitchenObjectParent selectedCounter;
    private KitchenObject kitchenObject;

    public void Awake()
    {
        // Singleton kontrolü iyileştirildi
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Birden fazla Player instance'ı var! Yeni instance yok ediliyor.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Null check eklendi
        if (gameInput == null)
        {
            Debug.LogError("GameInput referansı atanmamış!");
            return;
        }

        gameInput.OnInteractAction += GameInput_OnInteractAction;
         gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        HandleCounterInteraction((counter) => counter.Interact(this));
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        HandleCounterInteraction((counter) => counter.InteractAlternate(this));
    }

    private void HandleCounterInteraction(System.Action<BaseCounter> interactionAction)
    {
        if (selectedCounter != null)
        {
            // IKitchenObjectParent BaseCounter türüne cast edebiliriz
            if (selectedCounter is BaseCounter baseCounter)
            {
                interactionAction(baseCounter);
            }
            else
            {
                Debug.LogWarning("selectedCounter BaseCounter türü değil!");
            }
        }
        else
        {
            Debug.LogWarning("selectedCounter null! Counter aralığında değilsiniz.");
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

    private void HandleMovement()
    {
        // Null check ekleyin
        if (gameInput == null)
        {
            Debug.LogError("GameInput referansı null!");
            return;
        }

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * PLAYER_HEIGHT,
            PLAYER_RADIUS,
            moveDir,
            moveDistance
        );

        if (!canMove)
        {
            // X ekseni hareketi dene
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (Mathf.Abs(moveDir.x) > MOVEMENT_THRESHOLD) &&
                     !Physics.CapsuleCast(
                         transform.position,
                         transform.position + Vector3.up * PLAYER_HEIGHT,
                         PLAYER_RADIUS,
                         moveDirX,
                         moveDistance
                     );

            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                // Z ekseni hareketi dene
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (Mathf.Abs(moveDir.z) > MOVEMENT_THRESHOLD) &&
                         !Physics.CapsuleCast(
                             transform.position,
                             transform.position + Vector3.up * PLAYER_HEIGHT,
                             PLAYER_RADIUS,
                             moveDirZ,
                             moveDistance
                         );

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleInteraction()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(
            transform.position,
            interactionDistance,
            counterLayerMask,
            QueryTriggerInteraction.Ignore
        );

        if (collidersInRange.Length == 0)
        {
            SetSelectedCounter(null);
            return;
        }

        IKitchenObjectParent closestCounter = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in collidersInRange)
        {
            // Hem ClearCounter hem ContainerCounter'ı al
            if (!collider.TryGetComponent<IKitchenObjectParent>(out IKitchenObjectParent counter))
                continue;

            // IKitchenObjectParent'tan GameObject'e erişmek için
            GameObject counterGameObject = collider.gameObject;
            
            float distanceToCounter = Vector3.Distance(transform.position, counterGameObject.transform.position);
            Vector3 directionToCounter = (counterGameObject.transform.position - transform.position).normalized;
            float angleAlignment = Vector3.Dot(transform.forward, directionToCounter);

            if (angleAlignment < FORWARD_CHECK_THRESHOLD)
                continue;

            if (distanceToCounter < closestDistance)
            {
                closestDistance = distanceToCounter;
                closestCounter = counter;
            }
        }

        if (closestCounter != selectedCounter)
        {
            SetSelectedCounter(closestCounter);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * interactionDistance);
    }

    private void SetSelectedCounter(IKitchenObjectParent counter)
    {
        selectedCounter = counter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = counter });
    }

    // ========== IKitchenObjectParent Implementation ==========
    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    // ========== CLEANUP ==========
    private void OnDestroy()
    {
        if (gameInput != null)
        {
            gameInput.OnInteractAction -= GameInput_OnInteractAction;
            gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
        }
    }
}