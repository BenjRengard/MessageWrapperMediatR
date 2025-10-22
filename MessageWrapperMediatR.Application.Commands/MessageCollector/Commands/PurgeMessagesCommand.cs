using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Application.Commands.MessageCollector.Commands
{
    public class PurgeMessagesCommand : IRequest<bool>
    {
    }
}
