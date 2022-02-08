using KissLog;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SistemaDeControleDeTCCs.Extentions.Filters
{
    public class AuditoriaILoggerFilter : IActionFilter

    {
        private readonly ILogger _logger;

        public AuditoriaILoggerFilter(ILogger logger)
        {
            _logger = logger;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = context.HttpContext.User.Identity.Name;
                var tipoAuth = context.HttpContext.User.Identity.AuthenticationType;
                var urlAcessada = context.HttpContext.Request.GetDisplayUrl();
                var valueHost = context.HttpContext.Request.Host.Value;
                var tipoContent = context.HttpContext.Request.ContentType;
                var dadoauterado = context.HttpContext.Items.Values;

                var logMsg = $"O usuário {user} acessou a Url {urlAcessada} \nEm: {DateTime.Now} \n =============================\n{valueHost}\n{tipoContent}\nTipo de Autenticação: {tipoAuth}\nO seguinte dado foi auterado:{dadoauterado}";

                _logger.Info(logMsg);
              
            }
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            
            _logger.Info($"Url Acessada: {context.HttpContext.Request.GetDisplayUrl()} \n " +
                $"\n ________________________________________\n\n");
        }
    }
}
