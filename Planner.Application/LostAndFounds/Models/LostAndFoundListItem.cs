using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Application.LostAndFounds.Models
{
    public class LostAndFoundListItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
        public DateTime? LostOn { get; set; }
        public FoundStatus FoundStatus { get; set; }
        public GuestStatus GuestStatus { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public OtherStatus OtherStatus { get; set; }
        public TypeOfLoss TypeOfLoss { get; set; }
    }
}
