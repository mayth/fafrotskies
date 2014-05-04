using System;

namespace Fafrotskies
{
    public struct Stage
    {
        private readonly int numberOfCases;
        public int NumberOfCases
        {
            get { return numberOfCases; }
        }

        private readonly string enterMessage;
        public string EnterMessage
        {
            get { return enterMessage; }
        }

        private readonly string clearMessage;
        public string ClearMessage
        {
            get { return clearMessage; }
        }

        public Stage(int num, string enterMessage, string clearMessage)
        {
            this.numberOfCases = num;
            this.enterMessage = enterMessage;
            this.clearMessage = clearMessage;
        }
    }
}

