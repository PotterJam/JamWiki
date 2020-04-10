using System.Collections.Generic;
using System.Threading.Tasks;
using JamWiki.Api.Users;

namespace JamWiki.Api.Wikis
{
    public interface IWikiStore
    {
        Task<Wiki> GetWikiByName(string wikiName, WikiUser wikiUser);
        Task<IEnumerable<Wiki>> GetWikisWithTag(string tag, WikiUser wikiUser);
        Task AddWiki(Wiki newWiki, WikiUser wikiUser);
        Task DeleteWikiByName(string name, WikiUser wikiUser);
        Task<IEnumerable<string>> GetWikiNames(WikiUser wikiUser);
        Task UpdateWiki(Wiki updatedWiki, WikiUser wikiUser);
        Task<IEnumerable<string>> GetAllWikiTags(WikiUser wikiUser);
    }
}