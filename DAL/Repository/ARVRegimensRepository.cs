using DAL.IRepository;
using DAL.Models;

namespace DAL.Repository;

public class ARVRegimensRepository : IARVRegimensRepository
{
    private readonly IRepository<Arvregimen> _arvRegimensRepository;
    public ARVRegimensRepository(IRepository<Arvregimen> arvRegimensRepository)
    {
        _arvRegimensRepository = arvRegimensRepository;
    }
    public async Task<Arvregimen?> GetARVRegimensToCheckAsync(int id)
    {
        return await _arvRegimensRepository.GetAsync(u => u.RegimenId == id 
                                                          && u.IsActive == true, true);
    }
}