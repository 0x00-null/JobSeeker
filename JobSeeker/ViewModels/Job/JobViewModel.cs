using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Models;

namespace JobSeeker.ViewModels.Job
{
    public class JobViewModel
    {
        public string Title { get; set; }
 
        public List<JobTags> JobTags { get; set; }
        public string Location { get; set; }

        public string Description { get; set; }
        public string SkillsAndRequirements { get; set; }
        public string Salary { get; set; }
        public int CompanyId { get; set; }
    }
}
