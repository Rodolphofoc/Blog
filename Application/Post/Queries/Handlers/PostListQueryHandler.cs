using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Applications.Abstracts;
using Applications.Interfaces.Repository;
using Applications.Mappers.Interface;
using Applications.Project.Model;
using Domain;
using Domain.Domain;
using MediatR;

namespace Applications.Project.Queries.Handlers
{
    public class PostListQueryHandler : IRequestHandler<PostListQuery, Response>
    {
        private readonly IResponse _response;
        private readonly IPostRepository _repository;
        private readonly IPostMappers _taskMangerMappers;

        public PostListQueryHandler(IResponse response, IPostRepository metaRepository, IPostMappers taskManagerMappers)
        {
            _response = response;
            _repository = metaRepository;
            _taskMangerMappers = taskManagerMappers;
        }

        public async Task<Response> Handle(PostListQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var filter = await _repository.Filter(request.Name, request.Deleted, request.PageSize.Value, request.Page.Value);

                var listModel = _taskMangerMappers.Map(filter.entities);


                var paged = new Paged<PostModel>
                {
                    CurrentPage = request.Page.Value,
                    PageSize = request.PageSize.Value,
                    Records = listModel,
                    RecordsInPage = filter.entities.Count,
                    TotalRecords = filter.totalRecords,
                };


                return await _response.CreateSuccessResponseAsync(paged, string.Empty);
            }
            catch (Exception ex)
            {
                return await _response.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError);
            }
        }
    }
}

