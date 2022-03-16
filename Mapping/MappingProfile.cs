using AutoMapper;
using KiddieParadies.Controllers.Apis;
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
            CreateMap<Message, MessageDto>();


            CreateMap<YearDto, Year>();
            CreateMap<StudentRegistrationInfoDto, StudentRegistrationInfo>();
            CreateMap<EmployeeRegistrationInfoDto, EmployeeRegistrationInfo>();
            CreateMap<ClassRoomCourseSaveDto, CourseClassRoom>()
                .ForMember(c => c.Id, opt => opt.Ignore());
            CreateMap<MessageDto, Message>()
                .ForMember(i => i.Id, opt => opt.Ignore());

            CreateMap<BlogFormViewModel, Blog>();
            CreateMap<ParentFormViewModel, Parent>()
                .ForMember(p => p.FatherIdentityImageName, opt => opt.Ignore())
                .ForMember(p => p.MotherIdentityImageName, opt => opt.Ignore())
                .BeforeMap((vm, p) =>
                {
                    p.HomeLocation.Longitude = vm.Longitude;
                    p.HomeLocation.Latitude = vm.Latitude;
                    p.HomeLocation.Zoom = vm.Zoom;
                });
            CreateMap<StudentFormViewModel, Student>();
            CreateMap<EmployeeFormViewModel, Employee>()
                .ForMember(e => e.ImageName, opt => opt.Ignore())
                .ForMember(e => e.ResumeName, opt => opt.Ignore());

            CreateMap<Blog, BlogFormViewModel>();
            CreateMap<Parent, ParentFormViewModel>()
                .BeforeMap((p, vm) =>
                {
                    vm.Longitude = p.HomeLocation.Longitude;
                    vm.Latitude = p.HomeLocation.Latitude;
                    vm.Zoom = p.HomeLocation.Zoom;
                });
            CreateMap<Student, StudentFormViewModel>();
            CreateMap<Employee, EmployeeFormViewModel>();
        }
    }
}