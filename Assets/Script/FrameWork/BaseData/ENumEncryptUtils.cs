using System;

namespace com.initialworld.framework
{
    
    public class ENumEncryptUtils
    {
        private static byte[] _encryBytes;
        private static byte[] _decryBytes;
        private static bool _isInited;

        static void InitEncryBytes()
        {
            if (_isInited)
                return;
            Random random = new Random();
            _encryBytes = new byte[256];
            _decryBytes = new byte[256];
            for (int j = 0; j < 256; j++)
            {
                _encryBytes[j] = (byte)j;
                _decryBytes[j] = (byte)j;
            }
            byte i = (byte)random.Next(256);
            for (int j = 0; j < 256; j++)
            {
                byte temp = _encryBytes[i];
                _encryBytes[i] = _encryBytes[j];
                _encryBytes[j] = temp;
                i = (byte)random.Next(256);
            }

            for (int j = 0; j < 256; j++)
            {
                _decryBytes[_encryBytes[j]] = (byte)j;
            }

            _isInited = true;
        }

        public static byte[] DecryptDes(byte[] bytes)
        {
            InitEncryBytes();
            byte[] tempBytes = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                tempBytes[i] = _decryBytes[bytes[i]];
            }
            return tempBytes;
        }

        public static byte[] EncryptDes(byte[] bytes)
        {
            InitEncryBytes();
            byte[] tempBytes = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                tempBytes[i] = _encryBytes[bytes[i]];
            }
            return tempBytes;
        }

        
    }

    
}