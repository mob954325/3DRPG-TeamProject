using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_EquipWeapons : TestBase
{
    Transform target;
    ItemDataManager manager;

    private void Start()
    {
        target = transform.GetChild(0);
        manager = GameManager.Instance.ItemDataManager;
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.GetItemObject(manager.datas[(uint)ItemCode.Bow], 1, target.position);
        Factory.Instance.GetItemObject(manager.datas[(uint)ItemCode.Sword], 1, target.position);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Factory.Instance.GetItemObject(manager.datas[(uint)ItemCode.Arrow], 10, target.position);
    }
}