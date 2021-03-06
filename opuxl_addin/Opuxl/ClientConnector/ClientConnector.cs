﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ClientConnector
{
    // Currency and Logical are not yet supported
    public enum ExcelType { TEXT, NUMBER, DATETIME, LOGICAL }

    public class SocketConnector
    {
        /// <summary>
        /// The Message Type of the Initial Message to the ServerSocket to retrieve all available functions.
        /// </summary>
        public const string INITIALIZE = "INITIALIZE";

        private IPAddress address;
        private int port;

        public SocketConnector()
        {
            IPHostEntry entry = Dns.GetHostEntry(ClientConfiguration.Host);
            address = entry.AddressList[0];           
            port = ClientConfiguration.Port;
        }

        /// <summary>
        /// Executes the function described with the given function payload towards the configured serverSocket.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public ResponsePayload executeFn(FunctionPayload payload)
        {
            TcpClient tcpClient = new TcpClient();
            ResponsePayload received = null;

            try
            {
                tcpClient.Connect(address, port);
                using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
                using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
                {

                    SendMessage(JsonConvert.SerializeObject(payload), streamWriter);

                    String response = streamReader.ReadLine();

                    Debug.WriteLine("Received response ...");
                    Debug.WriteLine(response);

                    received = JsonConvert.DeserializeObject<ResponsePayload>(response);

                    Debug.WriteLine("Received Message: " + received.ToString());

                    streamWriter.Close();
                    streamReader.Close();
                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on connecting to client: " + ex);
            }



            return received;
        }

        private void SendMessage(String message, StreamWriter streamWriter)
        {
            Debug.WriteLine("Sending Message ...");
            Debug.WriteLine(message);
            if (message != "")
            {
                streamWriter.WriteLine(message);
                streamWriter.Flush();
            }
        }
    }
}
