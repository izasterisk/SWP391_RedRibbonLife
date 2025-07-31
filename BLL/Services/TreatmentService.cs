using AutoMapper;
using BLL.DTO.Treatment;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class TreatmentService : ITreatmentService
{
    private readonly IMapper _mapper;
    private readonly ITreatmentRepository _treatmentRepository;
    private readonly IARVRegimenUtils _arvRegimenUtils;
    private readonly IUserUtils _userUtils;
    
    public TreatmentService(IMapper mapper, ITreatmentRepository treatmentRepository, IARVRegimenUtils arvRegimenUtils, IUserUtils userUtils)
    {
        _mapper = mapper;
        _treatmentRepository = treatmentRepository;
        _arvRegimenUtils = arvRegimenUtils;
        _userUtils = userUtils;
    }
    
    public async Task<TreatmentDTO> CreateTreatmentAsync(TreatmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _arvRegimenUtils.CheckARVRegimenExistAsync(dto.RegimenId);
        await _userUtils.CheckTestResultExistAsync(dto.TestResultId);
        
        var treatmentExists = await _treatmentRepository.CheckTreatmentExistsByTestResultIdAsync(dto.TestResultId);
        if (treatmentExists)
        {
            throw new Exception("This test result has already been treated.");
        }
        
        Treatment treatment = _mapper.Map<Treatment>(dto);
        var createdTreatment = await _treatmentRepository.CreateTreatmentWithTransactionAsync(treatment);
        var detailedTreatment = await _treatmentRepository.GetTreatmentWithDetailsByIdAsync(createdTreatment.TreatmentId);
        
        return _mapper.Map<TreatmentDTO>(detailedTreatment);
    }
    
    public async Task<TreatmentDTO> UpdateTreatmentAsync(TreatmentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var treatment = await _treatmentRepository.GetTreatmentForUpdateAsync(dto.TreatmentId);
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
        var updatedTreatment = await _treatmentRepository.UpdateTreatmentWithTransactionAsync(treatment);
        var detailedTreatment = await _treatmentRepository.GetTreatmentWithDetailsByIdAsync(updatedTreatment.TreatmentId);
        
        return _mapper.Map<TreatmentDTO>(detailedTreatment);
    }
    
    public async Task<List<TreatmentDTO>> GetAllTreatmentAsync()
    {
        var treatments = await _treatmentRepository.GetAllTreatmentsWithDetailsAsync();
        return _mapper.Map<List<TreatmentDTO>>(treatments);
    }
    
    public async Task<TreatmentDTO> GetTreatmentByIdAsync(int id)
    {
        var treatment = await _treatmentRepository.GetTreatmentWithDetailsByIdAsync(id);
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        return _mapper.Map<TreatmentDTO>(treatment);
    }
    
    public async Task<TreatmentDTO> GetTreatmentByTestResultIdAsync(int id)
    {
        var treatment = await _treatmentRepository.GetTreatmentWithDetailsByTestResultIdAsync(id);
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        return _mapper.Map<TreatmentDTO>(treatment);
    }
    
    public async Task<List<TreatmentDTO>> GetTreatmentByPatientIdAsync(int id)
    {
        var treatments = await _treatmentRepository.GetTreatmentsByPatientIdAsync(id);
        return _mapper.Map<List<TreatmentDTO>>(treatments);
    }
    
    public async Task<bool> DeleteTreatmentByIdAsync(int id)
    {
        var treatment = await _treatmentRepository.GetTreatmentForUpdateAsync(id);
        if (treatment == null)
        {
            throw new Exception("Treatment not found.");
        }
        return await _treatmentRepository.DeleteTreatmentWithTransactionAsync(treatment);
    }
}