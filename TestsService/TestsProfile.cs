using AutoMapper;
using TestsService.Contracts;
using TestsService.Models;

namespace TestsService
{
    /// <summary>
    ///     Настройка маппера для дисциплин
    /// </summary>
    public class TestsProfile : Profile
    {
        /// <summary>
        ///     Создание экземпляра класса <see cref="TestsProfile"/>.
        /// </summary>
        public TestsProfile()
        {
            CreateMap<Test, TestDTO>()
                .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.IsApproved, o => o.MapFrom(x => x.IsApproved))
                .ForMember(x => x.Created, o => o.MapFrom(x => x.Created));

            CreateMap<Question, QuestionDTO>()
                .ForMember(x => x.Choices, o => o.MapFrom(x => x.Choices))
                .ForMember(x => x.HasMany, o => o.MapFrom(x => x.HasMany))
                .ForMember(x => x.IsRequired, o => o.MapFrom(x => x.IsRequired))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name));

            CreateMap<Test, TestDetailsDTO>()
                .IncludeBase<Test, TestDTO>()
                .ForMember(x =>  x.Author, o => o.MapFrom(x => x.Author))
                .ForMember(x => x.HasCreativePart, o => o.MapFrom(x => x.HasCreativePart))
                .ForMember(x => x.Questions, o => o.MapFrom(x => x.Questions));
        }
    }
}
