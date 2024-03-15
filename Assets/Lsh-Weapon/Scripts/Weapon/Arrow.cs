using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rigid;

    public float power = 5f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        //rigid.AddForce(Vector3.forward * power, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;  
        gameObject.transform.parent = other.transform;

        // 적 데미지 스크립트

        // destory(gameObjet, 3f); // 3초뒤 파괴
    }
}
