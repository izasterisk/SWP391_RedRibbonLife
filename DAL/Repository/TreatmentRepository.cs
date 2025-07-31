using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class TreatmentRepository : ITreatmentRepository
{
    private readonly IRepository<Treatment> _treatmentRepository;
    public TreatmentRepository(IRepository<Treatment> treatmentRepository)
    {
        _treatmentRepository = treatmentRepository;
    }
    public async Task<Treatment?> GetTreatmentToCheckAsync(int id)
    {
        return await _treatmentRepository.GetAsync(u => u.RegimenId == id, true);
    }
    public async Task<Treatment?> GetTreatmentById4NotificationAsync(int id)
    {
        return await _treatmentRepository.GetWithRelationsAsync(
            t => t.TreatmentId == id,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                .ThenInclude(tr => tr.Patient)
                .ThenInclude(p => p.User)
        );
    }
}