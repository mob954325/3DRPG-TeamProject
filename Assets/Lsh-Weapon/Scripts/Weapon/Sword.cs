using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    Collider weaponCollider;

    void Start()
    {
        weaponCollider = GetComponent<Collider>();
        //weaponCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("적에게 공격이 닿았습니다");
            // 적 체력관련 스크립트
        }
    }

    /// <summary>
    /// 무기 콜라이더 활성화
    /// </summary>
    public void MeleeWeaponColliderEnable()
    {
        weaponCollider.enabled = true;
    }

    /// <summary>
    /// 무기 콜라이더 비활성화
    /// </summary>
    public void MeleeWeaponColliderDisable()
    {
        weaponCollider.enabled = false;
    }
}
