using Microsoft.AspNetCore.Mvc;

namespace MerryClosets.Utils
{
    public class ForbiddenObjectResult : ObjectResult
    {
        public ForbiddenObjectResult(object value)
            : base(value)
        {
            this.StatusCode = new int?(403);
        }
    }
}