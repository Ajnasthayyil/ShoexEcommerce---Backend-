using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheoxEcommerce.Application.DTOs.Size
{
    public class CreateSizeDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
