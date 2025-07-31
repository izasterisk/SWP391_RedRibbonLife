using AutoMapper;
using BLL.DTO.Doctor;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IMapper _mapper;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserUtils _userUtils;

        public DoctorService(IMapper mapper, IDoctorRepository doctorRepository, IUserUtils userUtils)
        {
            _mapper = mapper;
            _doctorRepository = doctorRepository;
            _userUtils = userUtils;
        }

        public async Task<DoctorReadOnlyDTO> CreateDoctorAsync(DoctorCreateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            
            if (string.IsNullOrEmpty(dto.Username))
            {
                throw new ArgumentException("Username cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(dto.Email))
            {
                throw new ArgumentException("Email cannot be null or empty.");
            }
            
            var usernameExists = await _doctorRepository.CheckUsernameExistsAsync(dto.Username);
            if (usernameExists)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            await _userUtils.CheckEmailExistAsync(dto.Email);
            
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Doctor";
            user.IsVerified = false;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            
            Doctor doctor = new Doctor
            {
                // DoctorImage = dto.DoctorImage,
                Bio = dto.Bio
            };
            
            var createdDoctor = await _doctorRepository.CreateDoctorWithTransactionAsync(user, doctor);
            var detailedDoctor = await _doctorRepository.GetDoctorWithUserAsync(createdDoctor.DoctorId, true);
            return _mapper.Map<DoctorReadOnlyDTO>(detailedDoctor);
        }
        
        public async Task<DoctorReadOnlyDTO> UpdateDoctorAsync(DoctorUpdateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            
            var doctor = await _doctorRepository.GetDoctorForUpdateAsync(dto.DoctorId);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            var user = await _doctorRepository.GetUserByUserIdAsync(doctor.UserId, false);
            if (user == null)
            {
                throw new Exception($"User associated with doctor ID {dto.DoctorId} not found.");
            }
            _mapper.Map(dto, user);
            _mapper.Map(dto, doctor);
            
            var updatedDoctor = await _doctorRepository.UpdateDoctorWithTransactionAsync(user, doctor);
            var detailedDoctor = await _doctorRepository.GetDoctorWithUserAsync(updatedDoctor.DoctorId, true);
            return _mapper.Map<DoctorReadOnlyDTO>(detailedDoctor);
        }
        
        public async Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsWithUserAsync();
            return _mapper.Map<List<DoctorReadOnlyDTO>>(doctors);
        }
        
        public async Task<DoctorReadOnlyDTO> GetDoctorByDoctorIDAsync(int id)
        {
            var doctor = await _doctorRepository.GetDoctorWithUserAsync(id, true);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            return _mapper.Map<DoctorReadOnlyDTO>(doctor);
        }
        
        public async Task<bool> DeleteDoctorByDoctorIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetDoctorForUpdateAsync(id);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            var user = await _doctorRepository.GetUserByUserIdAsync(doctor.UserId, false);
            if (user == null)
            {
                throw new Exception($"User associated with Doctor ID {id} not found.");
            }
            return await _doctorRepository.DeleteDoctorWithTransactionAsync(doctor, user);
        }
    }
}
