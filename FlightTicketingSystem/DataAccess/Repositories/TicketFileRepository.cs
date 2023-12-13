using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TicketFileRepository : ITicket 
    {
        private string _path;
        private TicketDbRepository _ticketDbRepository;

        public TicketFileRepository(string path)
        {

            _path = path;

            if (System.IO.File.Exists(path) == false) //if the file 'path' does not exist
            {
                using (var myFile = System.IO.File.Create(path)) //create the file and assign to variable myFile
                {
                    myFile.Close();
                }
            }
        }

        public TicketFileRepository(TicketDbRepository ticketDbRepository) {
            _ticketDbRepository = ticketDbRepository;
        }

        public Ticket? GetTicket(int id)
        {
            var ticketList = GetTickets().ToList();
            var ticket = ticketList.FirstOrDefault(t => t.Id == id);

            return ticket;
        }

        public IQueryable<Ticket> GetTickets() 
        {
            string allTicketsToJson = "";
            
            using(var myFile = System.IO.File.OpenText(_path)) //if file is not created; it will throw an exception
            {
                allTicketsToJson = myFile.ReadToEnd();
                myFile.Close();
            }

            if (string.IsNullOrEmpty(allTicketsToJson))
            {
                return new List<Ticket>().AsQueryable();
            }
            else 
            {
                //Deserialisation >>> transforming string to object -> converting from json string into a List<Product>
                List<Ticket> allTickets = JsonSerializer.Deserialize<List<Ticket>>(allTicketsToJson);
                return allTickets.AsQueryable();
            }
        }

        public void Book(Ticket ticket)
        {
            var ticketList = GetTickets().ToList();
            ticket.Id = GetNextId(); // getting the next Id 

            ticketList.Add(ticket);

            string myTicketsJson = JsonSerializer.Serialize(ticketList);

            System.IO.File.WriteAllText(_path, myTicketsJson);
        }

        public void Cancel(int id) 
        {
            var ticketList = GetTickets().ToList();
            var removableTicket = ticketList.FirstOrDefault(t => t.Id == id);

            if (removableTicket != null)
            {
                ticketList.Remove(removableTicket);

                string updatedTickets = JsonSerializer.Serialize(ticketList);
                System.IO.File.WriteAllText(_path, updatedTickets);
            }
            else
            {
                throw new ArgumentException("Ticket not found with the given ID");
            }
        }

        private int GetNextId()
        {
            int maxId = _ticketDbRepository.GetTickets().Max(t => t.Id);
            return maxId + 1;
        }
    }
}
