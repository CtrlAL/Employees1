namespace DAL.Interfaces
{
    public interface IRepository<TEnity, TQuery, TId>
    {
        public Task<TEnity> GetAsync(TId id);
        public Task<IList<TEnity>> GetAsync(TQuery query);
        public Task<TId> CreateAsync(TEnity id);
        public Task<TId> UpdateAsync(TEnity id);
        public Task<TEnity> DeleteAsync(TId id);
    }
}
