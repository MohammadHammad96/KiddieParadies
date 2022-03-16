using KiddieParadies.Core.Services;
using System.Collections.Generic;

namespace KiddieParadies.Infrastructure.Services
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private static readonly Dictionary<string, List<string>> UserConnectionMap = new Dictionary<string, List<string>>();
        private static readonly string UserConnectionMapLocker = string.Empty;
        public void KeepUserConnection(string userId, string connectionId)
        {
            lock (UserConnectionMapLocker)
            {
                if (!UserConnectionMap.ContainsKey(userId))
                {
                    UserConnectionMap[userId] = new List<string>();
                }
                UserConnectionMap[userId].Add(connectionId);
            }
        }
        public void RemoveUserConnection(string connectionId)
        {
            //This method will remove the connectionId of user
            lock (UserConnectionMapLocker)
            {
                foreach (var userId in UserConnectionMap.Keys)
                {
                    if (!UserConnectionMap.ContainsKey(userId))
                        continue;
                    if (!UserConnectionMap[userId].Contains(connectionId))
                        continue;
                    UserConnectionMap[userId].Remove(connectionId);
                    break;
                }
            }
        }
        public List<string> GetUserConnections(string userId)
        {
            var con = new List<string>();
            lock (UserConnectionMapLocker)
            {
                con = UserConnectionMap[userId];
            }
            return con;
        }
    }
}