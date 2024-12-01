using System.Net;
using Applications.Interfaces.Repository;
using Applications.Mappers.Interface;
using Applications.Project.Commands;
using Applications.Project.Commands.Handlers;
using Domain;
using Domain.Domain;
using Moq;

namespace Application.Test.HandlersTests.Project
{
    public class ProjectUpdateCommandHandlerTests
    {
        private readonly Mock<IResponse> _mockResponse;
        private readonly Mock<IPostRepository> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPostMappers> _mockTaskManagerMappers;
        private readonly ProjectUpdateCommandHandler _handler;

        public ProjectUpdateCommandHandlerTests()
        {
            _mockResponse = new Mock<IResponse>();
            _mockRepository = new Mock<IPostRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTaskManagerMappers = new Mock<IPostMappers>();

            _handler = new ProjectUpdateCommandHandler(
                _mockResponse.Object,
                _mockRepository.Object,
                _mockTaskManagerMappers.Object,
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            var command = new PostUpdateCommand
            {
                Title = "Updated Name",
                Description = "Updated Description",
                Deleted = false
            };

            command.SetId(Guid.NewGuid());

            _mockRepository
                .Setup(r => r.FindByIdAsNoTrackingAsync(command.GetId()))
                .ReturnsAsync((PostEntity?)null);

            var expectedResponse = new Response
            {
                StatusCode = HttpStatusCode.NotFound
            };

            _mockResponse
                .Setup(r => r.CreateErrorResponseAsync(null, HttpStatusCode.NotFound))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            _mockRepository.Verify(r => r.FindByIdAsNoTrackingAsync(command.GetId()), Times.Once);
            _mockResponse.Verify(r => r.CreateErrorResponseAsync(null, HttpStatusCode.NotFound), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProjectSuccessfully()
        {
            // Arrange
            var command = new PostUpdateCommand
            {
                Title = "Updated Name",
                Description = "Updated Description",
                Deleted = false
            };

            command.SetId(Guid.NewGuid());


            var existingProject = new PostEntity
            {
                Id = command.GetId(),
                Title = "Old Name",
                Description = "Old Description",
                Deleted = false
            };

            _mockRepository
                .Setup(r => r.FindByIdAsNoTrackingAsync(command.GetId()))
                .ReturnsAsync(existingProject);

            var expectedResponse = new Response
            {
                StatusCode = HttpStatusCode.OK,
                Message = string.Empty
            };

            _mockResponse
                .Setup(r => r.CreateSuccessResponseAsync(null, string.Empty))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(string.Empty, result.Message);

            _mockRepository.Verify(r => r.FindByIdAsNoTrackingAsync(command.GetId()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<PostEntity>(e =>
                e.Id == command.GetId() &&
                e.Title == command.Title &&
                e.Description == command.Description &&
                e.Deleted == command.Deleted
            )), Times.Once);

            _mockResponse.Verify(r => r.CreateSuccessResponseAsync(null, string.Empty), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            var command = new PostUpdateCommand
            {
                Title = "Updated Name",
                Description = "Updated Description",
                Deleted = false
            };

            command.SetId(Guid.NewGuid());


            _mockRepository
                .Setup(r => r.FindByIdAsNoTrackingAsync(command.GetId()))
                .ThrowsAsync(new Exception());

            var expectedResponse = new Response
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockResponse
                .Setup(r => r.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            _mockRepository.Verify(r => r.FindByIdAsNoTrackingAsync(command.GetId()), Times.Once);
            _mockResponse.Verify(r => r.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError), Times.Once);
        }
    }

}
