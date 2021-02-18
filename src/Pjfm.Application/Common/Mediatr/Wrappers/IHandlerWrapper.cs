﻿using MediatR;

namespace Pjfm.Application.MediatR.Wrappers
{
    public interface IHandlerWrapper<Tin, Tout> : IRequestHandler<Tin, Response<Tout>> where Tin : IRequestWrapper<Tout>
    {
        
    }
}