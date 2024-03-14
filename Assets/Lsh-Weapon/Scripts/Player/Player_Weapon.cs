using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어와 무기상태따른 공격 스크립트
public class Player_Weapon : MonoBehaviour
{
    Player_WeaponControl weaponControl;
    Animator animator;

    enum WeaponState
    {
        Sword = 0,
        Bow
    }
    [SerializeField] WeaponState weaponState;    // 현재 가지고 있는 무기 Enum

    Sword sword;

    private bool isAttack = false;                       // 공격했는지 확인

    // Hashes
    readonly int AttackToHash = Animator.StringToHash("Attack");            // 공격용 파라미터
    readonly int IsEquipToHash = Animator.StringToHash("IsEquip");          // 무기장비착용 여부 

    void Start()
    {
        weaponControl = gameObject.GetComponent<Player_WeaponControl>();
        animator = gameObject.GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();

        weaponControl.OnAttackEnd += DisableIsAttack;
        weaponControl.OnMeleeAttack += OnMeleeAttack;
        weaponControl.OnAiming += OnAimDown;
        weaponControl.OnRangeAttack += OnRangeAttack;
        weaponControl.OnWeaponChange += OnWeaponChange;
    }

    private void OnWeaponChange()
    {
        if (weaponState == WeaponState.Sword)
        {
            weaponState = WeaponState.Bow;
        }
        else if (weaponState == WeaponState.Bow)
        {
            weaponState = WeaponState.Sword;
        }
    }

    private void OnMeleeAttack() // 근접무기
    {
        if (weaponState == WeaponState.Bow)
            return;

        isAttack = true;            // 공격 확인 
        animator.SetTrigger(AttackToHash);
        animator.SetBool(IsEquipToHash, true);
    }

    private void OnAimDown()
    {
        if (!animator.GetBool("AimDown"))
        {
            animator.SetTrigger("RangeAttack");
            animator.SetBool("AimDown", true);
            Debug.Log("화살 조준");
        }
    }

    private void OnRangeAttack()
    {
        if (animator.GetBool("AimDown"))
        {
            animator.SetBool("AimDown", false);
            Debug.Log("화살 발사");
        }
    }

    /// <summary>
    /// 공격 플레그 비활성화하는 함수 
    /// </summary>
    void DisableIsAttack()
    {
        isAttack = false;
    }

    /// <summary>
    /// 근접무기 콜라이더를 활성화 시키는 함수 ( 애니메이션 이벤트 함수 )
    /// </summary>
    void MeleeColliderEnable()
    {
        sword.MeleeWeaponColliderEnable();
    }

    /// <summary>
    /// 근접무기 콜라이더를 비활성화 시키는 함수 ( 애니메이션 이벤트 함수 )
    /// </summary>
    void MeleeColliderDisable()
    {
        sword.MeleeWeaponColliderDisable();
    }
}