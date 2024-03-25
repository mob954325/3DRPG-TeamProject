using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// 인벤토리
    /// </summary>
    Inventory inventory;

    /// <summary>
    /// 인벤토리 접근용 프로퍼티
    /// </summary>
    Inventory Inventory => inventory;

    /// <summary>
    /// UI slots
    /// </summary>
    InventorySlotUI[] slotsUIs;

    /// <summary>
    /// 임시 슬롯 UI
    /// </summary>
    TempSlotUI tempSlotUI;

    /// <summary>
    /// 아이템 정보 UI
    /// </summary>
    InventoryDetailUI detailUI;

    /// <summary>
    /// 나누는 창
    /// </summary>
    DividUI dividUI;

    public Action<uint> onSlotDragBegin;
    public Action<uint> onSlotDragEnd;
    public Action onSlotDragEndFail;
    public Action<uint> onShowDetail;
    public Action onCloseDetail;
    public Action<uint> onDivdItem;

    /// <summary>
    /// 인벤토리 UI를 초기화하는 함수
    /// </summary>
    /// <param name="playerInventory">플레이어 인벤토리</param>
    public void InitializeInventoryUI(Inventory playerInventory)
    {
        inventory = playerInventory;    // 초기화한 인벤토리 내용 받기
        slotsUIs = new InventorySlotUI[Inventory.slotSize]; // 슬롯 크기 할당
        slotsUIs = GetComponentsInChildren<InventorySlotUI>();  // 일반 슬롯
        tempSlotUI = GetComponentInChildren<TempSlotUI>(); // 임시 슬롯
        detailUI = GetComponentInChildren<InventoryDetailUI>(); // 아이템 정보 패널
        dividUI = GetComponentInChildren<DividUI>(); // 아이템 나누기 패널

        for (uint i = 0; i < Inventory.slotSize; i++)
        {
            slotsUIs[i].InitializeSlotUI(Inventory[i]); // 인벤토리슬롯을 slotUI와 연결
        }
        tempSlotUI.InitializeSlotUI(Inventory.TempSlot); // null 참조

        onSlotDragBegin += OnSlotDragBegin;
        onSlotDragEnd += OnSlotDragEnd;
        onShowDetail += OnShowDetail;
        onCloseDetail += OnCloseDetail;
        onSlotDragEndFail += OnSlotDragFail;
        onDivdItem += OnDividItem;
        dividUI.onDivid += DividItem;
    }

    /// <summary>
    /// 슬롯 드래그 시작
    /// </summary>
    /// <param name="index">임시 슬롯에 들어갈 인벤토리 슬롯 인덱스</param>
    private void OnSlotDragBegin(uint index)
    {
        if (Inventory[index].SlotItemData != null)
        {
            uint targetSlotIndex = index;
            uint targetSlotItemCode = (uint)Inventory[index].SlotItemData.itemCode;
            int targetItemSlotCount = Inventory[index].CurrentItemCount;

            tempSlotUI.OpenTempSlot();

            Inventory.AccessTempSlot(targetSlotIndex, targetSlotItemCode, targetItemSlotCount);
            inventory[index].ClearItem();
        }
    }

    /// <summary>
    /// 슬롯 드래그 종료
    /// </summary>
    /// <param name="index">아이템을 넣을 인벤토리 슬롯 인덱스</param>
    private void OnSlotDragEnd(uint index)
    {
        uint tempFromIndex = Inventory.TempSlot.FromIndex;

        // 임시 슬롯에 들어있는 내용
        uint tempSlotItemCode = (uint)Inventory.TempSlot.SlotItemData.itemCode;
        int tempSlotItemCount = Inventory.TempSlot.CurrentItemCount;

        if(Inventory[index].SlotItemData != null)   // 아이템이 들어있다.
        {
            if(Inventory[index].SlotItemData.itemCode == Inventory.TempSlot.SlotItemData.itemCode)
            {
                Inventory[index].AssignItem(tempSlotItemCode, tempSlotItemCount, out int overCount);

                if(overCount > 0) // 슬롯에 넣었는데 넘쳤으면
                {
                    OnSlotDragFail();
                }

                Inventory.TempSlot.ClearItem();
            }
            else
            {
                uint targetSlotItemCode = (uint)Inventory[index].SlotItemData.itemCode;
                int targetSlotItemCount = Inventory[index].CurrentItemCount;

                inventory[index].ClearItem();
                Inventory.AccessTempSlot(index, tempSlotItemCode, tempSlotItemCount); // target 슬롯에 아이템 저장
                Inventory.AccessTempSlot(index, targetSlotItemCode, targetSlotItemCount); // target 슬롯에 있었던 아이템 내용 임시 슬롯에 저장
            
                tempSlotItemCode = (uint)Inventory.TempSlot.SlotItemData.itemCode;
                tempSlotItemCount = Inventory.TempSlot.CurrentItemCount;

                Inventory.AccessTempSlot(tempFromIndex, tempSlotItemCode, tempSlotItemCount);
            }
        }
        else
        {
            Inventory.AccessTempSlot(index, tempSlotItemCode, tempSlotItemCount);
        }

        tempSlotUI.CloseTempSlot();
    }

    private void OnSlotDragFail()
    {
        uint fromIndex = Inventory.TempSlot.FromIndex;
        uint tempSlotItemCode = (uint)Inventory.TempSlot.SlotItemData.itemCode;
        int tempSlotItemCount = Inventory.TempSlot.CurrentItemCount;

        Inventory.AccessTempSlot(fromIndex, tempSlotItemCode, tempSlotItemCount);
        tempSlotUI.CloseTempSlot();
    }

    private void OnShowDetail(uint index)
    {
        if (Inventory[index].SlotItemData != null)
        {
            string name = Inventory[index].SlotItemData.itemName;
            string desc = Inventory[index].SlotItemData.desc;
            uint price = Inventory[index].SlotItemData.price;

            detailUI.SetDetailText(name, desc, price);
            detailUI.OpenTempSlot();
        }
    }

    /// <summary>
    /// 아이템 나눌 때 실행되는 함수
    /// </summary>
    /// <param name="index">나눌 아이템 슬롯 인덱스</param>
    private void OnDividItem(uint index)
    {
        dividUI.InitializeValue(Inventory[index], 1, (int)Inventory[index].CurrentItemCount - 1);
        dividUI.DividUIOpen();
    }

    /// <summary>
    /// 아이템을 나눌 때 델리게이트에 신호를 보내는 함수
    /// </summary>
    /// <param name="InventorySlot">나눌 아이템 슬롯</param>
    /// <param name="count">나눌 아이템양</param>
    private void DividItem(InventorySlot slot, int count)
    {
        uint slotItemCode = (uint)slot.SlotItemData.itemCode;
        uint nextIndex = slot.SlotIndex;

        if(!Inventory.IsVaildSlot(nextIndex)) // 다음 슬롯만 찾아서 없으면 실행 X
        {
            Debug.Log("슬롯이 존재하지 않습니다.");
            return;
        }
        else
        {
            while(Inventory.IsVaildSlot(nextIndex))
            {
                nextIndex++;
                if(nextIndex >= Inventory.slotSize)
                {
                    Debug.LogError($"해당 인벤토리에 공간이 부족합니다.");
                    return;
                }
            }

            Inventory.DividItem(slot.SlotIndex, nextIndex, count);
        }
    }

    private void OnCloseDetail()
    {
        detailUI.ClearText();
        detailUI.CloseTempSlot();
    }
    // UI 열기
    // UI 닫기
}
