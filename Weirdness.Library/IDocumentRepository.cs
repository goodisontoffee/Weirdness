namespace Weirdness.Library
{
    using System;
    using System.Threading.Tasks;

    public interface IDocumentRepository
    {
        Task<T> Get<T>(Guid id);
    }
}