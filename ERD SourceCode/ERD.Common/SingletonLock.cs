using System;

namespace ERD.Common
{
    public sealed class SingletonLock
    {
        private static SingletonLock instance = null;

        private static readonly object singletonCreateLock = new Object();

        public static SingletonLock Instance
        {
            get
            {
                if (SingletonLock.instance == null)
                {
                    lock(SingletonLock.singletonCreateLock)
                    {
                        SingletonLock.instance = new SingletonLock();

                        SingletonLock.instance.LockObject = new object();
                    }
                }

                return SingletonLock.instance;
            }
        }

        public object LockObject { get; set; }
    }
}
