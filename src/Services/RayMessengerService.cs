using AutoLaunder.Models;
using MelonLoader;

namespace AutoLaunder.Services;

public static class RayMessengerService
{
    public static void SendWaitingMessage(string businessName, int activeOperations)
    {
        string message = RayMessages.GetWaiting(businessName, activeOperations);
        MelonLogger.Msg($"[AutoLaunder] Sending waiting message: {message}");
        CustomMessengerService.SendMessage(message);
    }

    public static void SendStatusMessage(string businessName, float cashTaken, float capacity, int runsLeft, float totalCashLeft)
    {
        string message;
        if (cashTaken == 0)
        {
            message = RayMessages.GetDry(businessName);
        }
        else if (totalCashLeft == 0)
        {
            message = RayMessages.GetEmpty(businessName, cashTaken);
        }
        else if (totalCashLeft <= capacity)
        {
            message = RayMessages.GetAlmostEmpty(businessName, totalCashLeft);
        }
        else
        {
            message = RayMessages.GetHealthy(businessName, runsLeft, totalCashLeft);
        }

        MelonLogger.Msg($"[AutoLaunder] Sending status message: {message}");
        CustomMessengerService.SendMessage(message);
    }
}