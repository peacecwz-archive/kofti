using System;
using System.ComponentModel.DataAnnotations;

namespace Kofti.Manager.Data
{
    public class EntityBase<T>
    {
        [Key] public T Id { get; set; } = default;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
    }
}