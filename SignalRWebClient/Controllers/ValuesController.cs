using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRWebClient.Hubs;

namespace SignalRWebClient.Controllers
{
    public class MessageViewModel
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public MessageViewModel(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
    
    public class MessagesViewModel
    {
        // public List<MessageViewModel> Items { get; set; }
        public List<string> Items { get; set; }

        public MessagesViewModel()
        {
            Items = new List<string>();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //private readonly IHubContext<ClientChatHub> HubContext;
        //public ValuesController(IHubContext<ClientChatHub> hubcontext)
        //{
        //    HubContext = hubcontext;
        //}

        HubConnection connection;
        MessagesViewModel messagesList = new MessagesViewModel();


        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // this.HubContext.Clients.All.SendAsync("SendMessage", "nesto");

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/ChatHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            //var signalRConnection = new HubConnectionBuilder()
            //    .WithUrl("" + "/hubs/integrationserver", options =>
            //    {
            //        options.Headers.Add("Authorization", "Basic " + Header);
            //    }).Build();
            //signalRConnection.On<string, string>("ReceiveMessage", (entityName, json) =>
            //{
            //});

            ConnectToServer();
            SendMessage("Muhamed", "From Client");

            return new string[] { "value1", "value2" };
        }


        public async void ConnectToServer()
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                //this.Dispatcher.Invoke(() =>
                //{
                //    var newMessage = $"{user}: {message}";
                //    messagesList.Items.Add(newMessage);
                //});
                var newMessage = $"{user}: {message}";
                messagesList.Items.Add(newMessage);
                PrintMessages();
            });

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async void SendMessage(string user, string message)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", user, message);
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void PrintMessages()
        {
            foreach (var message in messagesList.Items)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
