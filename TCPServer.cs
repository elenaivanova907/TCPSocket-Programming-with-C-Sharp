using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace TCPServer
{

    class Account
    {
        public string accnum;
        public string name;
        public string PIN;
        public double balance;
        

        public Account(string accnum, string name, string PIN, double balance)
        {
            this.accnum = accnum;
            this.name = name;
            this.PIN = PIN;
            this.balance = balance;

        }

        
    }
    class Program
    {
        private const int port = 10572;

        static Account[] accounts = {
                    new Account("1256985632965874", "Elena Ivanova", "5694", 5000),
                    new Account("5236945269751302", "Dimitar Kirov", "1520", 3000),
                    new Account("1204593015671356", "Vladimir Georgiev", "8546", 10000),
                    new Account("3201569014358762", "Vesela Koleva", "4219", 9000),
                    new Account("7542630264809642", "Nikola Kolev", "5743", 12000),
        };



        public static Account CheckPIN(string PIN, Account[] accounts)
        {
            //check PIN
            foreach (Account account in accounts)
            {
                if (account.PIN == PIN)
                {
                    return account;
                }
            }
            return null;
        }




        static void Main(string[] args)
        {
      
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
             

        Console.WriteLine("Started listening ...");

            while (true)
            {
                // wait for a client connection
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("client.Connected: " + client.Connected);
                // handle the communication with the client in a separate thread
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
            }
        }

        public static void HandleClient(object param)
        {

            try
            {

            TcpClient client = (TcpClient)param;
            Socket sock = client.Client;
            string clientId = sock.RemoteEndPoint.ToString();

            Console.WriteLine("Accepted client connection from {0}", clientId);
                bool clientAuth = false;
                Account accAuth = null;

                byte[] bytes;
                int bytesRec;
                string request ="";
                string response ="";
                byte[] replyBuffer;

            while (true)
            {
                    
                    //recieve message
                    bytes = new byte[1024];
                bytesRec = sock.Receive(bytes);
                request = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                response = "";
                    

                    if (request != null)
                    {
                        Console.WriteLine("Received: " + request);

                        if (request.StartsWith("AUTH"))
                        {

                                string PIN = request.Split(" ")[1];
                                accAuth = CheckPIN(PIN, accounts);

                                if (accAuth != null)
                                {
                                    clientAuth = true;
                                    //send response
                                    response = "OK";
                                    replyBuffer = Encoding.ASCII.GetBytes(response);
                                    sock.Send(replyBuffer);

                                }
                                else
                                {
                                    clientAuth = false;
                                    //send response
                                    response = "NOTOK";
                                    replyBuffer = Encoding.ASCII.GetBytes(response);
                                    sock.Send(replyBuffer);

                                }

                        }
                        if(request == "BALANCE")
                        {
                            if (clientAuth)
                            {
                               
                                //send response
                                response = "OK\nName: " + accAuth.name + " Number: " + accAuth.accnum + " Balance: " + accAuth.balance;
                                replyBuffer = Encoding.ASCII.GetBytes(response);
                                sock.Send(replyBuffer);

                            }
                            else
                            {
                                Console.WriteLine("ERROR You are not Authenticated!");
                                //send response
                                response = "NOTOK";
                                replyBuffer = Encoding.ASCII.GetBytes(response);
                                sock.Send(replyBuffer);
                            }
                                
                            
                        }
                        if (request.StartsWith("DEBIT"))
                        {

                            if (clientAuth)
                            {
                                double amount = double.Parse(request.Split(" ")[1]);

                                if (accAuth.balance >= amount)
                                {
                                    accAuth.balance = accAuth.balance - amount;
                                    //send response
                                    response = "OK. Now the Account holds: " + accAuth.balance;
                                    replyBuffer = Encoding.ASCII.GetBytes(response);
                                    sock.Send(replyBuffer);
                                }
                                else
                                {
                                    //send response
                                    response = "NOTOK";
                                    replyBuffer = Encoding.ASCII.GetBytes(response);
                                    sock.Send(replyBuffer);
                                }

                            }
                            else
                            {
                                Console.WriteLine("ERROR You are not Authenticated!");
                                //send response
                                response = "NOT AUTHENTICATED";
                                replyBuffer = Encoding.ASCII.GetBytes(response);
                                sock.Send(replyBuffer);
                            }
                            
                                
                            
                        }
                        if (request.StartsWith("CREDIT"))
                        {
                            if (clientAuth)
                            {
                                double amount = double.Parse(request.Split(" ")[1]);
                                accAuth.balance = accAuth.balance + amount;
                                //send response
                                response = "OK. Now the Account holds: " + accAuth.balance;
                                replyBuffer = Encoding.ASCII.GetBytes(response);
                                sock.Send(replyBuffer);
                            }
                            else
                            {
                                Console.WriteLine("ERROR You are not Authenticated!");
                                //send response
                                response = "NOT AUTHENTICATED";
                                replyBuffer = Encoding.ASCII.GetBytes(response);
                                sock.Send(replyBuffer);
                            }
                           
                               
                            
                        }else if(!(request.StartsWith("CREDIT")|| request.StartsWith("DEBIT")|| request.StartsWith("AUTH")|| request.StartsWith("BALANCE")))
                        {
                            Console.WriteLine("ERROR Unkown request");
                        }

                    }
            }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.ToString());
            }

        }
    }

    
    
}

