using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgresGameObject; 
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;
    private void Start()
    {
        hasProgress = hasProgresGameObject.GetComponent<IHasProgress>();
        
        if (hasProgress == null)
        {
            Debug.LogError($"ProgressBarUI: hasProgresGameObject Inspector'da atanmamýþ veya IHasProgress component'i yok!", gameObject);
            return;
        }   


        // Null check ekledim
        if (hasProgress == null)
        {
            Debug.LogError($"ProgressBarUI: hasProgress Inspector'da atanmamýþ!", gameObject);
            return;
        }

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;


        Hide();    
    }

    private void OnDestroy()
    {
        if (hasProgress != null)
        {
            hasProgress.OnProgressChanged -= HasProgress_OnProgressChanged;
        }
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        if (barImage != null)
        {
            barImage.fillAmount = e.progressNormalized;
        }
        
        // Progress 0 ise gizle, aksi halde göster
        if (e.progressNormalized <= 0f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetProgress(float progressNormalized)
    {
        if (barImage != null)
        {
            barImage.fillAmount = progressNormalized;
        }
    }
}
