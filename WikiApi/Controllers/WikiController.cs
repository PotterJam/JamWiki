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
        public async Task<string> AddWiki()
        {
            throw new NotImplementedException("AddWiki not implemented.");
        }
        
        [HttpDelete]
        public async Task<string> RemoveWiki()
        {
            throw new NotImplementedException("RemoveWiki not implemented.");
        }
    }
}