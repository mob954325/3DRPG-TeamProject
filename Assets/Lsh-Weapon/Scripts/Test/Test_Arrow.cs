using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Arrow : TestInput
{
    public GameObject ArrowPrefab;
    public Vector3 spawnPoint = Vector3.zero;
    public Quaternion rotation = Quaternion.Euler(90f, 0, 0);

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        spawnPoint = transform.position;

        Instantiate(ArrowPrefab, spawnPoint, rotation, gameObject.transform);
    }
}
