using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Auth.Api
{
    public class IdentityService : Identity.IdentityBase
    {
        private readonly ILogger<IdentityService> _logger;
        public IdentityService(ILogger<IdentityService> logger)
        {
            _logger = logger;
        }

        
    }
}
