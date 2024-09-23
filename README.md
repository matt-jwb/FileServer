# FileServer
This is the server for my file storage system. I made this system to solidify my skills in Client/Server systems in C#. I wanted to see how I could manage file transfers without using FTP.

This server is capable of concurrently handling multiple clients and processing their requests simultaneously. The server listens for TCP requests on port 4444 and will then handle incoming requests to Upload, Retrieve or List all files.

## Usage
Open the solution in Visual Studio and click run. The console will display an IP and port which the client can connect to. A ServerStorage folder will be created at C:\MattFileSystem\ServerStorage. You can download the client to interact with this server from https://github.com/matt-jwb/FileClient
