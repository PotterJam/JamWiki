using System;
using System.Threading.Tasks;

namespace WikiApi
{
    public interface IWikiStore
    {
        Task<Wiki> GetWikiByName(string wikiName);
    }
}