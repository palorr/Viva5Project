using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;

[assembly: OwinStartup(typeof(VivaWallet.Server.Web.Api.Startup))]

namespace VivaWallet.Server.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            var  OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            app.UseOAuthBearerAuthentication(OAuthBearerOptions);


            var issuer = "8737e3f7a7984167b4d09f658a76bf32";
            var audience = "8737e3f7a7984167b4d09f658a76bf32";
            var secret = TextEncodings.Base64Url.Decode("Lo7zvQVYx2erstPrS27DrV0DQ8XzL_0unfiut8SVBP4");
            
            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                    },
                    Provider = new OAuthBearerAuthenticationProvider
                    {
                        OnValidateIdentity = context =>
                        {
                            context.Ticket.Identity.AddClaim(new System.Security.Claims.Claim("newCustomClaim", "newValue"));
                            return Task.FromResult<object>(null);
                        }
                    }
                     
                }
            );
            
        }
    }
}
