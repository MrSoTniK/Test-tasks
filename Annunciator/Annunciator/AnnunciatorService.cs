using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Annunciator
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AnnunciatorService : IAnnunciatorService
    {
        private List<ServerUser> _users = new List<ServerUser>();
        private int nextId = 1;

        public int Connect()
        {
            ServerUser user = new ServerUser(nextId, OperationContext.Current);
            nextId++;
            _users.Add(user);

            return user.Id;
        }

        public void Disconnect(int id)
        {
            var user = _users.FirstOrDefault(t => t.Id == id);
            if (user != null) 
            {
                _users.Remove(user);
            }         
        }

        public void SendMessage(string message)
        {
            string currentTime = DateTime.Now.ToString();
            string currentMessage = currentTime + ": " + message;

            foreach (var user in _users)
            {
                user.OperationContext.GetCallbackChannel<IAnnunciatorServiceCallBack>().CallBackMessage(currentMessage);
            }
        }
    }
}
