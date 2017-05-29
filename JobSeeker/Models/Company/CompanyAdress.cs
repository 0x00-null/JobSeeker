using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSeeker.Models.Company
{
    public class CompanyAdress
    {
        public int Id { get; set; }
        public string FirstLine { get; set; }
        public string SecondLine { get; set; }
        public string PostCode { get; set; }
    }
}
