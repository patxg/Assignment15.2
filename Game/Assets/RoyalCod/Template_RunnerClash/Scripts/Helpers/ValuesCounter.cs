using System.Linq;
using UnityEngine;

namespace Helpers
{
    public static class ValuesCounter
    {
        public const int CROWD_CAPACITY = 300;
        public const float ROAD_WIDTH = 4f;
        public const float MEN_OFFSET = .6f;
        private const float MEN_FIGHT_OFFSET = .3f;
        private const int HEXAGON_SIDES_COUNT = 6;

        private static Vector3[] _hexagonPositions;
        private static Vector3[] _bossFightPositions;

        public static Vector3[] HexagonPositions => _hexagonPositions != null && _hexagonPositions.Length > 0
            ? _hexagonPositions
            : _hexagonPositions = CountHexagonPositions();

        public static Vector3[] BossFightPositions => _bossFightPositions != null && _bossFightPositions.Length > 0
            ? _bossFightPositions
            : _bossFightPositions = CountBossFightPositions();

        private static Vector3[] CountHexagonPositions()
        {
            var hexagonPositions = new Vector3[CROWD_CAPACITY];

            int index = 0;

            float countBorder_a = MEN_OFFSET;
            float countBorder_b = -MEN_OFFSET;
            int lineNumber = 0;
            Vector3 localPosition;
            Vector3[] sideDirection = new Vector3[HEXAGON_SIDES_COUNT];
            int sideDistance = 1;
            float angleStep;

            for (int i = 0; i < HEXAGON_SIDES_COUNT; i++)
            {
                angleStep = (360f / HEXAGON_SIDES_COUNT) * i;
                sideDirection[i] =
                    new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleStep), 0, Mathf.Sin(Mathf.Deg2Rad * angleStep)) *
                    MEN_OFFSET;
            }

            localPosition = sideDirection[HEXAGON_SIDES_COUNT - 2];

            for (var i = 0; i < CROWD_CAPACITY; i++)
            {
                countBorder_b += MEN_OFFSET;
                if (countBorder_b > countBorder_a || Mathf.Approximately(countBorder_b, countBorder_a))
                {
                    countBorder_b = 0;
                    lineNumber++;
                    if (lineNumber == HEXAGON_SIDES_COUNT)
                    {
                        sideDistance++;
                        countBorder_a += MEN_OFFSET;
                        lineNumber = 0;
                        localPosition = sideDirection[HEXAGON_SIDES_COUNT - 2] * sideDistance;
                    }
                }

                localPosition += sideDirection[lineNumber];
                hexagonPositions[index++] = localPosition;
            }

            hexagonPositions = hexagonPositions.OrderBy(x => x.sqrMagnitude).ToArray();
            hexagonPositions[0] = Vector3.zero;
            return hexagonPositions;
        }

        private static Vector3[] CountBossFightPositions()
        {
            var bossFightPositions = new Vector3[CROWD_CAPACITY];

            var rowsCount = (int) (ROAD_WIDTH / MEN_FIGHT_OFFSET) + 1;
            var rowLenght = (int) (CROWD_CAPACITY / rowsCount) + 1;
            var offsetsZ = ROAD_WIDTH / rowsCount;
            var curPos = Vector3.zero;
            var ind = 0;

            for (var i = 0; i < rowsCount; i++)
            {
                var modifier = i > rowsCount / 2 ? rowsCount - i : i;
                curPos = Vector3.left * (ROAD_WIDTH * 0.5f) + Vector3.right * MEN_FIGHT_OFFSET * i +
                         Vector3.back * offsetsZ * modifier;
                for (var j = 0; j < rowLenght && ind < CROWD_CAPACITY; j++)
                {
                    bossFightPositions[ind++] = curPos;
                    curPos += Vector3.back * MEN_FIGHT_OFFSET;
                }
            }

            bossFightPositions = bossFightPositions.OrderBy(x => x.sqrMagnitude).ToArray();

            return bossFightPositions;
        }
    }
}