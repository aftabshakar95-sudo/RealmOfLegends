using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace RealmOfLegends.Web.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IModelMetadataProvider modelMetadataProvider)
        {
            _logger = logger;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred during request to {Path}", context.HttpContext.Request.Path);

            // Create a ViewDataDictionary to pass error details to the view
            var viewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                Model = new RealmOfLegends.Web.Models.ErrorViewModel 
                { 
                    RequestId = System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier 
                }
            };
            viewData["ErrorMessage"] = "An unexpected error occurred. Our team has been notified.";

            var result = new ViewResult
            {
                ViewName = "Error", // Ensure this view exists or route to ErrorController
                ViewData = viewData
            };
            
            // To ensure we use the global error controller layout and logic:
            context.Result = new RedirectToActionResult("Error500", "Error", null);
            context.ExceptionHandled = true;
        }
    }
}
