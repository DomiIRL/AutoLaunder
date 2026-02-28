using AutoLaunder.Models;

namespace AutoLaunder.Services;

public static class RayMessengerService
{
    public static void SendDryMessage(string businessName)
    {
        CustomMessengerService.SendMessage(RayMessages.GetDry(businessName));
    }

    public static void SendWaitingMessage(string businessName, int activeOperations)
    {
        CustomMessengerService.SendMessage(RayMessages.GetWaiting(businessName, activeOperations));
    }

    public static void SendStatusMessage(string businessName, float cashTaken, float capacity, int runsLeft, float totalCashLeft)
    {
        string message;
        if (cashTaken == 0)
            message = RayMessages.GetDry(businessName);
        else if (totalCashLeft == 0)
            message = RayMessages.GetEmpty(businessName, cashTaken);
        else if (totalCashLeft <= capacity)
            message = RayMessages.GetAlmostEmpty(businessName, totalCashLeft);
        else
            message = RayMessages.GetHealthy(businessName, runsLeft, totalCashLeft);

        CustomMessengerService.SendMessage(message);
    }
}