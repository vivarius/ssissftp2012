using System;

namespace Tamir.SharpSsh.jsch
{
    public class SftpException : java.Exception
    {
        public int id;

        public SftpException(int id, String message)
            : base(message)
        {
            this.id = id;
        }

        public SftpException(int id, string message, Exception innerException)
            : base(message, innerException)
        {
            this.id = id;
        }

        public override String ToString()
        {
            return this.Message;
        }
    }
}
