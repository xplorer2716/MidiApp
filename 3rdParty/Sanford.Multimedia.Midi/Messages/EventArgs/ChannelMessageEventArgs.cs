using System;

namespace Sanford.Multimedia.Midi
{
    public class ChannelMessageEventArgs : EventArgs
    {
        private ChannelMessage message;

        public ChannelMessageEventArgs(ChannelMessage message)
        {
            this.message = message;
        }

        public ChannelMessage Message
        {
            get
            {
                return message;
            }
        }
    }
}