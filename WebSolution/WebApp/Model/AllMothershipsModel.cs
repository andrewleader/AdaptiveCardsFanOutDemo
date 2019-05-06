using RandomNameGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebApp.Model
{
    public static class AllMothershipsModel
    {
        public static readonly List<MothershipModel> Motherships = new List<MothershipModel>();

        private static readonly PlaceNameGenerator s_placeNameGenerator = new PlaceNameGenerator();

        public static MothershipModel CreateNewMothership(WebSocket socket)
        {
            lock (Motherships)
            {
                string newName = GetNewMothershipName();

                var newMothership = new MothershipModel(socket, newName);
                Motherships.Add(newMothership);

                newMothership.StartConnection();

                return newMothership;
            }
        }

        public static MothershipModel ReconnectMothership(WebSocket socket, string mothershipName)
        {
            lock (Motherships)
            {
                var existing = Motherships.FirstOrDefault(i => i.Name == mothershipName);
                if (existing != null)
                {
                    if (existing.DisconnectedTime != null)
                    {
                        existing.Reconnect(socket);

                        return existing;
                    }
                    else
                    {
                        throw new Exception("Mothership wasn't disconnected");
                    }
                }
                else
                {
                    throw new Exception("No existing mothership to reconnect to");
                }
            }
        }

        public static string[] GetAllMothershipNames()
        {
            lock (Motherships)
            {
                return Motherships.Select(i => i.Name).ToArray();
            }
        }

        public static void RemoveMothership(MothershipModel mothership)
        {
            lock (Motherships)
            {
                Motherships.Remove(mothership);
            }
        }

        private static string GetNewMothershipName()
        {
            while (true)
            {
                string newName = s_placeNameGenerator.GenerateRandomPlaceName();

                // Only allow names with just normal letters
                // and skip names that are already in use
                if (newName.All(c => Char.IsLetter(c)) && !Motherships.Any(i => i.Name == newName))
                {
                    return newName;
                }
            }
        }

        public static ClientModel TryCreateClient(WebSocket socket, string mothershipName)
        {
            lock (Motherships)
            {
                var mothership = Motherships.FirstOrDefault(i => i.Name == mothershipName);
                if (mothership == null)
                {
                    return null;
                }

                return mothership.TryCreateClient(socket);
            }
        }
    }
}
