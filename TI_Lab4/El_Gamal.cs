using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TI_Lab4
{
    public class El_Gamal
    {
        private int Q { get; set; }
        private int P { get; set; }
        private int H { get; set; }
        private int X { get; set; }
        private int K { get; set; }
        private int G { get; set; }
        private int Y { get; set; }
        public int R { get; set; }
        public int V { get; set; }
        private byte[] data { get; set; }

        public El_Gamal(int q, int p, int h, int x, int k, byte[] data)
        {
            if(IsPrime(q))
                Q = q;
            else
                throw new LogicException("Число q должно быть простое");

            if (IsPrime(p))
            {
                if ((p - 1) % q == 0)
                {
                    P = p;
                }                 
                else
                    throw new LogicException("Число q должно является делителем p-1");
            }       
            else 
                throw new LogicException("Число p должно быть простым");

            if (h > 1 && h < p - 1)
            {
                int g = FastExponentiation(p, h, (p - 1) / q);
                if (g > 1)
                {
                    G = g;
                    H = h;
                }
                else
                    throw new LogicException("Число g должно быть больше 1, выберите другое h");
            }
            else
                throw new LogicException("Число h должно быть больше 1 и меньше p-1");

            if(x > 0 && x < q)
                X = x;
            else 
                throw new LogicException("Число x должно быть больше 0 и меньше q");

            if (k > 0 && k < q)
                K = k;
            else
                throw new LogicException("Число k должно быть больше 0 и меньше q");

            this.data = data;
        }

        private bool IsPrime(int number)
        {
            if (number == 1 || number == 2)
                return true;

            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }

        private int FastExponentiation(BigInteger mod, BigInteger num, BigInteger deg)
        {
            BigInteger y = 1;
            while (deg != 0)
            {
                while (deg % 2 == 0)
                {
                    deg /= 2;
                    num = (num * num) % mod;
                }
                deg--;
                y = (y * num) % mod;
            }
            return (int)y;
        }

        public (int, int, int) SignatureMessage()
        {
            int y = FastExponentiation(P, G, X);
            Y = y;

            int hash = 100;
            foreach(byte b in data)
            {
                hash = FastExponentiation(Q, hash + b, 2);   
            }

            int r = FastExponentiation(Q, FastExponentiation(P, G, K), 1);
            R = r;
            int s = FastExponentiation(Q, FastExponentiation(Q, K, Q-2)*FastExponentiation(Q, hash + X*r, 1), 1);

            return (hash, r, s);
        }

        public bool CheckSignature(byte[] data, int r, int s)
        {
            int hash = 100;
            foreach (byte b in data)
            {
                hash = FastExponentiation(Q, hash + b, 2);
            }

            int w = FastExponentiation(Q, s, Q - 2);
            int u1 = FastExponentiation(Q, FastExponentiation(Q, hash, 1) * FastExponentiation(Q, w, 1), 1);
            int u2 = FastExponentiation(Q, FastExponentiation(Q, r, 1) * FastExponentiation(Q, w, 1), 1);
            int v = FastExponentiation(Q, FastExponentiation(P, FastExponentiation(P, G, u1)*FastExponentiation(P, Y, u2), 1), 1);
            V = v;

            if (v == r)
                return true;
            else 
                return false;
        } 
    }
}
