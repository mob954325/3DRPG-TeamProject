using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<int, QuestData> questList = new Dictionary<int, QuestData>();

    private QuestMessage questMessage;

    private List<QuestInfoPanel> questInfoPanels = new List<QuestInfoPanel>();

    public GameObject questInfoPanelPrefab; // QuestInfoPanel 프리팹
    public Transform questInfoPanelParent;  // QuestInfoPanel이 생성될 부모 Transform


    private void Awake()
    {
        questMessage = FindObjectOfType<QuestMessage>();
        GenerateData();
    }

    private void GenerateData()
    {
        questList.Add(0, new QuestData(QuestData.QuestType.None, "퀘스트 이름", "퀘스트 내용", "퀘스트 목표"));
        questList.Add(10, new QuestData(QuestData.QuestType.Hunt, "퀘스트 사냥", "퀘스트 내용 사냥", "퀘스트 목표 10마리"));
        questList.Add(20, new QuestData(QuestData.QuestType.GiveItem, "퀘스트 아이템 기부", "퀘스트 내용 아이템 기부", "퀘스트 목표 10개"));
        questList.Add(30, new QuestData(QuestData.QuestType.ClearDungeon, "퀘스트 던전 클리어", "퀘스트 내용 던전 클리어", "퀘스트 목표 던전"));
    }

    /// <summary>
    /// Quest관련 대화가 진행되었을때 실행되는 함수
    /// </summary>
    /// <param name="id"></param>
    /// <param name="complete"></param>
    public void GetQuestTalkIndex(int id, bool complete)
    {
        if (questList.ContainsKey(id))
        {
            QuestData questData = questList[id];
            questMessage.OnQuestMessage(questData.questName, complete);

            
            if (!complete)
            {

                // 퀘스트 시작일 때
                // 해당 퀘스트에 대한 QuestInfoPanel이 이미 생성되었는지 확인
                QuestInfoPanel existingPanel = questInfoPanels.Find(panel => panel.questId == id);
                if (existingPanel == null)
                {
                    // QuestInfoPanel 동적 생성 및 초기화
                    QuestInfoPanel newQuestInfoPanel = CreateQuestInfoPanel();
                    newQuestInfoPanel.Initialize(questData.questType , id, questData.questName, questData.questContents, questData.questObjectives);

                    // 생성된 QuestInfoPanel을 리스트에 추가
                    questInfoPanels.Add(newQuestInfoPanel);
                }
            }
            else
            {
                // 퀘스트 완료일 때
                // id에 해당하는 QuestInfoPanel 찾아서 제거
                QuestInfoPanel panelToRemove = questInfoPanels.Find(panel => panel.questId == id);
                if (panelToRemove != null)
                {
                    DestroyQuestInfoPanel(panelToRemove);
                }
            }

        }
    }
    private QuestInfoPanel CreateQuestInfoPanel()
    {
        // QuestInfoPanel 프리팹을 Instantiate하여 생성
        GameObject newPanelObject = Instantiate(questInfoPanelPrefab, questInfoPanelParent);
        QuestInfoPanel newQuestInfoPanel = newPanelObject.GetComponent<QuestInfoPanel>();

        return newQuestInfoPanel;
    }

    private void DestroyQuestInfoPanel(QuestInfoPanel panel)
    {
        // 리스트에서 제거하고 GameObject를 파괴
        questInfoPanels.Remove(panel);
        Destroy(panel.gameObject);
    }
}
