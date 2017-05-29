using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Models.Company;

namespace JobSeeker.ViewModels.Company
{
    public class CompanyViewModel
    {
        public string UserName { get; set; }
        // Main Information
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string AboutUs { get; set; }
        public CompanyAdress CompanyAdress { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public DateTime YearFounded { get; set; }
        [Required]
        public CompanySize Size { get; set; }
        [Required]
        public string WebsiteUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string LinkedInUrl { get; set; }
    }
}
