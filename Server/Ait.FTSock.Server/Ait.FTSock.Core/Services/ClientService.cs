using Ait.FTSock.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ait.FTSock.Core.Services
{
    public class ClientService
    {
        public List<Client> Clients { get; private set; }
        public ClientService()
        {
            Clients = new List<Client>();
        }
        public void AddClient(string username, Guid id, string activePath)
        {
            Clients.Add(new Client { Username = username, Id = id, ActivePath = activePath });
        }
        public void UpdateClient(Client client, string newActivePath)
        {
            client.ActivePath = newActivePath;
            Clients.Remove(Clients.FirstOrDefault(c => c.Id.Equals(client.Id)));
            Clients.Add(client);
        }
        public void RemoveClientById(Guid id)
        {
            Client client = Clients.FirstOrDefault(c => c.Id.Equals(id));
            Clients.Remove(client);
        }
        public Client GetClientById(Guid id)
        {
            return Clients.FirstOrDefault(c => c.Id.Equals(id));
        }
    }
}
