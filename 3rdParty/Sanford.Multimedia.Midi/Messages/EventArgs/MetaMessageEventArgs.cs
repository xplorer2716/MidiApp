using System;

namespace Sanford.Multimedia.Midi
{
    public class MetaMessageEventArgs : EventArgs
    {
        private MetaMessage message;

        public MetaMessageEventArgs(MetaMessage message)
        {
            this.message = message;
        }

        public MetaMessage Message
        {
            get
            {
                return message;
            }
        }
    }
}