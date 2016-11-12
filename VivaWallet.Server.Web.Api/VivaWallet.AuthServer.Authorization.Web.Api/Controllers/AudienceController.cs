using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VivaWallet.AuthServer.Authorization.Web.Api.Entities;
using VivaWallet.AuthServer.Authorization.Web.Api.Models;

namespace VivaWallet.AuthServer.Authorization.Web.Api.Controllers
{
    [RoutePrefix("api/audience")]
    public class AudienceController : ApiController
    {
      
            [Route("")]
            public IHttpActionResult Post(AudienceModel audienceModel)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Audience newAudience = AudiencesStore.AddAudience(audienceModel.Name);

                return Ok<Audience>(newAudience);

            }
        [HttpGet]
        [Route("all")]
        public Object Get()
        {

            var aud = AudiencesStore.AudiencesList;

            return aud;
            //return Ok<System.Collections.Concurrent.ConcurrentDictionary>(aud);

        }
    }
    }

