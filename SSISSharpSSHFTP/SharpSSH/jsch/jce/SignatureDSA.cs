using System;

namespace Tamir.SharpSsh.jsch.jce
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

    public class SignatureDSA : Tamir.SharpSsh.jsch.SignatureDSA
    {

        //java.security.Signature signature;
        //  KeyFactory keyFactory;
        System.Security.Cryptography.DSAParameters DSAKeyInfo;
        System.Security.Cryptography.SHA1CryptoServiceProvider sha1;
        System.Security.Cryptography.CryptoStream cs;

        public void init()
        {
            sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            cs = new System.Security.Cryptography.CryptoStream(System.IO.Stream.Null, sha1, System.Security.Cryptography.CryptoStreamMode.Write);
        }
        public void setPubKey(byte[] y, byte[] p, byte[] q, byte[] g)
        {
            DSAKeyInfo.Y = Util.stripLeadingZeros(y);
            DSAKeyInfo.P = Util.stripLeadingZeros(p);
            DSAKeyInfo.Q = Util.stripLeadingZeros(q);
            DSAKeyInfo.G = Util.stripLeadingZeros(g);
        }
        public void setPrvKey(byte[] x, byte[] p, byte[] q, byte[] g)
        {
            DSAKeyInfo.X = Util.stripLeadingZeros(x);
            DSAKeyInfo.P = Util.stripLeadingZeros(p);
            DSAKeyInfo.Q = Util.stripLeadingZeros(q);
            DSAKeyInfo.G = Util.stripLeadingZeros(g);
        }

        public byte[] sign()
        {
            //byte[] sig=signature.sign();   
            cs.Close();
            System.Security.Cryptography.DSACryptoServiceProvider DSA = new System.Security.Cryptography.DSACryptoServiceProvider();
            DSA.ImportParameters(DSAKeyInfo);
            System.Security.Cryptography.DSASignatureFormatter DSAFormatter = new System.Security.Cryptography.DSASignatureFormatter(DSA);
            DSAFormatter.SetHashAlgorithm("SHA1");

            byte[] sig = DSAFormatter.CreateSignature(sha1);
            return sig;
        }
        public void update(byte[] foo)
        {
            //signature.update(foo);
            cs.Write(foo, 0, foo.Length);
        }

        public bool verify(byte[] sig)
        {
            cs.Close();

            BigInteger r, s, w, gu1p, yu2p, gu1yu2p, u1, u2, sha, v;

            BigInteger P = new BigInteger(DSAKeyInfo.P);
            BigInteger Q = new BigInteger(DSAKeyInfo.Q);
            BigInteger G = new BigInteger(DSAKeyInfo.G);
            BigInteger Y = new BigInteger(DSAKeyInfo.Y);

            // from PuTTY:
            /*
             * Commercial SSH (2.0.13) and OpenSSH disagree over the format
             * of a DSA signature. OpenSSH is in line with the IETF drafts:
             * it uses a string "ssh-dss", followed by a 40-byte string
             * containing two 160-bit integers end-to-end. Commercial SSH
             * can't be bothered with the header bit, and considers a DSA
             * signature blob to be _just_ the 40-byte string containing
             * the two 160-bit integers. We tell them apart by measuring
             * the length: length 40 means the commercial-SSH bug, anything
             * else is assumed to be IETF-compliant.
             */

            long i = 0;

            if (sig.Length != 40)
            {
                int n = (int)((sig[i++] << 24) & 0xff000000) | ((sig[i++] << 16) & 0x00ff0000) |
                    ((sig[i++] << 8) & 0x0000ff00) | ((sig[i++]) & 0x000000ff);

                if (n != 7 || Util.getString(sig, (int)i, 7) != "ssh-dss")
                    throw new System.Security.Cryptography.CryptographicException("Bad Data!\r\n");

                i += 7;

                n = (int)((sig[i++] << 24) & 0xff000000) | ((sig[i++] << 16) & 0x00ff0000) |
                    ((sig[i++] << 8) & 0x0000ff00) | ((sig[i++]) & 0x000000ff);

                if (n != 40 || i + 40 > sig.Length)
                    throw new System.Security.Cryptography.CryptographicException("Bad Data!\r\n");
            }

            // sig data 40 bytes (2 x 20-byte blocks)
            {
                byte[] tmp = new byte[20];
                Array.Copy(sig, i, tmp, 0, 20);

                r = new BigInteger(tmp);

                tmp = new byte[20];
                Array.Copy(sig, i + 20, tmp, 0, 20);

                s = new BigInteger(tmp);
            }

            /*
             * Step 1. w <- s^-1 mod q.
             */
            w = modinv(s, Q);

            /*
             * Step 2. u1 <- SHA(message) * w mod q.
             */
            if (sha1.Hash.Length != 20)
                throw new System.Security.Cryptography.CryptographicException("Bad Data!\r\n");

            sha = new BigInteger(sha1.Hash);

            u1 = modmul(sha, w, Q);

            /*
             * Step 3. u2 <- r * w mod q.
             */
            u2 = modmul(r, w, Q);

            /*
             * Step 4. v <- (g^u1 * y^u2 mod p) mod q.
             */
            gu1p = modpow(G, u1, P);
            yu2p = modpow(Y, u2, P);
            gu1yu2p = modmul(gu1p, yu2p, P);
            v = BigInteger.Modulus(gu1yu2p, Q);//v = modmul(gu1yu2p, One, Q);

            /*
             * Step 5. v should now be equal to r.
             */
            return bignum_cmp(v, r) == 0;
        }

        private BigInteger modinv(BigInteger a, BigInteger b) { return a.ModInverse(b); }
        private BigInteger modmul(BigInteger a, BigInteger b, BigInteger n) { return BigInteger.Modulus(a * b, n); }
        private BigInteger modpow(BigInteger a, BigInteger exp, BigInteger n) { return a.ModPow(exp, n); }
        private BigInteger.Sign bignum_cmp(BigInteger a, BigInteger b) { return a.Compare(b); }
        private BigInteger bigmod(BigInteger a, BigInteger b)
        {
            return BigInteger.Modulus(a, b);
        }

        /*
        public bool verify(byte[] sig)
        {			
            cs.Close();
            System.Security.Cryptography.DSACryptoServiceProvider DSA = new System.Security.Cryptography.DSACryptoServiceProvider();
            DSA.ImportParameters(DSAKeyInfo);
            System.Security.Cryptography.DSASignatureDeformatter DSADeformatter = new System.Security.Cryptography.DSASignatureDeformatter(DSA);
            DSADeformatter.SetHashAlgorithm("SHA1");

            long i=0;
            long j=0;
            byte[] tmp;

            //This makes sure sig is always 40 bytes?
            if(sig[0]==0 && sig[1]==0 && sig[2]==0)
            {
                long i1 = (sig[i++]<<24)&0xff000000;
                long i2 = (sig[i++]<<16)&0x00ff0000;
                long i3 = (sig[i++]<<8)&0x0000ff00;
                long i4 = (sig[i++])&0x000000ff;
                j = i1 | i2 | i3 | i4;

                i+=j;

                i1 = (sig[i++]<<24)&0xff000000;
                i2 = (sig[i++]<<16)&0x00ff0000;
                i3 = (sig[i++]<<8)&0x0000ff00;
                i4 = (sig[i++])&0x000000ff;
                j = i1 | i2 | i3 | i4;

                tmp=new byte[j]; 
                Array.Copy(sig, i, tmp, 0, j); sig=tmp;
            }
            bool res = DSADeformatter.VerifySignature(sha1, sig);
            return res;
        }
        */


    }

}
