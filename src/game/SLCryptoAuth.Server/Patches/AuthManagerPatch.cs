using System;
using CentralAuth;
using HarmonyLib;
using JetBrains.Annotations;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using SLCryptoAuth.Server.Core;
using Exiled.API.Features;

namespace SLCryptoAuth.Server.Patches;

[HarmonyPatch]
internal static class AuthManagerPatch
{
    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.FixedUpdate))]
    public static void FixedUpdatePostfix(PlayerAuthenticationManager __instance)
    {
        if (!NetworkServer.active) return;
        if (PlayerAuthenticationManager.OnlineMode) return;
        
        if (__instance.isLocalPlayer) return;

        if (!__instance.connectionToClient.isReady) return;
        if (__instance._authenticationRequested) return;
        
        var netPeer = LiteNetLib4MirrorServer.Peers[__instance.connectionToClient.connectionId];
        var endPoint = netPeer.EndPoint;
        if (!ServerContext.Instance.PreAuthRecords.TryGetValue(endPoint.Address, out var preauthRecord))
        {
            // Failed to get user id
            return;
        }

        try
        {
            __instance._authenticationRequested = true;
            __instance._hub.nicknameSync.UpdateNickname(preauthRecord.Nickname);
            
            Timing.CallDelayed(1.0f, () =>
            {
                __instance._hub.serverRoles.RefreshPermissions();
                __instance._hub.gameConsoleTransmission.SendToClient(
                    $"Hi, {preauthRecord.Nickname}! You have been authenticated on this server", "green");
            });
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.Start))]
    public static bool StartPrefix(PlayerAuthenticationManager __instance)
    {
        if (!NetworkServer.active) return true;
        if (PlayerAuthenticationManager.OnlineMode) return true;
        
        if (__instance.isLocalPlayer) return true;

        var netPeer = LiteNetLib4MirrorServer.Peers[__instance.connectionToClient.connectionId];
        var endPoint = netPeer.EndPoint;
        if (!ServerContext.Instance.PreAuthRecords.TryGetValue(endPoint.Address, out var preauthRecord))
        {
            // Failed to get user id
            return true;
        }

        __instance.UserId = preauthRecord.UserId;
        __instance.DoNotTrack = preauthRecord.DoNotTrack.Value;
        try
        {
            __instance._hub.encryptedChannelManager.EncryptionKey = preauthRecord.SharedSecret; // shared secret
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return false;
    }
}
