using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniMini;

public class AnimTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void ValueNode()
    {
        // Use the Assert class to test conditions
        var valueNode = SpritzAnim.CreateValueNode("cr", 100);

        var content = new AnimNode();
        var result = valueNode.Evaluate(0.5f, content, 0);
        Assert.AreNotEqual(result, valueNode);
        Assert.IsTrue(result.hasValues);
        Assert.IsTrue(!content.hasValues);
        Assert.AreEqual(100, result.values["cr"]);
    }

    [Test]
    public void DelayNode()
    {
        // Use the Assert class to test conditions
        var node = SpritzAnim.CreateDelayNode(0.5f);
        var content = new AnimNode();
        var result = node.Evaluate(0.5f, content, 0);
        Assert.AreNotEqual(result, node);
        Assert.IsTrue(!result.hasValues);
        Assert.IsTrue(!content.hasValues);
    }

    [Test]
    public void InterpolateNode()
    {
        // Use the Assert class to test conditions
        var node = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 100, 1);
        var content = new AnimNode();
        content.values["cr"] = 0;
        var result = node.Evaluate(0.5f, content, 0);
        Assert.AreNotEqual(result, node);
        Assert.IsTrue(result.hasValues);
        Assert.IsTrue(content.hasValues);
        Assert.AreEqual(result.values, content.values);
        Assert.AreEqual(50, result.values["cr"]);
    }

    [Test]
    public void Sequence1()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 50);

        var seq = n1.Sequence(n2);
        Assert.AreNotEqual(n1,seq);
        Assert.AreNotEqual(n2, seq);

        var result = seq.Evaluate(0.5f);
        Assert.AreNotEqual(result, seq);
        Assert.AreEqual(100, result.values["cr"]);
        Assert.AreEqual(50, result.values["cx"]);
    }

    [Test]
    public void Sequence2()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 50);
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);

        var seq = n1.Sequence(n2).Sequence(i1);
        var result = seq.Evaluate(0.75f);
        Assert.AreNotEqual(result, seq);
        Assert.AreEqual(25, result.values["cr"]);
        Assert.AreEqual(50, result.values["cx"]);
    }

    [Test]
    public void CombineSequence()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 50);
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);

        var seq = SpritzAnim.CombineSequenceList(new [] { n1, n2, i1 });
        var result = seq.Evaluate(0.75f);
        Assert.AreNotEqual(result, seq);
        Assert.AreEqual(25, result.values["cr"]);
        Assert.AreEqual(50, result.values["cx"]);
    }

    [Test]
    public void CombineSequence2()
    {
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 100);
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);
        var i2 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cx", 0, 2);

        var seq = SpritzAnim.CombineSequenceList(new[] { n1, n2, i1, i2});
        {
            var result = seq.Evaluate(0.75f);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(25, result.values["cr"]);
            Assert.AreEqual(100, result.values["cx"]);
        }
        
        {
            var result = seq.Evaluate(1.50f);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(0, result.values["cr"]);
            Assert.AreEqual(75, result.values["cx"]);
        }

        {
            var result = seq.Evaluate(3f);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(0, result.values["cr"]);
            Assert.AreEqual(0, result.values["cx"]);
        }
    }


    [Test]
    public void Parallel1()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 50);

        var seq = n1.Parallel(n2);
        Assert.AreNotEqual(n1, seq);
        Assert.AreNotEqual(n2, seq);

        var result = seq.Evaluate(0.5f);
        Assert.AreNotEqual(result, seq);
        Assert.AreEqual(100, result.values["cr"]);
        Assert.AreEqual(50, result.values["cx"]);
    }

    [Test]
    public void Parallel2()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 50);
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);

        var seq = n1.Parallel(n2).Parallel(i1);
        var result = seq.Evaluate(0.75f);
        Assert.AreNotEqual(result, seq);
        Assert.AreEqual(25, result.values["cr"]);
        Assert.AreEqual(50, result.values["cx"]);
    }

    [Test]
    public void CombineParallel()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 50);
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);

        var seq = SpritzAnim.CombineParallelList(new[] { n1, n2, i1 });
        var result = seq.Evaluate(0.75f);
        Assert.AreNotEqual(result, seq);
        Assert.AreEqual(25, result.values["cr"]);
        Assert.AreEqual(50, result.values["cx"]);
    }

    [Test]
    public void CombineParallel2()
    {
        // Use the Assert class to test conditions
        var n1 = SpritzAnim.CreateValueNode("cr", 100);
        var n2 = SpritzAnim.CreateValueNode("cx", 100);
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);
        var i2 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cx", 0, 2);

        var seq = SpritzAnim.CombineParallelList(new[] { n1, n2, i1, i2 });
        {
            var result = seq.Evaluate(0.75f);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(25, result.values["cr"]);
            Assert.AreEqual(62.5, result.values["cx"]);
        }

        {
            var result = seq.Evaluate(1.50f);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(0, result.values["cr"]);
            Assert.AreEqual(25, result.values["cx"]);
        }
    }

    [Test]
    public void Stagger()
    {
        // Use the Assert class to test conditions
        var i1 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cr", 0, 1);
        var i2 = SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "cx", 100, 1);
        var seq = SpritzAnim.Stagger(new[] { i1, i2 }, 0.25f);
        {
            var node = new AnimNode();
            node.values["cr"] = 100;
            node.values["cx"] = 0;
            var result = seq.Evaluate(0.75f, node);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(50, result.values["cr"]);
            Assert.AreEqual(50, result.values["cx"]);
        }

        {
            var node = new AnimNode();
            node.values["cr"] = 100;
            node.values["cx"] = 0;
            var result = seq.Evaluate(0.1f, node);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(100, result.values["cr"]);
            Assert.AreEqual(0, result.values["cx"]);
        }

        {
            var node = new AnimNode();
            node.values["cr"] = 100;
            node.values["cx"] = 0;
            var result = seq.Evaluate(1.25f, node);
            Assert.AreNotEqual(result, seq);
            Assert.AreEqual(0, result.values["cr"]);
            Assert.AreEqual(100, result.values["cx"]);
        }
    }

    [Test]
    public void AnimCircleExample()
    {
        var animCircle = SpritzAnim.CreateValueNode("cx", 100)
                            .Sequence(SpritzAnim.CreateValueNode("cr", 0))
                            .Sequence(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cr", 10, 3))
                            .Sequence(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cx", 300, 1).Parallel(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cr", 70, 1)))
                            .Sequence(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cr", 0, 1));

        {
            var result = animCircle.Evaluate(0f);
            Assert.AreEqual(0, result.values["cr"]);
            Assert.AreEqual(100, result.values["cx"]);
        }

        {
            var result = animCircle.Evaluate(1f);
            Assert.AreEqual(7.03703785f, result.values["cr"]);
            Assert.AreEqual(100, result.values["cx"]);
        }

        {
            var result = animCircle.Evaluate(3.5f);
            Assert.AreEqual(62.5f, result.values["cr"]);
            Assert.AreEqual(275, result.values["cx"]);
        }

        {
            var result = animCircle.Evaluate(4f);
            Assert.AreEqual(70f, result.values["cr"]);
            Assert.AreEqual(300, result.values["cx"]);
        }

        {
            var result = animCircle.Evaluate(4.5f);
            Assert.AreEqual(8.75f, result.values["cr"]);
            Assert.AreEqual(300, result.values["cx"]);
        }

        {
            var result = animCircle.Evaluate(6f);
            Assert.AreEqual(0, result.values["cr"]);
            Assert.AreEqual(300, result.values["cx"]);
        }
    }
}