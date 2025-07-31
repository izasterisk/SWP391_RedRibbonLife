using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO.DoctorCertificate;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class DoctorCertificateService : IDoctorCertificateService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<DoctorCertificate> _certificateRepository;
    private readonly IUserUtils _userUtils;

    public DoctorCertificateService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IRepository<DoctorCertificate> certificateRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _certificateRepository = certificateRepository;
        _userUtils = userUtils;
    }

    public async Task<DoctorCertificateDTO> CreateDoctorCertificateAsync(DoctorCertificateCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        await _userUtils.CheckDoctorExistAsync(dto.DoctorId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            DoctorCertificate certificate = _mapper.Map<DoctorCertificate>(dto);
            var created = await _certificateRepository.CreateAsync(certificate);
            await transaction.CommitAsync();

            var detailed = await _certificateRepository.GetWithRelationsAsync(
                filter: c => c.CertificateId == created.CertificateId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(c => c.Doctor)
                        .ThenInclude(d => d.User)
            );
            return _mapper.Map<DoctorCertificateDTO>(detailed);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DoctorCertificateDTO> UpdateDoctorCertificateAsync(DoctorCertificateUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var certificate = await _certificateRepository.GetAsync(c => c.CertificateId == dto.CertificateId, true);
        if (certificate == null)
        {
            throw new Exception("Doctor certificate not found.");
        }
        await dto.DoctorId.ValidateIfNotNullAsync(_userUtils.CheckDoctorExistAsync);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _mapper.Map(dto, certificate);
            var updated = await _certificateRepository.UpdateAsync(certificate);
            await transaction.CommitAsync();

            var detailed = await _certificateRepository.GetWithRelationsAsync(
                filter: c => c.CertificateId == updated.CertificateId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(c => c.Doctor)
                        .ThenInclude(d => d.User)
            );
            return _mapper.Map<DoctorCertificateDTO>(detailed);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<DoctorCertificateDTO>> GetAllDoctorCertificateAsync()
    {
        var certificates = await _certificateRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.User)
        );
        return _mapper.Map<List<DoctorCertificateDTO>>(certificates);
    }

    public async Task<DoctorCertificateDTO> GetDoctorCertificateByIdAsync(int id)
    {
        var certificate = await _certificateRepository.GetWithRelationsAsync(
            filter: c => c.CertificateId == id,
            useNoTracking: true,
            includeFunc: query => query
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.User)
        );
        if (certificate == null)
        {
            throw new Exception("Doctor certificate not found.");
        }
        return _mapper.Map<DoctorCertificateDTO>(certificate);
    }

    public async Task<List<DoctorCertificateDTO>> GetDoctorCertificatesByDoctorIdAsync(int doctorId)
    {
        await _userUtils.CheckDoctorExistAsync(doctorId);
        var certificates = await _certificateRepository.GetAllWithRelationsByFilterAsync(
            filter: c => c.DoctorId == doctorId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.User)
        );
        return _mapper.Map<List<DoctorCertificateDTO>>(certificates);
    }

    public async Task<bool> DeleteDoctorCertificateByIdAsync(int id)
    {
        var certificate = await _certificateRepository.GetAsync(c => c.CertificateId == id, true);
        if (certificate == null)
        {
            throw new Exception("Doctor certificate not found.");
        }
        await _certificateRepository.DeleteAsync(certificate);
        return true;
    }
}