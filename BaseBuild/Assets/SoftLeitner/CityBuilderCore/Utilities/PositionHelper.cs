using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public static class PositionHelper
    {
        /// <summary>
        /// returns positions for L shaped path between the points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> GetRoadPositions(Vector2Int start, Vector2Int end)
        {
            if (start == end)
            {
                yield return end;
                yield break;
            }

            var xDirection = Math.Sign(end.x - start.x);
            var yDirection = Math.Sign(end.y - start.y);

            if (Math.Abs(start.x - end.x) >= Math.Abs(start.y - end.y))
            {
                for (int x = start.x; x != end.x; x += xDirection)
                {
                    yield return new Vector2Int(x, start.y);//move x towards end
                }

                for (int y = start.y; y != end.y; y += yDirection)
                {
                    yield return new Vector2Int(end.x, y);//move y towards end
                }
            }
            else
            {
                for (int y = start.y; y != end.y; y += yDirection)
                {
                    yield return new Vector2Int(start.x, y);//move y towards end
                }

                for (int x = start.x; x != end.x; x += xDirection)
                {
                    yield return new Vector2Int(x, end.y);//move x towards end
                }
            }

            yield return end;
        }

        /// <summary>
        /// returns positions in a box with the passed points as corners
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> GetBoxPositions(Vector2Int start, Vector2Int end)
        {
            var xDirection = Math.Sign(end.x - start.x);
            var yDirection = Math.Sign(end.y - start.y);

            if (xDirection == 0)
                xDirection = 1;
            if (yDirection == 0)
                yDirection = 1;

            for (int x = start.x; x != end.x + xDirection; x += xDirection)
            {
                for (int y = start.y; y != end.y + yDirection; y += yDirection)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }
        /// <summary>
        /// returns positions in a box in multiples of size with the passed points as corners<br/>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> GetBoxPositions(Vector2Int start, Vector2Int end, Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0)
                yield break;

            var xDirection = Math.Sign(end.x - start.x);
            var yDirection = Math.Sign(end.y - start.y);

            if (xDirection == 0)
                xDirection = 1;
            if (yDirection == 0)
                yDirection = 1;

            int deltaX = 0;
            for (int x = start.x; x != end.x + xDirection; x += xDirection)
            {
                int deltaY = 0;
                for (int y = start.y; y != end.y + yDirection; y += yDirection)
                {
                    if (deltaX % size.x == 0 && deltaY % size.y == 0)
                        yield return new Vector2Int(x, y);

                    deltaY++;
                }
                deltaX++;
            }
        }

        public static IEnumerable<Vector2Int> GetStructurePositions(Vector2Int position, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    yield return new Vector2Int(position.x + x, position.y + y);
                }
            }
        }
        public static IEnumerable<Vector2Int> GetStructurePositions(Vector2Int position, Vector2Int size,BuildingRotation rotation)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    yield return rotation.RotateBuildingPoint(position, new Vector2Int(x, y), size);
                }
            }
        }

        public static IEnumerable<Vector2Int> GetAreaAroundPositions(Vector2Int position, int range)
        {
            for (int i = 1; i <= range; i++)
            {
                //NORTH
                for (int x = 0; x < i * 2; x++)
                {
                    yield return new Vector2Int(position.x - i + x, position.y + i);
                }
                //EAST
                for (int y = 0; y < i * 2; y++)
                {
                    yield return new Vector2Int(position.x + i, position.y + i - y);
                }
                //SOUTH
                for (int x = 0; x < i * 2; x++)
                {
                    yield return new Vector2Int(position.x + i - x, position.y - i);
                }
                //WEST
                for (int y = 0; y < i * 2; y++)
                {
                    yield return new Vector2Int(position.x - i, position.y - i + y);
                }
            }
        }

        public static IEnumerable<Vector2Int> GetRing(Vector2Int point, int distance)
        {
            if (distance == 0)
            {
                yield return point;
            }
            else
            {
                //NORTH
                for (int x = 0; x < distance * 2; x++)
                {
                    yield return new Vector2Int(point.x - distance + x, point.y + distance);
                }
                //EAST
                for (int y = 0; y < distance * 2; y++)
                {
                    yield return new Vector2Int(point.x + distance, point.y + distance - y);
                }
                //SOUTH
                for (int x = 0; x < distance * 2; x++)
                {
                    yield return new Vector2Int(point.x + distance - x, point.y - distance);
                }
                //WEST
                for (int y = 0; y < distance * 2; y++)
                {
                    yield return new Vector2Int(point.x - distance, point.y - distance + y);
                }
            }
        }

        public static IEnumerable<Vector2Int> GetAreaCrossPositions(Vector2Int position, int range)
        {
            for (int i = 1; i <= range; i++)
            {
                yield return new Vector2Int(position.x + i, position.y);
                yield return new Vector2Int(position.x, position.y + i);
                yield return new Vector2Int(position.x - i, position.y);
                yield return new Vector2Int(position.x, position.y - i);
            }
        }
    }
}