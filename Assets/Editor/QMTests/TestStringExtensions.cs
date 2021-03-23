using NUnit.Framework;

namespace Tests
{
    public class TestStringExtensions
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SplitWithMaskedSeparator()
        {
            var original = "";
            Assert.That(original.SplitWithMaskedSeparator().Count == 0);

            original = ",";
            Assert.That(original.SplitWithMaskedSeparator().Count == 0);
            
            original = ",,";
            Assert.AreEqual(1, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == ",");
            
            original = ",,,";
            Assert.AreEqual(1, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == ",");
             
            original = "a";
            Assert.AreEqual(1, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == "a");
             
            original = "abcabcabcabc";
            Assert.AreEqual(1, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == "abcabcabcabc");
              
            original = "eins,zwei,drei";
            Assert.AreEqual(3, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == "eins");
            Assert.That(original.SplitWithMaskedSeparator()[1] == "zwei");
            Assert.That(original.SplitWithMaskedSeparator()[2] == "drei");
                          
            original = "eins,,zwei,,drei";
            Assert.AreEqual(1, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == "eins,zwei,drei");
            
            original = ",eins,,zwei,,drei,";
            Assert.AreEqual(1, original.SplitWithMaskedSeparator().Count);
            Assert.That(original.SplitWithMaskedSeparator()[0] == "eins,zwei,drei");
        }
    }
}
