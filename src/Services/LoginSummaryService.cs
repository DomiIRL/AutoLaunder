using System.Collections;
using System.Collections.Generic;
using AutoLaunder.Models;
using Il2CppScheduleOne.Property;
using MelonLoader;
using UnityEngine;

namespace AutoLaunder.Services;

public static class LoginSummaryService
{

    public static IEnumerator Run()
    {
        while (GameObject.Find("Player_Local") == null)
            yield return null;

        while (!CustomMessengerService.TryInitialize())
            yield return null;

        yield return WaitForBusinessDataStable();

        SendLoginSummary();
    }

    private static IEnumerator WaitForBusinessDataStable()
    {
        const int requiredStableFrames = 10;
        int stableFrames = 0;
        int lastCount = -1;

        while (stableFrames < requiredStableFrames)
        {
            int count = 0;
            var businesses = Object.FindObjectsOfType<Business>();
            foreach (var biz in businesses)
            {
                if (biz.IsOwned)
                    count += biz.LaunderingOperations.Count;
            }

            if (count == lastCount)
                stableFrames++;
            else
            {
                stableFrames = 0;
                lastCount = count;
            }

            yield return null;
        }
    }

    private static void SendLoginSummary()
    {
        try
        {
            var businesses = Object.FindObjectsOfType<Business>();
            if (businesses == null || businesses.Length == 0)
                return;

            var underCapacity = new List<string>();
            var idle          = new List<string>();

            foreach (var biz in businesses)
            {
                if (!biz.IsOwned)
                {
                    continue;
                }
                
                int ops = biz.LaunderingOperations.Count;
                float capacity = biz.LaunderCapacity;

                if (ops > 0)
                {
                    float committed = 0f;
                    foreach (var operation in biz.LaunderingOperations)
                    {
                        committed += operation.amount;
                    }

                    if (committed < capacity)
                    {
                        underCapacity.Add(biz.propertyName);
                    }
                }
                else
                {
                    // Idle = no ops running, regardless of how much cash is present
                    idle.Add(biz.propertyName);
                }
            }
            
            string message;
            if (underCapacity.Count > 0 && idle.Count > 0)
            {
                message = RayMessages.GetLoginMixed(NaturalJoin(underCapacity), NaturalJoin(idle));
            } 
            else if (underCapacity.Count > 0) 
            {
                message = RayMessages.GetLoginUnderCapacity(NaturalJoin(underCapacity));
            } 
            else if (idle.Count > 0)
            {
                message = RayMessages.GetLoginIdle(NaturalJoin(idle));
            }
            else
            {
                return;
            }

            CustomMessengerService.SendMessage(message);
        }
        catch (System.Exception ex)
        {
            MelonLogger.Error($"[AutoLaunder] LoginSummaryService failed: {ex}");
        }
    }

    private static string NaturalJoin(List<string> list)
    {
        if (list.Count == 0) return "";
        if (list.Count == 1) return list[0];
        if (list.Count == 2) return $"{list[0]} and {list[1]}";
        return string.Join(", ", list.GetRange(0, list.Count - 1)) + " and " + list[list.Count - 1];
    }
}
