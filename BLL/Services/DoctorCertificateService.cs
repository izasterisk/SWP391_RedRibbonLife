using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO.DoctorCertificate;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class DoctorCertificateService : IDoctorCertificateService
{
    private readonly IMapper _mapper;
    private readonly IDoctorCertificateRepository _certificateRepository;
    private readonly IUserUtils _userUtils;

    public DoctorCertificateService(IMapper mapper, IDoctorCertificateRepository certificateRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _certificateRepository = certificateRepository;
        _userUtils = userUtils;
    }

    public async Task<DoctorCertificateDTO> CreateDoctorCertificateAsync(DoctorCertificateCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        await _userUtils.CheckDoctorExistAsync(dto.DoctorId);
        
        DoctorCertificate certificate = _mapper.Map<DoctorCertificate>(dto);
        var created = await _certificateRepository.CreateDoctorCertificateWithTransactionAsync(certificate);
        
        var detailed = await _certificateRepository.GetDoctorCertificateWithRelationsAsync(created.CertificateId, true);
        return _mapper.Map<DoctorCertificateDTO>(detailed);
    }

    public async Task<DoctorCertificateDTO> UpdateDoctorCertificateAsync(DoctorCertificateUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var certificate = await _certificateRepository.GetDoctorCertificateForUpdateAsync(dto.CertificateId);
        if (certificate == null)
        {
            throw new Exception("Doctor certificate not found.");
        }
        await dto.DoctorId.ValidateIfNotNullAsync(_userUtils.CheckDoctorExistAsync);

        _mapper.Map(dto, certificate);
        var updated = await _certificateRepository.UpdateDoctorCertificateWithTransactionAsync(certificate);
        
        var detailed = await _certificateRepository.GetDoctorCertificateWithRelationsAsync(updated.CertificateId, true);
        return _mapper.Map<DoctorCertificateDTO>(detailed);
    }

    public async Task<List<DoctorCertificateDTO>> GetAllDoctorCertificateAsync()
    {
        var certificates = await _certificateRepository.GetAllDoctorCertificatesWithRelationsAsync();
        return _mapper.Map<List<DoctorCertificateDTO>>(certificates);
    }

    public async Task<DoctorCertificateDTO> GetDoctorCertificateByIdAsync(int id)
    {
        var certificate = await _certificateRepository.GetDoctorCertificateWithRelationsAsync(id, true);
        if (certificate == null)
        {
            throw new Exception("Doctor certificate not found.");
        }
        return _mapper.Map<DoctorCertificateDTO>(certificate);
    }

    public async Task<List<DoctorCertificateDTO>> GetDoctorCertificatesByDoctorIdAsync(int doctorId)
    {
        await _userUtils.CheckDoctorExistAsync(doctorId);
        var certificates = await _certificateRepository.GetDoctorCertificatesByDoctorIdWithRelationsAsync(doctorId, true);
        return _mapper.Map<List<DoctorCertificateDTO>>(certificates);
    }

    public async Task<bool> DeleteDoctorCertificateByIdAsync(int id)
    {
        var certificate = await _certificateRepository.GetDoctorCertificateForUpdateAsync(id);
        if (certificate == null)
        {
            throw new Exception("Doctor certificate not found.");
        }
        await _certificateRepository.DeleteDoctorCertificateWithTransactionAsync(certificate);
        return true;
    }
}