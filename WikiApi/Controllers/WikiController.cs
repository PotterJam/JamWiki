using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WikiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WikiController : ControllerBase
    {
        private readonly IWikiStore _wikiStore;
        
        public WikiController(IWikiStore wikiStore)
        {
            _wikiStore = wikiStore ?? throw new ArgumentNullException(nameof(wikiStore));
        }
        
        [HttpGet("name")]
        public async Task<string> Index()
        {
            var wiki = await _wikiStore.GetWiki(Guid.Parse("e7437f88-98ab-4dfc-a7a8-d71644c601da"));
            return wiki != null ? wiki.Name : "Wiki not found";
        }
    }
}