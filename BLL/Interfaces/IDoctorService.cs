using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO.Doctor;

namespace BLL.Interfaces
{
    public interface IDoctorService
    {
        Task<bool> CreateDoctorAsync(DoctorDTO dto);
        Task<bool> UpdateDoctorAsync(DoctorDTO dto);
        Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync();
    }
}
