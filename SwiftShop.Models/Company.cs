using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftShop.Models
{
    public class Company
    {
        [Key]   
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public String StreetAdress { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public int PostalCode { get; set; }
        public String PhoneNumber { get; set; }
    }
}
