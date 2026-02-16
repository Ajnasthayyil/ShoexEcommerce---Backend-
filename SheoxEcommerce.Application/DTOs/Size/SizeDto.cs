using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheoxEcommerce.Application.DTOs.Size
{
    public class SizeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
