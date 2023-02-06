# TCPSocket-Programming-with-C-Sharp

How the program works: in TCPClient
At the beginning the user is presented with 6 different options to choose from. 

        Console.Write("Enter a request: \n1\tSTART\n2\tCLOSE\n3\tAUTH\n4\tBALANCE\n5\tDEBIT\n6\tCREDIT\n");

The user has to enter 1 in order to start the connection. Then the user can have to choose option 3 AUTH in order to have access to balance, credit and debit. Without authentication, they will still execute, no exception will be thrown but a message will be displayed saying that the user is not authenticated. Choosing option 3 and providing a valid pin that matches 1 of the existing records, the user will be able to see balance and make credit/debit transactions. When choosing balance the user is able to see the account details. After a successful transaction the user can se the updated balance of the account. The application allows multiple authentication in other to access multiple accounts, meaning that the user can “switch” through the different accounts, if the user provides in option 3 AUTH the right PIN. 
Note: I was not sure if the application was supposed to do this but it makes more sense to me this account “switching” to be available because in order to access another account, the user would have to restart the application.
TCPClient is organized in a giant switch statement , where every case matches the user input the commands executed on the server side and displays the appropriate message.

        while (start)
            {
                switch (input)
                {
                    case "1": //START
                        client.Connect(serverAddress, serverPort);
                        
                        break;
                    case "2": //CLOSE
                            
                            start = false;
                            client.GetStream().Close();
                            client.Close();
                            break;
                    case "3": //AUTH
                        Console.WriteLine("Please enter PIN: ");
                        string PIN = Console.ReadLine();

                        //send client input to server                        
				…
                        //recieve message from server
                        	…
                        if (response == "OK")
                        {
                            Console.WriteLine("Authenticated");
                        }
                        else Console.WriteLine("NOT Authenticated");

                        break;
                    case "4": //BALANCE
                        //send client input to server
                        …
                        //recieve message from server
                        …
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
                        	…
                        //recieve message from server
                          …
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
				                …
                        //recieve message from server
                        …
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
                
How it works: in TCPServer
Every connection is executed in its own thread, allowing operation on multiple ports simultaneously. I have created an additional class called Account to create an array of Account objects to store the 5 pre-defined accounts, according to the task description. The function HandleClient is the “brains” of the server. It handles the requests from the client and checks for an appropriate response – this is done through a series of if-statements. All of the checks follow a simple logic, all of them first check if the user is authenticated – if not an error message is sent to the client, if the user is authenticated then the user is given access to balance information/ debit transaction or credit transaction.

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
                            
                            
For some of the if-statements I have used the build-in string function Split(), such as when checking if the request is “AUTH”, “CREDIT” or “DEBIT”. When one of these 3 requests are sent to the server a number (PIN or money amount) is sent with the command as 1 message. I use Split() to separate the actual command from that number that I also need, and assign the number using a Parse function to a double for further computations.

          double amount = double.Parse(request.Split(" ")[1]);
          string PIN = request.Split(" ")[1]; 

I have decided to define a function responsible for PIN authentication. This function returns the Account object that matches the PIN number, then I use this object to access the account details such as balance, name, account number, etc.

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
