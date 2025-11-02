using System;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using LiteNetLib;
using SLCryptoAuth.Server.Core;
using Exiled.API.Features;

namespace SLCryptoAuth.Server.Patches;


[HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
public static class TransportPatch
{
    [UsedImplicitly]
    public static bool Prefix(ConnectionRequest request)
    {
        try
        {
            var originalPosition = request.Data.Position;
            var packetPayload = request.Data.GetRemainingBytes();
            var authResult = ServerContext.Instance.ProcessPacket(packetPayload, request);
            request.Data.SetPosition(originalPosition);

            if (authResult.IsHandled)
            {
                if (authResult.SendAnswer)
                {
                    request.RejectForce(authResult.Data);
                }

                return false;
            }

            if (authResult.ReplaceData)
                request.Data.SetSource(authResult.Data);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return true;
    }
}
