using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SwiftShop.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        [DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Range must be in between 1 and 100")]
        public int DisplayOrder { get; set; }

    }
}
