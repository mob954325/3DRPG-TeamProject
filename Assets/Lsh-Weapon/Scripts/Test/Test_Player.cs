using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 움직임 상태 enum
/// </summary>
enum MoveState
{   // 움직임 상태
    Idle = 0,
    Walk,
    Sprint,
}

enum WeaponState
{
    Sword = 0,
    Bow
}

// Player-Weapon 테스트 인풋용 스크립트
public class Test_Player : MonoBehaviour
{

    // 버튼으로 무기 교체 추가하고 활 인풋 실험

    MoveState moveState;        // 움직임 상태 Enum
    [SerializeField] WeaponState weaponState;    // 현재 가지고 있는 무기 Enum

    WeaponState CurrentWeaponState
    {
        get => weaponState;
        set
        {
            weaponState = value;

            if(weaponState == WeaponState.Bow)
            {
                inputActions.Bow.Enable();
                Debug.Log("활 활성화");
            }
            else
            {
                inputActions.Bow.Disable();
                Debug.Log("활 비활성화");
            }
        }
    }

    /// <summary>
    /// 플레이어 움직임 상태에 따른 변수값 변경 프로퍼티
    /// </summary>
    MoveState PlayerMoveState
    {
        get => moveState;
        set
        {
            moveState = value;
            Debug.Log(moveState);
            switch(moveState)
            {
                case MoveState.Idle:
                    animMoveSpeed = 0f;
                    break;
                case MoveState.Walk:
                    animMoveSpeed = 0.5f;
                    break;
                case MoveState.Sprint:
                    animMoveSpeed = 1f;
                    break;
            }
        }
    }

    // Components
    PlayerInputActions inputActions;
    CharacterController controller;
    Animator animator;

    // Values
    Vector3 inputDirection = Vector3.zero;                          // 입력 받는 방향 벡터
    public float moveSpeed = 3f;                                    // 실제 플레이어 이동속도
    Vector3 smoothInputVelocity = Vector3.zero;                     // SmoothDamp용 현재 Velocity값
    [Tooltip("플레이어가 최고 속도에 도달할 속도 값( 값이 낮을 수록 더 빨리 도달함 , Default : 0.2)")]
    [SerializeField] float smoothInputSpeed = .2f;                  // SmoothDamp가 도달할 값 ( 값이 작을 수록 더 빨리 도달함 )
    float animMoveSpeed = 0f;                                       // 애니메이션 파라미터 전달용 함수

    [SerializeField] Vector3 currentMoveVector = Vector3.zero;
    Quaternion targetRotation = Quaternion.identity;                // 회전할 목표 회전값
    public float turnspeed = 10.0f;                                 // 회전 속도

    [SerializeField]bool isAttack = false;                          // 공격했는지 확인
   
    // Hashes
    readonly int SpeedToHash = Animator.StringToHash("Speed");              // 이동용 파라미터
    readonly int AttackToHash = Animator.StringToHash("Attack");            // 공격용 파라미터
    readonly int IsEquipToHash = Animator.StringToHash("IsEquip");          // 무기장비착용 여부 애니메이션

    // delegate
    public Action OnAttackEnd;  // 플레이어 공격이 종료되면 실행되는 델리게이트

    void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        PlayerMoveState = MoveState.Idle;

        OnAttackEnd += DisableIsAttack;
    }

    void OnEnable()
    {
        inputActions.Sword.Enable();
        inputActions.Sword.Move.performed += OnMoveInput;
        inputActions.Sword.Move.canceled += OnMoveInput;
        inputActions.Sword.Attack.performed += OnAttackInput;
        inputActions.Sword.Attack.canceled += OnAttackInput;
        inputActions.Sword.Sprint.performed += OnSpritInput;
        inputActions.Sword.Sprint.canceled += OnSpritInput;
        //inputActions.Sword.Look.performed += OnLookInput;
        inputActions.Bow.Shot.performed += OnBowShotInput;
        inputActions.Bow.AimDown.performed += OnAimDownInput;
    }

    void OnDisable()
    {
        //inputActions.Sword.Look.performed -= OnLookInput;
        inputActions.Sword.Sprint.canceled -= OnSpritInput;
        inputActions.Sword.Sprint.performed -= OnSpritInput;
        inputActions.Sword.Attack.canceled -= OnAttackInput;
        inputActions.Sword.Attack.performed -= OnAttackInput;
        inputActions.Sword.Move.canceled -= OnMoveInput;
        inputActions.Sword.Move.performed -= OnMoveInput;
        inputActions.Sword.Disable();      
        inputActions.Bow.Disable();      
    }

    void Update()
    {
        if (!isAttack)
            OnMove();
    }

    private void OnAttackInput(InputAction.CallbackContext context) // 근접무기
    {
        if (weaponState == WeaponState.Bow)
            return;

        if (context.performed && !isAttack)
        {
            isAttack = true;            // 공격 확인 
            animator.SetTrigger(AttackToHash);
            animator.SetBool(IsEquipToHash, true);
        }
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        inputDirection.x = inputVector.x;
        inputDirection.z = inputVector.y;
        
        if (context.performed)
        {
            // turn
            Quaternion camY = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0); // 카메라 Y값
            inputDirection = camY * inputDirection;                                     // 카메라 기준 회전값
            targetRotation = Quaternion.LookRotation(inputDirection);                   // 회전할 방향값

            PlayerMoveState = MoveState.Walk;
        }
        else
        {
            PlayerMoveState = MoveState.Idle;
        }
    }

    private void OnSpritInput(InputAction.CallbackContext context)
    {
        if (context.performed && PlayerMoveState == MoveState.Walk)
        {
            PlayerMoveState = MoveState.Sprint;
        }
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        Vector3 mousepos = context.ReadValue<Vector2>();
        Debug.Log(mousepos);
    }

    private void OnAimDownInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("화살 조준");
        }
    }

    private void OnBowShotInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("화살 발사");
        }
    }

    /// <summary>
    /// 움직일 때 실행하는 함수
    /// </summary>
    void OnMove()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnspeed);     // 회전

        currentMoveVector = Vector3.SmoothDamp(currentMoveVector, inputDirection, ref smoothInputVelocity, smoothInputSpeed);   // 움직임 보정
        controller.Move(Time.fixedDeltaTime * currentMoveVector * moveSpeed);   // 플레이어 움직임

        animator.SetFloat(SpeedToHash, animMoveSpeed);  // 애니메이션 파라미터 변경
    }    

    void DisableIsAttack()
    {
        isAttack = false;
    }
}