using System;
using System.Collections.Generic;

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
    public bool IsCancelled { get; set; }
    //connection to join table ActiviyAttnedee.cs 
    //we have to initialize it to new List<ActivityAttendee> because we were getting error messge "Object reference not set to an instance of an object." This will make sure that we will not get null reference as we are initializing it. 
    public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
  }
}