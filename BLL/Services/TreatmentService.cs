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
    
    public async Task<dynamic> CreateTreatmentAsync(TreatmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        _arvRegimenUtils.CheckARVRegimenExist(dto.RegimenId);
        dto.TestResultId.ValidateIfNotNull(_userUtils.CheckTestResultExist);
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            Treatment treatment = _mapper.Map<Treatment>(dto);
            // treatment.Status = "Active";
            var createdTreatment = await _treatmentRepository.CreateAsync(treatment);
            await transaction.CommitAsync();
            var detailedTreatment = await GetTreatmentByIdAsync(createdTreatment.TreatmentId);
            return new
            {
                TreatmentInfo = detailedTreatment
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<dynamic> UpdateTreatmentAsync(TreatmentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        dto.TestResultId.ValidateIfNotNull(_userUtils.CheckTestResultExist);
        dto.RegimenId.ValidateIfNotNull(_arvRegimenUtils.CheckARVRegimenExist);
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var treatment = await _treatmentRepository.GetAsync(t => t.TreatmentId == dto.TreatmentId, true);
            if (treatment == null)
            {
                throw new Exception("Treatment not found.");
            }
            var startDate = dto.StartDate ?? treatment.StartDate;
            var endDate = dto.EndDate ?? treatment.EndDate;
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }
            _mapper.Map(dto, treatment);
            var updatedTreatment = await _treatmentRepository.UpdateAsync(treatment);
            await transaction.CommitAsync();
            var detailedTreatment = await GetTreatmentByIdAsync(updatedTreatment.TreatmentId);
            return new
            {
                TreatmentInfo = detailedTreatment
            };
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
        );
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        return _mapper.Map<TreatmentDTO>(treatment);
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