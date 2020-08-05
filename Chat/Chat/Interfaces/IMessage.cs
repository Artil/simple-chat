using ChatCore.Models;
using ChatDbCore.ChatModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Interfaces
{
    public interface IMessage
    {
        IEnumerable<MessageModel> GetMessagesByChatId(string chatId, int position = 0);
        Task<MessageModel> SendMessage(MessageModel message);
        Task DeleteMessage(MessageModel message, bool isForMe = false);
        Task<MessageModel> UpdateMessage(MessageModel newMessage);
        MessageModel GetLastMessage(string messageId);
        Task<MessageModel> ForwardMessage(string chatId, string messageId);
        Task ReadMessages(string chatId);
    }
}
