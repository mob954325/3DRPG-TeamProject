using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CameraOffset : MonoBehaviour
{
    Test_Player player;

    public Vector3 offset = new Vector3(0, 1.75f, 0);
    void Awake()
    {
        player = FindAnyObjectByType<Test_Player>();
    }
    void Update()
    {
        transform.localPosition = player.transform.position + offset;
    }
}
