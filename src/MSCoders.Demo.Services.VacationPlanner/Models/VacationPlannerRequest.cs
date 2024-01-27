using System.ComponentModel.DataAnnotations;

namespace MSCoders.Demo.Services.VacationPlanner.Models;

public class VacationPlannerRequest
{
    [Required]
    public string Ask { get; init; }

    [Required]
    public string ChatId { get; init; }
}
