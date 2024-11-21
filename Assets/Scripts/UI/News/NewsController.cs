﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewsController : MonoBehaviour
{
    public int GameLevel = 0;

    private bool startNewsTriggered = false;

    private Dictionary<int, bool> wardInfectionNewsTriggered = new Dictionary<int, bool>();
    private Dictionary<int, bool> wardDoctorStressNewsTriggered = new Dictionary<int, bool>();
    private Dictionary<int, bool> wardNurseStressNewsTriggered = new Dictionary<int, bool>();
    private Dictionary<int, bool> wardClosedNewsTriggered = new Dictionary<int, bool>();

    private Dictionary<int, HashSet<int>> wardInfectionLevelsTriggered = new Dictionary<int, HashSet<int>>(); // 감염률 뉴스 단계 트리거

    public NewsTicker moveTextController;

    private bool virusOutbreakNewsTriggered = false;    // 감염병 발생 뉴스

    private void Awake()
    {
        if (moveTextController == null)
        {
            moveTextController = FindObjectOfType<NewsTicker>();
        }
        InitializeNewsTriggers();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckNewsRequirements), 1f, 1f);
    }

    private void InitializeNewsTriggers()
    {
        foreach (Ward ward in Ward.wards)
        {
            wardInfectionNewsTriggered[ward.num] = false;
            wardDoctorStressNewsTriggered[ward.num] = false;
            wardNurseStressNewsTriggered[ward.num] = false;
            wardClosedNewsTriggered[ward.num] = false;
            wardInfectionLevelsTriggered[ward.num] = new HashSet<int>();
        }
    }

    // 바이러스 발생 뉴스
    public void TriggerVirusOutbreakNews()
    {
        if (!virusOutbreakNewsTriggered)
        {
            EnqueueNews("원인불명의 바이러스 병이 발생했습니다.");
            virusOutbreakNewsTriggered = true;
        }
    }

    private void CheckNewsRequirements()
    {
        List<Ward> allWards = Ward.wards;

        CheckFirstInfectionNews(allWards);
        CheckInfectionNews(allWards);
        CheckWardClosedNews(allWards);
    }

    private void CheckFirstInfectionNews(List<Ward> wards)
    {
        if (!startNewsTriggered && InfectionManager.Instance.GetOverallInfectionRate(wards) > 0)
        {
            EnqueueNews("국내 최초 감염자 발생! 각 병원은 감염병을 주의하시기 바랍니다!");
            startNewsTriggered = true;
        }
    }

    private void CheckInfectionNews(List<Ward> wards)
    {
        foreach (Ward ward in wards)
        {
            int infectionRate = Mathf.RoundToInt(InfectionManager.Instance.GetInfectionRate(ward));

<<<<<<< HEAD
            UpdateNewsTrigger(ward.num, wardInfectionNewsTriggered, infectionRate >= threshold,
                $"<color=#FF0000>경고!!</color> {ward.WardName} 내 감염률이 <color=#FF0000>{threshold}%</color>에 도달했습니다!"
            );
=======
            CheckAndTriggerInfectionLevelNews(ward.num, infectionRate, 20, "경고!! {ward.WardName} 내 감염률이 20%에 도달했습니다!");
            CheckAndTriggerInfectionLevelNews(ward.num, infectionRate, 50, "경고!! {ward.WardName} 내 감염률이 50%에 도달했습니다!");
            CheckAndTriggerInfectionLevelNews(ward.num, infectionRate, 80, "경고!! {ward.WardName} 내 감염률이 80%에 도달했습니다!");
>>>>>>> upstream/main
        }
    }

    private void CheckWardClosedNews(List<Ward> wards)
    {
        foreach (Ward ward in wards)
        {
            UpdateNewsTrigger(ward.num, wardClosedNewsTriggered, ward.isClosed,
                $"<color=#FF0000>경고!!</color> {ward.WardName} 병동의 감염률이 <color=#FF0000>50%</color>를 초과하였습니다!");
        }
    }

    private void CheckAndTriggerInfectionLevelNews(int wardNum, int infectionRate, int threshold, string newsMessage)
    {
        if (!wardInfectionLevelsTriggered[wardNum].Contains(threshold) && infectionRate >= threshold)
        {
            EnqueueNews(newsMessage.Replace("{ward.WardName}", Ward.wards.Find(w => w.num == wardNum).WardName));
            wardInfectionLevelsTriggered[wardNum].Add(threshold);
        }
    }

    private void UpdateNewsTrigger(int wardNum, Dictionary<int, bool> triggerDictionary, bool condition, string mainNews)
    {
        if (!triggerDictionary[wardNum] && condition)
        {
            EnqueueNews(mainNews);
            triggerDictionary[wardNum] = true;
        }
        else if (triggerDictionary[wardNum] && !condition)
        {
            triggerDictionary[wardNum] = false;
        }
    }

    private float GetInfectionThreshold()
    {
        return GameLevel switch
        {
            0 => 20,
            1 => 15,
            2 => 10,
            _ => 20
        };
    }

    private void EnqueueNews(string mainNews)
    {
        moveTextController.EnqueueNews(mainNews);
    }
}
