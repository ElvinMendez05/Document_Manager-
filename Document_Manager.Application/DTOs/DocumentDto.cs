using System;
using System.Collections.Generic;
using System.Text;

namespace Document_Manager.Application.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
