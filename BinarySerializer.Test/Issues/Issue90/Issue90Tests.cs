using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue90
{
    [TestClass]
    public class Issue90Tests : TestBase
    {
        [TestMethod]
        public void RoundtripTest()
        {
            var expected = new TxpTextureAtlas
            {
                Textures = new List<TxpTexture>
                {
                    new TxpTexture
                    {
                        Mipmaps = new List<TxpMipmap>
                        {
                            new TxpMipmap()
                        },
                        OffsetTable = new List<int> {0}
                    }
                },
                OffsetTable = new List<int> {0}
            };

            var actual = Roundtrip(expected);
        }
    }
}
