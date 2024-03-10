using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackComboControl : StateMachineBehaviour
{
    [Tooltip("공격 콤보 갯수")]
    public int attackComboCount = 3;
    int currentAttack = 1;  // 현재 공격

    const string meleeAttackBase = "MeleeAttack";

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetTrigger($"{meleeAttackBase}{currentAttack}");
        currentAttack++;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentAttack > attackComboCount)
        {
            // 공격콤보가 공격 갯수보다 넘으면 초기화
            currentAttack = 1;
        }
    }
}
