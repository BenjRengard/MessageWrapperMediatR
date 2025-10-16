using MediatR;
using MessageWrapperMediatR.Application.Publisher;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Controllers
{
    /// <summary>
    /// Controller to publish.
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublisherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Publish a generic message
        /// </summary>
        /// <param name="command"></param>
        /// <returns> </returns>
        [HttpPost("publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublishAsync([FromBody] DirectPublishMessageCommand command)
        {
            if (await _mediator.Send(command))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
