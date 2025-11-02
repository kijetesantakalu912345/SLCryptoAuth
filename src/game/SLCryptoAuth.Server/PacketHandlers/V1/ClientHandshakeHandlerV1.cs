using System;
using System.Linq;
using LiteNetLib;
using SLCryptoAuth.Cryptography.DigitalSignature;
using SLCryptoAuth.Cryptography.Encryption;
using SLCryptoAuth.IO;
using SLCryptoAuth.Network.DTO;
using SLCryptoAuth.Server.Core;
using Exiled.API.Features;

namespace SLCryptoAuth.Server.PacketHandlers.V1;

public class ClientHandshakeHandlerV1 : ServerAuthPacketHandler
{
    /// <inheritdoc />
    public override byte Id => 51;

    /// <inheritdoc />
    public override byte Version => 1;

    /// <inheritdoc />
    public override AuthResult ProcessPayload(BinaryReaderExtended payloadReader, ConnectionRequest request)
    {
        var clientIp = request.RemoteEndPoint.Address;

        if (!ServerContext.Instance.PreAuthRecords.TryGetValue(clientIp, out var preAuth))
        {
            return AuthResult.Answer([2]);
        }
        
        try
        {
            #region Parse Packet

            var encryptedData = payloadReader.ReadByteArrayWithLength();
            var signature = payloadReader.ReadRemainingBytes();
            
            #endregion

            #region Decrypt Data

            // TODO: AES shared secret from context
            var sharedEncryption = new Aes(preAuth.SharedSecret);

            var decryptedData = sharedEncryption.Decrypt(encryptedData);
            var decryptedReader = new BinaryReaderExtended(decryptedData);
            
            #endregion

            #region Parse Decrypted Block

            var timestamp = decryptedReader.ReadInt64();
            var clientIdentityPublicKey = decryptedReader.ReadBytes(33);
            var serverSessionPublicKey = decryptedReader.ReadBytes(33);
            var doNotTrack = decryptedReader.ReadBoolean();
            var nickname = decryptedReader.ReadString();

            #endregion

            #region Process Packet
            
            // Check signature
            var clientIdentity = new EcdsaVerify(clientIdentityPublicKey);
            if (!clientIdentity.Verify(encryptedData, signature))
            {
                //Logger.Error("[FAILED]: Client sent invalid signature!");
                return AuthResultFactory.InvalidSignature();
            }
            
            // Check Server Session Public Key
            if (!preAuth.ServerSession.PublicKeyBytes.SequenceEqual(serverSessionPublicKey))
            {
                //Logger.Error("[FAILED]: MITM detected!");
                return AuthResultFactory.InvalidToken();
            }

            {
                preAuth.SetClientIdentity(clientIdentityPublicKey);
                preAuth.SetDoNotTrack(doNotTrack);
                preAuth.SetNickname(nickname);
                
                var netPeer = request.Accept();
                CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                
                return AuthResult.Handled();
            }
            #endregion
        }
        catch (Exception ex)
        {
            Log.Error("[EXCEPTION]: " + ex);
            return AuthResultFactory.InvalidToken();
        }
    }
}
