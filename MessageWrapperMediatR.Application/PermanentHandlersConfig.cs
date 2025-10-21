using MessageWrapperMediatR.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Application
{
    public class PermanentHandlersConfig
    {
        public List<HandlerContract> Handlers { get; set; } = new List<HandlerContract>();

    }
}
