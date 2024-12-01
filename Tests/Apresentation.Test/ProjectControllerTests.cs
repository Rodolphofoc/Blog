using System.Net;
using Api.Controllers;
using Applications.Post.Commands;
using Applications.Project.Model;
using Applications.Project.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests.Controllers
{
    public class ProjectControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly PostController _controller;

        public ProjectControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new PostController(_mockMediator.Object);
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var command = new PostAddCommand { Title = "Test Project", Description = "Description" };
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<PostAddCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK });

            // Act
            var result = await _controller.Post(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var command = new PostUpdateCommand { Title = "Updated Project" };
            var projectId = Guid.NewGuid();
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<PostUpdateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK });

            // Act
            var result = await _controller.Put(command, projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }


        [Fact]
        public async Task Get_ShouldReturnOk_WhenProjectIsFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var query = new PostGetQuery { Id = projectId };
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<PostGetQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK, Data = new PostModel() });

            // Act
            var result = await _controller.Get(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new PostDeleteCommand { Id = projectId };
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<PostDeleteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK });

            // Act
            var result = await _controller.Delete(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }


        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var query = new PostListQuery { Page = 1, PageSize = 10 };
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<PostListQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK, Data = new List<PostModel>() });

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }
    }
}
