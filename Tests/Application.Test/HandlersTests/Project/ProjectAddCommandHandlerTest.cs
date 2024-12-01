using System.Net;
using Applications.Interfaces.Repository;
using Applications.Mappers.Interface;
using Domain;
using Domain.Domain;
using Moq;

namespace Application.Test.HandlersTests.Project
{
    public class PostAddCommandHandlerTests
    {
        private readonly Mock<IResponse> _mockResponse;
        private readonly Mock<IPostRepository> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPostMappers> _mockTaskManagerMappers;
        private readonly ProjectAddCommandHandler _handler;

        public PostAddCommandHandlerTests()
        {
            _mockResponse = new Mock<IResponse>();
            _mockRepository = new Mock<IPostRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTaskManagerMappers = new Mock<IPostMappers>();

            _handler = new ProjectAddCommandHandler(
                _mockResponse.Object,
                _mockRepository.Object,
                _mockUnitOfWork.Object,
                _mockTaskManagerMappers.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenNameOrDescriptionIsEmpty()
        {
            // Arrange
            var command = new PostAddCommand
            {
                Title = null, // Missing Name
                Description = "Some description"
            };

            var expectedResponse = new Response
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Name or description be empty"
            };

            _mockResponse
                .Setup(r => r.CreateErrorResponseAsync("Name or description be empty", HttpStatusCode.BadRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Name or description be empty", result.Message);
            _mockResponse.Verify(r => r.CreateErrorResponseAsync("Name or description be empty", HttpStatusCode.BadRequest), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAddEntityAndReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var command = new PostAddCommand
            {
                Title = "Project Name",
                Description = "Project Description",
                User = "TestUser"
            };

            var mappedEntity = new PostEntity
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = command.User,
            };

            var expectedResponse = new Response
            {
                StatusCode = HttpStatusCode.OK,
                Message = string.Empty
            };

            _mockTaskManagerMappers
                .Setup(m => m.Map(command))
                .Returns(mappedEntity);

            _mockResponse
                .Setup(r => r.CreateSuccessResponseAsync(null, string.Empty))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(string.Empty, result.Message);

            _mockTaskManagerMappers.Verify(m => m.Map(command), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<PostEntity>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(CancellationToken.None), Times.Once);
            _mockResponse.Verify(r => r.CreateSuccessResponseAsync(null, string.Empty), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            var command = new PostAddCommand
            {
                Title = "Project Name",
                Description = "Project Description",
                User = "TestUser"
            };

            var expectedResponse = new Response
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockTaskManagerMappers
                .Setup(m => m.Map(command))
                .Throws(new Exception());

            _mockResponse
                .Setup(r => r.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            _mockResponse.Verify(r => r.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError), Times.Once);
        }
    }
}
