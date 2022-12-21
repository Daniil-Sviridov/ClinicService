using ClinicService.Models;
using ClinicService.Models.Requests;
using ClinicService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace ClinicService.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<AuthenticationResponse> Login([FromBody] AuthenticationRequest authenticationRequest)
        {

            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            if (authenticationResponse.Status == Models.AuthenticationStatus.Success)
            {
                Response.Headers.Add("X-Sesion-Token", authenticationResponse.SessionContext.SessionToken);
            }

            return Ok(authenticationResponse);
        }
        [HttpGet("session")]
        public ActionResult<SessionContext> GetSession()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var sesionToken = headerValue.Parameter;

                if(string.IsNullOrEmpty(sesionToken)) 
                {
                    return Unauthorized();
                }

                SessionContext sessionContext = _authenticateService.GetSessionInfo(sesionToken);

                if(sessionContext == null) 
                {
                    return Unauthorized();
                }

                return Ok(sessionContext);
            }

            return Unauthorized();
        }

    }
}
