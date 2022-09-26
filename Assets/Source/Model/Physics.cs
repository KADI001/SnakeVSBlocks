using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnakeVsBlocks.Model
{
    public class Collision2D
    {
        public readonly Vector2[] Contacts;
        public readonly Vector2 Normal;

        public Collision2D() { }

        public Collision2D(IEnumerable<Vector2> contacts, Vector2 normal)
        {
            Contacts = contacts.ToArray();
            Normal = normal;
        }
    }

    public abstract class Collider
    {
        public Transformable Transform { get; private set; }

        protected Collider(Transformable transform)
        {
            Transform = transform;
        }

        public static bool isCollided(BoxCollider box, Vector2 circleCenter, float radius)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Intersected(circleCenter, radius, box.Edges.ElementAt(i)) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<Collision2D> Collided(BoxCollider box, CircleCollider2D circle)
        {
            return Collided(box, circle.Transform.Position, circle.Radius);
        }

        public static IEnumerable<Collision2D> Collided(BoxCollider box1, BoxCollider box2)
        {
            return null;
        }

        public static bool Collided(CircleCollider2D circle, CircleCollider2D other)
        {
            float sqrDistance = (other.Transform.Position - circle.Transform.Position).sqrMagnitude;
            float sqrRadius = (circle.Radius * circle.Radius) + (other.Radius * other.Radius);

            return sqrDistance <= sqrRadius;
        }

        public static IEnumerable<Collision2D> Collided(BoxCollider box, Vector2 circleCenter, float radius)
        {
            List<Collision2D> collisions = new List<Collision2D>();

            for (int i = 0; i < 4; i++)
            {
                Collision2D collision = Intersected(circleCenter, radius, box.Edges.ElementAt(i));

                if (collision != null)
                {
                    collisions.Add(collision);
                }
            }

            return collisions;
        }

        private static Collision2D Intersected(Vector2 circleCenter, float radius, LineSegment lineSegment)
        {
            List<Vector2> contacts = new List<Vector2>();

            Vector2 f = lineSegment.Start - circleCenter;
            float a = Vector2.Dot(lineSegment.Value, lineSegment.Value);
            float b = 2 * Vector2.Dot(f, lineSegment.Value);
            float c = Vector2.Dot(f, f) - (radius * radius);
            float des = (b * b) - (4 * a * c);

            if (des < 0)
            {
                return null;
            }
            else
            {
                des = Mathf.Sqrt(des);

                float t1 = (-b - des) / (2 * a);
                float t2 = (-b + des) / (2 * a);

                if (t1 >= 0 && t1 <= 1)
                {
                    contacts.Add(lineSegment.GetPointByClampedParameter(t1));
                }

                if (t2 >= 0 && t2 <= 1)
                {
                    contacts.Add(lineSegment.GetPointByClampedParameter(t2));
                }

                return contacts.Count != 0 ? new Collision2D(contacts, lineSegment.Normal) : null;
            }
        }
    }

    public sealed class CircleCollider2D : Collider
    {
        public float Radius => Transform.Scale.x * 0.5f;

        public CircleCollider2D(Transformable transform) : base(transform) { }

        public Collision2D Collided(CircleCollider2D other)
        {
            Vector2 position = Transform.Position - other.Transform.Position;

            float a = -2 * position.x;
            float b = -2 * position.y;
            float c = (position.x * position.x) + (position.y * position.y) + (other.Radius * other.Radius) - (Radius * Radius);

            float r = other.Radius;
            float x0 = (-a * c) / (a * a + b * b);
            float y0 = (-b * c) / (a * a + b * b);

            if (c * c > r * r * (a * a + b * b))
            {
                return null;
            }
            else if (Mathf.Abs(c * c - r * r * (a * a + b * b)) <= Mathf.Epsilon)
            {
                float d = r * r - c * c / (a * a + b * b);
                float mult = Mathf.Sqrt(d / (a * a + b * b));
                float ax, ay;
                ax = x0 + b * mult;
                ay = y0 + a * mult;

                Vector2 contact1 = new Vector2(ax, ay) + other.Transform.Position;
                Vector2 normal = new Vector2(a, b).normalized;
                return new Collision2D(new Vector2[1] { contact1 }, normal);
            }
            else
            {
                float d = r * r - c * c / (a * a + b * b);
                float mult = Mathf.Sqrt(d / (a * a + b * b));
                float ax, ay, bx, by;
                ax = x0 + b * mult;
                bx = x0 - b * mult;
                ay = y0 - a * mult;
                by = y0 + a * mult;

                Vector2 contact1 = new Vector2(ax, ay) + other.Transform.Position;
                Vector2 contact2 = new Vector2(bx, by) + other.Transform.Position;
                Vector2 normal = new Vector2(a, b).normalized;
                return new Collision2D(new Vector2[2] { contact1, contact2 }, normal);
            }
        }

        public IEnumerable<Collision2D> Collided(BoxCollider box)
        {
            List<Collision2D> collisions = new List<Collision2D>();

            for (int i = 0; i < 4; i++)
            {
                Collision2D collision = Intersected(box.Edges.ElementAt(i));

                if (collision != null)
                {
                    collisions.Add(collision);
                }
            }

            return collisions;
        }

        public Collision2D Intersected(LineSegment lineSegment)
        {
            List<Vector2> contacts = new List<Vector2>();

            Vector2 f = lineSegment.Start - Transform.Position;
            float a = Vector2.Dot(lineSegment.Value, lineSegment.Value);
            float b = 2 * Vector2.Dot(f, lineSegment.Value);
            float c = Vector2.Dot(f, f) - (Radius * Radius);
            float des = (b * b) - (4 * a * c);

            if (des < 0)
            {
                return null;
            }
            else if(des == 0)
            {
                float t = (-b) / (2 * a);

                if (t >= 0 && t <= 1)
                {
                    contacts.Add(lineSegment.GetPointByClampedParameter(t));
                }

                return contacts.Count != 0 ? new Collision2D(contacts, lineSegment.Normal) : null;
            }
            else
            {
                des = Mathf.Sqrt(des);

                float t1 = (-b - des) / (2 * a);
                float t2 = (-b + des) / (2 * a);

                if (t1 >= 0 && t1 <= 1)
                {
                    contacts.Add(lineSegment.GetPointByClampedParameter(t1));
                }

                if (t2 >= 0 && t2 <= 1)
                {
                    contacts.Add(lineSegment.GetPointByClampedParameter(t2));
                }

                return contacts.Count != 0 ? new Collision2D(contacts, lineSegment.Normal) : null;
            }
        }
    }

    public sealed class LineSegment
    {
        public readonly Vector2 Start;
        public readonly Vector2 End;
        public readonly Vector2 Value;
        public readonly Vector2 Direction;
        public readonly float SqrMagnitude;
        private float _magnitude = -1;

        public float Magnitude
        {
            get
            {
                if(_magnitude == -1)
                {
                    _magnitude = MathF.Sqrt(SqrMagnitude);
                }

                return _magnitude;
            }
        }

        public LineSegment(Vector2 start, Vector2 end) 
        {
            Start = start;
            End = end;
            Value = End - Start;
            Direction = Value.normalized;
            SqrMagnitude = Value.sqrMagnitude;
        }

        public LineSegment(Vector2 start, Vector2 normal, float length) : this(start, start + new Vector2(-normal.y, normal.x) * length)
        {
        }

        public Vector2 Normal => new Vector2(Value.y, -Value.x).normalized;

        public Vector2 GetPointByClampedParameter(float t)
        {
            t = Mathf.Clamp01(t);
            return Start + t * Value;
        }

        public Vector2 GetPointByParameter(float t)
        {
            return Start + t * Value;
        }

        public Collision2D InteresectedLine(LineSegment lineSegment)
        {
            float a1 = Direction.y;
            float b1 = Direction.x;
            float c1 = (Direction.x * Start.y) - (Direction.y * Start.x);

            float a2 = lineSegment.Direction.y;
            float b2 = lineSegment.Direction.x;
            float c2 = (lineSegment.Direction.x * lineSegment.Start.y) - (lineSegment.Direction.y * lineSegment.Start.x);

            if (((a1 * b2) - (a2 * b1)) == 0)
                return null;

            float Y = ((a1 * c2) - (c1 * a2)) / ((a1 * b2) - (a2 * b1));
            float X = ((-b2 * c1) + (b1 * c2)) / ((a1 * b2) - (a2 * b1));

            Vector2 contact = new Vector2(X, Y);

            return new Collision2D(new Vector2[1] { contact }, lineSegment.Normal);
        }

        public Collision2D InteresectedLineSegment(LineSegment lineSegment)
        {
            Collision2D collision = InteresectedLine(lineSegment);

            if (collision == null)
                return null;

            Vector2 contact = collision.Contacts[0];
            float t1;
            float t2;

            if (lineSegment.Value.x != 0)
                t1 = (contact.x - lineSegment.Start.x) / (lineSegment.Value.x);
            else
                t1 = (contact.y - lineSegment.Start.y) / (lineSegment.Value.y);

            if (Value.x != 0)
                t2 = (contact.x - Start.x) / (Value.x);
            else
                t2 = (contact.y - Start.y) / (Value.y);

            if ((t1 >= 0 && t1 <= 1) == false)
                return null;

            if ((t2 >= 0 && t2 <= 1) == false)
                return null;

            return new Collision2D(new Vector2[1] { lineSegment.GetPointByClampedParameter(t1) }, lineSegment.Normal);
        }
    }

    public sealed class BoxCollider : Collider
    {
        private Vector2[] _vertexes = new Vector2[4];
        private LineSegment[] _edges = new LineSegment[4];

        public float HalfWidth => Transform.Scale.x * 0.5f;
        public float HalfHeight => Transform.Scale.y * 0.5f;
        public Vector2 LeftTopVertex => _vertexes[0];
        public Vector2 RightDownVertex => _vertexes[2];

        public BoxCollider(Transformable transform) : base(transform) 
        {
            _vertexes[0] = Transform.Position + (-HalfWidth * Transform.Right) + (HalfHeight * Transform.Up);
            _vertexes[1] = Transform.Position + (-HalfWidth * Transform.Right) + (-HalfHeight * Transform.Up);
            _vertexes[2] = Transform.Position + (HalfWidth * Transform.Right) + (-HalfHeight * Transform.Up);
            _vertexes[3] = Transform.Position + (HalfWidth * Transform.Right) + (HalfHeight * Transform.Up);

            _edges[0] = new LineSegment(_vertexes[0], _vertexes[1]);
            _edges[1] = new LineSegment(_vertexes[1], _vertexes[2]);
            _edges[2] = new LineSegment(_vertexes[2], _vertexes[3]);
            _edges[3] = new LineSegment(_vertexes[3], _vertexes[0]);
        }

        public IReadOnlyCollection<LineSegment> Edges => _edges;
        public IReadOnlyCollection<Vector2> Vertexes => _vertexes;
    }

    public class Physics
    {
        public static bool TryHandleCollision(CircleCollider2D circle, CircleCollider2D other, out Collision2D collision)
        {
            Collision2D circleCollision = circle.Collided(other);

            if (circleCollision == null)
            {
                collision = null;
                return false;
            }

            Vector2 direction = circle.Transform.Position - other.Transform.Position;
            direction = direction.normalized;
            float delta = circle.Radius + other.Radius;

            circle.Transform.MoveTo(other.Transform.Position + (direction * delta));
            collision = circleCollision;
            return true;
        }

        public static bool TryHandleCollision(CircleCollider2D circle, BoxCollider box, out Collision2D collision)
        {
            var collisions = Collider.isCollided(box, circle.Transform.Position, circle.Radius);
            bool insideBox = PointInsideBox(circle.Transform.Position, box);

            if (collisions == false && insideBox == false)
            {
                collision = null;
                return false;
            }
            
            Vector2 closestPoint = ClosestRectanglePointToCircleCenter(circle, box);
            Vector2 delta = closestPoint - circle.Transform.Position;
            int sign = insideBox ? 1 : -1;
            Vector2 offset = delta + ((circle.Radius) * (sign * delta.normalized));

            circle.Transform.Move(offset);

            Vector2 normal = CirclePointNormal(circle, closestPoint).Normal;
            collision = new Collision2D(new Vector2[1] { closestPoint }, normal);

            UnityEngine.Debug.DrawRay(closestPoint, normal);
            return true;
        }

        public static LineSegment CirclePointNormal(CircleCollider2D circle, Vector2 point)
        {
            Vector2 start = point;
            Vector2 normal = (point - circle.Transform.Position).normalized;

            LineSegment result = new LineSegment(start, normal, 5);

            return result;
        }

        public static Vector2 ClosestRectanglePointToCircleCenter(CircleCollider2D circle, BoxCollider box)
        {
            Vector2[] nearPoints = new Vector2[4];

            for (int i = 0; i < box.Edges.Count(); i++) 
            {
                nearPoints[i] = ClosestLineSegmentPointToCircle(circle, box.Edges.ElementAt(i));
            }

            return ClosestPointToCircle(circle, nearPoints);
        }

        public static LineSegment ClosestLineSegmentToCircel(CircleCollider2D circle, IEnumerable<LineSegment> lineSegments)
        {
            Dictionary<Vector2, LineSegment> lineSegmentContactPairs = new Dictionary<Vector2, LineSegment>();
            Vector2[] nearPoints = new Vector2[lineSegments.Count()];

            for (int i = 0; i < lineSegments.Count(); i++)
            {
                nearPoints[i] = ClosestLineSegmentPointToCircle(circle, lineSegments.ElementAt(i));
                lineSegmentContactPairs.Add(nearPoints[i], lineSegments.ElementAt(i));
            }

            Vector2 closestPoint = ClosestPointToCircle(circle, nearPoints);

            return lineSegmentContactPairs[closestPoint];
        }

        public static Vector2 ClosestLineSegmentPointToCircle(CircleCollider2D circle, LineSegment lineSegment)
        {
            Vector2 circelDirection = (circle.Transform.Position + lineSegment.Normal);
            Vector2 contact = lineSegment.InteresectedLine(new LineSegment(circle.Transform.Position, circelDirection)).Contacts[0];
            float t;

            if (lineSegment.Value.x != 0)
                t = (contact.x - lineSegment.Start.x) / (lineSegment.Value.x);
            else
                t = (contact.y - lineSegment.Start.y) / (lineSegment.Value.y);

            return lineSegment.GetPointByClampedParameter(t);
        }

        public static Vector2 ClosestPointToCircle(CircleCollider2D circle, IEnumerable<Vector2> points)
        {
            Vector2[] nearPoints = SortCloseToCircle(circle, points).ToArray();

            return nearPoints[0];
        }

        public static IEnumerable<Vector2> SortCloseToCircle(CircleCollider2D circle, IEnumerable<Vector2> points)
        {
            Vector2[] nearPoints = points.ToArray();

            for (int i = 0; i < nearPoints.Length; i++)
            {
                for (int j = 0; j < nearPoints.Length - 1; j++)
                {
                    if ((circle.Transform.Position - nearPoints[j]).sqrMagnitude > (circle.Transform.Position - nearPoints[j + 1]).sqrMagnitude)
                    {
                        Vector2 z = nearPoints[j];
                        nearPoints[j] = nearPoints[j + 1];
                        nearPoints[j + 1] = z;
                    }
                }
            }

            return nearPoints;
        }

        public static bool PointInsideBox(Vector2 point, BoxCollider box)
        {
            float minX = box.LeftTopVertex.x;
            float maxX = box.RightDownVertex.x;
            float maxY = box.LeftTopVertex.y;
            float minY = box.RightDownVertex.y;

            return (point.x >= minX) && (point.x <= maxX) && (point.y >= minY) && (point.y <= maxY);
        }
    }
}
