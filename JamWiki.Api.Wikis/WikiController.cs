﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JamWiki.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamWiki.Api.Wikis
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
        public async Task<Wiki> GetWiki(string name)
        {
            var wikiUser = await _userService.GetWikiUser(User);
            return await _wikiStore.GetWikiByName(name, wikiUser);
        }
        
        [HttpGet("with-tag")]
        public async Task<IEnumerable<Wiki>> GetWikiWithTag(string tag)
        {
            var wikiUser = await _userService.GetWikiUser(User);
            return await _wikiStore.GetWikisWithTag(tag, wikiUser);
        }
        
        [HttpPost]
        public async Task AddWiki([FromBody] AddWikiRequest addWikiRequest)
        {
            var wikiUser = await _userService.GetWikiUser(User);
            var newWiki = new Wiki(Guid.NewGuid(), addWikiRequest.Name, addWikiRequest.Body, addWikiRequest.Tags);
            await _wikiStore.AddWiki(newWiki, wikiUser);
        }
        
        [HttpPost("update")]
        public async Task UpdateWiki([FromBody] AddWikiRequest addWikiRequest)
        {
            var wikiUser = await _userService.GetWikiUser(User);
            var newWiki = new Wiki(Guid.NewGuid(), addWikiRequest.Name, addWikiRequest.Body, addWikiRequest.Tags);
            await _wikiStore.UpdateWiki(newWiki, wikiUser);
        }
        
        [HttpDelete]
        public async Task DeleteWiki([FromBody] DeleteWikiRequest addWikiRequest)
        {
            var wikiUser = await _userService.GetWikiUser(User);
            await _wikiStore.DeleteWikiByName(addWikiRequest.Name, wikiUser);
        }

        [HttpGet("names")]
        public async Task<IEnumerable<string>> GetWikiNames()
        {
            var wikiUser = await _userService.GetWikiUser(User);
            return await _wikiStore.GetWikiNames(wikiUser);
        }
        
        [HttpGet("tags")]
        public async Task<IEnumerable<string>> GetWikiTags()
        {
            var wikiUser = await _userService.GetWikiUser(User);
            return await _wikiStore.GetAllWikiTags(wikiUser);
        }
        
        public class AddWikiRequest
        {
            public string Name { get; set; }
            public string Body { get; set; }
            public IEnumerable<string> Tags { get; set; }
        }
        
        public class DeleteWikiRequest
        {
            public string Name { get; set; }
        }
    }
}