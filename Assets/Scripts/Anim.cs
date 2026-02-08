using UnityEngine;

public class Anim : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator bileþeni bulunamadý!");
            return;
        }

        // Player.Instance'ý kullanarak event'e abone ol
        if (Player.Instance != null)
        {
            // Ýsteðe baðlý: Player'ýn seçilen counter deðiþtiðinde animasyonu güncelle
        }
    }

    void Update()
    {
        if (animator == null)
        {
            return;
        }

        // Player.Instance'ý doðrudan kullan
        if (Player.Instance != null)
        {
            animator.SetBool(IS_WALKING, Player.Instance.IsWalking());
        }
    }
}