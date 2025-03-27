namespace RadiusCore.Tests
{
    public class UtilsTest
    {
        /// <summary>
        /// Test 3GPP location info parsing from various bytes
        /// </summary>
        [Theory]
        [InlineData("0032f4030921b8e8", "23430")]
        [InlineData("001300710921b8e8", "310170")]
        [InlineData("071300710921b8e8", null)]
        public void Test3GppLocationInfoParsing2(string hexBytes, string? mccmnc)
        {
            Assert.Equal(mccmnc, Utils.GetMccMncFrom3GPPLocationInfo(Utils.StringToByteArray(hexBytes)).mccmnc);
        }
    }
}
