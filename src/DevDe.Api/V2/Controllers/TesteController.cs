using DevDe.Api.Controllers;
using DevIO.Business.Intefaces;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DevDe.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        private readonly ILogger _logger;
        public TesteController(INotificador notificador, IUser appUser, ILogger<TesteController> logger) : base(notificador, appUser)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Mensagem()
        {
            //throw new Exception("Error");
            //try
            //{
            //    var x = 0;
            //    var result = 10 / x;
            //}
            //catch (DivideByZeroException e)
            //{
            //    e.Ship(HttpContext);
            //}
            
            _logger.LogTrace("Log de Trace");
            _logger.LogDebug("Log de Debug");
            _logger.LogInformation("Log de Informação");
            _logger.LogWarning("Log de Aviso");
            _logger.LogError("Log de Error");
            _logger.LogCritical("Log Critico");
            
            
            return "Version V2 (2.0)";
        }

    }
}
