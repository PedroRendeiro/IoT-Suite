using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace IoTSuite.Server.Helpers
{
	/// <summary>
	/// Based on Microsoft's DefaultProblemDeatilsFactory
	/// https://github.com/aspnet/AspNetCore/blob/2e4274cb67c049055e321c18cc9e64562da52dcf/src/Mvc/Mvc.Core/src/Infrastructure/DefaultProblemDetailsFactory.cs
	/// </summary>
	public class ProblemDetailsFactoryHelper : Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory
	{
		private readonly ApiBehaviorOptions _options;

		/// <inheritdoc />
		public ProblemDetailsFactoryHelper(
			IOptions<ApiBehaviorOptions> options)
		{
			_options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		/// <inheritdoc />
		public override ProblemDetails CreateProblemDetails(
			HttpContext httpContext,
			int? statusCode = null,
			string title = null,
			string type = null,
			string detail = null,
			string instance = null)
		{
			statusCode ??= 500; // <-- Microsoft hard codes the value? Why aren't they using StatusCodes.Status500InternalServerError?

			ProblemDetails problemDetails = null;

			var context = httpContext.Features.Get<IExceptionHandlerFeature>();

			if (context?.Error != null)
			{
				if (context.Error is YourBussException ybe)
				{
					statusCode = 400;
					httpContext.Response.StatusCode = statusCode.Value; // <-- The result serializer doesn't use the status from the 
																		//	ProblemDetails object to set this code. You have to set
																		//	it by hand.

					problemDetails = new ProblemDetails
					{
						Status = statusCode,
						Title = ybe.Title,
						Type = ybe.Type,
						Detail = ybe.Message,
						Instance = instance,
						Extensions =
						{
							{ "extinfo", ybe.ExtendedInfo }
						}
					};
				}
			}

			if (problemDetails == null)
			{
				//	default exception handler
				problemDetails = new ProblemDetails
				{
					Status = statusCode,
					Title = title,
					Type = type,
					Detail = detail,
					Instance = instance,
				};
			}

			ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

			return problemDetails;
		}

		/// <inheritdoc />
		public override ValidationProblemDetails CreateValidationProblemDetails(
			HttpContext httpContext,
			ModelStateDictionary modelStateDictionary,
			int? statusCode = null,
			string title = null,
			string type = null,
			string detail = null,
			string instance = null)
		{
			if (modelStateDictionary == null)
			{
				throw new ArgumentNullException(nameof(modelStateDictionary));
			}

			statusCode ??= 400;

			var problemDetails = new ValidationProblemDetails(modelStateDictionary)
			{
				Status = statusCode,
				Type = type,
				Detail = detail,
				Instance = instance,
			};

			if (title != null)
			{
				// For validation problem details, don't overwrite the default title with null.
				problemDetails.Title = title;
			}

			ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

			return problemDetails;
		}

		private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
		{
			//problemDetails.Status ??= statusCode;
			problemDetails.Status = problemDetails.Status ?? statusCode;

			if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
			{
				problemDetails.Title ??= clientErrorData.Title;
				problemDetails.Type ??= clientErrorData.Link;
			}

			var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
			if (traceId != null)
			{
				problemDetails.Extensions["traceId"] = traceId;
			}
		}
	}

	public class YourBussException : Exception
	{
		public string Type { get; } = "http://your.business.com/rfc7807/businessexception";
		public string Title { get; } = "A business validation exception occurred.";
		public object ExtendedInfo { get; } // this will be used to add more info to the returned object

		public YourBussException(
			[NotNull] string message,
			object extendedInfo = null,
			string type = null,
			string title = null)
			: base(message)
		{
			ExtendedInfo = extendedInfo ?? ExtendedInfo;
			Type = type ?? Type;
			Title = title ?? Title;
		}
	}
}