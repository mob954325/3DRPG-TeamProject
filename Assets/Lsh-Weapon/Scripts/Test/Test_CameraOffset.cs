using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CameraOffset : MonoBehaviour
{
    Test_Player player;

    public Vector3 offset = Vector3.zero;
    void Awake()
    {
        player = FindAnyObjectByType<Test_Player>();
    }
    void Update()
    {
        transform.localPosition = player.transform.position + offset;
    }
}
