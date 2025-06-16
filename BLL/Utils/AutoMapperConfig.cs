using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using BLL.DTO.Doctor;
using BLL.DTO.User;
using BLL.DTO.Article;
using BLL.DTO.Category;
using BLL.DTO.Patient;

namespace BLL.Utils
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<UserDTO, User>().ReverseMap(); // Includes IsVerified field mapping
            CreateMap<UserReadonlyDTO, User>().ReverseMap(); // Includes IsVerified field mapping
            
            CreateMap<DoctorDTO, Doctor>().ReverseMap();
            CreateMap<DoctorDTO, User>().ReverseMap();
            CreateMap<DoctorReadOnlyDTO, Doctor>().ReverseMap();
            CreateMap<DoctorReadOnlyDTO, User>().ReverseMap();
            CreateMap<DoctorUpdateDTO, Doctor>().ReverseMap();
            CreateMap<DoctorUpdateDTO, User>().ReverseMap();
            
            CreateMap<PatientDTO, Patient>().ReverseMap();
            CreateMap<PatientDTO, User>().ReverseMap();
            CreateMap<PatientReadOnlyDTO, Patient>().ReverseMap();
            CreateMap<PatientReadOnlyDTO, User>().ReverseMap();
            CreateMap<PatientUpdateDTO, Patient>().ReverseMap();
            CreateMap<PatientUpdateDTO, User>().ReverseMap();

            CreateMap<ArticleDTO, Article>().ReverseMap();
            CreateMap<ArticleReadOnlyDTO, Article>().ReverseMap()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null));
            CreateMap<ArticleReadOnlyDTO, Category>().ReverseMap();
            CreateMap<ArticleUpdateDTO, Article>().ReverseMap();
            
            CreateMap<CategoryDTO, Category>().ReverseMap();

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
