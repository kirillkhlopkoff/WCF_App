using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_Chat
{
    [ServiceBehavior(InstanceContextMode= InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;
        public int Connect(string name)
        {
            ServerUser user = new ServerUser() { 
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;

            SendMessage(user.Name + " подключился к чату!", 0);
            users.Add(user);
            return user.ID;
        }

        public void DisConnect(int id)
        {
            ServerUser user = users.FirstOrDefault(x => x.ID == id);
            if(user != null)
            {
                users.Remove(user);
                SendMessage(user.Name + " покинул чат!", 0);
            }
        }

        public void SendMessage(string message, int id)
        {
            foreach(var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                ServerUser user = users.FirstOrDefault(x => x.ID == id);
                if (user != null)
                {
                    answer += ": " + user.Name;
                }
                answer += message;
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MessageCallbeck(answer);
            }
        }
    }
}
