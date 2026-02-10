using System;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    [SerializeField] ContainerCounter containerCounter;
    private Animator animator;
    private const string OPEN_CLOSE = "OpenClose";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        containerCounter.OnPlayerInteract += ContainerCounter_OnPlayerInteract;
    }

    private void ContainerCounter_OnPlayerInteract(object sender, EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
