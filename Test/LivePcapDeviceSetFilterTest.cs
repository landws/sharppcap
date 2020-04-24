using System;
using System.Linq;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;

namespace Test
{
    [TestFixture]
    [NonParallelizable]
    public class LivePcapDeviceSetFilterTest
    {
        [Test]
        public void SimpleFilter([CaptureDevices] DeviceFixture fixture)
        {
            // BPF is known to support those link layers, 
            // support for other link layers such as NFLOG and USB is unknown
            var supportedLinks = new[]
            {
                LinkLayers.Ethernet,
                LinkLayers.Raw,
                LinkLayers.Null
            };
            var device = fixture.GetDevice();
            device.Open();
            if (!supportedLinks.Contains(device.LinkType))
            {
                device.Close();
                Assert.Inconclusive("NFLOG link-layer not supported");
            }
            device.Filter = "tcp port 80";
            device.Close(); // close the device
        }

        /// <summary>
        /// Test that we get the expected exception if PcapDevice.SetFilter()
        /// is called on a PcapDevice that has not been opened
        /// </summary>
        [Test]
        public void SetFilterExceptionIfDeviceIsClosed([CaptureDevices] DeviceFixture fixture)
        {
            var device = fixture.GetDevice();
            Assert.Throws<DeviceNotReadyException>(
                () => device.Filter = "tcp port 80",
                "Did not catch the expected PcapDeviceNotReadyException"
            );
        }

        [SetUp]
        public void SetUp()
        {
            TestHelper.ConfirmIdleState();
        }

        [TearDown]
        public void Cleanup()
        {
            TestHelper.ConfirmIdleState();
        }
    }
}
