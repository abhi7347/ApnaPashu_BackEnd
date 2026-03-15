using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APNAPASHU.DataContract.Models
{
    public class FilterDto
    {
        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int? PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of records per page.
        /// </summary>
        public int? PageSize { get; set; } = 10;

        /// <summary>
        /// Search keyword (optional).
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Column name to sort by (optional).
        /// </summary>
        public string? SortCulumn { get; set; }

        /// <summary>
        /// Sort direction: ASC or DESC.
        /// </summary>
        public string? SortDirection { get; set; } = "DESC";


        /// <summary>
        /// Screen Key for specific screen (optional).
        /// </summary>
        public string? ModuleKey { get; set; }
        public int? RoleId { get; set; }
    }
}
