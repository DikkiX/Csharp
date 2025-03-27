using Schoolproject.Sensordata;

namespace Schoolproject.Services
{
    public static class SensorDataSingletonFactory
    {
        public static SensorDataSingleton Create()
        {
            return SensorDataSingleton.Instance;
        }
    }
}