using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using VivaWallet.AuthServer.Authorization.Web.Api.Entities;

namespace VivaWallet.AuthServer.Authorization.Web.Api
{

    public static class AudiencesStore
        {
            public static ConcurrentDictionary<string, Audience> AudiencesList = new ConcurrentDictionary<string, Audience>();

            static AudiencesStore()
            {
                AudiencesList.TryAdd("8737e3f7a7984167b4d09f658a76bf32",
                    new Audience
                    {
                        ClientId = "8737e3f7a7984167b4d09f658a76bf32",
                        Base64Secret = "Lo7zvQVYx2erstPrS27DrV0DQ8XzL_0unfiut8SVBP4",
                        Name = "8737e3f7a7984167b4d09f658a76bf32"
                    }
                );
            }

            public static Audience AddAudience(string name)
            {
                var clientId = Guid.NewGuid().ToString("N");

                var key = new byte[32];
                RNGCryptoServiceProvider.Create().GetBytes(key);
                var base64Secret = TextEncodings.Base64Url.Encode(key);

                Audience newAudience = new Audience { ClientId = clientId, Base64Secret = base64Secret, Name = name };
                AudiencesList.TryAdd(clientId, newAudience);
                return newAudience;
            }

            public static Audience FindAudience(string clientId)
            {
                Audience audience = null;
                if (AudiencesList.TryGetValue(clientId, out audience))
                {
                    return audience;
                }
                return null;
            }
        }
    }
