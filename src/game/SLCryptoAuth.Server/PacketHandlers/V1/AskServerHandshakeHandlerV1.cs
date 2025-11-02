using System;
using LiteNetLib;
using SLCryptoAuth.Cryptography.DigitalSignature;
using SLCryptoAuth.IO;
using SLCryptoAuth.Network.DTO;
using SLCryptoAuth.Server.Core;
using Exiled.API.Features;

namespace SLCryptoAuth.Server.PacketHandlers.V1;

public class AskServerHandshakeHandlerV1(Ecdsa serverIdentity) : ServerAuthPacketHandler
{
    /// <inheritdoc />
    public override byte Id => 50;

    /// <inheritdoc />
    public override byte Version => 1;

    /// <inheritdoc />
    public override AuthResult ProcessPayload(BinaryReaderExtended payloadReader, ConnectionRequest request)
    {
        try
        {
            #region Parse Packet

            var clientSessionPublicKey = payloadReader.ReadBytes(33);

            #endregion

            #region Process Packet

            var preAuth = new PreAuthRecord(clientSessionPublicKey);
            var clientIp = request.RemoteEndPoint.Address;
            ServerContext.Instance.PreAuthRecords[clientIp] = preAuth;
            
            {
                var responseBinaryWriter = new BinaryWriterExtended();
                
                // { HEADER_BLOCK }
                responseBinaryWriter.Write((byte)50);
                responseBinaryWriter.Write((byte)1);

                // { DATA_BLOCK }
                var dataBlock = new BinaryWriterExtended();
                dataBlock.Write(preAuth.Added); // timestamp
                dataBlock.Write(serverIdentity.PublicKeyBytes); // server_identity_pub_key
                dataBlock.Write(preAuth.ServerSession.PublicKeyBytes); // server_session_pub_key
                dataBlock.Write(clientSessionPublicKey); // client_session_pub_key
                responseBinaryWriter.Write(dataBlock);
                
                // { SIGNATURE_BLOCK }
                var signature = serverIdentity.Sign(dataBlock.ToArray());
                responseBinaryWriter.Write(signature);

                return AuthResult.Answer(responseBinaryWriter.ToArray());
            }
            
            #endregion
        }
        catch (Exception ex)
        {
            Log.Error("Error: " + ex);
            return AuthResultFactory.InvalidToken();
        }
    }
}