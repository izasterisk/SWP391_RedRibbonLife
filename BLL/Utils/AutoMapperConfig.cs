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

namespace BLL.Utils
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<UserDTO, User>().ReverseMap(); // Includes IsVerified field mapping
            CreateMap<User, UserReadonlyDTO>().ReverseMap(); // Includes IsVerified field mapping
            CreateMap<Doctor, DoctorDTO>().ReverseMap().ForMember(n => n.UserId, opt => opt.Ignore());
            CreateMap<User, DoctorDTO>().ReverseMap();
            CreateMap<User, DoctorReadOnlyDTO>().ReverseMap();
            CreateMap<Doctor, DoctorReadOnlyDTO>().ReverseMap().ForMember(n => n.UserId, opt => opt.Ignore());
            CreateMap<Doctor, DoctorUpdateDTO>().ReverseMap();
            CreateMap<Article, ArticleDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();

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
