using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 무기 조작이 있는 스크립트
/// </summary>
public class Player_WeaponControl : MonoBehaviour
{
    Test_Player player;
    PlayerInputActions inputActions;

    // delegate
    public Action OnAttackEnd;  // 플레이어 공격이 종료되면 실행되는 델리게이트
    public Action OnSwitchWeapon; // 무기 교체시 실행하는 델리게이트 , key 1 (true : 무기를 바꿧다(원거리무기), Defalut = false : 무기를 안바꿧다(검) )
    public Action OnMeleeAttack; // 근접공격시 실행하는 델리게이트 , key MLB
    public Action OnRangeAttack; // 원거리 공격시 실행하는 델리게이트 , key MLB
    public Action OnAiming;     // 원거리 조준시 실행되는 델리게이트

    void Start()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Main.SwitchWeapon.performed += OnWeaponSwitchInput;

        inputActions.Sword.Attack.performed += OnAttackInput;
        inputActions.Sword.Attack.canceled += OnAttackInput;

        inputActions.Bow.Shot.performed += OnRangeShotInput;
        inputActions.Bow.AimDown.performed += OnAimDownInput;        
    }

    void OnDisable()
    {
        inputActions.Bow.AimDown.performed -= OnAimDownInput;
        inputActions.Bow.Shot.performed -= OnRangeShotInput;

        inputActions.Sword.Attack.canceled -= OnAttackInput;
        inputActions.Sword.Attack.performed -= OnAttackInput;

        inputActions.Sword.Disable();
        inputActions.Bow.Disable();
    }

    private void OnWeaponSwitchInput(InputAction.CallbackContext context)
    {
        OnSwitchWeapon?.Invoke();
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        OnMeleeAttack?.Invoke();
    }


    private void OnRangeShotInput(InputAction.CallbackContext context)
    {
        OnRangeAttack?.Invoke();
    }

    private void OnAimDownInput(InputAction.CallbackContext context)
    {
        OnAiming?.Invoke();
    }

}
