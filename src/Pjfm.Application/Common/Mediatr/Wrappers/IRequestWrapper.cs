﻿using MediatR;

 namespace Pjfm.Application.Common.Mediatr.Wrappers
{
    public interface IRequestWrapper<T> : IRequest<Response<T>>
    {
        
    }
}