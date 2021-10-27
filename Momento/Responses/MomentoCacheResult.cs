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

    }
}
