using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WikiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WikiController : ControllerBase
    {
        private readonly IWikiStore _wikiStore;
        
        public WikiController(IWikiStore wikiStore)
        {
            _wikiStore = wikiStore ?? throw new ArgumentNullException(nameof(wikiStore));
        }
        
        [HttpGet]
        public async Task<Wiki> GetWiki(string name)
        {
           return await _wikiStore.GetWikiByName(name);
        }
        
        [HttpPost]
        public async Task AddWiki(string name, string body, string tags)
        {
            var tagsArr = tags.Split(",");
            var newWiki = new Wiki(Guid.NewGuid(), name, body, tagsArr);
            await _wikiStore.AddWiki(newWiki);
        }
        
        [HttpDelete]
        public async Task DeleteWiki(string name)
        {
            await _wikiStore.DeleteWikiByName(name);
        }
    }
}