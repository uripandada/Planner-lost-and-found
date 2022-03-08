using Planner.Domain.Values;
using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
    public class LostAndFound: ChangeTrackingBaseEntity
    {
        public LostAndFound()
        {
            Files = new List<LostAndFoundFile>();
        }

        public string HotelId { get; set; }
        public Hotel Hotel { get; set; }

        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
        public Guid? RoomId { get; set; }
        public Room Room { get; set; }
        public string ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public DateTime? LostOn { get; set; }
        public FoundStatus FoundStatus { get; set; }
        public GuestStatus GuestStatus { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public OtherStatus OtherStatus { get; set; }
        public RccLostAndFoundStatus RccStatus { get; set; }
        public TypeOfLoss? TypeOfLoss { get; set; }
        public LostAndFoundRecordType Type { get; set; }

        public ICollection<LostAndFoundFile> Files { get; set; }
    }
}
