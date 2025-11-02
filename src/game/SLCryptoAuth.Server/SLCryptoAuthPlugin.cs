using System;
using HarmonyLib;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Events;
using Exiled.Events.Patches.Events.Player;

namespace SLCryptoAuth.Server;

public class SLCryptoAuthPlugin : Plugin<Config>
{
    private const string HarmonyId = "com.alexanderk.sl-crypto-auth";

    private Harmony? _harmonyInstance;
    
    /// <inheritdoc />
    public override string Name => "SLCryptoAuth";

    /// <inheritdoc />
    public override string Author => "SLCryptoAuth's Team";

    /// <inheritdoc />
    public override Version Version => new(1, 0, 0);

    /// <inheritdoc />
    public override Version RequiredExiledVersion => new(8, 12, 2);
    

    /// <inheritdoc />
    public override void OnEnabled()
    {
        _harmonyInstance = new Harmony(HarmonyId);
        _harmonyInstance.PatchAll();
        base.OnEnabled();
    }

    /// <inheritdoc />
    public override void OnDisabled()
    {
        _harmonyInstance?.UnpatchAll(HarmonyId);
        _harmonyInstance = null;
        base.OnDisabled();
    }
}
