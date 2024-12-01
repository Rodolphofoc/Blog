using Applications.Mappers.Interface;
using Applications.Post.Commands;
using Applications.Project.Model;
using AutoMapper;
using Domain.Domain;

namespace Applications.Mappers
{
    public class PostMappers : IPostMappers
    {
        private readonly IMapper _mapper;

        public PostMappers()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PostAddCommand, PostEntity>()
                .ForMember(x => x.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.Description)).ReverseMap();

                cfg.CreateMap<PostUpdateCommand, PostEntity>()
                .ForMember(x => x.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(x => x.Deleted, opt => opt.MapFrom(src => src.Deleted))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.Description)).ReverseMap();

                cfg.CreateMap<PostEntity, PostModel>()
                  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name)) // Mapeia o nome do usuário
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                  .ForMember(dest => dest.Deleted, opt => opt.MapFrom(src => src.Deleted))
                  .ReverseMap();



            });

            _mapper = config.CreateMapper();
        }

        public PostEntity Map(PostAddCommand command)
        {
            return _mapper.Map<PostAddCommand, PostEntity>(command);
        }

        public PostEntity Map(PostUpdateCommand command)
        {
            return _mapper.Map<PostUpdateCommand, PostEntity>(command);
        }

        public List<PostModel> Map(List<PostEntity> command)
        {
            return _mapper.Map<List<PostEntity>, List<PostModel>>(command);
        }
    }
}
