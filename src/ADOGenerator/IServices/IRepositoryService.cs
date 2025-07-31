using ADOGenerator.Models;

namespace ADOGenerator.IServices
{
    public interface IRepositoryService
    {
        Task<bool> ImportLocalRepositoryAsync(ImportSourceCodeLocalSource localSource, string repositoryName);
    }
}