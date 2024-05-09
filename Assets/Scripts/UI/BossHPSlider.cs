using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPSlider : MonoBehaviour
{
    public Slider hpSlider; // Inspector에서 연결할 슬라이더
    public Boss boss;       // Inspector에서 연결할 Boss 오브젝트

    void Start()
    {
        hpSlider = GetComponent<Slider>();
        boss = FindObjectOfType<Boss>();

        // 게임 시작 시 슬라이더 최대값을 설정
        hpSlider.maxValue = boss.MaxHP;
        hpSlider.value = boss.HP;
    }

    void Update()
    {
        // 매 프레임마다 Boss의 HP를 슬라이더에 반영
        hpSlider.value = boss.HP;
    }
}
