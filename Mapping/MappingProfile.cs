using AutoMapper;
using KiddieParadies.Controllers.Apis.Dtos;
using KiddieParadies.Core.Models;
using KiddieParadies.ViewModels;

namespace KiddieParadies.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Year, YearDto>();
            CreateMap<StudentRegistrationInfo, StudentRegistrationInfoDto>()
                .BeforeMap((s, sd) =>
                {
                    sd.YearName = s.Year.Name;
                });
            CreateMap<EmployeeRegistrationInfo, EmployeeRegistrationInfoDto>()
                .BeforeMap((e, ed) =>
                {
                    ed.YearName = e.Year.Name;
                });

            CreateMap<YearDto, Year>();
            CreateMap<StudentRegistrationInfoDto, StudentRegistrationInfo>();
            CreateMap<EmployeeRegistrationInfoDto, EmployeeRegistrationInfo>();

            CreateMap<BlogFormViewModel, Blog>();
            CreateMap<ParentFormViewModel, Parent>();
            CreateMap<StudentFormViewModel, Student>();

            CreateMap<Blog, BlogFormViewModel>();
            CreateMap<Parent, ParentFormViewModel>();
            CreateMap<Student, StudentFormViewModel>();
        }
    }
}