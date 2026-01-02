using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Topic
{
    public int TopicId { get; set; }

    public string TopicName { get; set; } = null!;

    public int CourseId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;
}
