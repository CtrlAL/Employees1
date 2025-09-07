namespace DAL.Interfaces
{
    public interface IRepository<TEnity, TQuery, TId>
    {
        public Task<TEnity?> GetAsync(TId id);
        public Task<IList<TEnity>> GetAsync(TQuery query);
        public Task<TId> CreateAsync(TEnity enity);
        public Task<bool> UpdateAsync(TEnity enity);
        public Task<bool> DeleteAsync(TId id);
    }
}
