using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.InputSystem;

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
    /// 아이템 나누기 패널
    /// </summary>
    InventoryDividUI dividUI;

    /// <summary>
    /// 아이템 정렬 UI
    /// </summary>
    InventorySortUI sortUI;

    /// <summary>
    /// 아이템 골드 UI 패널
    /// </summary>
    InventoryGoldUI goldUI;

    /// <summary>
    /// 오른쪽 클릭하면 나오는 아이템 매뉴 UI
    /// </summary>
    InventorySelectedMenuUI selectedMenuUI;

    CanvasGroup canvasGroup;

    public Action<uint> onSlotDragBegin;
    public Action<GameObject> onSlotDragEnd;
    public Action onSlotDragEndFail;
    public Action<uint> onShowDetail;
    public Action onCloseDetail;
    public Action<uint> onLeftClickItem;
    public Action<uint, Vector2> onRightClickItem;

    /// <summary>
    /// 인벤토리 UI를 초기화하는 함수
    /// </summary>
    /// <param name="playerInventory">플레이어 인벤토리</param>
    public void InitializeInventoryUI(Inventory playerInventory)
    {
        inventory = playerInventory;    // 초기화한 인벤토리 내용 받기
        slotsUIs = new InventorySlotUI[Inventory.SlotSize]; // 슬롯 크기 할당
        slotsUIs = GetComponentsInChildren<InventorySlotUI>();  // 일반 슬롯
        tempSlotUI = GetComponentInChildren<TempSlotUI>(); // 임시 슬롯
        detailUI = GetComponentInChildren<InventoryDetailUI>(); // 아이템 정보 패널
        dividUI = GetComponentInChildren<InventoryDividUI>(); // 아이템 나누기 패널
        sortUI = GetComponentInChildren<InventorySortUI>(); // 아이템 정렬 UI
        goldUI = GetComponentInChildren<InventoryGoldUI>(); // 아이템 골드 UI
        selectedMenuUI = GetComponentInChildren<InventorySelectedMenuUI>(); // 아이템 슬롯 매뉴 UI
        canvasGroup = GetComponent<CanvasGroup>();

        for (uint i = 0; i < Inventory.SlotSize; i++)
        {
            slotsUIs[i].InitializeSlotUI(Inventory[i]); // 인벤토리슬롯을 slotUI와 연결
        }
        tempSlotUI.InitializeSlotUI(Inventory.TempSlot); // null 참조

        onSlotDragBegin += OnSlotDragBegin;
        onSlotDragEnd += OnSlotDragEnd;
        onShowDetail += OnShowDetail;
        onCloseDetail += OnCloseDetail;
        onSlotDragEndFail += OnSlotDragFail;
        onLeftClickItem += OnLeftClickItem;
        onRightClickItem += OnRightClickItem;
        dividUI.onDivid += DividItem;
        sortUI.onSortItem += OnSortItem;

        Inventory.onInventoryGoldChange += goldUI.onGoldChange; // Iventory의 골드량이 수정될 때 goldUI도 수정되게 함수 추가

        goldUI.onGoldChange?.Invoke(Inventory.Gold);            // 골드 초기화
    }

    /// <summary>
    /// 아이템을 정렬하는 함수
    /// </summary>
    /// <param name="sortMode">아이템 정렬 모드</param>
    /// <param name="isAcending">true면 오름차순, false면 내림차순</param>
    private void OnSortItem(uint sortMode, bool isAcending)
    {
        // 아이템이 연속적으로 없으면 아이템을 땡기고 정렬하기

        Inventory.SortSlot((SortMode)sortMode, isAcending);        
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
            bool targetIsEquip = Inventory[index].IsEquip;

            tempSlotUI.OpenTempSlot();

            Inventory.AccessTempSlot(targetSlotIndex, targetSlotItemCode, targetItemSlotCount);
            Inventory.TempSlot.IsEquip = targetIsEquip;
            inventory[index].ClearItem();
        }
    }

    /// <summary>
    /// 슬롯 드래그 종료
    /// </summary>
    /// <param name="index">아이템을 넣을 인벤토리 슬롯 인덱스</param>
    private void OnSlotDragEnd(GameObject slotObj)
    {
        if(slotObj == null) // 드래그 종료 시점에 감지되는 슬롯이 없다.
        {
            OnSlotDragFail();
            Debug.Log("존재하지 않는 오브젝트입니다");
            return;
        }
        else
        {
            SlotUI_Base slotUI = slotObj.GetComponent<SlotUI_Base>();
            bool isSlot = slotUI is SlotUI_Base;

            if (slotUI == null)
            {
                OnSlotDragFail();
                Debug.Log("아이템 슬롯 베이스 스크립트가 존재하지 않는 오브젝트 입니다.");
                return;
            }

            if(!isSlot) // 드래그 끝나는 지점이 슬롯이 아니다.
            {
                OnSlotDragFail();
                Debug.Log("슬롯이 아닙니다");
                return;
            }

            // 슬롯 인덱스
            uint index = slotUI.InventorySlotData.SlotIndex;
            uint tempFromIndex = Inventory.TempSlot.FromIndex;

            // 임시 슬롯에 들어있는 내용
            uint tempSlotItemCode = (uint)Inventory.TempSlot.SlotItemData.itemCode;
            int tempSlotItemCount = Inventory.TempSlot.CurrentItemCount;
            bool tempSlotIsEqiup = Inventory.TempSlot.IsEquip;

            if (Inventory[index].SlotItemData != null)   // 아이템이 들어있다.
            {
                if (Inventory[index].SlotItemData.itemCode == Inventory.TempSlot.SlotItemData.itemCode) // 교환하려는 아이템이 같으면
                {
                    Inventory[index].AssignItem(tempSlotItemCode, tempSlotItemCount, out int overCount);

                    if (overCount > 0) // 슬롯에 넣었는데 넘쳤으면
                    {
                        OnSlotDragFail();
                    }

                    Inventory.TempSlot.ClearItem();
                }
                else // 아이템이 들어있고 목표 슬롯이 존재한다.
                {
                    uint targetSlotItemCode = (uint)Inventory[index].SlotItemData.itemCode;
                    int targetSlotItemCount = Inventory[index].CurrentItemCount;
                    bool targetSlotIsEquip = Inventory[index].IsEquip;

                    inventory[index].ClearItem();
                    Inventory.AccessTempSlot(index, tempSlotItemCode, tempSlotItemCount); // target 슬롯에 아이템 저장
                    Inventory[index].IsEquip = tempSlotIsEqiup;

                    // 장비 위치 바꾸기
                    IEquipTarget equipTarget = Inventory.Owner.GetComponent<IEquipTarget>();    // 인벤토리를 가진 오브젝트
                    ItemData_Equipment itemData = Inventory[index].SlotItemData as ItemData_Equipment; // 선택한 인벤토리의 아이템 데이터
                    if (itemData != null && Inventory[index].IsEquip)    // 아이템이 장비이다
                    {
                        equipTarget.EquipPart[(int)itemData.equipPart] = Inventory[index];  // 장비 아이템 정보 변경 
                    }

                    Inventory.AccessTempSlot(index, targetSlotItemCode, targetSlotItemCount); // target 슬롯에 있었던 아이템 내용 임시 슬롯에 저장
                    Inventory.TempSlot.IsEquip = targetSlotIsEquip;

                    tempSlotItemCode = (uint)Inventory.TempSlot.SlotItemData.itemCode;
                    tempSlotItemCount = Inventory.TempSlot.CurrentItemCount;
                    tempSlotIsEqiup = Inventory.TempSlot.IsEquip;

                    Inventory[tempFromIndex].IsEquip = tempSlotIsEqiup;
                    Inventory.AccessTempSlot(tempFromIndex, tempSlotItemCode, tempSlotItemCount);
                }
            }
            else // 아이템이 들어있지 않으면
            {
                Inventory[tempFromIndex].IsEquip = Inventory[index].IsEquip; // 이전 칸에 장착여부는 target의 장착 여부로 변경

                Inventory[index].IsEquip = tempSlotIsEqiup;
                Inventory.AccessTempSlot(index, tempSlotItemCode, tempSlotItemCount);

                IEquipTarget equipTarget = Inventory.Owner.GetComponent<IEquipTarget>();    // 인벤토리를 가진 오브젝트
                ItemData_Equipment itemData = Inventory[index].SlotItemData as ItemData_Equipment; // 선택한 인벤토리의 아이템 데이터
                if(itemData != null && Inventory[index].IsEquip)    // 아이템이 장비이고 장착중이다
                {
                    equipTarget.EquipPart[(int)itemData.equipPart] = Inventory[index];  // 장비 아이템 정보 변경
                }
            }
            tempSlotUI.CloseTempSlot();
        }
    }

    /// <summary>
    /// 아이템 드래그를 성공적으로 실행하지 못했을 때 실행하는 함수 ( 다시 원래 슬롯으로 되돌린다. )
    /// </summary>
    private void OnSlotDragFail()
    {
        if (Inventory.TempSlot.SlotItemData == null)
            return;

        uint fromIndex = Inventory.TempSlot.FromIndex;
        uint tempSlotItemCode = (uint)Inventory.TempSlot.SlotItemData.itemCode;
        int tempSlotItemCount = Inventory.TempSlot.CurrentItemCount;
        bool tempSlotIsEquip = Inventory.TempSlot.IsEquip;

        Inventory.AccessTempSlot(fromIndex, tempSlotItemCode, tempSlotItemCount);
        Inventory[fromIndex].IsEquip = tempSlotIsEquip;
        
        tempSlotUI.CloseTempSlot();
    }

    /// <summary>
    /// 아이템 상세정보 패널을 보여주는 함수
    /// </summary>
    /// <param name="index">보여줄려는 아이템 슬롯 인덱스</param>
    private void OnShowDetail(uint index)
    {
        if (Inventory[index].SlotItemData != null)
        {
            string name = Inventory[index].SlotItemData.itemName;
            string desc = Inventory[index].SlotItemData.desc;
            uint price = Inventory[index].SlotItemData.price;

            detailUI.SetDetailText(name, desc, price);
            detailUI.ShowItemDetail();
        }
    }

    /// <summary>
    /// 아이템 왼쪽 클릭할 때 실행하는 함수
    /// </summary>
    /// <param name="index">클릭한 슬롯 인덱스</param>
    private void OnLeftClickItem(uint index)
    {
        bool isEquip = Inventory[index].SlotItemData is IEquipable; // 장비 아이템이면 true 아니면 false
        if (isEquip)    // 클릭한 슬롯 아이템이 장비이면
        {
            EquipItem(index);
        }
        bool isConsumalbe = Inventory[index].SlotItemData is IConsumable; // 회복 아이템이면 true 아니면 false
        if(isConsumalbe)
        {
            ConsumItem(index);
        }
    }

    /// <summary>
    /// 아이템 오른쪽 클릭할 때 실행하는 함수 (Slot Menu)
    /// </summary>
    /// <param name="index">클릭한 슬롯 인덱스</param>
    private void OnRightClickItem(uint index, Vector2 position)
    {
        // 버튼 이벤트 부여 index번 슬롯에 대한 내용 
        selectedMenuUI.OnDividButtonClick = () =>
        {
            if (Inventory[index].CurrentItemCount <= 1)
            {
                Debug.Log($"[{Inventory[index].SlotItemData.itemName}]은 아이템이 [{Inventory[index].CurrentItemCount}]개 있습니다.");
                return;
            }
            dividUI.InitializeValue(Inventory[index], 1, (int)Inventory[index].CurrentItemCount - 1);
            dividUI.DividUIOpen();

            selectedMenuUI.HideMenu();
        };

        selectedMenuUI.OnDropButtonClick = () =>
        {
            DropItem(index);
            selectedMenuUI.HideMenu();
        };

        selectedMenuUI.SetPosition(position);
        selectedMenuUI.ShowMenu(); // 매뉴 보여주기
    }

    /// <summary>
    /// 아이템을 나눌 때 델리게이트에 신호를 보내는 함수
    /// </summary>
    /// <param name="InventorySlot">나눌 아이템 슬롯</param>
    /// <param name="count">나눌 아이템양</param>
    private void DividItem(InventorySlot slot, int count)
    {
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
                if(nextIndex >= Inventory.SlotSize)
                {
                    Debug.LogError($"해당 인벤토리에 공간이 부족합니다.");
                    return;
                }
            }

            Inventory.DividItem(slot.SlotIndex, nextIndex, count);
        }
    }

    /// <summary>
    /// 아이템을 장착할 때 실행하는 함수
    /// </summary>
    /// <param name="index">장착할 아이템의 인덱스</param>
    private void EquipItem(uint index)
    {
        IEquipable equipable = Inventory[index].SlotItemData as IEquipable;

        bool isEquip = Inventory[index].IsEquip;
        if (equipable != null)
        {
            if (isEquip) // 장착이 되어있으면 장착해제
            {
                equipable.UnEquipItem(Inventory.Owner, Inventory[index]);
                Inventory[index].IsEquip = false;
            }
            else if (!isEquip) // 장착이 안되있으면 장착
            {
                IEquipTarget equipTarget = Inventory.Owner.GetComponent<IEquipTarget>();    // 인벤토리를 가진 오브젝트의 IEquipTarget
                ItemData_Equipment itemData = Inventory[index].SlotItemData as ItemData_Equipment;  // 장착하려는 아이템 데이터

                int partInedex = (int)itemData.equipPart;   // 장착할려는 장비 위치 인덱스
                InventorySlot equipedItem = equipTarget.EquipPart[partInedex];

                if (equipedItem != null)  // 장착할 해당 부위에 아이템이 있다
                {
                    for(uint i = 0; i < inventory.SlotSize; i++)    // 모든 슬롯 체크
                    {
                        ItemData_Equipment data = Inventory[i].SlotItemData as ItemData_Equipment;
                        if(data != null) // 해당 장착부위가 있는 장비다
                        {
                            Inventory[i].IsEquip = false;
                        }
                    }
                }

                equipable.EquipItem(Inventory.Owner, Inventory[index]); // 아이템 장착                
                Inventory[index].IsEquip = true;
            }
        }

        //showEquip();
    }

    /// <summary>
    /// 아이템 소비할 때 실행하는 함수
    /// </summary>
    /// <param name="index">소비할 아이템 슬롯 인덱스</param>
    private void ConsumItem(uint index)
    {
        IConsumable consumable = Inventory[index].SlotItemData as IConsumable;

        consumable.Consum(Inventory.Owner, Inventory[index]);
    }

    private void DropItem(uint index)
    {
        if(!Inventory[index].IsEquip)
        {
            Inventory.DropItem(index);
        }            
    }

    /// <summary>
    /// 아이템 상세정보 패널을 닫는 함수
    /// </summary>
    private void OnCloseDetail()
    {
        detailUI.ClearText();
        detailUI.CloseItemDetail();
    }
    // UI 열기
    // UI 닫기


    public void ShowInventory()
    {
        if(canvasGroup.alpha == 1) // 비활성화
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if(canvasGroup.alpha < 1)// 활성화
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// 각 슬롯 장착 여부확인 함수
    /// </summary>
    void showEquip()
    {
        foreach (var items in slotsUIs)
        {
            Debug.Log($"{items.InventorySlotData.SlotIndex} : {items.InventorySlotData.IsEquip}");
        }
    }
#endif
}