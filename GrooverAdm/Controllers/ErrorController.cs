using GrooverAdm.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorController: ControllerBase
    {
		private readonly ILogger log;
		public ErrorController(ILogger<ErrorController> log)
		{
			this.log = log;
		}

		[Route("error")]
		internal ActionResult HandleError()
		{
			var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
			var ex = context?.Error;
			var code = StatusCodes.Status500InternalServerError;

			if (ex is GrooverAuthException)
			{
				var internalException = (GrooverAuthException)ex;
				code = internalException.HttpCode;
				log.LogInformation(internalException, $"Handled grooverException with message: {internalException.Message}");

				return StatusCode(code, internalException.Message);
			}
			else if (ex is GrooverException)
			{
				var internalException = (GrooverException)ex;
				code = internalException.HttpCode;
				log.LogInformation(internalException, $"Handled grooverException with message: {internalException.Message}");

				return StatusCode(code, internalException.Message);
			} else if (ex is NotImplementedException)
			{
				code = StatusCodes.Status501NotImplemented;
				log.LogInformation(ex, "ErrorController, handled notImplementedException");

				return StatusCode(code, "Not implemented");
			}
			else
			{
				log.LogError(ex, "Unexpected error occurred");

				return StatusCode(code, ex.Message);
			}
		}
	}
}
