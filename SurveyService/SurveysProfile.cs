using AutoMapper;
using SurveyService.Contracts;
using SurveyService.Models.Subordinates;

namespace SurveyService
{
    /// <summary>
    ///     Настройка маппера для анкет
    /// </summary>
    public class SurveysProfile : Profile
    {
        /// <summary>
        ///     Создание экземпляра класса <see cref="SurveysProfile"/>.
        /// </summary>
        public SurveysProfile()
        {
            CreateMap<Discipline, DisciplineDTO>()
                .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name));

            CreateMap<Test, TestDTO>()
                .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.QuestionsCount, o => o.MapFrom(x => x.QuestionsCount));
        }
    }
}
