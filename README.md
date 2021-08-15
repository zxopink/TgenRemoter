# TgenRemoter
basically a TeamViewer app based on my protocol (TgenNetProtocol)  

The remoter's special features are:<br/> 
instances remote control,<br/>
drag and drop file sharing,<br/>
input control over clients<br/>

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

The server is a dedicated server,  
it accepts incoming client,  
hands incoming clients a connection code,  
pairs two clients into a room  
and manages the communication between the clients.  

client side
--------------------------------------------------------------------------------------------------------------------------------

The client side is a bit more complicated as it includes both a network layer and a GUI layer with several different windows.  

`Menu Window`:
The menu is the first window to pop up when a client opens the app, this window will only open if the client manages to open the main server to aquire a connection code.  
The menu offers multiple settings options

`send a message`:
you can send a message to the server by using the "Send" method.

`receive a message`:
this one is fairly easy too and similar to the server.
just like the server you need to make a new method inside your class (make sure the class inherits from the "NetworkBehavour" class or "FormNetworkBehavour" if you work with forms) and put a "[ClientNetworkReciver]" attributes on it!
the method must return void and take one custom argument (whatever type you choose).
the method will be invoked whenever the type of it's first argument was recived by the server, if the first argument of the method is type of object the method will be called everytime a message is Recived.

notes
--------------------------------------------------------------------------------------------------------------------------------
please credit me if you use this protocol!
and please leave a review or report any bugs! it's my first github project and I want to improve so I can make more projects like this!

made by: Yoav Haik