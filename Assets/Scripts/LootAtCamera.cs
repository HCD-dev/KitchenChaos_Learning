using UnityEngine;

public class LootAtCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }



}

