using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleShopApp.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Category name could not be empty.")]
        [Display(Name ="Category name")]
        public string Name { get; set; }
    }

}
