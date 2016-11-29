using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using VivaWallet.AuthServer.Authorization.Web.Api.Models;

namespace VivaWallet.AuthServer.Authorization.Web.Api.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {

        ApplicationTypes AppType;
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //without refresh tokens
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            string symmetricKeyAsBase64 = string.Empty;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.SetError("invalid_clientId", "client_Id is not set");
                return Task.FromResult<object>(null);
            }

            var audience = AudiencesStore.FindAudience(context.ClientId);

            if (audience == null)
            {
                context.SetError("invalid_clientId", string.Format("Invalid client_id '{0}'", context.ClientId));
                return Task.FromResult<object>(null);
            }
            
            context.Validated();
            return Task.FromResult<object>(null);
             

            //string clientId = string.Empty;
            //string clientSecret = string.Empty;
            //Client client = null;

            //if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            //{
            //    context.TryGetFormCredentials(out clientId, out clientSecret);
            //}

            //if (context.ClientId == null)
            //{
            //    //Remove the comments from the below line context.SetError, and invalidate context 
            //    //if you want to force sending clientId/secrects once obtain access tokens. 
            //    //context.Validated();
            //    context.SetError("invalid_clientId", "ClientId should be sent.");
            //    return Task.FromResult<object>(null);
            //}

            //using (AuthRepository _repo = new AuthRepository())
            //{
            //    client = _repo.FindClient(context.ClientId);
            //}

            //if (client == null)
            //{
            //    context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
            //    return Task.FromResult<object>(null);
            //}

            /*
            * set App Type
            * tharvanits 31/5/2016
            * Set AppType For Admin Panel
            */
            //AppType = client.ApplicationType;
            //if (client.ApplicationType == Models.ApplicationTypes.NativeConfidential)
            //{
            //    if (string.IsNullOrWhiteSpace(clientSecret))
            //    {
            //        context.SetError("invalid_clientId", "Client secret should be sent.");
            //        return Task.FromResult<object>(null);
            //    }
            //    else
            //    {
            //        if (client.Secret != clientSecret)//Helper.GetHash(clientSecret))
            //        {
            //            context.SetError("invalid_clientId", "Client secret is invalid.");
            //            return Task.FromResult<object>(null);
            //        }
            //    }
            //}

            //if (!client.Active)
            //{
            //    context.SetError("invalid_clientId", "Client is inactive.");
            //    return Task.FromResult<object>(null);
            //}

            //var audience = AudiencesStore.FindAudience(context.ClientId);

            //if (audience == null)
            //{
            //    context.SetError("invalid_clientId", string.Format("Invalid client_id '{0}'", context.ClientId));
            //    return Task.FromResult<object>(null);
            //}

            //context.Validated();
            //return Task.FromResult<object>(null);
        }

         //without refresh tokens
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (AuthRepository _repo = new AuthRepository())
            {
                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity("JWT");
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("sub", context.UserName));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         "audience", (context.ClientId == null) ? string.Empty : context.ClientId
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }
         
        //public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{

        //    var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

        //    if (allowedOrigin == null) allowedOrigin = "*";

        //    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

        //    using (AuthRepository _repo = new AuthRepository())
        //    {
        //        IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

        //        if (user == null)
        //        {
        //            context.SetError("invalid_grant", "The user name or password is incorrect.");
        //            return;
        //        }

        //        /*
        //        * tharvanitis 31/5/2016
        //        * Check if the application send the request for the token requires Administrator Rights
        //        * Check if the user has admin rights
        //        * Added for the Admin Panel
        //        */
        //        //if (!await UsersDALHelper.isActivated(context.UserName))
        //        //{
        //        //    context.SetError("Invalid_grant", "The user is deactivated");
        //        //    return;
        //        //}

        //        if (AppType == Models.ApplicationTypes.AdminPanel)
        //        {
        //            if (user.Roles.Where(e => e.RoleId.Equals("1")).ToList().Count == 0)
        //            {
        //                context.SetError("Invalid_grant", "The user don't have admin rights");
        //                return;
        //            }
        //        }


        //    }

        //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        //    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
        //    identity.AddClaim(new Claim("sub", context.UserName));
        //    identity.AddClaim(new Claim("role", "Admin"));

        //    var props = new AuthenticationProperties(new Dictionary<string, string>
        //        {
        //            {
        //                "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
        //            },
        //            {
        //                "userName", context.UserName
        //            }
        //        });

        //    var ticket = new AuthenticationTicket(identity, props);
        //    context.Validated(ticket);

        //}

        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{
        //    foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
        //    {
        //        context.AdditionalResponseParameters.Add(property.Key, property.Value);
        //    }

        //    return Task.FromResult<object>(null);
        //}

        //public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        //{
        //    var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
        //    var currentClient = context.ClientId;

        //    if (originalClient != currentClient)
        //    {
        //        context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
        //        return Task.FromResult<object>(null);
        //    }

        //    // Change auth ticket for refresh token requests
        //    var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

        //    //if (!UsersDALHelper._isActivated(newIdentity.Name))
        //    //{
        //    //    context.SetError("Invalid_grant", "The user is deactivated");
        //    //    return Task.FromResult<object>(null);
        //    //}

        //    newIdentity.AddClaim(new Claim("newClaim", "newValue"));

        //    var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
        //    context.Validated(newTicket);

        //    return Task.FromResult<object>(null);
        //}

    }
}