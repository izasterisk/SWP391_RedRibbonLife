using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using BLL.DTO.Admin;
using BLL.DTO.Appointment;
using BLL.DTO.Doctor;
using BLL.DTO.User;
using BLL.DTO.Article;
using BLL.DTO.ARVRegimens;
using BLL.DTO.Category;
using BLL.DTO.DoctorSchedule;
using BLL.DTO.Login;
using BLL.DTO.Patient;
using BLL.DTO.TestResult;
using BLL.DTO.TestType;

namespace BLL.Utils
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<LoginDTO, User>().ReverseMap();
            CreateMap<LoginResponseDTO, User>().ReverseMap();
            
            CreateMap<UserDTO, User>().ReverseMap(); // Includes IsVerified field mapping
            CreateMap<UserReadonlyDTO, User>().ReverseMap(); // Includes IsVerified field mapping
            
            CreateMap<DoctorCreateDTO, Doctor>().ReverseMap();
            CreateMap<DoctorCreateDTO, User>().ReverseMap();
            CreateMap<DoctorReadOnlyDTO, Doctor>().ReverseMap();
            CreateMap<DoctorReadOnlyDTO, User>().ReverseMap();
            CreateMap<DoctorUpdateDTO, Doctor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DoctorUpdateDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<PatientCreateDTO, Patient>().ReverseMap();
            CreateMap<PatientCreateDTO, User>().ReverseMap();
            CreateMap<PatientReadOnlyDTO, Patient>().ReverseMap();
            CreateMap<PatientReadOnlyDTO, User>().ReverseMap();
            CreateMap<PatientUpdateDTO, Patient>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PatientUpdateDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ArticleDTO, Article>().ReverseMap();
            CreateMap<ArticleReadOnlyDTO, Article>().ReverseMap()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null));
            CreateMap<ArticleReadOnlyDTO, Category>().ReverseMap();
            CreateMap<ArticleUpdateDTO, Article>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<CategoryDTO, Category>().ReverseMap();
            
            CreateMap<AdminDTO, User>().ReverseMap();
            CreateMap<AdminReadOnlyDTO, User>().ReverseMap();
            CreateMap<AdminUpdateDTO, User>()//.ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<DoctorScheduleDTO, DoctorSchedule>().ReverseMap();
            CreateMap<DoctorScheduleCreateDTO, DoctorSchedule>().ReverseMap();
            CreateMap<DoctorScheduleUpdateDTO, DoctorSchedule>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<AppointmentCreateDTO, Appointment>().ReverseMap();
            CreateMap<AppointmentUpdateDTO, Appointment>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Condition(src => src.DoctorId.HasValue))
                .ForMember(dest => dest.AppointmentDate, opt => opt.Condition(src => src.AppointmentDate.HasValue))
                .ForMember(dest => dest.AppointmentTime, opt => opt.Condition(src => src.AppointmentTime.HasValue))
                .ForMember(dest => dest.AppointmentType, opt => opt.Condition(src => !string.IsNullOrEmpty(src.AppointmentType)))
                .ForMember(dest => dest.Status, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Status)))
                .ForMember(dest => dest.IsAnonymous, opt => opt.Ignore());
            CreateMap<AppointmentReadOnlyDTO, Appointment>().ReverseMap()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.FullName))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FullName));
            
            CreateMap<ARVRegimensDTO, Arvregimen>().ReverseMap();
            CreateMap<ARVRegimensCreateDTO, Arvregimen>().ReverseMap();
            CreateMap<ARVRegimensReadOnlyDTO, Arvregimen>().ReverseMap();
            CreateMap<ARVRegimensUpdateDTO, Arvregimen>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<TestTypeDTO, TestType>().ReverseMap();
            CreateMap<TestTypeCreateDTO, TestType>().ReverseMap();
            CreateMap<TestTypeUpdateDTO, TestType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<TestResultDTO, TestResult>().ReverseMap();
            CreateMap<TestResultDTO, TestType>().ReverseMap();
            CreateMap<TestResultDTO, User>().ReverseMap();
            CreateMap<TestResultDTO, Patient>().ReverseMap();
            CreateMap<TestResultDTO, Appointment>().ReverseMap();
            CreateMap<TestResultCreateDTO, TestResult>().ReverseMap();
            CreateMap<TestResultUpdateDTO, TestResult>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //Khi có 2 trường khác tên, ví dụ: studentName và Name
            //CreateMap<StudentDTO, Student>().ForMember(n => n.studentName, opt => opt.MapFrom(x => x.Name)).ReverseMap();

            //Khi muốn map tất cả ngoại trừ studentName
            //CreateMap<StudentDTO, Student>().ReverseMap().ForMember(n => n.studentName, opt => opt.Ignore());

            //Khi giá trị bị null
            //CreateMap<StudentDTO, Student>().ReverseMap()
            //.ForMember(n => n.Address, opt => opt.MapFrom(n => string.IsNullOrEmpty(n.Address) ? "No value found" : n.Address));
        }
    }
}
