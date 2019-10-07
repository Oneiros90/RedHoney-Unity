using UnityEngine;

public class StateMachineStarter : MonoBehaviour
{
    [Tooltip("The animator controller representing the state machine")]
    public StateMachine StateMachine;

    private void Start()
    {
        DontDestroyOnLoad(this);

        Animator animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = StateMachine.Controller;

        StateMachine.Initialize(animator);
    }
}
