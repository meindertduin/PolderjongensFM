using MediatR;

 namespace Pjfm.Application.Common.Mediatr.Wrappers
{
    public interface IHandlerWrapper<Tin, Tout> : IRequestHandler<Tin, Response<Tout>> where Tin : IRequestWrapper<Tout>
    {
        
    }
}