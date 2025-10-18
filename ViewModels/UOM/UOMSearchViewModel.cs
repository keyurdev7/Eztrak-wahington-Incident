﻿using Pagination;

namespace ViewModels
{
    public class UOMSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
