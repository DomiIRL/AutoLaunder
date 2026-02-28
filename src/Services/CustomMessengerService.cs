using System.Linq;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.UI.Phone.Messages;
using MelonLoader;
using UnityEngine;

namespace AutoLaunder.Services;

public static class CustomMessengerService
{
    private const string RayNpcId    = "ray_hoffman";
    private const string ContactName = "R";

    private static MSGConversation _conversation;

    public static void SendMessage(string message)
    {
        try
        {
            MSGConversation conv = GetOrCreateConversation();
            if (conv == null)
            {
                MelonLogger.Warning("[AutoLaunder] Could not get R conversation — falling back to log only.");
                MelonLogger.Msg($"[AutoLaunder] [R] {message}");
                return;
            }

            conv.SendMessage(new Message(message, Message.ESenderType.Other, true), notify: true, network: false);
            MelonLogger.Msg($"[AutoLaunder] [R] {message}");
        }
        catch (System.Exception ex)
        {
            MelonLogger.Error($"[AutoLaunder] Failed to send message from R: {ex}");
        }
    }

    /// <summary>Clears the cached conversation (e.g. on scene unload).</summary>
    public static void Reset() => _conversation = null;

    private static MSGConversation GetOrCreateConversation()
    {
        if (_conversation != null)
            return _conversation;

        NPC ray = Object.FindObjectsOfType<NPC>()
            .FirstOrDefault(npc => npc.ID == RayNpcId);
        
        foreach (NPC npc in Object.FindObjectsOfType<NPC>())
        {
            MelonLogger.Msg($"[AutoLaunder] Found NPC: {npc.ID}");
        }

        if (ray == null)
        {
            MelonLogger.Warning("[AutoLaunder] Ray not found in scene — cannot create R conversation.");
            return null;
        }

        // Create a new conversation with Ray as the NPC backing (avatar/photo) but
        // with a distinct contact name so it appears as a separate thread on the phone.
        _conversation = new MSGConversation(ray, ContactName);

        return _conversation;
    }
}