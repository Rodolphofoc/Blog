using System.Net;
using Applications.Interfaces.Repository;
using Applications.Project.Queries;
using Applications.Project.Queries.Handlers;
using Domain;
using Domain.Domain;
using Moq;

namespace Application.Test.HandlersTests.Project
{
    public class ProjectGetQueryHandlerTests
    {
        private readonly Mock<IResponse> _responseMock;
        private readonly Mock<IPostRepository> _projectRepositoryMock;
        private readonly ProjectGetQueryHandler _handler;

        public ProjectGetQueryHandlerTests()
        {
            _responseMock = new Mock<IResponse>();
            _projectRepositoryMock = new Mock<IPostRepository>();
            _handler = new ProjectGetQueryHandler(
                _responseMock.Object,
                _projectRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_GivenValidProjectId_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new PostGetQuery { Id = Guid.NewGuid() };

            var projectEntity = new PostEntity
            {
                Id = command.Id,
                Title = "Test Project",
                Description = "Test Description",
                Deleted = false
            };

            _projectRepositoryMock.Setup(r => r.FindById(command.Id)).Returns(projectEntity);
            _responseMock.Setup(r => r.CreateSuccessResponseAsync(projectEntity, string.Empty))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            _projectRepositoryMock.Verify(r => r.FindById(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenDeletedProject_ShouldReturnNotFoundResponse()
        {
            // Arrange
            var command = new PostGetQuery { Id = Guid.NewGuid() };

            var projectEntity = new PostEntity
            {
                Id = command.Id,
                Title = "Deleted Project",
                Description = "Test Description",
                Deleted = true
            };

            _projectRepositoryMock.Setup(r => r.FindById(command.Id)).Returns(projectEntity);
            _responseMock.Setup(r => r.CreateErrorResponseAsync(null, HttpStatusCode.NotFound))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.NotFound });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            _projectRepositoryMock.Verify(r => r.FindById(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenNonExistentProject_ShouldReturnNotFoundResponse()
        {
            // Arrange
            var command = new PostGetQuery { Id = Guid.NewGuid() };

            _projectRepositoryMock.Setup(r => r.FindById(command.Id)).Returns((PostEntity)null);
            _responseMock.Setup(r => r.CreateErrorResponseAsync(null, HttpStatusCode.NotFound))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.NotFound });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            _projectRepositoryMock.Verify(r => r.FindById(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new PostGetQuery { Id = Guid.NewGuid() };

            _projectRepositoryMock.Setup(r => r.FindById(command.Id)).Throws(new Exception("Database error"));
            _responseMock.Setup(r => r.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.InternalServerError });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            _projectRepositoryMock.Verify(r => r.FindById(command.Id), Times.Once);
        }
    }
}
