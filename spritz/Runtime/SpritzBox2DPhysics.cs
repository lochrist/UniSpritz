using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;


namespace UniMini.Physics.Box2D
{
    public class Body
    {
        public Vector2 position;
        public float rotation;
        public Vector2 velocity;
        public float angularVelocity;
        public Vector2 force;
        public float torque;
        public Vector2 width;
        public float friction;
        public float mass;
        public float invMass;
        public float I;
        public float invI;

        public Body()
        {
            friction = 0.2f;
            width = new Vector2(1f,1f);
            mass = float.MaxValue;
            I = float.MaxValue;
            invI = 0;
        }

        public void AddForce(Vector2 f)
        {
            force += f;
        }

        public void Set(Vector2 w, float m)
        {
            position.Set(0.0f, 0.0f);
            rotation = 0.0f;
            velocity.Set(0.0f, 0.0f);
            angularVelocity = 0.0f;
            force.Set(0.0f, 0.0f);
            torque = 0.0f;
            friction = 0.2f;

            width = w;
            mass = m;

            if (mass < float.MaxValue)
            {
                invMass = 1.0f / mass;
                I = mass * (width.x * width.x + width.y * width.y) / 12.0f;
                invI = 1.0f / I;
            }
            else
            {
                invMass = 0.0f;
                I = float.MaxValue;
                invI = 0.0f;
            }
        }
    }

    public class Joint
    {
        public Matrix2x2 M;
        public Vector2 localAnchor1;
        public Vector2 localAnchor2;
        public Vector2 r1;
        public Vector2 r2;
        public Vector2 bias;
        public Vector2 P;
        public Body body1;
        public Body body2;
        public float biasFactor;
        public float softness;

        public Joint()
        {
            biasFactor = 0.2f;
        }

        public void Set(Body b1, Body b2, Vector2 anchor)
        {
            body1 = b1;
            body2 = b2;
            var rot1 = new Matrix2x2(body1.rotation);
            var rot2 = new Matrix2x2(body2.rotation);
            var rot1T = rot1.Transpose();
            var rot2T = rot2.Transpose();
            localAnchor1 = rot1T * (anchor - body1.position);
            localAnchor2 = rot2T * (anchor - body2.position);

            P.Set(0f,0f);

            softness = 0;
            biasFactor = 0.2f;
        }

        public void PreStep(float invDt)
        {
            // Pre-compute anchors, mass matrix, and bias.
            var rot1 = new Matrix2x2(body1.rotation);
            var rot2 = new Matrix2x2(body2.rotation);

            r1 = rot1 * localAnchor1;
            r2 = rot2 * localAnchor2;

            // deltaV = deltaV0 + K * impulse
            // invM = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
            //      = [1/m1+1/m2     0    ] + invI1 * [r1.y*r1.y -r1.x*r1.y] + invI2 * [r1.y*r1.y -r1.x*r1.y]
            //        [    0     1/m1+1/m2]           [-r1.x*r1.y r1.x*r1.x]           [-r1.x*r1.y r1.x*r1.x]
            var K1 = new Matrix2x2();
            K1.col1.x = body1.invMass + body2.invMass; 
            K1.col2.x = 0.0f;
            K1.col1.y = 0.0f;
            K1.col2.y = body1.invMass + body2.invMass;

            var K2 = new Matrix2x2();
            K2.col1.x = body1.invI * r1.y * r1.y;
            K2.col2.x = -body1.invI * r1.x * r1.y;
            K2.col1.y = -body1.invI * r1.x * r1.y;
            K2.col2.y = body1.invI * r1.x * r1.x;

            var K3 = new Matrix2x2();
            K3.col1.x = body2.invI * r2.y * r2.y;
            K3.col2.x = -body2.invI * r2.x * r2.y;
            K3.col1.y = -body2.invI * r2.x * r2.y; 
            K3.col2.y = body2.invI * r2.x * r2.x;

            var K = K1 + K2 + K3;
            K.col1.x += softness;
            K.col2.y += softness;

            M = K.Invert();

            var p1 = body1.position + r1;
            var p2 = body2.position + r2;
            var dp = p2 - p1;

            if (World.positionCorrection)
            {
                bias = -biasFactor * invDt * dp;
            }
            else
            {
                bias.Set(0.0f, 0.0f);
            }

            if (World.warmStarting)
            {
                // Apply accumulated impulse.
                body1.velocity -= body1.invMass * P;
                body1.angularVelocity -= body1.invI * r1.Cross(P);

                body2.velocity += body2.invMass * P;
                body2.angularVelocity += body2.invI * r2.Cross(P);
            }
            else
            {
                P.Set(0.0f, 0.0f);
            }
        }

        public void ApplyImpulse()
        {
            var dv = body2.velocity + Vector2Extensions.Cross(body2.angularVelocity, r2) - body1.velocity - Vector2Extensions.Cross(body1.angularVelocity, r1);

            var impulse = M * (bias - dv - softness * P);

            body1.velocity -= body1.invMass * impulse;
            body1.angularVelocity -= body1.invI * r1.Cross(impulse);

            body2.velocity += body2.invMass * impulse;
            body2.angularVelocity += body2.invI * r2.Cross(impulse);

            P += impulse;
        }
    }

    public struct FeaturePair
    {
        public char inEdge1;
        public char outEdge1;
        public char inEdge2;
        public char outEdge2;

        public override bool Equals(object obj)
        {
            return obj is FeaturePair pair &&
                   inEdge1 == pair.inEdge1 &&
                   outEdge1 == pair.outEdge1 &&
                   inEdge2 == pair.inEdge2 &&
                   outEdge2 == pair.outEdge2;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(inEdge1, outEdge1, inEdge2, outEdge2);
        }

        public static bool operator ==(FeaturePair p1, FeaturePair p2)
        {
            return p1.inEdge1 == p2.inEdge1 &&
                    p1.outEdge1 == p2.outEdge1 &&
                    p1.inEdge2 == p2.inEdge2 &&
                    p1.outEdge2 == p2.outEdge2;
        }

        public static bool operator !=(FeaturePair p1, FeaturePair p2)
        {
            return !(p1 == p2);
        }
    }

    public struct Contact
    {
        public Vector2 position;
        public Vector2 normal;
        public Vector2 r1;
        public Vector2 r2;
        public float separation;
        public float Pn;
        public float Pt;
        public float Pnb;
        public float massNormal;
        public float massTangent;
        public float bias;
        public FeaturePair feature;
    }

    public struct ArbiterKey
    {
        public int key;
        public ArbiterKey(Body b1, Body b2)
        {

            if (b1.GetHashCode() < b2.GetHashCode())
            {
                key = System.HashCode.Combine(b1, b2);
            }
            else
            {
                key = System.HashCode.Combine(b2, b1);
            }
        }

        public override int GetHashCode()
        {
            return key;
        }
    }

    public class Arbiter
    {
        public Contact[] contacts = new Contact[2];
        public int numContacts;
        public Body body1;
        public Body body2;
        public float friction;

        public Arbiter(Body b1, Body b2)
        {
            if (b1.GetHashCode() < b2.GetHashCode())
            {
                body1 = b1;
                body2 = b2;
            }
            else
            {
                body1 = b2;
                body2 = b1;
            }

            numContacts = CollisionHandler.Collide(contacts, body1, body2);
            friction = Mathf.Sqrt(body1.friction * body2.friction);
        }

        public void Update(Contact[] newContacts, int numNewContacts)
        {
            var mergedContacts = new Contact[2];

            for (int i = 0; i < numNewContacts; ++i)
            {
                var cNew = newContacts[i];
                int k = -1;
                for (int j = 0; j < numContacts; ++j)
                {
                    var cOld = contacts[j];
                    if (cNew.feature == cOld.feature)
                    {
                        k = j;
                        break;
                    }
                }

                if (k > -1)
                {
                    var cOld = contacts[k];
                    var c = cNew;
                    if (World.warmStarting)
                    {
                        c.Pn = cOld.Pn;
                        c.Pt = cOld.Pt;
                        c.Pnb = cOld.Pnb;
                    }
                    else
                    {
                        c.Pn = 0.0f;
                        c.Pt = 0.0f;
                        c.Pnb = 0.0f;
                    }

                    mergedContacts[i] = c;
                }
                else
                {
                    mergedContacts[i] = newContacts[i];
                }
            }

            for (int i = 0; i < numNewContacts; ++i)
                contacts[i] = mergedContacts[i];

            numContacts = numNewContacts;
        }

        public void PreStep(float invDt)
        {
            const float k_allowedPenetration = 0.01f;
            float k_biasFactor = World.positionCorrection ? 0.2f : 0.0f;

            for (int i = 0; i < numContacts; ++i)
            {
                var c = contacts[i];

                var r1 = c.position - body1.position;
                var r2 = c.position - body2.position;

                // Precompute normal mass, tangent mass, and bias.
                float rn1 = r1.Dot(c.normal);
                float rn2 = r2.Dot(c.normal);
                float kNormal = body1.invMass + body2.invMass;
                kNormal += body1.invI * (r1.Dot(r1) - rn1 * rn1) + body2.invI * (r2.Dot(r2) - rn2 * rn2);
                c.massNormal = 1.0f / kNormal;

                var tangent = c.normal.Cross(1.0f);
                float rt1 = r1.Dot(tangent);
                float rt2 = r2.Dot(tangent);
                float kTangent = body1.invMass + body2.invMass;
                kTangent += body1.invI * (r1.Dot(r1) - rt1 * rt1) + body2.invI * (r2.Dot(r2) - rt2 * rt2);
                c.massTangent = 1.0f / kTangent;

                c.bias = -k_biasFactor * invDt * Mathf.Min(0.0f, c.separation + k_allowedPenetration);

                contacts[i] = c;

                if (World.accumulateImpulses)
                {
                    // Apply normal + friction impulse
                    var P = c.Pn * c.normal + c.Pt * tangent;

                    body1.velocity -= body1.invMass * P;
                    body1.angularVelocity -= body1.invI * r1.Cross(P);

                    body2.velocity += body2.invMass * P;
                    body2.angularVelocity += body2.invI * r2.Cross(P);
                }
            }

        }

        public void ApplyImpulse()
        {
            var b1 = body1;
            var b2 = body2;

            for (int i = 0; i < numContacts; ++i)
            {
                var c = contacts[i];
                c.r1 = c.position - b1.position;
                c.r2 = c.position - b2.position;

                // Relative velocity at contact
                var dv = b2.velocity + Vector2Extensions.Cross(b2.angularVelocity, c.r2) - b1.velocity - Vector2Extensions.Cross(b1.angularVelocity, c.r1);

                // Compute normal impulse
                var vn = dv.Dot(c.normal);

                var dPn = c.massNormal * (-vn + c.bias);

                if (World.accumulateImpulses)
                {
                    // Clamp the accumulated impulse
                    var Pn0 = c.Pn;
                    c.Pn = Mathf.Max(Pn0 + dPn, 0.0f);
                    dPn = c.Pn - Pn0;
                }
                else
                {
                    dPn = Mathf.Max(dPn, 0.0f);
                }

                // Apply contact impulse
                var Pn = dPn * c.normal;

                b1.velocity -= b1.invMass * Pn;
                b1.angularVelocity -= b1.invI * c.r1.Cross(Pn);

                b2.velocity += b2.invMass * Pn;
                b2.angularVelocity += b2.invI * c.r2.Cross(Pn);

                // Relative velocity at contact
                dv = b2.velocity + Vector2Extensions.Cross(b2.angularVelocity, c.r2) - b1.velocity - Vector2Extensions.Cross(b1.angularVelocity, c.r1);

                var tangent = c.normal.Cross(1.0f);
                float vt = dv.Dot(tangent);
                float dPt = c.massTangent * (-vt);

                if (World.accumulateImpulses)
                {
                    // Compute friction impulse
                    float maxPt = friction * c.Pn;

                    // Clamp friction
                    float oldTangentImpulse = c.Pt;
                    c.Pt = Mathf.Clamp(oldTangentImpulse + dPt, -maxPt, maxPt);
                    dPt = c.Pt - oldTangentImpulse;
                }
                else
                {
                    float maxPt = friction * dPn;
                    dPt = Mathf.Clamp(dPt, -maxPt, maxPt);
                }

                // Apply contact impulse
                var Pt = dPt * tangent;

                b1.velocity -= b1.invMass * Pt;
                b1.angularVelocity -= b1.invI * c.r1.Cross(Pt);

                b2.velocity += b2.invMass * Pt;
                b2.angularVelocity += b2.invI * c.r2.Cross(Pt);
            }
        }
    }

    public static class CollisionHandler
    {
        enum Axis
        {
            FACE_A_X,
            FACE_A_Y,
            FACE_B_X,
            FACE_B_Y
        };

        enum EdgeNumbers
        {
            NO_EDGE = 0,
            EDGE1,
            EDGE2,
            EDGE3,
            EDGE4
        };

        struct ClipVertex
        {
            public Vector2 v;
            public FeaturePair fp;
        };

        public static int Collide(Contact[] contacts, Body bodyA, Body bodyB)
        {
            // Setup
            var hA = 0.5f * bodyA.width;
            var hB = 0.5f * bodyB.width;

            var posA = bodyA.position;
            var posB = bodyB.position;

            var RotA = new Matrix2x2(bodyA.rotation);
            var RotB = new Matrix2x2(bodyB.rotation);

            var RotAT = RotA.Transpose();
            var RotBT = RotB.Transpose();

            var dp = posB - posA;
            var dA = RotAT * dp;
            var dB = RotBT * dp;

            var C = RotAT * RotB;
            var absC = C.Abs();
            var absCT = absC.Transpose();

            // Box A faces
            var faceA = dA.Abs() - hA - absC * hB;
            if (faceA.x > 0.0f || faceA.y > 0.0f)
                return 0;

            // Box B faces
            var faceB = dB.Abs() - absCT * hA - hB;
            if (faceB.x > 0.0f || faceB.y > 0.0f)
                return 0;

            // Box A faces
            var axis = Axis.FACE_A_X;
            var separation = faceA.x;
            var normal = dA.x > 0.0f ? RotA.col1 : -RotA.col1;

            const float relativeTol = 0.95f;
            const float absoluteTol = 0.01f;

            if (faceA.y > relativeTol * separation + absoluteTol * hA.y)
            {
                axis = Axis.FACE_A_Y;
                separation = faceA.y;
                normal = dA.y > 0.0f ? RotA.col2 : -RotA.col2;
            }

            // Box B faces
            if (faceB.x > relativeTol * separation + absoluteTol * hB.x)
            {
                axis = Axis.FACE_B_X;
                separation = faceB.x;
                normal = dB.x > 0.0f ? RotB.col1 : -RotB.col1;
            }

            if (faceB.y > relativeTol * separation + absoluteTol * hB.y)
            {
                axis = Axis.FACE_B_Y;
                separation = faceB.y;
                normal = dB.y > 0.0f ? RotB.col2 : -RotB.col2;
            }

            // Setup clipping plane data based on the separating axis
            var frontNormal = new Vector2();
            var sideNormal = new Vector2();
            var incidentEdge = new ClipVertex[2];
            float front = 0f, negSide = 0f, posSide = 0f;
            char negEdge = (char)0, posEdge = (char)0;

            // Compute the clipping lines and the line segment to be clipped.
            switch (axis)
            {
                case Axis.FACE_A_X:
                    {
                        frontNormal = normal;
                        front = posA.Dot(frontNormal) + hA.x;
                        sideNormal = RotA.col2;
                        float side = posA.Dot(sideNormal);
                        negSide = -side + hA.y;
                        posSide = side + hA.y;
                        negEdge = (char)EdgeNumbers.EDGE3;
                        posEdge = (char)EdgeNumbers.EDGE1;
                        ComputeIncidentEdge(incidentEdge, hB, posB, RotB, frontNormal);
                    }
                    break;

                case Axis.FACE_A_Y:
                    {
                        frontNormal = normal;
                        front = posA.Dot(frontNormal) + hA.y;
                        sideNormal = RotA.col1;
                        float side = posA.Dot(sideNormal);
                        negSide = -side + hA.x;
                        posSide = side + hA.x;
                        negEdge = (char)EdgeNumbers.EDGE2;
                        posEdge = (char)EdgeNumbers.EDGE4;
                        ComputeIncidentEdge(incidentEdge, hB, posB, RotB, frontNormal);
                    }
                    break;

                case Axis.FACE_B_X:
                    {
                        frontNormal = -normal;
                        front = posB.Dot(frontNormal) + hB.x;
                        sideNormal = RotB.col2;
                        float side = posB.Dot(sideNormal);
                        negSide = -side + hB.y;
                        posSide = side + hB.y;
                        negEdge = (char)EdgeNumbers.EDGE3;
                        posEdge = (char)EdgeNumbers.EDGE1;
                        ComputeIncidentEdge(incidentEdge, hA, posA, RotA, frontNormal);
                    }
                    break;

                case Axis.FACE_B_Y:
                    {
                        frontNormal = -normal;
                        front = posB.Dot(frontNormal) + hB.y;
                        sideNormal = RotB.col1;
                        float side = posB.Dot(sideNormal);
                        negSide = -side + hB.x;
                        posSide = side + hB.x;
                        negEdge = (char)EdgeNumbers.EDGE2;
                        posEdge = (char)EdgeNumbers.EDGE4;
                        ComputeIncidentEdge(incidentEdge, hA, posA, RotA, frontNormal);
                    }
                    break;
            }

            // clip other face with 5 box planes (1 face plane, 4 edge planes)

            var clipPoints1 = new ClipVertex[2];
            var clipPoints2 = new ClipVertex[2];
            int np;

            // Clip to box side 1
            np = ClipSegmentToLine(clipPoints1, incidentEdge, -sideNormal, negSide, negEdge);

            if (np < 2)
                return 0;

            // Clip to negative box side 1
            np = ClipSegmentToLine(clipPoints2, clipPoints1, sideNormal, posSide, posEdge);

            if (np < 2)
                return 0;

            // Now clipPoints2 contains the clipping points.
            // Due to roundoff, it is possible that clipping removes all points.

            int numContacts = 0;
            for (int i = 0; i < 2; ++i)
            {
                separation = frontNormal.Dot(clipPoints2[i].v) - front;

                if (separation <= 0)
                {
                    contacts[numContacts].separation = separation;
                    contacts[numContacts].normal = normal;
                    // slide contact point onto reference face (easy to cull)
                    contacts[numContacts].position = clipPoints2[i].v - separation * frontNormal;
                    contacts[numContacts].feature = clipPoints2[i].fp;
                    if (axis == Axis.FACE_B_X || axis == Axis.FACE_B_Y)
                        Flip(ref contacts[numContacts].feature);
                    ++numContacts;
                }
            }

            return numContacts;
        }

        static int ClipSegmentToLine(ClipVertex[] vOut, ClipVertex[] vIn, Vector2 normal, float offset, char clipEdge)
        {
            // Start with no output points
            var numOut = 0;

            // Calculate the distance of end points to the line
            float distance0 = normal.Dot(vIn[0].v) - offset;
            float distance1 = normal.Dot(vIn[1].v) - offset;

            // If the points are behind the plane
            if (distance0 <= 0.0f) vOut[numOut++] = vIn[0];
            if (distance1 <= 0.0f) vOut[numOut++] = vIn[1];

            // If the points are on different sides of the plane
            if (distance0 * distance1 < 0.0f)
            {
                // Find intersection point of edge and plane
                float interp = distance0 / (distance0 - distance1);
                vOut[numOut].v = vIn[0].v + interp * (vIn[1].v - vIn[0].v);
                if (distance0 > 0.0f)
                {
                    vOut[numOut].fp = vIn[0].fp;
                    vOut[numOut].fp.inEdge1 = clipEdge;
                    vOut[numOut].fp.inEdge2 = (char)EdgeNumbers.NO_EDGE;
                }
                else
                {
                    vOut[numOut].fp = vIn[1].fp;
                    vOut[numOut].fp.outEdge1 = clipEdge;
                    vOut[numOut].fp.outEdge2 = (char)EdgeNumbers.NO_EDGE;
                }
                ++numOut;
            }
            return numOut;
        }

        static void ComputeIncidentEdge(ClipVertex[] c, Vector2 h, Vector2 pos, Matrix2x2 rot, Vector2 normal)
        {
            // The normal is from the reference box. Convert it
            // to the incident boxe's frame and flip sign.
            var rotT = rot.Transpose();
            var n = -(rotT * normal);
            var nAbs = n.Abs();

            if (nAbs.x > nAbs.y)
            {
                if (Mathf.Sign(n.x) > 0.0f)
                {
                    c[0].v.Set(h.x, -h.y);
                    c[0].fp.inEdge2 = (char)EdgeNumbers.EDGE3;
                    c[0].fp.outEdge2 = (char)EdgeNumbers.EDGE4;

                    c[1].v.Set(h.x, h.y);
                    c[1].fp.inEdge2 = (char)EdgeNumbers.EDGE4;
                    c[1].fp.outEdge2 = (char)EdgeNumbers.EDGE1;
                }
                else
                {
                    c[0].v.Set(-h.x, h.y);
                    c[0].fp.inEdge2 = (char)EdgeNumbers.EDGE1;
                    c[0].fp.outEdge2 = (char)EdgeNumbers.EDGE2;

                    c[1].v.Set(-h.x, -h.y);
                    c[1].fp.inEdge2 = (char)EdgeNumbers.EDGE2;
                    c[1].fp.outEdge2 = (char)EdgeNumbers.EDGE3;
                }
            }
            else
            {
                if (Mathf.Sign(n.y) > 0.0f)
                {
                    c[0].v.Set(h.x, h.y);
                    c[0].fp.inEdge2 = (char)EdgeNumbers.EDGE4;
                    c[0].fp.outEdge2 = (char)EdgeNumbers.EDGE1;

                    c[1].v.Set(-h.x, h.y);
                    c[1].fp.inEdge2 = (char)EdgeNumbers.EDGE1;
                    c[1].fp.outEdge2 = (char)EdgeNumbers.EDGE2;
                }
                else
                {
                    c[0].v.Set(-h.x, -h.y);
                    c[0].fp.inEdge2 = (char)EdgeNumbers.EDGE2;
                    c[0].fp.outEdge2 = (char)EdgeNumbers.EDGE3;

                    c[1].v.Set(h.x, -h.y);
                    c[1].fp.inEdge2 = (char)EdgeNumbers.EDGE3;
                    c[1].fp.outEdge2 = (char)EdgeNumbers.EDGE4;
                }
            }

            c[0].v = pos + rot * c[0].v;
            c[1].v = pos + rot * c[1].v;
        }

        static void Flip(ref FeaturePair fp)
        {
            Swap(ref fp.inEdge1, ref fp.inEdge2);
            Swap(ref fp.outEdge1, ref fp.outEdge2);
        }

        static void Swap(ref char v1, ref char v2)
        {
            var t = v1;
            v1 = v2;
            v2 = t;
        }
    }

    public class World
    {
        public List<Body> bodies;
        public List<Joint> joints;
        public Dictionary<ArbiterKey, Arbiter> arbiters;
        public Vector2 gravity;
        public int iterations;
        static public bool accumulateImpulses;
        static public bool warmStarting;
        static public bool positionCorrection;

        public World(Vector2 gravity, int iterations)
        {
            this.gravity = gravity;
            this.iterations = iterations;

            bodies = new List<Body>();
            joints = new List<Joint>();
            arbiters = new Dictionary<ArbiterKey, Arbiter>();
        }

        public void Add(Body b)
        {
            bodies.Add(b);
        }

        public void Add(Joint j)
        {
            joints.Add(j);
        }

        public void Clear()
        {
            bodies.Clear();
            joints.Clear();
            arbiters.Clear();
        }

        public void Step(float dt)
        {
            float invDt = dt > 0.0f ? 1.0f / dt : 0.0f;

            // Determine overlapping bodies and update contact points.
            BroadPhase();

            // Integrate forces.
            for (var i = 0; i < bodies.Count; ++i)
            {
                Body b = bodies[i];

                if (b.invMass == 0.0f)
                    continue;

                b.velocity += dt * (gravity + b.invMass * b.force);
                b.angularVelocity += dt * b.invI * b.torque;
            }

            // Perform pre-steps.
            foreach (var kvp in arbiters)
                kvp.Value.PreStep(invDt);

            foreach (var j in joints)
                j.PreStep(invDt);

            // Perform iterations
            for (var i = 0; i < iterations; ++i)
            {
                foreach (var kvp in arbiters)
                    kvp.Value.ApplyImpulse();

                foreach (var j in joints)
                    j.ApplyImpulse();
            }

            // Integrate Velocities
            for (var i = 0; i < bodies.Count; ++i)
            {
                var b = bodies[i];

                b.position += dt * b.velocity;
                b.rotation += dt * b.angularVelocity;

                b.force.Set(0.0f, 0.0f);
                b.torque = 0.0f;
            }
        }

        public void PrintReport(string file = null)
        {
            var str = new StringBuilder();
            str.AppendLine($"#Frame: {Spritz.frame} - G {gravity} - iterations {iterations}");
            if (bodies.Count > 0)
            {
                str.AppendLine($"#Bodies {bodies.Count}");
                int index = 0;
                foreach (var b in bodies)
                {
                    str.AppendLine($"#{index} pos: {b.position} vel: {b.velocity} angVel: {b.angularVelocity} F: {b.force} Torq: {b.torque} Width: {b.width}");
                    ++index;
                }
            }

            if (joints.Count > 0)
            {
                int index = 0;
                str.AppendLine($"#Joints {joints.Count}");
                foreach (var j in joints)
                {
                    str.AppendLine($"#{index} Anc1: {j.localAnchor1} Anc1: {j.localAnchor2} r1: {j.r1} r2: {j.r2} P: {j.P}");
                    ++index;
                }
            }

            if (arbiters.Count > 0)
            {
                str.AppendLine($"#Arbiters {arbiters.Count}");
                foreach (var a in arbiters)
                {

                }
            }

            if (file == null)
            {
                Debug.Log(str.ToString());
            }
            else
            {
                File.AppendAllText(file, str.ToString());
            }
        }

        private void BroadPhase()
        {
            // O(n^2) broad-phase
            for (var i = 0; i < bodies.Count; ++i)
            {
                var bi = bodies[i];

                for (int j = i + 1; j < bodies.Count; ++j)
                {
                    var bj = bodies[j];

                    if (bi.invMass == 0.0f && bj.invMass == 0.0f)
                        continue;

                    var newArb = new Arbiter(bi, bj);
                    var key = new ArbiterKey(bi, bj);

                    if (newArb.numContacts > 0)
                    {
                        if (!arbiters.TryGetValue(key, out var existingArbiter))
                        {
                            arbiters.Add(key, newArb);
                        }
                        else
                        {
                            existingArbiter.Update(newArb.contacts, newArb.numContacts);
                        }
                    }
                    else
                    {
                        arbiters.Remove(key);
                    }
                }
            }
        }
    }
}
