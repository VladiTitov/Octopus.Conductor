﻿namespace Octopus.Conductor.Infrastructure.RabbitMQ.Config
{
    public class Connection
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
