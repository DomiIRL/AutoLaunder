using System.Linq;
using Il2CppScheduleOne.NPCs;
using MelonLoader;
using AutoLaunder.Models;
using UnityEngine;

namespace AutoLaunder.Services;

public static class RayMessengerService
{
    private const string NpcID = "ray_hoffman";

    public static void SendStatusMessage(string businessName, bool launchFailed, int runsLeft, float totalCashLeft)
    {
        NPC ray = FindRay();

        if (ray == null)
        {
            MelonLogger.Warning("[AutoLaunder] Ray not found in scene: could not send status message.");
            return;
        }

        var message = launchFailed
            ? RayMessages.GetEmpty(businessName)
            : runsLeft <= 1
                ? RayMessages.GetAlmostEmpty(businessName, totalCashLeft)
                : RayMessages.GetHealthy(businessName, runsLeft, totalCashLeft);

        MelonLogger.Msg($"[AutoLaunder] Sending message via Ray ({ray.fullName}): {message}");
        ray.SendTextMessage(message);
    }

    private static NPC FindRay()
    {
        return Object.FindObjectsOfType<NPC>().FirstOrDefault(npc => npc.ID == NpcID);
    }
}