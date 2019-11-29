using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiApi.Services;
using WikiApi.Stores.Wikis;

namespace WikiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class WikiController : ControllerBase
    {
        private readonly IWikiStore _wikiStore;
        private readonly IUserService _userService;
        
        public WikiController(IWikiStore wikiStore, IUserService userService)
        {
            _wikiStore = wikiStore ?? throw new ArgumentNullException(nameof(wikiStore));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<Wiki> GetWiki(string name)
        {
            var wikiUser = await _userService.GetWikiUser(User);
            return await _wikiStore.GetWikiByName(name);
        }
        
        [HttpPost]
        public async Task AddWiki([FromBody] AddWikiRequest addWikiRequest)
        {
            var tagsArr = addWikiRequest.tags == null ? new string[] {} : addWikiRequest.tags.Split(",");
            var newWiki = new Wiki(Guid.NewGuid(), addWikiRequest.name, addWikiRequest.body, tagsArr);
            await _wikiStore.AddWiki(newWiki);
        }
        
        [HttpPost("update")]
        public async Task UpdateWiki([FromBody] AddWikiRequest addWikiRequest)
        {
            var tagsArr = addWikiRequest.tags == null ? new string[] {} : addWikiRequest.tags.Split(",");
            var newWiki = new Wiki(Guid.NewGuid(), addWikiRequest.name, addWikiRequest.body, tagsArr);
            await _wikiStore.UpdateWiki(newWiki);
        }
        
        [HttpDelete]
        public async Task DeleteWiki([FromBody] DeleteWikiRequest addWikiRequest)
        {
            await _wikiStore.DeleteWikiByName(addWikiRequest.name);
        }

        [AllowAnonymous]
        [HttpGet("names")]
        public async Task<IEnumerable<string>> GetWikiNames()
        {
            return await _wikiStore.GetWikiNames();
        }

        public class AddWikiRequest
        {
            public string name { get; set; }
            public string body { get; set; }
            public string tags { get; set; }
        }
        
        public class DeleteWikiRequest
        {
            public string name { get; set; }
        }
    }
}