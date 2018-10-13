using System;

namespace com.initialworld.framework
{

    /// <summary>
    /// 加密数值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ENum<T>
    {
        private byte[] _val;
        private int _intKey;
        private int _intVal;

        public T Value
        {
            get
            {
                if (typeof(T) == typeof(int))
                {
                    var changeType = Convert.ChangeType(_intKey ^ _intVal, typeof(T));
                    if (changeType != null)
                        return (T)changeType;
                    throw new Exception("Impossible!");
                }
                return GetVal(_val);
            }
            set
            {
                if (typeof(T) == typeof(int))
                {
                    _intKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                    var intValTemp = Convert.ChangeType(value, typeof(int));
                    if (intValTemp != null)
                    {
                        _intVal = _intKey ^ (int)intValTemp;
                    }
                }
                else
                    SetValue(value, out _val);
            }
        }

        public ENum(T val)
            : this()
        {
            Value = val;
        }

        static T GetVal(byte[] value)
        {
            byte[] temp = ENumEncryptUtils.DecryptDes(value);
            var result = new object();
            if (typeof(T) == typeof(long))
            {
                result = BitConverter.ToInt64(temp, 0);
            }
            if (typeof(T) == typeof(float))
            {
                result = BitConverter.ToSingle(temp, 0);
            }
            if (typeof(T) == typeof(double))
            {
                result = BitConverter.ToDouble(temp, 0);
            }
            var valTemp = Convert.ChangeType(result, typeof(T));
            if (valTemp != null) return (T)valTemp;
            throw new Exception("Impossible!");
        }

        static void SetValue(T value, out byte[] val)
        {
            byte[] temp = new byte[] { };
            if (typeof(T) == typeof(long))
            {
                var lVal = Convert.ChangeType(value, typeof(long));
                if (lVal != null)
                {
                    temp = BitConverter.GetBytes((long)lVal);
                }
            }
            else if (typeof(T) == typeof(float))
            {
                var lVal = Convert.ChangeType(value, typeof(float));
                if (lVal != null)
                {
                    temp = BitConverter.GetBytes((float)lVal);
                }
            }
            else if (typeof(T) == typeof(double))
            {
                var lVal = Convert.ChangeType(value, typeof(double));
                if (lVal != null)
                {
                    temp = BitConverter.GetBytes((double)lVal);
                }
            }
            val = ENumEncryptUtils.EncryptDes(temp);
        }

    }
}