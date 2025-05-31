using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class MockServerStatusChecker
{
    static void Main(string[] args)
    {
        TcpListener? server = null;
        try
        {
            // Set the TcpListener on port 999
            Int32 port = 999;
            IPAddress localAddr = IPAddress.Any;
            
            // Create and start the TCP listener
            server = new TcpListener(localAddr, port);
            server.Start();
            
            Console.WriteLine("Mock Server Status Checker running on port 999");
            Console.WriteLine("Press Ctrl+C to stop the server");
            
            // Enter the listening loop
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                
                // Perform a blocking call to accept requests
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected to a client!");
                
                // Create a thread to handle this client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients
            server?.Stop();
        }
        
        Console.WriteLine("Server stopped");
    }
    
    static void HandleClient(object? obj)
    {
        if (obj == null) return;
        
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        
        try
        {
            // Buffer for reading data
            byte[] bytes = new byte[8]; // Request is 8 bytes
            int bytesRead;
            
            while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                if (bytesRead == 8)
                {
                    // Extract the request ID (bytes 6-7)
                    ushort requestId = BitConverter.ToUInt16(bytes, 6);
                    
                    // Extract IP and port for logging
                    uint ipAddress = BitConverter.ToUInt32(bytes, 0);
                    ushort port = BitConverter.ToUInt16(bytes, 4);
                    
                    // Convert IP to string
                    IPAddress ip = new IPAddress(ipAddress);
                    
                    // Create a response (always success)
                    byte[] response = new byte[3];
                    BitConverter.GetBytes(requestId).CopyTo(response, 0);
                    response[2] = 1; // 1 = success
                    
                    // Send back a response
                    stream.Write(response, 0, response.Length);
                    
                    Console.WriteLine($"Processed request for {ip}:{port}, ID: {requestId}, responded with success");
                }
                else
                {
                    Console.WriteLine($"Received unexpected data length: {bytesRead} bytes");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e.ToString());
        }
        finally
        {
            // Close the connection
            client.Close();
        }
    }
}
