using System;

namespace TgenRemoter
{
    public class NetworkCodes
    {
        [Serializable]
        public class PassCode
        {
            public string passCode;
            public PassCode(string passCode)
            {
                this.passCode = passCode;
            }
        }

        [Serializable]
        public class StartEvent
        {
            public string message;
            public StartEvent(string message)
            {
                this.message = message;
            }
        }
    }
}
