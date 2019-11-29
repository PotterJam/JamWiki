using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikiApi.Stores.Wikis
{
    public interface IWikiStore
    {
        Task<WikiApi.Wiki> GetWikiByName(string wikiName);
        Task AddWiki(WikiApi.Wiki newWiki);
        Task DeleteWikiByName(string name);
        Task<IEnumerable<string>> GetWikiNames();
        Task UpdateWiki(Wiki updatedWiki);
    }
}