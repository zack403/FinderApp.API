using System.ComponentModel.DataAnnotations;

namespace FinderApp.API.Model
{
    public class Value
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}