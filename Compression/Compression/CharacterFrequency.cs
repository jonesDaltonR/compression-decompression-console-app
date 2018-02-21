using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compression
{
    public class CharacterFrequency : IComparable
    {
        public char Character { get; set; }
        public int Frequency { get; set; }

        public CharacterFrequency(char c)
        {
            Character = c;
        }
        public CharacterFrequency(char c,int freq)
        {
            Character = c;
            Frequency = freq;
        }

        public void increment()
        {
            Frequency++;
        }
        
        public int CompareTo(object obj)
        {
            if(Frequency > (obj as CharacterFrequency).Frequency)
            {
                return 1;
            }
            else if(Frequency < (obj as CharacterFrequency).Frequency)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Character;
        }
    }
}
