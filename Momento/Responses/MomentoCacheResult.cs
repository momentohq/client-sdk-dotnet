using System;

namespace MomentoSdk.Responses
{
    public class MomentoCacheResult
    {

        public string Value { get; private set; }
        private MomentoCacheResult(string value) { Value = value; }


        public static MomentoCacheResult Ok { get { return new MomentoCacheResult("OK"); } }
        public static MomentoCacheResult Hit { get { return new MomentoCacheResult("HIT"); } }
        public static MomentoCacheResult Miss { get { return new MomentoCacheResult("MISS"); } }
        public static MomentoCacheResult Unknown { get { return new MomentoCacheResult("UNKNOWN"); } }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                MomentoCacheResult r = (MomentoCacheResult)obj;
                return (Value == r.Value);
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
