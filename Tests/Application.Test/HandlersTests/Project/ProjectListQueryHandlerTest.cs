using System.Net;
using System.Xml.Linq;
using Applications.Abstracts;
using Applications.Interfaces.Repository;
using Applications.Mappers.Interface;
using Applications.Project.Model;
using Applications.Project.Queries;
using Applications.Project.Queries.Handlers;
using Domain;
using Domain.Domain;
using Moq;

namespace Application.Test.HandlersTests.Project
{
    public class ProjectListQueryHandlerTests
    {
        private readonly Mock<IResponse> _mockResponse;
        private readonly Mock<IPostRepository> _mockRepository;
        private readonly Mock<IPostMappers> _mockTaskManagerMappers;

        public ProjectListQueryHandlerTests()
        {
            _mockResponse = new Mock<IResponse>();
            _mockRepository = new Mock<IPostRepository>();
            _mockTaskManagerMappers = new Mock<IPostMappers>();
        }

        [Fact]
        public async Task Handle_WhenProjectsFound_ReturnsPagedListWithProjects()
        {
            // Arrange
            var handler = new ProjectListQueryHandler(
                _mockResponse.Object,
                _mockRepository.Object,
                _mockTaskManagerMappers.Object
            );

            var query = new PostListQuery
            {
                Name = "Project1",
                Deleted = false,
                PageSize = 10,
                Page = 1
            };

            var entities = new List<PostEntity>
            {
                new PostEntity { Id = Guid.NewGuid(), Title = "Project1", Deleted = false },
                new PostEntity { Id = Guid.NewGuid(), Title = "Project2", Deleted = false }
            };

    
            // Mocking repository call
            _mockRepository.Setup(r => r.Filter(query.Name, query.Deleted, query.PageSize.Value, query.Page.Value))
                .ReturnsAsync((entities : entities, totalPage: 1, totalRecords: 10));

            // Mocking the mapper call
            _mockTaskManagerMappers.Setup(m => m.Map(It.IsAny<List<PostEntity>>()))
                .Returns(new List<PostModel>()
                {
                   new PostModel(){Id = Guid.NewGuid(), Name = "Project1", Deleted = false },
                    new PostModel(){Id = Guid.NewGuid(), Name = "Project2", Deleted = false}
                });

            // Mocking the response
            _mockResponse.Setup(r => r.CreateSuccessResponseAsync(It.IsAny<object>(), string.Empty))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK });

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Handle_WhenNoProjectsFound_ReturnsEmptyPagedList()
        {
            // Arrange
            var handler = new ProjectListQueryHandler(
                _mockResponse.Object,
                _mockRepository.Object,
                _mockTaskManagerMappers.Object
            );

            var query = new PostListQuery
            {
                Name = "NonExistingProject",
                Deleted = false,
                PageSize = 10,
                Page = 1
            };

            var entities = new List<PostEntity>
            {
                new PostEntity { Id = Guid.NewGuid(), Title = "Project1", Deleted = false },
                new PostEntity { Id = Guid.NewGuid(), Title = "Project2", Deleted = false }
            };

            // Mocking repository call
            _mockRepository.Setup(r => r.Filter(query.Name, query.Deleted, query.PageSize.Value, query.Page.Value))
                .ReturnsAsync((entities: entities, totalPage: 1, totalRecords: 10));

            // Mocking the mapper call
            _mockTaskManagerMappers.Setup(m => m.Map(It.IsAny<List<PostEntity>>()))
                .Returns(new List<PostModel>());

            // Mocking the response
            _mockResponse.Setup(r => r.CreateSuccessResponseAsync(It.IsAny<object>(), string.Empty))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.OK });

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Handle_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var handler = new ProjectListQueryHandler(
                _mockResponse.Object,
                _mockRepository.Object,
                _mockTaskManagerMappers.Object
            );

            var query = new PostListQuery
            {
                Name = "Project1",
                Deleted = false,
                PageSize = 10,
                Page = 1
            };

            // Simulating an exception in the repository
            _mockRepository.Setup(r => r.Filter(query.Name, query.Deleted, query.PageSize.Value, query.Page.Value))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Mocking the response
            _mockResponse.Setup(r => r.CreateErrorResponseAsync(It.IsAny<object>(), HttpStatusCode.InternalServerError))
                .ReturnsAsync(new Response { StatusCode = HttpStatusCode.InternalServerError });

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}
