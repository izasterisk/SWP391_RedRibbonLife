using DAL.IRepository;
using DAL.Models;

namespace DAL.Repository;

public class ARVComponentRepository : IARVComponentRepository
{
    private readonly IRepository<Arvcomponent> _arvComponentRepository;
    public ARVComponentRepository(IRepository<Arvcomponent> arvComponentRepository)
    {
        _arvComponentRepository = arvComponentRepository;
    }
    public async Task<Arvcomponent?> GetARVComponentToCheckAsync(int id)
    {
        return await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
    }
}