using NUnit.Framework;
using StackECS;
using StackECS.Pools;

namespace StackECSTests
{
    [TestFixture]
    public class BitMaskTests
    {
        private readonly SpanStorage<ulong> _spanStorage = new(1);

        private BitMask CreateMask()
        {
            return new BitMask(_spanStorage);
        }

        [Test]
        public void SetAndGetBit()
        {
            var mask = CreateMask();
            mask[3] = true;
            Assert.IsTrue(mask[3]);
            mask[3] = false;
            Assert.IsFalse(mask[3]);
        }

        [Test]
        public void CopyWithBit_AddsBit()
        {
            var mask = CreateMask();
            var mask2 = mask.CopyWithBit(5);
            Assert.IsTrue(mask2[5]);
            Assert.IsFalse(mask[5]);
        }

        [Test]
        public void CopyWithoutBit_RemovesBit()
        {
            var mask = CreateMask();
            mask[7] = true;
            var mask2 = mask.CopyWithoutBit(7);
            Assert.IsFalse(mask2[7]);
            Assert.IsTrue(mask[7]);
        }

        [Test]
        public void OperatorPlus_OrsBits()
        {
            var mask1 = CreateMask();
            var mask2 = CreateMask();
            mask1[1] = true;
            mask2[2] = true;
            var mask3 = mask1 + mask2;
            Assert.IsTrue(mask3[1]);
            Assert.IsTrue(mask3[2]);
        }

        [Test]
        public void OperatorMinus_AndNotBits()
        {
            var mask1 = CreateMask();
            var mask2 = CreateMask();
            mask1[1] = true;
            mask1[2] = true;
            mask2[2] = true;
            var mask3 = mask1 - mask2;
            Assert.IsTrue(mask3[1]);
            Assert.IsFalse(mask3[2]);
        }

        [Test]
        public void EqualsAndHashCode()
        {
            var mask1 = CreateMask();
            var mask2 = CreateMask();
            mask1[4] = true;
            mask2[4] = true;
            Assert.IsTrue(mask1 == mask2);
            Assert.AreEqual(mask1.GetHashCode(), mask2.GetHashCode());
        }

        [Test]
        public void NotEquals()
        {
            var mask1 = CreateMask();
            var mask2 = CreateMask();
            mask1[4] = true;
            mask2[5] = true;
            Assert.IsTrue(mask1 != mask2);
            Assert.AreNotEqual(mask1.GetHashCode(), mask2.GetHashCode());
        }
    }
}