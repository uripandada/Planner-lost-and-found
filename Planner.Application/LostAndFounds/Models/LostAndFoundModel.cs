﻿using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;

namespace Planner.Application.LostAndFounds.Models
{
    public class LostAndFoundModel
    {

        public LostAndFoundModel()
        {
            this.Files = new List<LostAndFoundFileModel>();
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
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
        public TypeOfLoss TypeOfLoss { get; set; }

        public string PlaceOfStorage { get; set; }
        public string TrackingNumber { get; set; }

        public string HotelId { get; set; }

        public IList<LostAndFoundFileModel> Files { get; set; }

        public string ClientName { get; set; }
        public string FounderName { get; set; }
        public string FounderEmail { get; set; }
        public string FounderPhoneNumber { get; set; }
        public Guid? LostAndFoundCategoryId { get; set; }
        public Guid? StorageRoomId { get; set; }
    }
}
