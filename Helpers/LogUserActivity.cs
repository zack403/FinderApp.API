using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace FinderApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = resultContext.HttpContext.RequestServices.GetService<IFinderRepository>();
            var user = await userFromRepo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await userFromRepo.CompleteAsync();
        }
    }
}