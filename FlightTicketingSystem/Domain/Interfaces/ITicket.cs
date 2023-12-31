﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITicket
    {
        IQueryable<Ticket> GetTickets();

        Ticket? GetTicket(int id);

        void Book(Ticket ticket);

        void Cancel(int id);

        bool seatAvailable(Guid flightId, int row, int column);

        int totalSeatBooking(Guid flightId);

        int totalSeatAvailability(Guid flightId, int row, int column);
    }
}
