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
using BLL.DTO.Manager;
using BLL.DTO.Staff;
using BLL.DTO.Appointment;
using BLL.DTO.Doctor;
using BLL.DTO.User;
using BLL.DTO.Article;
using BLL.DTO.ARVComponent;
using BLL.DTO.ARVRegimens;
using BLL.DTO.Category;
using BLL.DTO.DoctorSchedule;
using BLL.DTO.Login;
using BLL.DTO.Notification;
using BLL.DTO.Patient;
using BLL.DTO.TestResult;
using BLL.DTO.TestType;
using BLL.DTO.Treatment;

namespace BLL.Utils
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<LoginDTO, User>().ReverseMap();
            
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<UserReadonlyDTO, User>().ReverseMap();
            
            CreateMap<DoctorCreateDTO, Doctor>().ReverseMap();
            CreateMap<DoctorCreateDTO, User>().ReverseMap();
            CreateMap<Doctor, DoctorReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User != null ? src.User.DateOfBirth : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User != null ? src.User.Gender : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User != null ? src.User.Address : null))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User != null ? src.User.UserRole : null))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.DoctorImage, opt => opt.MapFrom(src => src.DoctorImage))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));
            CreateMap<DoctorReadOnlyDTO, User>().ReverseMap();
            CreateMap<DoctorUpdateDTO, Doctor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DoctorUpdateDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<PatientCreateDTO, Patient>().ReverseMap();
            CreateMap<PatientCreateDTO, User>().ReverseMap();
            CreateMap<Patient, PatientReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User != null ? src.User.DateOfBirth : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User != null ? src.User.Gender : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User != null ? src.User.Address : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User != null ? src.User.IsActive : false))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.User != null ? src.User.IsVerified : false))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User != null ? src.User.UserRole : null))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType))
                .ForMember(dest => dest.IsPregnant, opt => opt.MapFrom(src => src.IsPregnant))
                .ForMember(dest => dest.SpecialNotes, opt => opt.MapFrom(src => src.SpecialNotes));
            CreateMap<PatientReadOnlyDTO, User>().ReverseMap();
            CreateMap<PatientUpdateDTO, Patient>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PatientUpdateDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ArticleDTO, Article>().ReverseMap();
            CreateMap<ArticleReadOnlyDTO, Article>().ReverseMap()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null));
            CreateMap<ArticleReadOnlyDTO, Category>().ReverseMap();
            CreateMap<ArticleUpdateDTO, Article>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<CategoryDTO, Category>().ReverseMap();
            
            CreateMap<AdminDTO, User>().ReverseMap();
            CreateMap<AdminReadOnlyDTO, User>().ReverseMap();
            CreateMap<AdminUpdateDTO, User>()//.ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<ManagerDTO, User>().ReverseMap();
            CreateMap<ManagerReadOnlyDTO, User>().ReverseMap();
            CreateMap<ManagerUpdateDTO, User>()//.ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<StaffDTO, User>().ReverseMap();
            CreateMap<StaffReadOnlyDTO, User>().ReverseMap();
            CreateMap<StaffUpdateDTO, User>()//.ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<DoctorSchedule, DoctorScheduleDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.User != null ? src.Doctor.User.FullName : null));
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
            
            CreateMap<ARVRegimensCreateDTO, Arvregimen>().ReverseMap();
            CreateMap<ARVRegimensReadOnlyDTO, Arvregimen>().ReverseMap();
            CreateMap<ARVRegimensUpdateDTO, Arvregimen>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<TestTypeDTO, TestType>().ReverseMap();
            CreateMap<TestTypeCreateDTO, TestType>().ReverseMap();
            CreateMap<TestTypeUpdateDTO, TestType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<TestResult, TestResultDTO>()
                .ForMember(dest => dest.TestTypeName, opt => opt.MapFrom(src => src.TestType != null ? src.TestType.TestTypeName : null))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.TestType != null ? src.TestType.Unit : null))
                .ForMember(dest => dest.NormalRange, opt => opt.MapFrom(src => src.TestType != null ? src.TestType.NormalRange : null))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName : null))
                .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.BloodType : null))
                .ForMember(dest => dest.IsPregnant, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.IsPregnant : null))
                .ForMember(dest => dest.SpecialNotes, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.SpecialNotes : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.User != null ? src.Doctor.User.FullName : null))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentDate : default(DateOnly)))
                .ForMember(dest => dest.AppointmentTime, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentTime : default(TimeOnly)))
                .ForMember(dest => dest.AppointmentType, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentType : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.Status : null))
                .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.IsAnonymous : null));
            CreateMap<TestResultCreateDTO, TestResult>().ReverseMap();
            CreateMap<TestResultUpdateDTO, TestResult>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<ARVComponentDTO, Arvcomponent>().ReverseMap();
            CreateMap<ARVComponentCreateDTO, Arvcomponent>().ReverseMap();
            CreateMap<ARVComponentUpdateDTO, Arvcomponent>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<Treatment, TreatmentDTO>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.TestResult != null ? src.TestResult.PatientId : (int?)null))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Patient != null && src.TestResult.Patient.User != null ? src.TestResult.Patient.User.FullName : null))
                .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Patient != null ? src.TestResult.Patient.BloodType : null))
                .ForMember(dest => dest.IsPregnant, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Patient != null ? src.TestResult.Patient.IsPregnant : null))
                .ForMember(dest => dest.SpecialNotes, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Patient != null ? src.TestResult.Patient.SpecialNotes : null))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.TestResult != null ? src.TestResult.DoctorId : (int?)null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Doctor != null && src.TestResult.Doctor.User != null ? src.TestResult.Doctor.User.FullName : null))
                .ForMember(dest => dest.DoctorImage, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Doctor != null ? src.TestResult.Doctor.DoctorImage : null))
                .ForMember(dest => dest.RegimenName, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.RegimenName : null))
                .ForMember(dest => dest.Component1Id, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.Component1Id : (int?)null))
                .ForMember(dest => dest.Component2Id, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.Component2Id : null))
                .ForMember(dest => dest.Component3Id, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.Component3Id : null))
                .ForMember(dest => dest.Component4Id, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.Component4Id : null))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.Description : null))
                .ForMember(dest => dest.SuitableFor, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.SuitableFor : null))
                .ForMember(dest => dest.SideEffects, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.SideEffects : null))
                .ForMember(dest => dest.UsageInstructions, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.UsageInstructions : null))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Regimen != null ? src.Regimen.Frequency : (int?)null))
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Appointment != null ? src.TestResult.Appointment.AppointmentId : (int?)null))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Appointment != null ? src.TestResult.Appointment.AppointmentDate : (DateOnly?)null))
                .ForMember(dest => dest.AppointmentTime, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Appointment != null ? src.TestResult.Appointment.AppointmentTime : (TimeOnly?)null))
                .ForMember(dest => dest.AppointmentType, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Appointment != null ? src.TestResult.Appointment.AppointmentType : null))
                .ForMember(dest => dest.AppointmentStatus, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Appointment != null ? src.TestResult.Appointment.Status : null))
                .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.TestResult != null && src.TestResult.Appointment != null ? src.TestResult.Appointment.IsAnonymous : null))
                .ForMember(dest => dest.TestResultNotes, opt => opt.MapFrom(src => src.TestResult != null ? src.TestResult.Notes : null));
            CreateMap<TreatmentCreateDTO, Treatment>().ReverseMap();
            CreateMap<TreatmentUpdateDTO, Treatment>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<NotificationCreateDTO, Notification>().ReverseMap();
            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<NotificationUpdateDTO, Notification>().ReverseMap();
            
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
