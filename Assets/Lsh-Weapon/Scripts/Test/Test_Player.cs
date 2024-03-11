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

// Player-Weapon 테스트 인풋용 스크립트
public class Test_Player : MonoBehaviour
{
    MoveState moveState;    // 움직임 상태 Enum

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
    readonly int SheathingToHash = Animator.StringToHash("Sheathing");      // 무기 집어넣기 여부 애니메이션

    // delegate
    public Action OnAttacAction;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        PlayerMoveState = MoveState.Idle;

        OnAttacAction += DisableIsAttack;
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Nomal.Move.performed += OnMoveInput;
        inputActions.Nomal.Move.canceled += OnMoveInput;
        inputActions.Nomal.Attack.performed += OnAttackInput;
        inputActions.Nomal.Attack.canceled += OnAttackInput;
        inputActions.Nomal.Sprint.performed += OnSpritInput;
        inputActions.Nomal.Sprint.canceled += OnSpritInput;
    }

    void OnDisable()
    {
        inputActions.Nomal.Sprint.canceled -= OnSpritInput;
        inputActions.Nomal.Sprint.performed -= OnSpritInput;
        inputActions.Nomal.Attack.canceled -= OnAttackInput;
        inputActions.Nomal.Attack.performed -= OnAttackInput;
        inputActions.Nomal.Move.canceled -= OnMoveInput;
        inputActions.Nomal.Move.performed -= OnMoveInput;
        inputActions.Disable();      
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttack)
        {
            isAttack = true;            // 공격 확인 
            animator.SetTrigger(AttackToHash);
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


    void Update()
    {
        if(!isAttack)
            OnMove();
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