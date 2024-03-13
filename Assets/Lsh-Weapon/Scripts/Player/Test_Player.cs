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
            switch (weaponState)
            {
                case WeaponState.Sword:
                    inputActions.Sword.Enable();
                    inputActions.Bow.Disable();
                    break;
                case WeaponState.Bow:
                    inputActions.Sword.Disable();
                    inputActions.Bow.Enable();
                    break;
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
    Sword sword;

    // Values
    Vector3 inputDirection = Vector3.zero;                          // 입력 받는 방향 벡터
    public float moveSpeed = 3f;                                    // 실제 플레이어 이동속도
    Vector3 smoothInputVelocity = Vector3.zero;                     // SmoothDamp용 현재 Velocity값
    [Tooltip("플레이어가 최고 속도에 도달할 속도 값( 값이 낮을 수록 더 빨리 도달함 , Default : 0.2)")]
    [SerializeField] float smoothInputSpeed = .2f;                  // SmoothDamp가 도달할 값 ( 값이 작을 수록 더 빨리 도달함 )
    float animMoveSpeed = 0f;                                       // 애니메이션 파라미터 전달용 함수

    //rotate
    Vector3 lookVector = Vector3.zero;                              // 마우스 인풋값
    public GameObject followCam;                                    // Cinemachine이 바라보는 오브젝트
    Quaternion camY;                                                // 메인 카메라 Y값
    public float rotatePower = 5f;                                  // 회전값

    [SerializeField] Vector3 currentMoveVector = Vector3.zero;
    Quaternion targetRotation = Quaternion.identity;                // 회전할 목표 회전값
    //public float turnspeed = 10.0f;                                 // 회전 속도

    [SerializeField]bool isAttack = false;                          // 공격했는지 확인
   
    // Hashes
    readonly int SpeedToHash = Animator.StringToHash("Speed");              // 이동용 파라미터
    readonly int AttackToHash = Animator.StringToHash("Attack");            // 공격용 파라미터
    readonly int IsEquipToHash = Animator.StringToHash("IsEquip");          // 무기장비착용 여부 
    readonly int IsWeaponBowToHash = Animator.StringToHash("IsWeaponBow");  // 현재 무기가 활인지 체크 ( true : 활 )

    // delegate
    public Action OnAttackEnd;  // 플레이어 공격이 종료되면 실행되는 델리게이트

    void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();

        PlayerMoveState = MoveState.Idle;
        CurrentWeaponState = WeaponState.Sword;

        OnAttackEnd += DisableIsAttack;
    }

    void OnEnable()
    {
        inputActions.Main.Enable();
        inputActions.Main.Move.performed += OnMoveInput;
        inputActions.Main.Move.canceled += OnMoveInput;
        inputActions.Main.Sprint.performed += OnSpritInput;
        inputActions.Main.Sprint.canceled += OnSpritInput;

        inputActions.Main.Look.performed += OnLookInput;
        inputActions.Main.Look.canceled += OnLookInput;

        inputActions.Main.SwitchWeapon.performed += OnWeaponSwitchInput;

        inputActions.Sword.Attack.performed += OnAttackInput;
        inputActions.Sword.Attack.canceled += OnAttackInput;

        inputActions.Bow.Shot.performed += OnBowShotInput;
        inputActions.Bow.AimDown.performed += OnAimDownInput;
    }

    void OnDisable()
    {
        inputActions.Bow.AimDown.performed -= OnAimDownInput;
        inputActions.Bow.Shot.performed -= OnBowShotInput;

        inputActions.Sword.Attack.canceled -= OnAttackInput;
        inputActions.Sword.Attack.performed -= OnAttackInput;

        inputActions.Main.Look.canceled -= OnLookInput;
        inputActions.Main.Look.performed -= OnLookInput;

        inputActions.Main.Sprint.canceled -= OnSpritInput;
        inputActions.Main.Sprint.performed -= OnSpritInput;
        inputActions.Main.Move.canceled -= OnMoveInput;
        inputActions.Main.Move.performed -= OnMoveInput;

        inputActions.Main.Disable();      
        inputActions.Sword.Disable();      
        inputActions.Bow.Disable();      
    }

    void Update()
    {
        LookRotation();

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
            camY = Quaternion.Euler(0, Camera.main.transform.localEulerAngles.y, 0);         // 카메라 Y값
            inputDirection = camY * inputDirection;                                          // 카메라 기준 회전값
            targetRotation = Quaternion.LookRotation(inputDirection * Time.deltaTime);       // 회전할 방향값
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

    public bool isLook = false;
    private void OnLookInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isLook = true;
            lookVector = context.ReadValue<Vector2>();
        }
        if(!context.performed)
        {
            isLook = false;
        }
    }

    private void OnWeaponSwitchInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (weaponState)
            {
                case WeaponState.Sword:
                    CurrentWeaponState = WeaponState.Bow;
                    animator.SetBool(IsWeaponBowToHash, true);
                    break;
                case WeaponState.Bow:
                    CurrentWeaponState = WeaponState.Sword;
                    animator.SetBool(IsWeaponBowToHash, false);
                    break;
            }
        }
    }

    private void OnAimDownInput(InputAction.CallbackContext context)
    {
        if (context.performed && !animator.GetBool("AimDown"))
        {
            animator.SetTrigger("RangeAttack");
            animator.SetBool("AimDown", true);
            Debug.Log("화살 조준");
        }
    }

    private void OnBowShotInput(InputAction.CallbackContext context)
    {
        if (context.performed && animator.GetBool("AimDown"))
        {
            animator.SetBool("AimDown", false);
            Debug.Log("화살 발사");
        }
    }

    /// <summary>
    /// 움직일 때 실행하는 함수
    /// </summary>
    void OnMove()
    {
        // turn
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);     // 회전

        currentMoveVector = Vector3.SmoothDamp(currentMoveVector, inputDirection, ref smoothInputVelocity, smoothInputSpeed);   // 움직임 보정\

        controller.Move(Time.fixedDeltaTime * currentMoveVector * moveSpeed);   // 플레이어 움직임

        animator.SetFloat(SpeedToHash, animMoveSpeed);  // 애니메이션 파라미터 변경
    }

    void LookRotation()
    {
        if (!isLook)
            return;

        #region Rotation

        followCam.transform.localRotation *= Quaternion.AngleAxis(lookVector.x * rotatePower, Vector3.up);
        followCam.transform.localRotation *= Quaternion.AngleAxis(-lookVector.y * rotatePower, Vector3.right);

        var angles = followCam.transform.localEulerAngles;
        angles.z = 0;

        var angle = followCam.transform.localEulerAngles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followCam.transform.localEulerAngles = angles;

        //transform.rotation = Quaternion.Euler(0, followCam.transform.rotation.eulerAngles.y + targetRotation, 0);

        followCam.transform.localEulerAngles = new Vector3(angles.x, angles.y, 0);

        #endregion

    }

    void DisableIsAttack()
    {
        isAttack = false;
    }

    void WeaponColliderEnable()
    {
        sword.MeleeWeaponColliderEnable();
    }

    void WeaponColliderDisable()
    {
        sword.MeleeWeaponColliderDisable();
    }
}