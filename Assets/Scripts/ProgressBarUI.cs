using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image barImage;
    
    
    private void Start()
    {
        // Null check ekledim
        if (cuttingCounter == null)
        {
            Debug.LogError($"ProgressBarUI: cuttingCounter Inspector'da atanmamýþ!", gameObject);
            return;
        }

        cuttingCounter.OnProgressChanged += CuttingCounter_OnProgressChanged;

        Hide();    
    }

    private void OnDestroy()
    {
        if (cuttingCounter != null)
        {
            cuttingCounter.OnProgressChanged -= CuttingCounter_OnProgressChanged;
        }
    }

    private void CuttingCounter_OnProgressChanged(object sender, CuttingCounter.OnProgressChangedEventArgs e)
    {
        if (barImage != null)
        {
            barImage.fillAmount = e.progressNormalized;
        }
        if (e.progressNormalized == 0f || e.progressNormalized == 1f)
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
