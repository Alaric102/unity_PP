import pickle
import socket

HOST = '10.30.33.12'
PORT = 12345
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
s.listen(1)
conn, addr = s.accept()
print ('Connected by', addr)
while 1:
    data = conn.recv(4096)
    data_arr = pickle.loads(data)
    print(data_arr)
conn.close()
