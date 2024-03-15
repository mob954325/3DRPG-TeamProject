using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어와 무기상태따른 공격 스크립트
public class Player_Weapon : MonoBehaviour
{
    enum WeaponState
    {
        Sword = 0,
        Bow
    }
    [SerializeField] WeaponState weaponState;    // 현재 가지고 있는 무기 Enum

    Player_WeaponControl weaponControl;
    Animator animator;    
    Sword sword;

    public GameObject arrowPrefab;
    [SerializeField] Transform rightHandTransform;

    private bool isAttack = false;                       // 공격했는지 확인

    readonly int AttackToHash = Animator.StringToHash("Attack");            // 공격용 파라미터
    readonly int IsEquipToHash = Animator.StringToHash("IsEquip");          // 무기장비착용 여부 
    readonly int AimDownToHash = Animator.StringToHash("AimDown");          // 원거리무기 조준 여부
    readonly int IsRangeWeaponToHash = Animator.StringToHash("IsRangeWeapon");  // 원거리무기 여부

    void Start()
    {
        weaponControl = gameObject.GetComponent<Player_WeaponControl>();
        animator = gameObject.GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();
        rightHandTransform = GetComponentInChildren<Player_RightHand>().gameObject.transform;

        weaponControl.OnAttackEnd += DisableIsAttack;
        weaponControl.OnMeleeAttack += OnMeleeAttack;
        weaponControl.OnAiming += OnAimDown;
        weaponControl.OnRangeAttack += OnRangeAttack;
        weaponControl.OnWeaponChange += OnWeaponChange;

        if(arrowPrefab == null)
        {
            Debug.LogError("화살 프리팹이 비어있습니다.");
        }
    }

    private void OnWeaponChange()
    {
        if (weaponState == WeaponState.Sword)
        {
            weaponState = WeaponState.Bow;
            animator.SetBool(IsRangeWeaponToHash, true);
        }
        else if (weaponState == WeaponState.Bow)
        {
            weaponState = WeaponState.Sword;
            animator.SetBool(IsRangeWeaponToHash, false);
        }
    }

    private void OnMeleeAttack() // 근접무기
    {
        isAttack = true;            // 공격 확인 
        animator.SetTrigger(AttackToHash);
        animator.SetBool(IsEquipToHash, true);
    }

    private void OnAimDown()
    {
        if (!animator.GetBool(AimDownToHash))
        {
            animator.SetTrigger(AttackToHash);
            animator.SetBool(AimDownToHash, true);
            Debug.Log("화살 조준");

            Vector3 playerDir = transform.forward;
            Vector3 arrowDir = arrowPrefab.transform.forward;

            Debug.Log($"{playerDir}, {arrowDir}");
            

            Instantiate(arrowPrefab, rightHandTransform); // 화살 소환
        }
    }

    private void OnRangeAttack()
    {
        if (animator.GetBool(AimDownToHash))
        {
            animator.SetBool(AimDownToHash, false);
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