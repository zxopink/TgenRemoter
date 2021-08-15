using TgenNetProtocol;

namespace TgenRemoterServer
{
    class Program : NetworkBehavour
    {
        static void Main(string[] args)
        {
            App app = new App(); //Start automatically
            while (true) { /*Keep server up*/ }
        }
    }
}
