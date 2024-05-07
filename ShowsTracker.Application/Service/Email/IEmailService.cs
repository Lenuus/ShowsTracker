using ShowsTracker.Application.Service.Email.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Email
{
    public interface IEmailService: IApplicationService
    {
        Task<ServiceResponse> Send(SendRequestDto request);
    }
}
