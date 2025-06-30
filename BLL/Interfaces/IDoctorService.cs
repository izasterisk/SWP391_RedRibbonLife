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
        Task<DoctorReadOnlyDTO> CreateDoctorAsync(DoctorCreateDTO dto);
        Task<DoctorReadOnlyDTO> UpdateDoctorAsync(DoctorUpdateDTO dto);
        Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync();
        Task<DoctorReadOnlyDTO> GetDoctorByDoctorIDAsync(int id);
        Task<bool> DeleteDoctorByDoctorIdAsync(int id);
    }
}
