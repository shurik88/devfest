using AutoMapper;
using DisciplinesService.Contracts;
using DisciplinesService.Models;

namespace DisciplinesService
{
    /// <summary>
    ///     Настройка маппера для дисциплин
    /// </summary>
    public class DisciplinesProfile : Profile
    {
        /// <summary>
        ///     Создание экземпляра класса <see cref="DisciplinesProfile"/>.
        /// </summary>
        public DisciplinesProfile()
        {
            CreateMap<Discipline, DisciplineDTO>()
                .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.Code, o => o.MapFrom(x => x.Code));

            CreateMap<Discipline, DisciplineDetailsDTO>()
                .IncludeBase<Discipline, DisciplineDTO>()
                .ForMember(x => x.Annotion, o => o.MapFrom(x => x.Annotion));
        }
    }
}
