using System.Net;
using Applications.Post.Commands;
using Applications.Project.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/blog/post")]
    public class PostController : Controller
    {
        private readonly IMediator _mediator;


        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]

        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody]PostAddCommand request)
        {
            return Ok(await _mediator.Send(request));
        }

        [Authorize]

        [HttpPut()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromBody]PostUpdateCommand request, [FromQuery] Guid Id)
        {
            request.SetId(Id);

            return Ok(await _mediator.Send(request));
        }


        [HttpGet("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromRoute] Guid Id)
        {
            var query = new PostGetQuery() { Id = Id };

            return Ok(await _mediator.Send(query));
        }


        [Authorize]
        [HttpDelete("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid Id)
        {
            var query = new PostDeleteCommand() { Id = Id };

            return Ok(await _mediator.Send(query));
        }


        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] PostListQuery request)
        {

            return Ok(await _mediator.Send(request));
        }

    }
}
