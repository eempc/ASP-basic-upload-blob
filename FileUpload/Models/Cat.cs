using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Models {
    public class Cat {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
