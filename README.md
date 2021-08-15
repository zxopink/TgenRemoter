# TgenRemoter
basically a TeamViewer application based on my protocol (TgenNetProtocol)

server side
--------------------------------------------------------------------------------------------------------------------------------

`starting a server`:
for the server side there's a ServerManager class which manages the server side listener.
to start the server all you need to do is create a new object of ServerManager type with the port you'd like the server to listen, then when use the method "start" to start the server.

`send a message`:
you can send a message to a specific client by using the "Send" method 
or you can send everyone a message by using the "SendToAll" method
or you can send to everyone except a specific client by using the "SendToAllExcept"

`receive a message`:
this one is fairly easy, a unique thing about this protocol is that the message receivement works by types and events and is being managed by attributes.
simply make a new method inside your class (make sure the class inherits from the "NetworkBehavour" class or "FormNetworkBehavour" if you work with forms) and put a "[ServerNetworkReciver]" attributes on it!
the method must return void and take one custom argument (whatever type you choose) and an option secondly type of integer argument in case you want the ID of the client who sent the message.
the method will be invoked whenever the type of it's first argument was recived, if the first argument of the method is type of object the method will be called everytime a message is Recived.

client side
--------------------------------------------------------------------------------------------------------------------------------

`starting a server`:
for the client side there's a ClientManager class which manages the client side listener.
to start the client all you need to do is create a new object of ClientManager type with the port and ip you'd like the client to connect the server, then when use the method "connect" to connect the server.

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
