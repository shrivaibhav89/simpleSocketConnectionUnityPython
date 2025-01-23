import socket
import random

# Server variables
turns = 5
scores = []

def start_server(host='127.0.0.1', port=8080):
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((host, port))
    server_socket.listen(5)
    print(f"Server listening on {host}:{port}")

    client_socket, address = server_socket.accept()
    print(f"Connection established with {address}")

    handle_client(client_socket)

def handle_client(client_socket):
    global turns, scores

    try:
        while True:
            data = client_socket.recv(1024).decode('utf-8').strip()
            if not data:
                break

            print(f"Received from client: {data}")

            # Check if the client requests a dice roll
            if data.lower() == "roll_dice":
                if turns > 0:
                    dice_roll = random.randint(1, 6)  # Simulate a dice roll (1-6)
                    scores.append(dice_roll)  # Save the dice roll
                    turns -= 1  # Decrease remaining turns

                    response = f"Dice roll: {dice_roll}, Remaining turns: {turns}\n"
                    print(response.strip())
                    client_socket.send(response.encode('utf-8'))
                else:
                    # All turns are complete, calculate the total score
                    total_score = sum(scores)
                    response = f"All turns used. Total score: {total_score}\n"
                    print(response.strip())
                    client_socket.send(response.encode('utf-8'))

                    # Reset the game for the next session
                    turns = 5
                    scores = []
            else:
                # Handle invalid commands
                client_socket.send("Invalid command. Use 'roll_dice' to roll the dice.\n".encode('utf-8'))

    except Exception as e:
        print(f"Error: {e}")
    finally:
        client_socket.close()
        print("Client disconnected.")

if __name__ == "__main__":
    start_server()
