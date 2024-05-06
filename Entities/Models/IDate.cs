using System;

namespace IdentityAuthLesson.Entities.Models;

public interface IDate
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }
    public DateTimeOffset DeletedDate { get; set; }
    public bool IsDeleted { get; set; }
}