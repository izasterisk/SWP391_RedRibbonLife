using AutoMapper;
using BLL.DTO.Treatment;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class TreatmentService : ITreatmentService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<Treatment> _treatmentRepository;
    private readonly IARVRegimenUtils _arvRegimenUtils;
    private readonly IUserUtils _userUtils;
    public TreatmentService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<Treatment> treatmentRepository, IARVRegimenUtils arvRegimenUtils, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _treatmentRepository = treatmentRepository;
        _arvRegimenUtils = arvRegimenUtils;
        _userUtils = userUtils;
    }
    
    public async Task<TreatmentDTO> CreateTreatmentAsync(TreatmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _arvRegimenUtils.CheckARVRegimenExistAsync(dto.RegimenId);
        await _userUtils.CheckTestResultExistAsync(dto.TestResultId);
        var treatmentExists = await _treatmentRepository.AnyAsync(t => t.TestResultId == dto.TestResultId);
        if (treatmentExists)
        {
            throw new Exception("This test result has already been treated.");
        }
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            Treatment treatment = _mapper.Map<Treatment>(dto);
            // treatment.Status = "Active";
            var createdTreatment = await _treatmentRepository.CreateAsync(treatment);
            await transaction.CommitAsync();
            var detailedTreatment = await _treatmentRepository.GetWithRelationsAsync(
                filter: t => t.TreatmentId == createdTreatment.TreatmentId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(t => t.Regimen)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.Patient)
                        .ThenInclude(p => p.User)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.Appointment)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.TestType)
            );
            return _mapper.Map<TreatmentDTO>(detailedTreatment);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<TreatmentDTO> UpdateTreatmentAsync(TreatmentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var treatment = await _treatmentRepository.GetAsync(t => t.TreatmentId == dto.TreatmentId, true);
            if (treatment == null)
            {
                throw new Exception("Treatment not found.");
            }
            if (treatment.Status != "Active")
            {
                throw new InvalidOperationException("Cannot update completed or cancelled treatment");
            }
            await dto.TestResultId.ValidateIfNotNullAsync(_userUtils.CheckTestResultExistAsync);
            await dto.RegimenId.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVRegimenExistAsync);
            var startDate = dto.StartDate ?? treatment.StartDate;
            var endDate = dto.EndDate ?? treatment.EndDate;
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }
            _mapper.Map(dto, treatment);
            var updatedTreatment = await _treatmentRepository.UpdateAsync(treatment);
            await transaction.CommitAsync();
            var detailedTreatment = await _treatmentRepository.GetWithRelationsAsync(
                filter: t => t.TreatmentId == updatedTreatment.TreatmentId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(t => t.Regimen)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.Patient)
                        .ThenInclude(p => p.User)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.Appointment)
                    .Include(t => t.TestResult)
                        .ThenInclude(tr => tr.TestType)
            );
            return _mapper.Map<TreatmentDTO>(detailedTreatment);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<TreatmentDTO>> GetAllTreatmentAsync()
    {
        var treatments = await _treatmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
        return _mapper.Map<List<TreatmentDTO>>(treatments);
    }
    
    public async Task<TreatmentDTO> GetTreatmentByIdAsync(int id)
    {
        var treatment = await _treatmentRepository.GetWithRelationsAsync(
            filter: t => t.TreatmentId == id,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        return _mapper.Map<TreatmentDTO>(treatment);
    }
    
    public async Task<TreatmentDTO> GetTreatmentByTestResultIdAsync(int id)
    {
        var treatment = await _treatmentRepository.GetWithRelationsAsync(
            filter: t => t.TestResultId == id,
            useNoTracking: true,
            includeFunc: query => query
                // .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        return _mapper.Map<TreatmentDTO>(treatment);
    }
    
    public async Task<List<TreatmentDTO>> GetTreatmentByPatientIdAsync(int id)
    {
        var treatments = await _treatmentRepository.GetAllWithRelationsByFilterAsync(
            filter: t => t.TestResult.PatientId == id,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(a => a.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(a => a.TestType)
        );
        return _mapper.Map<List<TreatmentDTO>>(treatments);
    }
    
    public async Task<bool> DeleteTreatmentByIdAsync(int id)
    {
        var treatment = await _treatmentRepository.GetAsync(t => t.TreatmentId == id, true);
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        await _treatmentRepository.DeleteAsync(treatment);
        return true;
    }
}