using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TCPClient
{
    class Program
    {
        private const int serverPort = 10572;
        private const string serverAddress = "127.0.0.1";

        static void Main(string[] args)
        {
            try
            {

            
            TcpClient client = new TcpClient();
            Socket sock = client.Client;

            byte[] receiveBuff;

            Console.Write("Enter a request: \n1\tSTART\n2\tCLOSE\n3\tAUTH\n4\tBALANCE\n5\tDEBIT\n6\tCREDIT\n");

            //client input
            string input = Console.ReadLine();
            bool start = true;

            while (start)
            {


                switch (input)
                {
                    case "1": //START
                        client.Connect(serverAddress, serverPort);
                        
                        break;
                    case "2": //CLOSE
                            
                            start = false;
                            
                            break;
                    case "3": //AUTH
                        Console.WriteLine("Please enter PIN: ");
                        string PIN = Console.ReadLine();

                        //send client input to server
                        byte[] requestBuff = Encoding.ASCII.GetBytes("AUTH " + PIN);
                        sock.Send(requestBuff);
                        

                        //recieve message from server
                        receiveBuff = new byte[1024];
                        int bytesRec = sock.Receive(receiveBuff);
                        string response = Encoding.ASCII.GetString(receiveBuff, 0, bytesRec);

                        if (response == "OK")
                        {
                            Console.WriteLine("Authenticated");
                        }
                        else Console.WriteLine("NOT Authenticated");

                        break;
                    case "4": //BALANCE
                        //send client input to server
                        requestBuff = Encoding.ASCII.GetBytes("BALANCE");
                        sock.Send(requestBuff);

                        //recieve message from server
                        receiveBuff = new byte[1024];
                        bytesRec = sock.Receive(receiveBuff);
                        response = Encoding.ASCII.GetString(receiveBuff, 0, bytesRec);

                        if (response.StartsWith("OK"))
                        {
                            Console.WriteLine(response);
                        }
                        else Console.WriteLine("NOT Authenticated");

                        break;
                    case "5": //DEBIT
                       Console.WriteLine("Please provide amount: ");
                        input = Console.ReadLine();

                        //send client input to server
                        requestBuff = Encoding.ASCII.GetBytes("DEBIT " + input);
                        sock.Send(requestBuff);

                        //recieve message from server
                        receiveBuff = new byte[1024];
                        bytesRec = sock.Receive(receiveBuff);
                        response = Encoding.ASCII.GetString(receiveBuff, 0, bytesRec);

                        if (response.StartsWith("OK"))
                        {
                            Console.WriteLine("Debit Successful!" + response);
                        }
                        else Console.WriteLine("Debit Fail\n" + response);
                        break;

                    case "6": //CREDIT
                        Console.WriteLine("Please provide amount: ");
                        input = Console.ReadLine();

                        //send client input to server
                        requestBuff = Encoding.ASCII.GetBytes("CREDIT " + input);
                        sock.Send(requestBuff);

                        //recieve message from server
                        receiveBuff = new byte[1024];
                        bytesRec = sock.Receive(receiveBuff);
                        response = Encoding.ASCII.GetString(receiveBuff, 0, bytesRec);

                        if (response.StartsWith("OK"))
                        {
                            Console.WriteLine("Credit Successful!" + response);
                        }
                        else Console.WriteLine("Credit Fail\n" + response);
                        break;

                    default:
                        Console.WriteLine("Invalid comand");
                        start = false;
                        break;

                }
                Console.WriteLine("Please enter command: ");
                input = Console.ReadLine();

            }

            }catch(Exception e)
            {
                Console.WriteLine("ERROR" + e.ToString());
            }

        }
    }
}
