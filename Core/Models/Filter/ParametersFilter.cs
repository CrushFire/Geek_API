using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Filter
{
    public class ParametersFilter
    {
        [Required(ErrorMessage = "NameSearchObjectIsRequired")]
        public string Name { get; set; } = string.Empty;
        public PostFilter PostFilter { get; set; } = new PostFilter();
        public CommunityFilter CommunityFilter { get; set; } = new CommunityFilter();
        public UserFilter UserFilter { get; set; } = new UserFilter();
        [Required(ErrorMessage = "SortByObjectIsRequired")]
        public string SortBy { get; set; }
        [RegularExpression(@"^(ask|desk)$", ErrorMessage = "SortDirectionIsInValid")]
        public string DirectionSort { get; set; } = "ask";
        public DateCreateRange DateCreateAt { get; set; } = DateCreateRange.None;
        public PaginationRequest Pagination {  get; set; } = new PaginationRequest() { Page = 1, PageSize = 10 };
    }
}
