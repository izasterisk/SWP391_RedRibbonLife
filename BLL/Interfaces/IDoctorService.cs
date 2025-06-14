using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO.Doctor;

namespace BLL.Interfaces
{
    public interface IDoctorService
    {
        Task<bool> CreateDoctorAsync(DoctorDTO dto);
        Task<bool> UpdateDoctorAsync(DoctorUpdateDTO dto);
        Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync();
        Task<DoctorReadOnlyDTO> GetDoctorByDoctorIDAsync(int id);
    }
}
