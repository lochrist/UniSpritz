using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniMini;

public class Tests
{
    [Test]
    public void RectCutTests()
    {
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutLeft(1);
            Assert.AreEqual(new RectInt(3, 3, 3, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 1, 6), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutLeft(5);
            Assert.AreEqual(new RectInt(6, 3, 0, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutRight(1);
            Assert.AreEqual(new RectInt(2, 3, 3, 6), r);
            Assert.AreEqual(new RectInt(5, 3, 1, 6), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutRight(5);
            Assert.AreEqual(new RectInt(2, 3, 0, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutTop(1);
            Assert.AreEqual(new RectInt(2, 4, 4, 5), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 1), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutTop(7);
            Assert.AreEqual(new RectInt(2, 9, 4, 0), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutBottom(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 5), r);
            Assert.AreEqual(new RectInt(2, 8, 4, 1), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.CutBottom(7);
            Assert.AreEqual(new RectInt(2, 3, 4, 0), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }
    }

    [Test]
    public void RectGetTests()
    {
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetLeft(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 1, 6), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetLeft(5);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetRight(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(5, 3, 1, 6), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetRight(5);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetTop(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 1), r1);

            
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetTop(7);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetBottom(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 8, 4, 1), r1);
        }
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.GetBottom(7);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r1);

        }
    }

    [Test]
    public void RectAddTests()
    {
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.AddLeft(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(1, 3, 5, 6), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.AddRight(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 5, 6), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.AddTop(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 2, 4, 7), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.AddBottom(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(2, 3, 4, 7), r1);
        }
    }

    [Test]
    public void RectUtilityTests()
    {
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.Extend(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(1, 2, 6, 8), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.Contract(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(3, 4, 2, 4), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.Contract(7);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(6, 9, 0, 0), r1);
        }
    }

    [Test]
    public void RectMarginTests()
    {
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.AddMargin(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(r.Extend(1), r1);
            Assert.AreEqual(new RectInt(1, 2, 6, 8), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            // vertical | horizontal
            var r1 = r.AddMargin(1, 2);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(0, 2, 8, 8), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            // top | horizontal | bottom
            var r1 = r.AddMargin(1, 2, 3);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(0, 2, 8, 10), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            // top | right | bottom | left
            var r1 = r.AddMargin(1, 2, 3, 4);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(-2, 2, 10, 10), r1);
        }
    }

    [Test]
    public void RectPaddingTests()
    {
        {
            var r = new RectInt(2, 3, 4, 6);
            var r1 = r.AddPadding(1);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(r.Contract(1), r1);
            Assert.AreEqual(new RectInt(3, 4, 2, 4), r1);
        }

        {
            var r = new RectInt(2, 3, 4, 6);
            // vertical | horizontal
            var r1 = r.AddPadding(1, 2);
            Assert.AreEqual(new RectInt(2, 3, 4, 6), r);
            Assert.AreEqual(new RectInt(4, 4, 0, 4), r1);
        }

        {
            var r = new RectInt(2, 3, 14, 16);
            // vertical | horizontal
            var r1 = r.AddPadding(1, 2);
            Assert.AreEqual(new RectInt(2, 3, 14, 16), r);
            Assert.AreEqual(new RectInt(4, 4, 10, 14), r1);
        }

        {
            var r = new RectInt(2, 3, 14, 16);
            // top | horizontal | bottom
            var r1 = r.AddPadding(1, 2, 3);
            Assert.AreEqual(new RectInt(2, 3, 14, 16), r);
            Assert.AreEqual(new RectInt(4, 4, 10, 12), r1);
        }

        {
            var r = new RectInt(2, 3, 14, 16);
            // top | right | bottom | left
            var r1 = r.AddPadding(1, 2, 3, 4);
            Assert.AreEqual(new RectInt(2, 3, 14, 16), r);
            Assert.AreEqual(new RectInt(6, 4, 8, 12), r1);
        }
    }

    [Test]
    public void FindNextPowerOf2()
    {
        Assert.AreEqual(2, SpritzTextureUtil.FindNextPowerOf2(1.1f));
        Assert.AreEqual(2, SpritzTextureUtil.FindNextPowerOf2(2));
        Assert.AreEqual(8, SpritzTextureUtil.FindNextPowerOf2(5));
        Assert.AreEqual(256, SpritzTextureUtil.FindNextPowerOf2(194.55f));
    }

}