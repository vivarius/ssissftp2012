using System;
using System.IO;

namespace Tamir.SharpSsh.jsch
{
    /* -*-mode:java; c-basic-offset:2; -*- */
    /*
    Copyright (c) 2002,2003,2004 ymnk, JCraft,Inc. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
        this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright 
        notice, this list of conditions and the following disclaimer in 
        the documentation and/or other materials provided with the distribution.

    3. The names of the authors may not be used to endorse or promote products
        derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
    INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
    FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
    INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
    INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
    OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
    EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    */

    public class JSch
    {

        static System.Collections.Hashtable config;

        public static void Init()
        {
            config = new System.Collections.Hashtable
                         {
                             {"kex", "diffie-hellman-group1-sha1,diffie-hellman-group-exchange-sha1"},
                             {"server_host_key", "ssh-rsa,ssh-dss"},
                             {"cipher.s2c", "3des-cbc,aes128-cbc"},
                             {"cipher.c2s", "3des-cbc,aes128-cbc"},
                             {"mac.s2c", "hmac-md5,hmac-sha1"},
                             {"mac.c2s", "hmac-md5,hmac-sha1"},
                             {"compression.s2c", "none"},
                             {"compression.c2s", "none"},
                             {"lang.s2c", ""},
                             {"lang.c2s", ""},
                             {"diffie-hellman-group-exchange-sha1", "Tamir.SharpSsh.jsch.DHGEX"},
                             {"diffie-hellman-group1-sha1", "Tamir.SharpSsh.jsch.DHG1"},
                             {"dh", "Tamir.SharpSsh.jsch.jce.DH"},
                             {"3des-cbc", "Tamir.SharpSsh.jsch.jce.TripleDESCBC"},
                             {"hmac-sha1", "Tamir.SharpSsh.jsch.jce.HMACSHA1"},
                             {"hmac-sha1-96", "Tamir.SharpSsh.jsch.jce.HMACSHA196"},
                             {"hmac-md5", "Tamir.SharpSsh.jsch.jce.HMACMD5"},
                             {"hmac-md5-96", "Tamir.SharpSsh.jsch.jce.HMACMD596"},
                             {"sha-1", "Tamir.SharpSsh.jsch.jce.SHA1"},
                             {"md5", "Tamir.SharpSsh.jsch.jce.MD5"},
                             {"signature.dss", "Tamir.SharpSsh.jsch.jce.SignatureDSA"},
                             {"signature.rsa", "Tamir.SharpSsh.jsch.jce.SignatureRSA"},
                             {"keypairgen.dsa", "Tamir.SharpSsh.jsch.jce.KeyPairGenDSA"},
                             {"keypairgen.rsa", "Tamir.SharpSsh.jsch.jce.KeyPairGenRSA"},
                             {"random", "Tamir.SharpSsh.jsch.jce.Random"},
                             {"aes128-cbc", "Tamir.SharpSsh.jsch.jce.AES128CBC"},
                             {"StrictHostKeyChecking", "ask"}
                         };

            //  config.Add("kex", "diffie-hellman-group-exchange-sha1");
            //config.Add("server_host_key", "ssh-dss,ssh-rsa");

            //			config.Add("cipher.s2c", "3des-cbc,blowfish-cbc");
            //			config.Add("cipher.c2s", "3des-cbc,blowfish-cbc");

            //			config.Add("mac.s2c", "hmac-md5,hmac-sha1,hmac-sha1-96,hmac-md5-96");
            //			config.Add("mac.c2s", "hmac-md5,hmac-sha1,hmac-sha1-96,hmac-md5-96");

            //config.Add("blowfish-cbc",  "Tamir.SharpSsh.jsch.jce.BlowfishCBC");

            //config.Add("zlib",          "com.jcraft.jsch.jcraft.Compression");
        }

        internal Tamir.SharpSsh.java.util.Vector pool = new Tamir.SharpSsh.java.util.Vector();
        internal Tamir.SharpSsh.java.util.Vector identities = new Tamir.SharpSsh.java.util.Vector();
        //private KnownHosts known_hosts=null;
        private HostKeyRepository known_hosts = null;

        public JSch()
        {
            //known_hosts=new KnownHosts(this);
            if (config == null)
                Init();
        }

        public Session getSession(String username, String host) { return getSession(username, host, 22); }
        public Session getSession(String username, String host, int port)
        {
            Session session = new Session(this);
            session.setUserName(username);
            session.setHost(host);
            session.setPort(port);
            pool.Add(session);
            return session;
        }

        internal bool RemoveSession(Session session)
        {
            lock (pool)
            {
                return pool.remove(session);
            }
        }

        public void SetHostKeyRepository(HostKeyRepository foo)
        {
            known_hosts = foo;
        }
        public void setKnownHosts(String foo)
        {
            if (known_hosts == null) known_hosts = new KnownHosts(this);
            if (known_hosts is KnownHosts)
            {
                lock (known_hosts)
                {
                    ((KnownHosts)known_hosts).setKnownHosts(foo);
                }
            }
        }
        public void setKnownHosts(StreamReader foo)
        {
            if (known_hosts == null) known_hosts = new KnownHosts(this);
            if (known_hosts is KnownHosts)
            {
                lock (known_hosts)
                {
                    ((KnownHosts)known_hosts).setKnownHosts(foo);
                }
            }
        }
        /*
        HostKeyRepository getKnownHosts(){ 
            if(known_hosts==null) known_hosts=new KnownHosts(this);
            return known_hosts; 
        }
        */
        public HostKeyRepository getHostKeyRepository()
        {
            if (known_hosts == null) known_hosts = new KnownHosts(this);
            return known_hosts;
        }
        /*
        public HostKey[] getHostKey(){
            if(known_hosts==null) return null;
            return known_hosts.getHostKey(); 
        }
        public void removeHostKey(String foo, String type){
            removeHostKey(foo, type, null);
        }
        public void removeHostKey(String foo, String type, byte[] key){
            if(known_hosts==null) return;
            known_hosts.remove(foo, type, key); 
        }
        */
        public void addIdentity(String foo)
        {
            addIdentity(foo, (String)null);
        }
        public void addIdentity(String foo, String bar)
        {
            Identity identity = new IdentityFile(foo, this);
            if (bar != null) identity.setPassphrase(bar);
            identities.Add(identity);
        }
        internal String getConfig(String foo) { return (String)(config[foo]); }

        private System.Collections.ArrayList proxies;
        void setProxy(String hosts, Proxy proxy)
        {
            String[] patterns = Util.split(hosts, ",");
            if (proxies == null) { proxies = new System.Collections.ArrayList(); }
            lock (proxies)
            {
                for (int index = 0; index < patterns.Length; index++)
                {
                    string t = patterns[index];
                    if (proxy == null)
                    {
                        proxies[0] = null;
                        proxies[0] = System.Text.Encoding.Default.GetBytes(t);
                    }
                    else
                    {
                        proxies.Add(System.Text.Encoding.Default.GetBytes(t));
                        proxies.Add(proxy);
                    }
                }
            }
        }
        internal Proxy getProxy(String host)
        {
            if (proxies == null) return null;
            byte[] _host = System.Text.Encoding.Default.GetBytes(host);
            lock (proxies)
            {
                for (int i = 0; i < proxies.Count; i += 2)
                {
                    if (Util.glob(((byte[])proxies[i]), _host))
                    {
                        return (Proxy)(proxies[i + 1]);
                    }
                }
            }
            return null;
        }
        internal void removeProxy()
        {
            proxies = null;
        }

        public static void setConfig(System.Collections.Hashtable foo)
        {
            lock (config)
            {
                System.Collections.IEnumerator e = foo.Keys.GetEnumerator();
                while (e.MoveNext())
                {
                    String key = (String)(e.Current);
                    config.Add(key, (String)(foo[key]));
                }
            }
        }
    }

}
