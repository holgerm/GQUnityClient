using QM.Util;

namespace GQTests.Layout
{

    public static class DeviceDefinitions {

		public enum DeviceType {
			iPad4,
			SamsungA3
		};

		public static void Use(DeviceType deviceType) {
			switch (deviceType) {
			case DeviceType.iPad4:
				Device.dpi = 264f;
				Device.height = 2048;
				Device.width = 1536;
				break;
			case DeviceType.SamsungA3:
				Device.dpi = 312f;
				Device.height = 1280;
				Device.width = 720;
				break;
			}
		}
	}
}