using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); // get hold of the context we get from next
            
            //check if authenticated
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            // var username = resultContext.HttpContext.User.GetUsername(); //swap this with GetUserId extn method
            var userId = resultContext.HttpContext.User.GetUserId();

            //access our repository
            //GetService is from Microsoft.Extensions.DependencyInjection
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            
            //access our User object
            // var user = await repo.GetUserByUsernameAsync(username);
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}