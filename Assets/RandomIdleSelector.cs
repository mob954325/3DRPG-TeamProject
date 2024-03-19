using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleSelector : StateMachineBehaviour
{
    public int test = -1;
    readonly int IdleSelect_Hash = Animator.StringToHash("AttackSelect");

    int prevSelect;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(IdleSelect_Hash, RandomSelect());
    }

    /// <summary>
    /// 랜덤하게 0~2 사이의 값을 선택하는 함수(수 별로 확률 다름)
    /// </summary>
    /// <returns>0~4</returns>
    int RandomSelect()
    {
        int select = 0;
        if (prevSelect == 0) // 이전에 0 일땜나 특수 Idle 재생
        {
            float num = Random.value;

            if (num < 0.3f)
            {
                select = 2;             // 30
            }
            else if (num < 0.45f)
            {
                select = 1;             // 15
            }
        }
        if (test != -1)
        {
            select = test;
        }
        prevSelect = select;

        return select;
    }
}
