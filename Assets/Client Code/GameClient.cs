using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    public Button sendButton;
    public InputField inputField;

    private void OnEnable()
    {
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }
    private void OnDisable()
    {
        sendButton.onClick.RemoveListener(OnSendButtonClicked);

    }
    private void OnSendButtonClicked()
    {
        RollDice();

    }
    private void Start()
    {
        ConnectToServer("127.0.0.1", 8080); // Match the IP and port of your Python server
    }

    private void ConnectToServer(string serverIp, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIp, port);
            stream = client.GetStream();

            Debug.Log("Connected to server!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection error: {e.Message}");
        }
    }

    public void RollDice()
    {
        if (stream != null && stream.CanWrite)
        {
            try
            {
                // Send the "roll_dice" command to the server
                string message = "roll_dice\n"; // Append newline character
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("Sent to server: " + message.Trim());

                // Wait for the server's response
                ReceiveMessageFromServer();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error sending message: {e.Message}");
            }
        }
    }

    private void ReceiveMessageFromServer()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead = 0;

            StringBuilder responseBuilder = new StringBuilder();

            // Read until newline character is found
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                responseBuilder.Append(chunk);
            } while (!responseBuilder.ToString().Contains("\n"));

            string response = responseBuilder.ToString().Trim();
            Debug.Log("Received from server: " + response);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving message: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }
}
