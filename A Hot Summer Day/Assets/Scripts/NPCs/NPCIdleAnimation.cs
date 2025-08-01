using UnityEngine;

public class NPCIdleAnimation : StateMachineBehaviour
{
    [SerializeField] private float _timeUntilBored;
    [SerializeField] private int _numberOfIdleAnimations;

    private readonly int _idleIndexHash = Animator.StringToHash("Idle Index");

    private bool _isBored;
    private float _idleTime;
    private int _idleIndex;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isBored)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime > _timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;

                // default idle animations added in between special idles to smooth transition
                // speical idles are only on odd numbers
                _idleIndex = Random.Range(1, _numberOfIdleAnimations + 1) * 2 - 1;

                animator.SetFloat(_idleIndexHash, _idleIndex - 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f)
        {
            ResetIdle();
        }

        animator.SetFloat(_idleIndexHash, _idleIndex, 0.2f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        if (_isBored)
        {
            _idleIndex--;
        }

        _isBored = false;
        _idleTime = 0;        
    }
}
