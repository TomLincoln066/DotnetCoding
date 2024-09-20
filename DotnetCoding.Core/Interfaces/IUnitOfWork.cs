

namespace DotnetCoding.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }

        //int Save();
        Task<int> Save();  // Make Save method asynchronous
    }
}
