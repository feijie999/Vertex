using System;
using System.ComponentModel.DataAnnotations;

namespace Transfer.IGrains.Dto
{
    public class ProjectDto
    {
        public string Id { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}