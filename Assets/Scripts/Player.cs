using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



/// <summary>
/// Oyuncunun hareket, rotasyon ve etkileşim kontrollerini yönetir.
/// Singleton pattern kullanarak global erişim sağlar.
/// Counter seçimini trackler ve event sistemi ile diğer sistemlere bildirir.
/// </summary>
public class Player : MonoBehaviour, IKitchenObjectParent
{
    // ========== SİNGLETON PATTERN ==========
    // Oyunun herhangi bir yerinden Player.Instance ile erişim sağlayan static property
    public static Player Instance { get; private set; }

    // ========== EVENTLER ==========
    // Counter seçimi değiştiğinde tetiklenen event
    // Diğer sistemler (UI, Visual) bu event'i dinleyerek kendilerini güncelleyebilir
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    /// <summary>
    /// OnSelectedCounterChanged event'i ile geçirilen veri.
    /// Seçilen counter'ı içerir.
    /// </summary>
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter; // Yeni seçili counter (null ise hiçbiri seçili değil)
    }

    // ========== İNCELEYİCİ AYARLARI ==========
    // Oyuncunun hareket hızı (birim/saniye)
    [SerializeField] private float moveSpeed = 7f;

    // Oyuncunun dönme hızı (başa çevirmesi ne kadar hızlı)
    [SerializeField] private float rotationSpeed = 10f;

    // Input sistemi referansı (Keyboard input okumak için)
    [SerializeField] private GameInput gameInput;



    // ========== ETKILEŞIM AYARLARI ==========
    // Counter'a etkileşim yapılabilecek maksimum mesafe (birim cinsinden)
    // Bu sayede çok uzaktaki counter'lar seçilemiyor
    [SerializeField] private float interactionDistance = 2f;

    // Raycast'ın bir counter'ı seçebilmesi için oyuncuya ne kadar yakın olması gerekir
    // (Yarıçap - counter'ın tespit edilebilir alanı)
    [SerializeField] private float detectionRadius = 0.5f;

    // Counter'ları seçebilmek için gerekli layer mask
    // (Raycast sadece bu layerdeki objeleri algılar)
    [SerializeField] private LayerMask counterLayerMask;

    //neden
    [SerializeField] private Transform KitchenObjectHoldPoint;


    // ========== HAREKET İÇİN FLAGLAR ==========
    // Oyuncu şu anda hareket ediyor mu? (Animasyon için kullanılır)
    public bool isWalking;

    // Etkileşim için son geçerli yön (oyuncu hareket ederken kaydedilir)
    // Bu yön T tuşu ile test amacında kullanılır
    private Vector3 lastInteractDir;

    // ========== COUNTER SEÇİMİ ==========
    // Şu anda seçili olan counter referansı (null = seçili yok)
    private ClearCounter selectedCounter;
    private KitchenObject kitchenObject;


    /// <summary>
    /// Game başladığında bir kez çalışır.
    /// Singleton ayarlanır ve başlatma kontrolleri yapılır.
    /// </summary>



    public void Awake()
    {
        // Eğer zaten bir Player instance'ı varsa, yeni olanı oluşturmayı engelleyin
        // (Sahne değiştiğinde yanlışlıkla çift Player oluşmasını önler)
        if (Instance != null)
        {
            Debug.LogError("Birden fazla Player instance'ı var!");
        }

        // Bu instance'ı global singleton yapın
        Instance = this;
    }

    /// <summary>
    /// Game başladığında Awake'den sonra çalışır.
    /// Event listener'ları kurulur.
    /// </summary>
    void Start()
    {
        // Oyuncu E tuşuna bastığında çağrılacak fonksiyonu kaydedin
        // Böylece GameInput sınıfı E tuşuna basıldığını algıladığında bunu bilir
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    /// <summary>
    /// E tuşuna basıldığında tetiklenen event handler.
    /// Seçili counter varsa onunla etkileşim kurar.
    /// </summary>
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        // Eğer seçili bir counter varsa (raycast tarafından bulundu)
        if (selectedCounter != null)
        {
            // Counter'ın Interact metodunu çağırarak etkileşime gir
            selectedCounter.Interact(this);
        }
        // Eğer seçili counter yoksa hiçbir şey yapmaz
    }   

    /// <summary>
    /// Her frame'de oyuncu input'unu işler.
    /// Hareket ve etkileşim kontrolleri yapılır.
    /// </summary>
    void Update()
    {
        // Hareket inputlarını oku ve oyuncuyu hareket ettir
        HandleMovement();

        // Counter seçimini kontrol et (raycast ile)
        HandleInteraction();
    }

    /// <summary>
    /// Oyuncunun şu anda yürüyüp yürümediğini döndürür.
    /// Animasyon sistemi tarafından kullanılır (walking animation trigger'ı için).
    /// </summary>
    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }


    /// <summary>
    /// Üç farklı yöntem karşılaştırması:
    /// 
    /// 1. RAYCAST (MEVCUT - Sorunlu):
    ///    - Sadece en yakın objeyi bulur
    ///    - Dar açı - yakındaki counter'lar seçilmeyebilir
    ///    - Hızlı ama sınırlı
    /// 
    /// 2. OVERLAPSPHEREAALL (ÖNERİLEN - SEÇILMIŞ):
    ///    - Belirli yarıçapta TÜM counter'ları bulur
    ///    - Sonra en yakınını seçer
    ///    - Doğru ve esnek - forward check yapılır
    /// 
    /// 3. BOX/CAPSULE CAST:
    ///    - Daha karmaşık hesaplamalar
    ///    - Büyük oyunlar için optimize edilmiş
    /// </summary>
    private void HandleInteraction()
    {
        // ========== YÖNTEM 1: RAYCAST (ESKI - SORUNLU) ==========
        // Vector3 rayDirection = transform.forward;
        // if (Physics.Raycast(transform.position, rayDirection, out RaycastHit raycastHit, interactionDistance, counterLayerMask))
        // {
        //     if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
        //     {
        //         if (clearCounter != selectedCounter)
        //         {
        //             SetSelectedCounter(clearCounter);
        //         }
        //     }
        //     else
        //     {
        //         SetSelectedCounter(null);
        //     }
        // }
        // else
        // {
        //     SetSelectedCounter(null);
        // }

        // ========== YÖNTEM 2: OVERLAPSPHEREAALL + FORWARD CHECK (ÖNERİLEN) ==========
        // Oyuncunun konumunda belirli yarıçapta TÜM collider'ları bul
        // QueryTriggerInteraction.Ignore = trigger collider'ları dahil etme
        Collider[] collidersInRange = Physics.OverlapSphere(
            transform.position,                          // Oyuncunun konumu merkezdir
            interactionDistance,                         // 2 birim içinde ara
            counterLayerMask,                            // Sadece Counter layer'ı ara
            QueryTriggerInteraction.Ignore               // Trigger olmayan collider'lar
        );

        // Eğer hiç counter bulunamadıysa, seçimi temizle
        if (collidersInRange.Length == 0)
        {
            SetSelectedCounter(null);
            return;
        }

        // En yakın counter'ı bul
        ClearCounter closestCounter = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in collidersInRange)
        {
            // Bu collider'dan ClearCounter bileşeni al
            if (!collider.TryGetComponent<ClearCounter>(out ClearCounter counter))
            {
                continue; // Bu obje ClearCounter'sa değil, atla
            }

            // Counter'a kadar olan mesafeyi hesapla
            float distanceToCounter = Vector3.Distance(transform.position, counter.transform.position);

            // Ayrıca OYUNCU BAKTAĞI YÖNE KONTROL ET
            // Counter oyuncunun arkasında mı? (90 derece kural)
            // direction = counter'a doğru yön
            Vector3 directionToCounter = (counter.transform.position - transform.position).normalized;

            // Dot product: 0.7 = ~45 derece açı (rahatlık için)
            // 1.0 = Tam önde, 0 = 90 derece, -1 = Tam arkada
            float angleAlignment = Vector3.Dot(transform.forward, directionToCounter);

            // Eğer counter oyuncunun arkasında 45 dereceden fazla ise yoksay
            if (angleAlignment < 0.7f)
            {
                continue; // Bu counter oyuncunun "önünde" değil, atla
            }

            // En yakın olanı bul
            if (distanceToCounter < closestDistance)
            {
                closestDistance = distanceToCounter;
                closestCounter = counter;
            }
        }

        // En yakın counter'ı seçin (ya da null)
        if (closestCounter != selectedCounter)
        {
            SetSelectedCounter(closestCounter);
        }
    }

    // ========== ALTERNATIF: BOXCAST YÖNTEMİ (İleri Seviye) ==========
    // Bu metod daha karmaşık ama daha hassas kontrol sağlar
    // private void HandleInteraction_BoxCastMethod()
    // {
    //     // Oyuncunun önünde bir "kutu" ile çarpışma kontrolü yap
    //     Vector3 boxSize = new Vector3(1f, 2f, 2f); // Genişlik, Yükseklik, Derinlik
    //     
    //     // Physics.BoxCastAll ile TÜM counter'ları bul
    //     RaycastHit[] hits = Physics.BoxCastAll(
    //         center: transform.position,
    //         halfExtents: boxSize / 2,
    //         direction: transform.forward,
    //         distance: interactionDistance,
    //         layerMask: counterLayerMask
    //     );
    //     
    //     // En yakınını seç...
    // }

    // ========== DEBUG VİZÜEL (Scene'de görüntülemek için) ==========
    // Update metodunda çağırıp görsel hatayı düzeltebilirsin
    private void OnDrawGizmosSelected()
    {
        // Etkileşim alanını visualize et (scene view'da)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // Oyuncunun baktığı yönü göster
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * interactionDistance);
    }

    private void SetSelectedCounter(ClearCounter counter)
    {
        selectedCounter = counter;
        // Event tetikleme
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = counter });
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

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
}

