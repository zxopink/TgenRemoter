# TgenRemoter
basically a TeamViewer app based on my protocol (TgenNetProtocol)  

The remoter's special features are:  
* Instances remote control  
* Drag and drop file sharing  
* Input control between clients  

This Remoter is made to heavily test the 'TgenNetProtocol' network layer and the serializer.  
The Remoter depends on the Tgen libraries, it must be referenced to the 'TgenNetProtocol' and 'TgenSerializer' DLLs to compile.  

The remoter has 2 sides, the server side and client side; both sides will be explained below:  

server side
--------------------------------------------------------------------------------------------------------------------------------
The server side is quite simple, it's a console app that runs in a single thread.  
The thread uses the "[TgenNetProtocol]" layer to incoming clients and sort their packets.  
The server hands each new client a special 'connection' code which can be used by other clients to connect and control the said client.  
once a user types another user's code, the server will make a new room for both clients and exchange the packets between the two.  
note that a server can host multiple rooms.  

* The server is a dedicated server  
* It accepts incoming client  
* It hands incoming clients a connection code  
* It pairs two clients into a room  
* And It manages the communication between the clients.  

client side
--------------------------------------------------------------------------------------------------------------------------------

The client side is a bit more complicated as it includes both a network layer and a GUI layer with several different windows.  

`Menu Window:`  
The menu is the first window to pop up when a client opens the app, this window will only open if the client manages to open the main server to aquire a connection code.  

The menu offers multiple setting options:  
	`Accept sent files:` using a checkbox you can check whether you want to download files sent by your room partner or decline them.  
	`File target:` If you decide to accept incoming files, you can choose in which folder to drop the incoming files.  
	* Settings are saved in a configurations folder


`Controller Window:`   
The controller window open once you write in another user's connection code and press the 'Connect' button in the menu.  
This window displays the other user's screen.  

while the controller's cursor is inside the window,  
every keyboard and mouse input signed in will be sent to the controlled user.


`Controlled Window:`  
The controller menu opens up when when another user who's connected to the dedicated server signs in your connection code and connects.
This window is small and boardless, it doesn't accept any input from the user except for files to share with the user's partner.


`File Sharing:`  
File works in both windows, to share a file the user simply needs to drag and drop the file into the form.
In case the user's partner has disabled file sharing in the settings, the user will be notified.  
* File sharing also works with folders.

notes
--------------------------------------------------------------------------------------------------------------------------------
please credit me if you use this protocol!
and please leave a review or report any bugs! it's my first github project and I want to improve so I can make more projects like this!

made by: Yoav Haik