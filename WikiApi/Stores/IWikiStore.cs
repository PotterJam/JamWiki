﻿using System;
using System.Threading.Tasks;

namespace WikiApi
{
    public interface IWikiStore
    {
        Task<Wiki> GetWikiByName(string wikiName);
        Task AddWiki(Wiki newWiki);
        Task DeleteWikiByName(string name);
    }
}