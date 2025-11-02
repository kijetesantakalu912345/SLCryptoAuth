using System.Collections.Generic;
using System.Net;
using System.Text;
using LiteNetLib;
using SLCryptoAuth.Cryptography.DigitalSignature;
using SLCryptoAuth.Network;
using SLCryptoAuth.Network.DTO;
using SLCryptoAuth.Server.PacketHandlers;

namespace SLCryptoAuth.Server.Core;

public class ServerContext
{
    public static ServerContext Instance { get; } = new();
    
    public Ecdsa ServerIdentity { get; }

    public readonly Dictionary<IPAddress, PreAuthRecord> PreAuthRecords = new();

    private readonly PacketProcessor _packetProcessor;

    private ServerContext()
    {
        ServerIdentity = new Ecdsa();
        
        var handlerFactory = new ServerSidePacketHandlerFactory(ServerIdentity);
        _packetProcessor = new PacketProcessor(handlerFactory);
    }

    public AuthResult ProcessPacket(byte[] packetData, ConnectionRequest request)
    {
        return _packetProcessor.ProcessPacket(packetData, request);
    }
}
