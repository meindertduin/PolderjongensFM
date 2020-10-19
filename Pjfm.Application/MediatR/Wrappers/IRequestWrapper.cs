﻿using MediatR;

namespace Pjfm.Application.MediatR.Wrappers
{
    public interface IRequestWrapper<T> : IRequest<Response<T>>
    {
        
    }
}