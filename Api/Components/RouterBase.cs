using MediatR;

namespace Api.Components
{
    public class RouterBase
    {
        protected ILogger? Logger;

        protected IMediator? Mediator;
        public virtual void AddRoutes(WebApplication app)
        {

        }
    }
}
