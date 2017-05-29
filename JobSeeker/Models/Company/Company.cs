using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSeeker.Models.Company
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string UserName { get; set; }

        // Main Information
        public string Name { get; set; }
        public string Description { get; set; }
        public string AboutUs { get; set; }
        public CompanyAdress CompanyAdress { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public DateTime YearFounded { get; set; }
        public CompanySize Size { get; set; }

        // Other Information
        public List<CompanyTechStack> TechStack { get; set; }
        public List<CompanyBenefits> Benefits { get; set; }

        // Jobs
        public ICollection<Job> Jobs { get; set; }

        // Website Link
        public string WebsiteUrl { get; set; }

        // Social Media
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string LinkedInUrl { get; set; }

    }
}
