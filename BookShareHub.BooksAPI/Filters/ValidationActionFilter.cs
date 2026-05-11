//using FluentValidation;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace BookShareHub.BooksAPI.Filters
//{
//    public class ValidationActionFilter : IActionFilter
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public ValidationActionFilter(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        public void OnActionExecuting(ActionExecutingContext context)
//        {
//            foreach (var arg in context.ActionArguments.Values)
//            {
//                if (arg == null) continue;

//                var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
//                var validator = _serviceProvider.GetService(validatorType) as IValidator;

//                if (validator != null)
//                {
//                    var result = validator.Validate(new ValidationContext<object>(arg));
//                    if (!result.IsValid)
//                    {
//                        context.Result = new BadRequestObjectResult(result.Errors);
//                        return;
//                    }
//                }
//            }
//        }

//        public void OnActionExecuted(ActionExecutedContext context) { }
//    }

//}
