using System;

namespace Domain
{
  public class Activity
  {
    // We have to call this Id, for entity framework to recognize it and use it as our primary key.
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string City { get; set; }
    public string Venue { get; set; }
  }
}