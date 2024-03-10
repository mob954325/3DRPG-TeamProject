using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControl : StateMachineBehaviour
{
    // 플레이어 공격 비활성화 하기 위한 스크립트

    Test_Player player;

    void Awake()
    {
        player = FindAnyObjectByType<Test_Player>();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.OnAttacAction?.Invoke();
    }
}
