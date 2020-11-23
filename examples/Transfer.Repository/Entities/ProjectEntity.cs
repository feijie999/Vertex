using System;
using System.ComponentModel.DataAnnotations;

namespace Transfer.Repository.Entities
{
    public class ProjectEntity
    {
        public Guid Id { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}