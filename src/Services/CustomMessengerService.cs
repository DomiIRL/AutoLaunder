using System.Linq;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using MelonLoader;
using UnityEngine;

namespace AutoLaunder.Services;

public static class CustomMessengerService
{
    private const string RayNpcId    = "ray_hoffman";
    private const string ContactName = "R";
    private const string ConversationId = "autolaunder_r";

    private static MSGConversation _conversation;

    public static void Reset() => _conversation = null;

    public static bool TryInitialize() => GetOrCreateConversation() != null;

    public static void SendMessage(string message)
    {
        try
        {
            MSGConversation conv = GetOrCreateConversation();
            if (conv == null)
            {
                MelonLogger.Warning("[AutoLaunder] Could not get R conversation — falling back to log only.");
                return;
            }

            conv.SendMessage(new Message(message + " - R", Message.ESenderType.Other, true), notify: true, network: false);
        }
        catch (System.Exception ex)
        {
            MelonLogger.Error($"[AutoLaunder] Failed to send message from R: {ex}");
        }
    }

    private static MSGConversation GetOrCreateConversation()
    {
        if (_conversation != null)
        {
            return _conversation;
        }

        NPC ray = NPCManager.GetNPC(RayNpcId);

        if (ray == null)
            return null;
        
        MessageContactInfo contact = new MessageContactInfo(
            ContactName,
            ray.ID,
            ray.MugshotSprite,
            canConversationBeHidden: false,
            displayRelationshipInfo: false);

        _conversation = new MSGConversation(contact, ConversationId);
        _conversation.SetIsKnown(false); // hide name and avatar
        return _conversation;
    }
}