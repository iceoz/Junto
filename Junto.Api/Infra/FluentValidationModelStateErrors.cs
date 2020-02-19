using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Junto.Api.Infra
{
    public class FluentValidationModelStateErrors
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
