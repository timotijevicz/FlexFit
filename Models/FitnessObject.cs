using FlexFit.Models;

public class FitnessObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public int Capacity { get; set; }
    public string WorkingHours { get; set; }
    public ICollection<Resource> Resources { get; set; }
    public virtual ICollection<DailyCard> DailyCards { get; set; } = new List<DailyCard>();
}

