using System.Net;
using Applications.Interfaces.Repository;
using Applications.Project.Commands;
using Applications.Project.Commands.Handlers;
using Domain;
using Domain.Domain;
using Moq;

namespace Application.Test.HandlersTests.Project
{
    public class ProjectDeleteCommandHandlerTests
    {
        private readonly Mock<IResponse> _mockResponse;
        private readonly Mock<IPostRepository> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ProjectDeleteCommandHandler _handler;

        public ProjectDeleteCommandHandlerTests()
        {
            _mockResponse = new Mock<IResponse>();
            _mockRepository = new Mock<IPostRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _handler = new ProjectDeleteCommandHandler(
                _mockResponse.Object,
                _mockRepository.Object,
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            var command = new PostDeleteCommand { Id = Guid.NewGuid() };

            _mockRepository
                .Setup(r => r.GetByIdAsync(command.Id))
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
            _mockRepository.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
            _mockResponse.Verify(r => r.CreateErrorResponseAsync(null, HttpStatusCode.NotFound), Times.Once);
        }



        [Fact]
        public async Task Handle_ShouldDeleteProject_WhenNoOpenTasksExist()
        {
            // Arrange
            var command = new PostDeleteCommand { Id = Guid.NewGuid() };

            var project = new PostEntity
            {
                Id = command.Id,
                Deleted = false,
                Tasks = new List<TaskEntity>()
            };

            _mockRepository
                .Setup(r => r.GetByIdAsync(command.Id))
                .ReturnsAsync(project);

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
            Assert.True(project.Deleted);

            _mockRepository.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(CancellationToken.None), Times.Once);
            _mockResponse.Verify(r => r.CreateSuccessResponseAsync(null, string.Empty), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            var command = new PostDeleteCommand { Id = Guid.NewGuid() };

            _mockRepository
                .Setup(r => r.GetByIdAsync(command.Id))
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

            _mockRepository.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
            _mockResponse.Verify(r => r.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError), Times.Once);
        }
    }
}
