namespace Weirdness.Library
{
    using System;
    using System.Threading.Tasks;

    public abstract class GetStuffBase
    {
        private IDocumentRepository Repository { get; }

        protected GetStuffBase(IDocumentRepository repository)
        {
            Repository = repository;
        }

        public virtual Task<T> Get<T>(Guid id)
        {
            return Repository.Get<T>(id);
        }
    }
}