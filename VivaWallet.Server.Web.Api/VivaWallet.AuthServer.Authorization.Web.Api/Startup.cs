using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Net.Http.Headers;
using VivaWallet.AuthServer.Authorization.Web.Api.Providers;
using Microsoft.Owin.Security.OAuth;
using VivaWallet.AuthServer.Authorization.Web.Api.Formats;

[assembly: OwinStartup(typeof(VivaWallet.AuthServer.Authorization.Web.Api.Startup))]

namespace VivaWallet.AuthServer.Authorization.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            HttpConfiguration config = new HttpConfiguration();


            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
           

            ConfigureOAuth(app);

            WebApiConfig.Register(config);

           app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {

         //   app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie); //not sure if we want this yet
           var  OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth2/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(1500), //ONE DAY ACCESS TOKEN
                Provider = new CustomOAuthProvider(),
                // RefreshTokenProvider = new CustomRefreshTokenProvider()
                AccessTokenFormat = new JwtFormatter("8737e3f7a7984167b4d09f658a76bf32")

            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
}
