using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSeeker.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public List<JobTags> JobTags { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string SkillsAndRequirements { get; set; }
        public string Salary { get; set; }

        public int CompanyId { get; set; }
        public Company.Company Company { get; set; }
    }
}
