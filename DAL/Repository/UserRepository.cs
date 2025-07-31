using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<TestType> _testTypeRepository;
        private readonly IRepository<TestResult> _testResultRepository;
        private readonly IRepository<Treatment> _treatmentRepository;
        private readonly IRepository<Category> _categoryRepository;

        public UserRepository(SWP391_RedRibbonLifeContext dbContext,
            IRepository<Doctor> doctorRepository,
            IRepository<Patient> patientRepository,
            IRepository<Appointment> appointmentRepository,
            IRepository<TestType> testTypeRepository,
            IRepository<TestResult> testResultRepository,
            IRepository<Treatment> treatmentRepository,
            IRepository<Category> categoryRepository) : base(dbContext)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _testTypeRepository = testTypeRepository;
            _testResultRepository = testResultRepository;
            _treatmentRepository = treatmentRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task CheckDoctorExistAsync(int doctorId)
        {
            var doctorExists = await _doctorRepository.AnyAsync(u => u.DoctorId == doctorId);
            if (!doctorExists)
            {
                throw new Exception("Doctor not found.");
            }
        }

        public async Task CheckPatientExistAsync(int patientId)
        {
            var patient = await _patientRepository.GetWithRelationsAsync(
                u => u.PatientId == patientId, 
                useNoTracking: true,
                includeFunc: q => q.Include(p => p.User));
            
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            if (patient.User.IsActive == false)
            {
                throw new Exception("This account has been deactivated.");
            }
            if (patient.User.IsVerified == false)
            {
                throw new Exception("This account has not been verified.");
            }
        }

        public async Task CheckUserExistAsync(int userId)
        {
            var userExists = await AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                throw new Exception("User not found.");
            }
        }

        public async Task CheckAppointmentExistAsync(int appointmentId)
        {
            var appointmentExists = await _appointmentRepository.AnyAsync(a => a.AppointmentId == appointmentId);
            if (!appointmentExists)
            {
                throw new Exception("Appointment not found.");
            }
        }

        public async Task CheckDuplicateAppointmentAsync(int appointmentId)
        {
            var duplicateAppointmentExists = await _testResultRepository.AnyAsync(a => a.AppointmentId == appointmentId);
            if (duplicateAppointmentExists)
            {
                throw new Exception("1 appointment can only have 1 test result.");
            }
        }

        public async Task CheckTestTypeExistAsync(int testTypeId)
        {
            var testTypeExists = await _testTypeRepository.AnyAsync(t => t.TestTypeId == testTypeId);
            if (!testTypeExists)
            {
                throw new Exception("Test type not found.");
            }
        }

        public async Task CheckTestResultExistAsync(int id)
        {
            var testResultExists = await _testResultRepository.AnyAsync(t => t.TestResultId == id);
            if (!testResultExists)
            {
                throw new Exception("Test result not found.");
            }
            var treatmentExists = await _treatmentRepository.AnyAsync(t => t.TestResultId == id);
            if (treatmentExists)
            {
                throw new Exception("1 test result can only link to 1 treatment.");
            }
        }

        public async Task CheckTreatmentExistAsync(int id)
        {
            var treatmentExists = await _treatmentRepository.AnyAsync(t => t.TreatmentId == id);
            if (!treatmentExists)
            {
                throw new Exception("Treatment not found.");
            }
        }

        public async Task CheckEmailExistAsync(string email)
        {
            var emailExists = await AnyAsync(u => u.Email.Equals(email));
            if (emailExists)
            {
                throw new Exception($"Email {email} already exists.");
            }
        }

        public async Task CheckCategoryExistAsync(int id)
        {
            var categoryExists = await _categoryRepository.AnyAsync(c => c.CategoryId == id && c.IsActive == true);
            if (!categoryExists)
            {
                throw new Exception("Category not found.");
            }
        }
    }
}