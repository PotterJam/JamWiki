using System;
using System.Threading.Tasks;

namespace WikiApi
{
    public interface IWikiStore
    {
        Task<Wiki> GetWiki(Guid wikiId);
    }
}