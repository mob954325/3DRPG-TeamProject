using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 무기 모드
    /// </summary>
    enum WeaponMode
    {
        None = 0,   // 무기 없음
        Sword,      // 칼
        Bow         // 활
    }

    /// <summary>
    /// 현재 무기 모드
    /// </summary>
    WeaponMode currentWeaponMode = WeaponMode.None;

    /// <summary>
    /// 캐릭터의 오른손
    /// </summary>
    //Transform rightHand;

    /// <summary>
    /// 캐릭터의 오른손
    /// </summary>
    //Transform leftHand;

    /// <summary>
    /// [무기1] 칼
    /// </summary>
    Transform swordWeapon;

    /// <summary>
    /// [무기2-1] 활
    /// </summary>
    Transform bowWeapon;

    /// <summary>
    /// [무기2-2] 화살
    /// </summary>
    //Transform arrowWeapon;

    /// <summary>
    /// 캐릭터가 활을 장비했는지 알기 위한 변수
    /// </summary>
    public bool IsBowEquip = false;

    /// <summary>
    /// 캐릭터의 활에 화살이 장전되었는지 알기 위한 변수
    /// </summary>
    public bool IsArrowEquip = false;

    /// <summary>
    /// 카메라 줌 설정 변수
    /// </summary>
    public bool IsZoomIn = false;

    // 애니메이터용 해시값
    readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    readonly int IsSwordHash = Animator.StringToHash("IsSword");
    readonly int IsBowHash = Animator.StringToHash("IsBow");
    //readonly int CriticalHitHash = Animator.StringToHash("CriticalHit");
    readonly int UseWeaponHash = Animator.StringToHash("UseWeapon");
    readonly int HaveArrowHash = Animator.StringToHash("HaveArrow");
    readonly int ZoomInHash = Animator.StringToHash("ZoomIn");

    // 컴포넌트들
    PlayerinputActions inputActions;
    Animator animator;
    Player player;
    Sword sword;
    Bow bow;
    Arrow arrow;
    PlayerFollowVCam vcam;

    PlayerController playerContoller;

    private void Awake()
    {
        inputActions = new PlayerinputActions();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        sword = GetComponentInChildren<Sword>();
        bow = GetComponentInChildren<Bow>();
        arrow = GetComponentInChildren<Arrow>();
        vcam = FindAnyObjectByType<PlayerFollowVCam>();

        playerContoller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        //rightHand = GameObject.Find("Character1_RightHand").transform;
        //leftHand = GameObject.Find("Character1_LeftHand").transform;
        swordWeapon = GameObject.FindWithTag("Sword").transform;
        bowWeapon = GameObject.FindWithTag("Bow").transform;
        //arrowWeapon = GameObject.FindWithTag("Arrow").transform;
        ShowWeapon(false, false);
        //arrowWeapon.gameObject.SetActive(false);
        //arrow.CloseArrow();
    }

    private void OnEnable()
    {
        inputActions.Weapon.Enable();
        inputActions.Player.Attack.performed += OnAttackInput;
        inputActions.Weapon.Attack.performed += OnAttackInput;
        inputActions.Weapon.Change.performed += OnChangeInput;

        inputActions.Weapon.Load.performed += OnLoadInput;
    }

    private void OnDisable()
    {
        inputActions.Weapon.Load.performed -= OnLoadInput;

        inputActions.Weapon.Change.performed -= OnChangeInput;
        inputActions.Weapon.Attack.performed -= OnAttackInput;
        inputActions.Player.Attack.performed -= OnAttackInput;
        inputActions.Weapon.Disable();
    }

    /// <summary>
    /// 무기 모드에 따른 공격 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnAttackInput(InputAction.CallbackContext context)
    {
        animator.SetTrigger(IsAttackHash);

        if (currentWeaponMode == WeaponMode.None)
        {
            animator.SetBool(UseWeaponHash, false);
            IsBowEquip = false;

            // 공격할 동안 Player의 이동이 불가하도록 설정
            StopAllCoroutines();
            //StartCoroutine(player.StopInput());
            StartCoroutine(playerContoller.StopInput());
        }
        else // 무기 모드가 Sword 또는 Bow인 경우
        {
            animator.SetBool(UseWeaponHash, true);

            if (currentWeaponMode == WeaponMode.Sword)
            {
                animator.SetTrigger(IsSwordHash);
                IsBowEquip = false;

                // 공격할 동안 Player의 이동이 불가하도록 설정
                StopAllCoroutines();
                //StartCoroutine(player.StopInput());
                StartCoroutine(playerContoller.StopInput());

                ////////// CriticalHit 설정하기
            }
            else if (currentWeaponMode == WeaponMode.Bow)
            {
                animator.SetTrigger(IsBowHash);

                if (IsArrowEquip == false)
                {
                    animator.SetBool(HaveArrowHash, false);

                    // 공격할 동안 Player의 이동이 불가하도록 설정
                    StopAllCoroutines();
                    //StartCoroutine(player.StopInput());
                    StartCoroutine(playerContoller.StopInput());
                }
                //else
                //{
                //    LoadArrowAfter(); // 화살이 장전된 후 작업
                //}
                
                IsBowEquip = true;
            }
        }
    }

    /// <summary>
    /// 무기 모드를 바꾸는 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnChangeInput(InputAction.CallbackContext context)
    {
        if (currentWeaponMode == WeaponMode.None)
        {
            // 무기를 들고 있지 않는 경우 => 칼을 들도록 한다.
            currentWeaponMode = WeaponMode.Sword;
            Debug.Log("WeaponMode_Change : None >> Sword");
        }
        else if (currentWeaponMode == WeaponMode.Sword)
        {
            // 칼을 무기로 사용하고 있던 경우 => 활을 들도록 한다.
            currentWeaponMode = WeaponMode.Bow;
            Debug.Log("WeaponMode_Change : Sword >> Bow");
        }
        else if (currentWeaponMode == WeaponMode.Bow)
        {
            // 활을 무기로 사용하고 있던 경우 => 무기를 넣도록 한다.
            currentWeaponMode = WeaponMode.None;
            Debug.Log("WeaponMode_Change : Bow >> None");
        }

        ChangeWeapon(currentWeaponMode);
    }

    /// <summary>
    /// 무기 모드에 따라 보여줄 무기이 변경되는 함수
    /// </summary>
    /// <param name="mode"></param>
    private void ChangeWeapon(WeaponMode mode)
    {
        switch (mode)
        {
            case WeaponMode.None:
                ShowWeapon(false, false);
                break;
            case WeaponMode.Sword:
                ShowWeapon(true, false);
                break;
            case WeaponMode.Bow:
                ShowWeapon(false, true);
                break;
        }
    }

    /// <summary>
    /// 무기를 보여줄지 말지 결정하는 함수
    /// </summary>
    /// <param name="isShow">true면 보여주고, false면 안보여준다.</param>
    public void ShowWeapon(bool isSwordShow = false, bool isBowShow = false)
    {
        swordWeapon.gameObject.SetActive(isSwordShow);
        bowWeapon.gameObject.SetActive(isBowShow);
    }

    /// <summary>
    /// 현재 무기 모드 확인용 함수
    /// </summary>
    /// <returns></returns>
    public int CheckWeaponMode()
    {
        int weaponNum = -1;
        if ((int)currentWeaponMode == 0)
        {
            weaponNum = 0;
            return weaponNum;
        }

        if ((int)currentWeaponMode == 1)
        {
            weaponNum = 1;
            return weaponNum;
        }

        if ((int)currentWeaponMode == 2)
        {
            weaponNum = 2;
            return weaponNum;
        }

        return weaponNum;
    }

    /// <summary>
    /// 화살 장전 함수
    /// </summary>
    private void OnLoadInput(InputAction.CallbackContext _)
    {
        Debug.Log($"IsArrowEquip : {IsArrowEquip}");
        if (IsArrowEquip == false)  // 장전된 화살이 없는 경우
        {
            animator.SetBool(HaveArrowHash, true); // 화살 장전
        }

        IsArrowEquip = true; // 화살이 장전됐다고 변수 설정
        Debug.Log($"IsArrowEquip : {IsArrowEquip}");
    }

    /// <summary>
    /// 화살이 장전된 후 화살 관련 변수 설정을 위한 함수
    /// </summary>
    public void LoadArrowAfter()
    {
        if (IsArrowEquip == true) // 화살이 장전된 상태인 경우
        {
            animator.SetBool(ZoomInHash, IsZoomIn); // 카메라 줌 설정
            Debug.Log($"IsZoomIn : {IsZoomIn}");

            if (IsZoomIn == false) // 카메라 줌아웃인 경우 ( = 화살을 쐈다.)
            {
                // 장전되었던 화살 사용 표시
                animator.SetBool(HaveArrowHash, false);
                IsArrowEquip = false;
            }
        }
    }

    /// <summary>
    /// 무기 [검]의 Collider를 켜는 함수 (Animation 설정용)
    /// </summary>
    public void SwordWeaponColliderEnable()
    {
        sword.SwordColliderEnable();
    }

    /// <summary>
    /// 무기 [검]의 Collider를 끄는 함수 (Animation 설정용)
    /// </summary>
    public void SwordWeaponColliderDisable()
    {
        sword.SwordColliderDisable();
    }

    /// <summary>
    /// 무기 [활]의 Collider를 켜는 함수 (Animation 설정용)
    /// </summary>
    public void BowWeaponColliderEnable()
    {
        bow.BowColliderEnable();
    }

    /// <summary>
    /// 무기 [활]의 Collider를 끄는 함수 (Animation 설정용)
    /// </summary>
    public void BowWeaponColliderDisable()
    {
        bow.BowColliderDisable();
    }

    /// <summary>
    /// 무기 [화살]의 Collider를 켜는 함수 (Animation 설정용)
    /// </summary>
    public void ArrowWeaponColliderEnable()
    {
        arrow.ArrowColliderEnable();
    }

    /// <summary>
    /// 무기 [화살]의 Collider를 끄는 함수 (Animation 설정용)
    /// </summary>
    public void ArrowWeaponColliderDisable()
    {
        arrow.ArrowColliderDisable();
    }

    ///// <summary>
    ///// 무기 [검]을 꺼내는 함수 (Animation 설정용)
    ///// </summary>
    //public void SwordWeaponOpen()
    //{
    //    sword.OpenSwordWeapon();
    //}

    ///// <summary>
    ///// 무기 [검]을 넣는 함수 (Animation 설정용)
    ///// </summary>
    //public void SwordWeaponClose()
    //{
    //    sword.CloseSwordWeapon();
    //}

    ///// <summary>
    ///// 무기 [활]을 꺼내는 함수 (Animation 설정용)
    ///// </summary>
    //public void BowWeaponOpen()
    //{
    //    bow.OpenBowWeapon();
    //}

    ///// <summary>
    ///// 무기 [활]을 넣는 함수 (Animation 설정용)
    ///// </summary>
    //public void BowWeaponClose()
    //{
    //    bow.CloseBowWeapon();
    //}

    ///// <summary>
    ///// 무기 [화살]을 꺼내는 함수 (Animation 설정용)
    ///// </summary>
    //public void ArrowWeaponOpen()
    //{
    //    arrow.OpenArrow();
    //}

    ///// <summary>
    ///// 무기 [화살]을 넣는 함수 (Animation 설정용)
    ///// </summary>
    //public void ArrowWeaponClose()
    //{
    //    arrow.CloseArrow();
    //}
}
