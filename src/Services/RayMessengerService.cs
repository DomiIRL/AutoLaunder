using System.Linq;
using Il2CppScheduleOne.NPCs;
using MelonLoader;
using AutoLaunder.Models;
using UnityEngine;

namespace AutoLaunder.Services;

public static class RayMessengerService
{
    private const string NpcID = "ray_hoffman";

    public static void SendWaitingMessage(string businessName, int activeOperations)
    {
        NPC ray = FindRay();

        if (ray == null)
        {
            MelonLogger.Warning("[AutoLaunder] Ray not found in scene: could not send waiting message.");
            return;
        }

        var message = RayMessages.GetWaiting(businessName, activeOperations);
        MelonLogger.Msg($"[AutoLaunder] Sending waiting message via Ray ({ray.fullName}): {message}");
        ray.SendTextMessage(message);
    }

    public static void SendStatusMessage(string businessName, float cashTaken, float capacity, int runsLeft, float totalCashLeft)
    {
        NPC ray = FindRay();

        if (ray == null)
        {
            MelonLogger.Warning("[AutoLaunder] Ray not found in scene: could not send status message.");
            return;
        }

        string message;
        if (cashTaken == 0)
        {
            // Nothing started at all — storage was completely dry
            message = RayMessages.GetDry(businessName);
        }
        else if (cashTaken < capacity)
        {
            // Started below capacity — storage ran out during this fill
            message = RayMessages.GetEmpty(businessName, cashTaken);
        }
        else if (totalCashLeft <= capacity)
        {
            // Started at full capacity, but not enough left for another full run
            message = RayMessages.GetAlmostEmpty(businessName, totalCashLeft);
        }
        else
        {
            // Started at full capacity, 2+ full runs still in storage
            message = RayMessages.GetHealthy(businessName, runsLeft, totalCashLeft);
        }

        MelonLogger.Msg($"[AutoLaunder] Sending message via Ray ({ray.fullName}): {message}");
        ray.SendTextMessage(message);
    }

    private static NPC FindRay()
    {
        return Object.FindObjectsOfType<NPC>().FirstOrDefault(npc => npc.ID == NpcID);
    }
}