using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTimer : StateMachineBehaviour
{
    [SerializeField] float timer = 0f;
    const float endEquipTime = 5f;

    readonly int IsEquipToHash = Animator.StringToHash("IsEquip");
    readonly int SheathingToHash = Animator.StringToHash("Sheathing");

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetBool(IsEquipToHash))
            timer += Time.deltaTime;

        if(timer >= endEquipTime)   // 무기 착용 시간이 endEquipTime만큼 진행되면 무기를 집어넣음
        {
            animator.SetBool(IsEquipToHash, false);
            animator.SetTrigger(SheathingToHash);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f; // 벗어나면 타이머 초기화        
    }
}
