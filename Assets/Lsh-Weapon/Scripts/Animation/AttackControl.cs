using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControl : StateMachineBehaviour
{
    // 플레이어 공격 비활성화 하기 위한 스크립트

    Player_WeaponControl player;

    void Awake()
    {
        player = FindAnyObjectByType<Player_WeaponControl>();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.OnAttackEnd?.Invoke();
    }
}
