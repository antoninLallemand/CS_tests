namespace SensorNameSpace
{   
    public class Sensor
    {
        //attributes
        private string id;
        private bool ready;
        private ushort samplingRate;

        //constructors
        public Sensor(string _id, bool _ready, ushort _samplingRate)
        {
            this.id = _id;
            this.ready =_ready;
            this.samplingRate = _samplingRate;
        }

        //accessors
        public void setId(string _id){id = _id;}
        public string getId(){return id;}
        public void setIsReady(bool _ready){ready = _ready;}
        public bool getIsReady(){return ready;}
        public void setSamplingRate(ushort _samplingRate){samplingRate = _samplingRate;}
        public ushort getSamplingRate(){return samplingRate;}

        //methods
        public void getSensorInfos()
        {
            Console.WriteLine($"Sensor id : {id}");
            Console.WriteLine(ready ? "Sensor is ready" : "Sensor isn't ready yet");
            Console.WriteLine($"Sensor samplingRate : {samplingRate}");
        }

        public override string ToString()=> $"Sensor {id}"; //change existing ToString() method
    }
}